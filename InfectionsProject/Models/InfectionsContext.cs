using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace InfectionsProject.Models; 

public class InfectionsContext : DbContext {
    private IHostingEnvironment _appHost;

    public InfectionsContext(DbContextOptions<InfectionsContext> options, IHostingEnvironment appHost) : base(options) {
        _appHost = appHost;
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options) {
        // Connection to DB should be handled much better - not using deprecated AppHost method, loading path from config, ...
        var dbPath = _appHost.ContentRootPath + "/../../../Infections.db";
        options.UseSqlite($"Data Source={dbPath}");
    }
    
    public DbSet<Infection> Infections { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder
            .Entity<Infection>()
            .Property(e => e.InfectionStatus)
            .HasConversion(
                v => v.ToString(),
                v => (InfectionStatus)Enum.Parse(typeof(InfectionStatus), v));
    }
}

