var builder = WebApplication.CreateBuilder(args);

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
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

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
