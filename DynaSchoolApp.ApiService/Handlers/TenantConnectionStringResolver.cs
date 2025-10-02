namespace DynaSchoolApp.ApiService.Handlers
{
    public interface IConnectionStringResolver
    {
        string GetConnectionString();
    }

    public class TenantConnectionStringResolver : IConnectionStringResolver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;

        public TenantConnectionStringResolver(IHttpContextAccessor httpContextAccessor, IConfiguration config)
        {
            _httpContextAccessor = httpContextAccessor;
            _config = config;
        }

        public string GetConnectionString()
        {
            string tenant = "";
            var username = _httpContextAccessor.HttpContext?.User.FindFirst("username")?.Value;
            if (!string.IsNullOrEmpty(username) && username.Contains("@"))
            {
                tenant = username.Split('@')[1] +"SchoolDB"; // pirerbag
            }
            else
            {
                tenant = "DefaultSchoolDB";
            }

                var baseConn = _config.GetConnectionString("DefaultConnection");

            return baseConn.Replace("{tenant}", tenant);
        }
    }
}
