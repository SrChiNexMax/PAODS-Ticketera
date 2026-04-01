using Microsoft.EntityFrameworkCore;
using PdsTk.Domain.Entities;

namespace PdsTk.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Ticket> Tickets => Set<Ticket>();
}