using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSO.API.Models;
using MiniSO.Domain.Entities;
using MiniSO.Infrastructure.Data;

namespace MiniSO.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/clients
        [HttpGet]
        public async Task<IActionResult> GetAllClients()
        {
            var clients = await _context.Clients
                .Where(c => c.IsActive)
                .Select(c => new ClientDto
                {
                    ClientId = c.ClientId,
                    CustomerName = c.CustomerName,
                    Address1 = c.Address1,
                    Address2 = c.Address2,
                    Address3 = c.Address3,
                    State = c.State,
                    PostCode = c.PostCode
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<ClientDto>>.SuccessResponse(clients));
        }

        // GET: api/clients/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClientById(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound(ApiResponse<ClientDto>.ErrorResponse("Client not found"));
            }

            var clientDto = new ClientDto
            {
                ClientId = client.ClientId,
                CustomerName = client.CustomerName,
                Address1 = client.Address1,
                Address2 = client.Address2,
                Address3 = client.Address3,
                State = client.State,
                PostCode = client.PostCode
            };

            return Ok(ApiResponse<ClientDto>.SuccessResponse(clientDto));
        }

        // POST: api/clients
        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] ClientDto clientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ClientDto>.ErrorResponse("Invalid data"));
            }

            var client = new Client
            {
                CustomerName = clientDto.CustomerName,
                Address1 = clientDto.Address1,
                Address2 = clientDto.Address2,
                Address3 = clientDto.Address3,
                State = clientDto.State,
                PostCode = clientDto.PostCode,
                IsActive = true
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            clientDto.ClientId = client.ClientId;

            return CreatedAtAction(nameof(GetClientById), new { id = client.ClientId },
                ApiResponse<ClientDto>.SuccessResponse(clientDto, "Client created successfully"));
        }

        // PUT: api/clients/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(int id, [FromBody] ClientDto clientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ClientDto>.ErrorResponse("Invalid data"));
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound(ApiResponse<ClientDto>.ErrorResponse("Client not found"));
            }

            client.CustomerName = clientDto.CustomerName;
            client.Address1 = clientDto.Address1;
            client.Address2 = clientDto.Address2;
            client.Address3 = clientDto.Address3;
            client.State = clientDto.State;
            client.PostCode = clientDto.PostCode;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<ClientDto>.SuccessResponse(clientDto, "Client updated successfully"));
        }

        // DELETE: api/clients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Client not found"));
            }

            // Soft delete
            client.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Client deleted successfully"));
        }
    }
}