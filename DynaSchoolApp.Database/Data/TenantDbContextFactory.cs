using Microsoft.EntityFrameworkCore;

namespace DynaSchoolApp.Database.Data;

public class TenantDbContextFactory
{
    public TenantDbContextFactory()
    {
        // Parameterless constructor is sufficient.
    }

    /// <summary>
    /// Creates a new AppDbContext instance configured for a specific connection string.
    /// </summary>
    /// <param name="connectionString">The dynamic connection string resolved by the TenantResolver during login.</param>
    /// <returns>A new AppDbContext instance.</returns>
    public AppDbContext Create(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // Configure the DbContext to use the specific SQL Server connection string
        optionsBuilder.UseSqlServer(connectionString);

        // Manually create the context, passing the options
        return new AppDbContext(optionsBuilder.Options);
    }
}
