using CommuniMerge.Library.Data;
using CommuniMerge.Library.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddAuthorization();

builder.Services.AddIdentityApiEndpoints<User>(options =>
{
    // Configure options here if needed
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<DataContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Map the identity API endpoints
app.MapGroup("/api/account").MapIdentityApi<User>().AllowAnonymous();

app.MapControllers();

app.Run();