using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MiniSO.Application.Interfaces;
using MiniSO.Domain.Entities;
using MiniSO.Infrastructure.Data;

namespace MiniSO.Application.Interfaces
{
    // Minimal repository and unit-of-work contracts used by the infrastructure.
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

    public interface IClientRepository : IRepository<Client> { }

    public interface IItemRepository : IRepository<Item> { }

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

namespace MiniSO.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public virtual Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
    }

    public class ClientRepository : Repository<Client>, IClientRepository
    {
        public ClientRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Client>> GetActiveClientsAsync()
        {
            return await _dbSet.Where(c => c.IsActive).OrderBy(c => c.CustomerName).ToListAsync();
        }
    }

    public class ItemRepository : Repository<Item>, IItemRepository
    {
        public ItemRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Item>> GetActiveItemsAsync()
        {
            return await _dbSet.Where(i => i.IsActive).OrderBy(i => i.ItemCode).ToListAsync();
        }
    }

    public class SalesOrderRepository : Repository<SalesOrder>, ISalesOrderRepository
    {
        public SalesOrderRepository(ApplicationDbContext context) : base(context) { }

        public async Task<SalesOrder> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(so => so.SalesOrderDetails.OrderBy(d => d.LineNumber))
                .Include(so => so.Client)
                .FirstOrDefaultAsync(so => so.SalesOrderId == id);
        }

        public async Task<IEnumerable<SalesOrder>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(so => so.SalesOrderDetails.OrderBy(d => d.LineNumber))
                .Include(so => so.Client)
                .Where(so => so.IsActive)
                .OrderByDescending(so => so.CreatedDate)
                .ToListAsync();
        }

        public async Task<bool> InvoiceNoExistsAsync(string invoiceNo, int? excludeOrderId = null)
        {
            var query = _dbSet.Where(so => so.InvoiceNo == invoiceNo);
            if (excludeOrderId.HasValue)
            {
                query = query.Where(so => so.SalesOrderId != excludeOrderId.Value);
            }
            return await query.AnyAsync();
        }
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction _transaction;
        
        public IClientRepository Clients { get; private set; }
        public IItemRepository Items { get; private set; }
        public ISalesOrderRepository SalesOrders { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Clients = new ClientRepository(_context);
            Items = new ItemRepository(_context);
            SalesOrders = new SalesOrderRepository(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
}