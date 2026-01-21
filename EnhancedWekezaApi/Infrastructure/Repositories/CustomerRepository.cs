using Microsoft.EntityFrameworkCore;
using EnhancedWekezaApi.Domain.Entities;
using EnhancedWekezaApi.Domain.Interfaces;
using EnhancedWekezaApi.Infrastructure.Data;

namespace EnhancedWekezaApi.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly WekezaDbContext _context;

    public CustomerRepository(WekezaDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Include(c => c.Accounts)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Customer?> GetByIdentificationNumberAsync(string identificationNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Include(c => c.Accounts)
            .FirstOrDefaultAsync(c => c.IdentificationNumber == identificationNumber, cancellationToken);
    }

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Include(c => c.Accounts)
            .FirstOrDefaultAsync(c => c.Email == email, cancellationToken);
    }

    public async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Include(c => c.Accounts)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync(cancellationToken);
    }
}