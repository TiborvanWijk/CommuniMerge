using CommuniMerge.Hubs;
using CommuniMerge.Library.Data;
using CommuniMerge.Library.Middleware;
using CommuniMerge.Library.Models;
using CommuniMerge.Library.Repositories.Interfaces;
using CommuniMerge.Library.Repositories;
using CommuniMerge.Library.Services;
using CommuniMerge.Library.Services.Interfaces;
using CommuniMerge.Middleware;
using Microsoft.EntityFrameworkCore;
using CommuniMerge.CookieRepositories.Interfaces;
using CommuniMerge.CookieRepositories;
using CommuniMerge.ApiServices.Interfaces;
using CommuniMerge.ApiServices;
using CommuniMerge.Library.Loggers.Interfaces;
using CommuniMerge.Library.Loggers;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<ICookieRepository, CookieRepository>();
builder.Services.AddScoped<ICustomLogger, LogService>();
builder.Services.AddScoped<IApiService, ApiService>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();


builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
}
    );
builder.Services.AddDbContext<LogContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LogConnection"))
    );
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseMiddleware<AuthorizationMiddleware>();
app.UseAuthorization();

using(var scope = app.Services.CreateScope())
{
    var datacontext = scope.ServiceProvider.GetRequiredService<DataContext>();

    var user = new User()
    {

    };

}

app.MapHub<FriendHub>("/friendHub");
app.MapHub<ChatHub>("/chatHub");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
