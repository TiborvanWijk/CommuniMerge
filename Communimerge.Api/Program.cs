using Communimerge.Api;
using CommuniMerge.Library.Data;
using CommuniMerge.Library.Loggers.Interfaces;
using CommuniMerge.Library.Loggers;
using CommuniMerge.Library.Models;
using CommuniMerge.Library.Repositories;
using CommuniMerge.Library.Repositories.Interfaces;
using CommuniMerge.Library.Services;
using CommuniMerge.Library.Services.Interfaces;
using CommuniMerge.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CommuniMerge.ApiServices.Interfaces;
using CommuniMerge.ApiServices;
using CommuniMerge.CookieRepositories.Interfaces;
using CommuniMerge.CookieRepositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<ICustomLogger, LogService>();
builder.Services.AddScoped<IFileUploadRepository, FileUploadRepository>();

builder.Services.AddCors();

builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));

builder.Services.AddSingleton<TokenSettings>();
builder.Services.AddSingleton<TokenService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    );

builder.Services.AddDbContext<LogContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LogConnection"))
    );

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<AuthorizationMiddleware>();


app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("https://localhost:7286"));

app.UseHttpsRedirection();
app.UseAuthorization();


app.MapControllers();

app.Run();