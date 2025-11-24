using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniSO.Application.Services
{
    public interface IClientService
    {
        Task<dynamic> GetAllClientsAsync();
        Task<dynamic> GetClientByIdAsync(int id);
    }

    public interface IItemService
    {
        Task<dynamic> GetAllItemsAsync();
        Task<dynamic> GetItemByIdAsync(int id);
    }

    public interface ISalesOrderService
    {
        Task<dynamic> GetAllOrdersAsync();
        Task<dynamic> GetOrderByIdAsync(int id);
        Task<dynamic> CreateOrderAsync(dynamic orderDto);
        Task<dynamic> UpdateOrderAsync(int id, dynamic orderDto);
        Task<dynamic> DeleteOrderAsync(int id);
    }
}