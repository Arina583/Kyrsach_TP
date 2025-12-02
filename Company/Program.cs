using Microsoft.EntityFrameworkCore;
using Company.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbClassContext>(options =>
       options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 44))));

// Add services to the container.
builder.Services.AddControllersWithViews();

// Добавление сервисов аутентификации
builder.Services.AddAuthentication("Cookies") // Указываем схему аутентификации по умолчанию
   .AddCookie("Cookies", options => // Конфигурируем опции для CookieAuthentication
   {
       options.LoginPath = "/Account/Login"; // Указываем путь к странице входа
       options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // время жизни куки
       options.SlidingExpiration = true; // Продлевать куки при каждом запросе
   });

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
