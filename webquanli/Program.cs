using Microsoft.EntityFrameworkCore;
using webquanli.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSession();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.MapControllerRoute(
name: "default",
pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();