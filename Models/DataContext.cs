using Microsoft.EntityFrameworkCore;

namespace Backend.Models;
public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<Mask> Masks { get; set; }
    public DbSet<MaskType> MaskTypes { get; set; }
    public DbSet<OpeningHour> OpeningHours { get; set; }
    public DbSet<Pharmacy> Pharmacies { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<User> Users { get; set; }
}