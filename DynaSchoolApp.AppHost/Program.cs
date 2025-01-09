var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.DynaSchoolApp_ApiService>("apiservice");

builder.AddProject<Projects.DynaSchoolApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
