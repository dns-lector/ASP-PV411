using ASP_PV411.Data;
using ASP_PV411.Middleware;
using ASP_PV411.Services.Hash;
using ASP_PV411.Services.Kdf;
using ASP_PV411.Services.Random;
using ASP_PV411.Services.Salt;
using ASP_PV411.Services.Signature;
using ASP_PV411.Services.Storage;
using ASP_PV411.Services.Timestamp;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Реєструємо наші сервіси
// Зазвичай, реєструються не об'єкти, а класи (типи), об'єкти створюються
// автоматично при першому запиті на інжекцію (ледаче створення)

// різні життєві цикли
// builder.Services.AddSingleton<RandomService>();
// builder.Services.AddTransient<RandomService>();
// builder.Services.AddScoped<RandomService>();
// традиція - використання класів-розширень
builder.Services.AddRandom();
builder.Services.AddTimestamp();
builder.Services.AddHash();
builder.Services.AddSalt();
builder.Services.AddKdf();
builder.Services.AddSignature();
builder.Services.AddStorage();

// сесії - як інструмент збереження даних між запитами
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(15);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Підключення контексту даних
String connectionString = builder.Configuration.GetConnectionString("LocalDb")
    ?? throw new KeyNotFoundException("Connection String Configuration: key not found 'LocalDb'");
// Для EF-контекстів є свій метод реєстрації - AddDbContext
builder.Services.AddDbContext<DataContext>(
    options => options.UseSqlServer(connectionString));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseSession();
app.MapStaticAssets();


app.UseAuthSession();   // додатковий Middleware.


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
/* Принцип маршрутизації задається шаблоном у MapControllerRoute:
 * адреса (URL) розділяється на 3 частини по знаку "/"
 * 1. Контролер, якщо не зазначений, то приймається за Home
 * 2. Дія (action), за відсутності - Index
 * 3. Опціонально (?) - id
 * 
 * Робота маршрутизації: 
 * - здійснюється пошук контролера за назвою:
 *    [controller]Controller (за замовчанням HomeController)
 * - у контролері шукається метод з назвою [action] (Index)
 * - передається виконання на нього
 */

app.Run();
