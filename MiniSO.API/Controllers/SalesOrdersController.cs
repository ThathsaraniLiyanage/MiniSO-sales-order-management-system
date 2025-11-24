using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSO.API.Models;
using MiniSO.Domain.Entities;
using MiniSO.Infrastructure.Data;

namespace MiniSO.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesOrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SalesOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/salesorders
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _context.SalesOrders
                .Include(so => so.SalesOrderDetails.OrderBy(d => d.LineNumber))
                .Include(so => so.Client)
                .Where(so => so.IsActive)
                .OrderByDescending(so => so.CreatedDate)
                .Select(so => new SalesOrderDto
                {
                    SalesOrderId = so.SalesOrderId,
                    InvoiceNo = so.InvoiceNo,
                    InvoiceDate = so.InvoiceDate,
                    ReferenceNo = so.ReferenceNo,
                    ClientId = so.ClientId,
                    CustomerName = so.CustomerName,
                    Address1 = so.Address1,
                    Address2 = so.Address2,
                    Address3 = so.Address3,
                    State = so.State,
                    PostCode = so.PostCode,
                    TotalExclAmount = so.TotalExclAmount,
                    TotalTaxAmount = so.TotalTaxAmount,
                    TotalInclAmount = so.TotalInclAmount,
                    CreatedDate = so.CreatedDate,
                    ModifiedDate = so.ModifiedDate,
                    OrderDetails = so.SalesOrderDetails.Select(d => new SalesOrderDetailDto
                    {
                        SalesOrderDetailId = d.SalesOrderDetailId,
                        ItemId = d.ItemId,
                        ItemCode = d.ItemCode,
                        Description = d.Description,
                        Note = d.Note,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        TaxRate = d.TaxRate,
                        ExclAmount = d.ExclAmount,
                        TaxAmount = d.TaxAmount,
                        InclAmount = d.InclAmount,
                        LineNumber = d.LineNumber
                    }).ToList()
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<SalesOrderDto>>.SuccessResponse(orders));
        }

        // GET: api/salesorders/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _context.SalesOrders
                .Include(so => so.SalesOrderDetails.OrderBy(d => d.LineNumber))
                .Include(so => so.Client)
                .FirstOrDefaultAsync(so => so.SalesOrderId == id);

            if (order == null)
            {
                return NotFound(ApiResponse<SalesOrderDto>.ErrorResponse("Order not found"));
            }

            var orderDto = new SalesOrderDto
            {
                SalesOrderId = order.SalesOrderId,
                InvoiceNo = order.InvoiceNo,
                InvoiceDate = order.InvoiceDate,
                ReferenceNo = order.ReferenceNo,
                ClientId = order.ClientId,
                CustomerName = order.CustomerName,
                Address1 = order.Address1,
                Address2 = order.Address2,
                Address3 = order.Address3,
                State = order.State,
                PostCode = order.PostCode,
                TotalExclAmount = order.TotalExclAmount,
                TotalTaxAmount = order.TotalTaxAmount,
                TotalInclAmount = order.TotalInclAmount,
                CreatedDate = order.CreatedDate,
                ModifiedDate = order.ModifiedDate,
                OrderDetails = order.SalesOrderDetails
                    .OrderBy(d => d.LineNumber)
                    .Select(d => new SalesOrderDetailDto
                    {
                        SalesOrderDetailId = d.SalesOrderDetailId,
                        ItemId = d.ItemId,
                        ItemCode = d.ItemCode,
                        Description = d.Description,
                        Note = d.Note,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        TaxRate = d.TaxRate,
                        ExclAmount = d.ExclAmount,
                        TaxAmount = d.TaxAmount,
                        InclAmount = d.InclAmount,
                        LineNumber = d.LineNumber
                    }).ToList()
            };

            return Ok(ApiResponse<SalesOrderDto>.SuccessResponse(orderDto));
        }

        // POST: api/salesorders
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateUpdateSalesOrderDto orderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<SalesOrderDto>.ErrorResponse("Validation failed", 
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            // Check invoice number uniqueness
            var invoiceExists = await _context.SalesOrders
                .AnyAsync(so => so.InvoiceNo == orderDto.InvoiceNo && so.IsActive);
            if (invoiceExists)
            {
                return BadRequest(ApiResponse<SalesOrderDto>.ErrorResponse("Invoice number already exists"));
            }

            // Verify client exists
            var clientExists = await _context.Clients.AnyAsync(c => c.ClientId == orderDto.ClientId);
            if (!clientExists)
            {
                return BadRequest(ApiResponse<SalesOrderDto>.ErrorResponse("Client not found"));
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new SalesOrder
                {
                    InvoiceNo = orderDto.InvoiceNo,
                    InvoiceDate = orderDto.InvoiceDate,
                    ReferenceNo = orderDto.ReferenceNo,
                    ClientId = orderDto.ClientId,
                    CustomerName = orderDto.CustomerName,
                    Address1 = orderDto.Address1,
                    Address2 = orderDto.Address2,
                    Address3 = orderDto.Address3,
                    State = orderDto.State,
                    PostCode = orderDto.PostCode,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };

                int lineNumber = 1;
                foreach (var detailDto in orderDto.OrderDetails)
                {
                    var item = await _context.Items.FindAsync(detailDto.ItemId);
                    if (item == null || !item.IsActive)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest(ApiResponse<SalesOrderDto>.ErrorResponse($"Item with ID {detailDto.ItemId} not found or inactive"));
                    }

                    var detail = new SalesOrderDetail
                    {
                        ItemId = detailDto.ItemId,
                        ItemCode = item.ItemCode,
                        Description = item.Description,
                        Note = detailDto.Note ?? string.Empty,
                        Quantity = detailDto.Quantity,
                        UnitPrice = item.UnitPrice,
                        TaxRate = detailDto.TaxRate,
                        LineNumber = lineNumber++
                    };
                    detail.CalculateAmounts();
                    order.SalesOrderDetails.Add(detail);
                }

                order.CalculateTotals();
                _context.SalesOrders.Add(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Fetch the created order with details
                var createdOrder = await _context.SalesOrders
                    .Include(so => so.SalesOrderDetails.OrderBy(d => d.LineNumber))
                    .FirstOrDefaultAsync(so => so.SalesOrderId == order.SalesOrderId);

                var resultDto = new SalesOrderDto
                {
                    SalesOrderId = createdOrder!.SalesOrderId,
                    InvoiceNo = createdOrder.InvoiceNo,
                    InvoiceDate = createdOrder.InvoiceDate,
                    ReferenceNo = createdOrder.ReferenceNo,
                    ClientId = createdOrder.ClientId,
                    CustomerName = createdOrder.CustomerName,
                    Address1 = createdOrder.Address1,
                    Address2 = createdOrder.Address2,
                    Address3 = createdOrder.Address3,
                    State = createdOrder.State,
                    PostCode = createdOrder.PostCode,
                    TotalExclAmount = createdOrder.TotalExclAmount,
                    TotalTaxAmount = createdOrder.TotalTaxAmount,
                    TotalInclAmount = createdOrder.TotalInclAmount,
                    CreatedDate = createdOrder.CreatedDate,
                    ModifiedDate = createdOrder.ModifiedDate,
                    OrderDetails = createdOrder.SalesOrderDetails
                        .OrderBy(d => d.LineNumber)
                        .Select(d => new SalesOrderDetailDto
                        {
                            SalesOrderDetailId = d.SalesOrderDetailId,
                            ItemId = d.ItemId,
                            ItemCode = d.ItemCode,
                            Description = d.Description,
                            Note = d.Note,
                            Quantity = d.Quantity,
                            UnitPrice = d.UnitPrice,
                            TaxRate = d.TaxRate,
                            ExclAmount = d.ExclAmount,
                            TaxAmount = d.TaxAmount,
                            InclAmount = d.InclAmount,
                            LineNumber = d.LineNumber
                        }).ToList()
                };

                return CreatedAtAction(nameof(GetOrderById), new { id = order.SalesOrderId },
                    ApiResponse<SalesOrderDto>.SuccessResponse(resultDto, "Order created successfully"));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, ApiResponse<SalesOrderDto>.ErrorResponse("Failed to create order", 
                    new List<string> { ex.Message }));
            }
        }

        // PUT: api/salesorders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] CreateUpdateSalesOrderDto orderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<SalesOrderDto>.ErrorResponse("Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var existingOrder = await _context.SalesOrders
                .Include(so => so.SalesOrderDetails)
                .FirstOrDefaultAsync(so => so.SalesOrderId == id);

            if (existingOrder == null)
            {
                return NotFound(ApiResponse<SalesOrderDto>.ErrorResponse("Order not found"));
            }

            // Check invoice number uniqueness (excluding current order)
            var invoiceExists = await _context.SalesOrders
                .AnyAsync(so => so.InvoiceNo == orderDto.InvoiceNo && so.SalesOrderId != id && so.IsActive);
            if (invoiceExists)
            {
                return BadRequest(ApiResponse<SalesOrderDto>.ErrorResponse("Invoice number already exists"));
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Update order header
                existingOrder.InvoiceNo = orderDto.InvoiceNo;
                existingOrder.InvoiceDate = orderDto.InvoiceDate;
                existingOrder.ReferenceNo = orderDto.ReferenceNo;
                existingOrder.ClientId = orderDto.ClientId;
                existingOrder.CustomerName = orderDto.CustomerName;
                existingOrder.Address1 = orderDto.Address1;
                existingOrder.Address2 = orderDto.Address2;
                existingOrder.Address3 = orderDto.Address3;
                existingOrder.State = orderDto.State;
                existingOrder.PostCode = orderDto.PostCode;
                existingOrder.ModifiedDate = DateTime.Now;

                // Remove existing details
                _context.SalesOrderDetails.RemoveRange(existingOrder.SalesOrderDetails);

                // Add new details
                int lineNumber = 1;
                foreach (var detailDto in orderDto.OrderDetails)
                {
                    var item = await _context.Items.FindAsync(detailDto.ItemId);
                    if (item == null || !item.IsActive)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest(ApiResponse<SalesOrderDto>.ErrorResponse($"Item with ID {detailDto.ItemId} not found or inactive"));
                    }

                    var detail = new SalesOrderDetail
                    {
                        SalesOrderId = existingOrder.SalesOrderId,
                        ItemId = detailDto.ItemId,
                        ItemCode = item.ItemCode,
                        Description = item.Description,
                        Note = detailDto.Note ?? string.Empty,
                        Quantity = detailDto.Quantity,
                        UnitPrice = item.UnitPrice,
                        TaxRate = detailDto.TaxRate,
                        LineNumber = lineNumber++
                    };
                    detail.CalculateAmounts();
                    existingOrder.SalesOrderDetails.Add(detail);
                }

                existingOrder.CalculateTotals();
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Fetch updated order
                var updatedOrder = await _context.SalesOrders
                    .Include(so => so.SalesOrderDetails.OrderBy(d => d.LineNumber))
                    .FirstOrDefaultAsync(so => so.SalesOrderId == id);

                var resultDto = new SalesOrderDto
                {
                    SalesOrderId = updatedOrder!.SalesOrderId,
                    InvoiceNo = updatedOrder.InvoiceNo,
                    InvoiceDate = updatedOrder.InvoiceDate,
                    ReferenceNo = updatedOrder.ReferenceNo,
                    ClientId = updatedOrder.ClientId,
                    CustomerName = updatedOrder.CustomerName,
                    Address1 = updatedOrder.Address1,
                    Address2 = updatedOrder.Address2,
                    Address3 = updatedOrder.Address3,
                    State = updatedOrder.State,
                    PostCode = updatedOrder.PostCode,
                    TotalExclAmount = updatedOrder.TotalExclAmount,
                    TotalTaxAmount = updatedOrder.TotalTaxAmount,
                    TotalInclAmount = updatedOrder.TotalInclAmount,
                    CreatedDate = updatedOrder.CreatedDate,
                    ModifiedDate = updatedOrder.ModifiedDate,
                    OrderDetails = updatedOrder.SalesOrderDetails
                        .OrderBy(d => d.LineNumber)
                        .Select(d => new SalesOrderDetailDto
                        {
                            SalesOrderDetailId = d.SalesOrderDetailId,
                            ItemId = d.ItemId,
                            ItemCode = d.ItemCode,
                            Description = d.Description,
                            Note = d.Note,
                            Quantity = d.Quantity,
                            UnitPrice = d.UnitPrice,
                            TaxRate = d.TaxRate,
                            ExclAmount = d.ExclAmount,
                            TaxAmount = d.TaxAmount,
                            InclAmount = d.InclAmount,
                            LineNumber = d.LineNumber
                        }).ToList()
                };

                return Ok(ApiResponse<SalesOrderDto>.SuccessResponse(resultDto, "Order updated successfully"));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, ApiResponse<SalesOrderDto>.ErrorResponse("Failed to update order",
                    new List<string> { ex.Message }));
            }
        }

        // DELETE: api/salesorders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.SalesOrders.FindAsync(id);
            if (order == null)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Order not found"));
            }

            // Soft delete
            order.IsActive = false;
            order.ModifiedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Order deleted successfully"));
        }
    }
}