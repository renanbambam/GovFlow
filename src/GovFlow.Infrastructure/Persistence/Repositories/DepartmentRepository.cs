using GovFlow.Domain.Organization;
using Microsoft.EntityFrameworkCore;

namespace GovFlow.Infrastructure.Persistence.Repositories;

internal sealed class DepartmentRepository : IDepartmentRepository
{
    private readonly GovFlowDbContext _context;

    public DepartmentRepository(GovFlowDbContext context) => _context = context;

    public Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Departments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(Department department, CancellationToken cancellationToken = default)
        => await _context.Departments.AddAsync(department, cancellationToken);
}
