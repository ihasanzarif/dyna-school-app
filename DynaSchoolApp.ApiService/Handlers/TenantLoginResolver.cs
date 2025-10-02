using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynaSchoolApp.ApiService.Handlers
{
    /// <summary>
    /// Stores the results of the tenant resolution performed during login.
    /// </summary>
    public record TenantLoginResolutionResult(
        string Username,
        string TenantId,
        string ConnectionString);

    /// <summary>
    /// Defines the contract for resolving tenant information during the unauthenticated login process.
    /// </summary>
    public interface ITenantLoginResolver
    {
        /// <summary>
        /// Parses the full username (e.g., "zarif@pirerbag") and resolves the tenant-specific connection details.
        /// </summary>
        /// <param name="fullUsername">The username submitted during login, expected format: user@tenantId.</param>
        /// <returns>A TenantLoginResolutionResult containing the clean username, tenant ID, and resolved connection string.</returns>
        TenantLoginResolutionResult ResolveTenant(string fullUsername);
    }

    /// <summary>
    /// Implementation of ITenantLoginResolver that builds the dynamic connection string.
    /// </summary>
    public class TenantLoginResolver : ITenantLoginResolver
    {
        private const string ActiveTemplateKeyConfigPath = "MultiTenancy:ActiveTemplateKey";
        private readonly IConfiguration _config;

        public TenantLoginResolver(IConfiguration config)
        {
            _config = config;
        }

        public TenantLoginResolutionResult ResolveTenant(string fullUsername)
        {
            if(string.IsNullOrWhiteSpace(fullUsername) || !fullUsername.Contains('@'))
            {
                throw new ArgumentException("Invalid username format. Expected: user@tenantId", nameof(fullUsername));
            }

            var parts = fullUsername.Split('@');
            var cleanUsername = parts[0].Trim();
            var tenantId = parts[1].Trim(); // e.g., 'pirerbag'

            // 1. Get the name of the currently active connection string template from configuration.
            var activeTemplateName = _config.GetValue<string>(ActiveTemplateKeyConfigPath);

            if (string.IsNullOrEmpty(activeTemplateName))
            {
                throw new InvalidOperationException($"Configuration setting '{ActiveTemplateKeyConfigPath}' is missing or empty. Cannot determine active connection template.");
            }

            // 2. Use the active name to fetch the actual template string from the ConnectionStrings section.
            var connectionTemplate = _config.GetConnectionString(activeTemplateName);

            if (string.IsNullOrEmpty(connectionTemplate))
            {
                throw new InvalidOperationException($"Configuration missing ConnectionString '{activeTemplateName}'. Check your appsettings.json.");
            }

            // Construct the dynamic database name based on your convention: {tenantId}SchoolDB
            var dynamicDatabaseName = $"{tenantId}SchoolDB";

            // Replace the placeholder in the template to get the specific connection string
            var resolvedConnectionString = connectionTemplate.Replace("{tenantDB}", dynamicDatabaseName);

            return new TenantLoginResolutionResult(
                Username: cleanUsername,
                TenantId: tenantId,
                ConnectionString: resolvedConnectionString
            );
        }
    }
}
