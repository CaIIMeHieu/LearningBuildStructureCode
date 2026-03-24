using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions;
using Persistance.DBContext;

namespace Persistance;

public class EfUnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public EfUnitOfWork(ApplicationDbContext context)
        => _context = context;

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync();

    async ValueTask IAsyncDisposable.DisposeAsync()
        => await _context.DisposeAsync();
}
