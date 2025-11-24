using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MiniSO.Domain.Entities;

namespace MiniSO.Application.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }

    public interface IClientRepository : IRepository<Client>
    {
        Task<IEnumerable<Client>> GetActiveClientsAsync();
    }

    public interface IItemRepository : IRepository<Item>
    {
        Task<IEnumerable<Item>> GetActiveItemsAsync();
    }

    public interface ISalesOrderRepository : IRepository<SalesOrder>
    {
        Task<SalesOrder> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<SalesOrder>> GetAllWithDetailsAsync();
        Task<bool> InvoiceNoExistsAsync(string invoiceNo, int? excludeOrderId = null);
    }

    public interface IUnitOfWork : IDisposable
    {
        IClientRepository Clients { get; }
        IItemRepository Items { get; }
        ISalesOrderRepository SalesOrders { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}