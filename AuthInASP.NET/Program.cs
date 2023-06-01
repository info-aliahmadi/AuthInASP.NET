using AuthInASP.NET.Cache;
using AuthInASP.NET.Endpoints;
using AuthInASP.NET.Infrastructure;
using AuthInASP.NET.Infrastructure.Data;
using AuthInASP.NET.Service.Interfaces;
using AuthInASP.NET.Service;
using AuthInASP.NET.Infrastructure.Repository;
using Hydra.Infrastructure.Data;
using AuthInASP.NET.Infrastructure.Localization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddlocalizationConfig();


builder.Services.AddCacheProvider(builder.Configuration);

builder.Services.AddDbContextConfig(builder.Configuration);

builder.Services.AddIdentityConfig(builder.Configuration);

builder.Services.AddScoped<ICommandRepository, CommandRepository>();
builder.Services.AddScoped<IQueryRepository, QueryRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPermissionChecker, PermissionChecker>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseLocalization();

// set the all endpoints
app.MapEndpoints();

app.Run();
