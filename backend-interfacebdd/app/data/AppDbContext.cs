using app.models;
using Microsoft.EntityFrameworkCore;
namespace app.data;

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
    // Table Files
    public DbSet<MigrationModel> MigrationModels { get; set; }
}