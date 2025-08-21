var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.TooliRent>("TooliRent");

builder.Build().Run();
