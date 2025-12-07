using Company.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Добавьте сервисы аутентификации и авторизации
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Укажите путь к странице входа
        options.AccessDeniedPath = "/Account/AccessDenied"; // Укажите путь к странице отказа в доступе
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Устанавливаем SecurePolicy
    });

builder.Services.AddAuthorization(); // Добавьте авторизацию

builder.Services.AddDbContext<DbClassContext>(options =>
       options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 44))));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Включаем обработку статических файлов

app.UseRouting();

// Добавляем middleware аутентификации и авторизации.  Важно, чтобы они были вызваны после UseRouting и до UseEndpoints
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
   name: "default",
   pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
