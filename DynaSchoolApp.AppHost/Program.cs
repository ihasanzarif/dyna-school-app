var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.DynaSchoolApp_ApiService>("apiservice");

builder.Build().Run();
