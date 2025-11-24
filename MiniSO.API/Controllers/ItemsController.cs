using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSO.API.Models;
using MiniSO.Infrastructure.Data;

namespace MiniSO.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/items
        [HttpGet]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _context.Items
                .Where(i => i.IsActive)
                .Select(i => new ItemDto
                {
                    ItemId = i.ItemId,
                    ItemCode = i.ItemCode,
                    Description = i.Description,
                    UnitPrice = i.UnitPrice
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<ItemDto>>.SuccessResponse(items));
        }

        // GET: api/items/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemById(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound(ApiResponse<ItemDto>.ErrorResponse("Item not found"));
            }

            var itemDto = new ItemDto
            {
                ItemId = item.ItemId,
                ItemCode = item.ItemCode,
                Description = item.Description,
                UnitPrice = item.UnitPrice
            };

            return Ok(ApiResponse<ItemDto>.SuccessResponse(itemDto));
        }

        // POST: api/items
        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody] ItemDto itemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ItemDto>.ErrorResponse("Invalid data"));
            }

            // Check if item code already exists
            var exists = await _context.Items.AnyAsync(i => i.ItemCode == itemDto.ItemCode);
            if (exists)
            {
                return BadRequest(ApiResponse<ItemDto>.ErrorResponse("Item code already exists"));
            }

            var item = new Domain.Entities.Item
            {
                ItemCode = itemDto.ItemCode,
                Description = itemDto.Description,
                UnitPrice = itemDto.UnitPrice,
                IsActive = true
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            itemDto.ItemId = item.ItemId;

            return CreatedAtAction(nameof(GetItemById), new { id = item.ItemId }, 
                ApiResponse<ItemDto>.SuccessResponse(itemDto, "Item created successfully"));
        }

        // PUT: api/items/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] ItemDto itemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ItemDto>.ErrorResponse("Invalid data"));
            }

            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound(ApiResponse<ItemDto>.ErrorResponse("Item not found"));
            }

            // Check if new item code conflicts with existing items
            var codeExists = await _context.Items
                .AnyAsync(i => i.ItemCode == itemDto.ItemCode && i.ItemId != id);
            if (codeExists)
            {
                return BadRequest(ApiResponse<ItemDto>.ErrorResponse("Item code already exists"));
            }

            item.ItemCode = itemDto.ItemCode;
            item.Description = itemDto.Description;
            item.UnitPrice = itemDto.UnitPrice;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<ItemDto>.SuccessResponse(itemDto, "Item updated successfully"));
        }

        // DELETE: api/items/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Item not found"));
            }

            // Soft delete - just mark as inactive
            item.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Item deleted successfully"));
        }
    }
}