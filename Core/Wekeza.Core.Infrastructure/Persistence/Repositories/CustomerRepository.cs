///4. CustomerRepository.cs
///The KYC hub. This handles the complex retrieval of corporate customers and their directors.
///
using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Infrastructure.Persistence;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerRepository(ApplicationDbContext context) => _context = context;

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task AddAsync(Customer customer, CancellationToken ct)
    {
        await _context.Customers.AddAsync(customer, ct);
    }

    public void Update(Customer customer)
    {
        _context.Customers.Update(customer);
    }

    public async Task<Customer?> GetByIdentificationNumberAsync(string idNumber, CancellationToken ct)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.IdentificationNumber == idNumber, ct);
    }
}
