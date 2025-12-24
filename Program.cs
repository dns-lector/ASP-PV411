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
// Comment from GitHub
var builder = WebApplication.CreateBuilder(args);
// Comment from VS
// Comment from GitHub
// Comment from VS

// Add services to the container.
builder.Services.AddControllersWithViews();

// Ðåºñòðóºìî íàø³ ñåðâ³ñè
// Çàçâè÷àé, ðåºñòðóþòüñÿ íå îá'ºêòè, à êëàñè (òèïè), îá'ºêòè ñòâîðþþòüñÿ
// àâòîìàòè÷íî ïðè ïåðøîìó çàïèò³ íà ³íæåêö³þ (ëåäà÷å ñòâîðåííÿ)

// ð³çí³ æèòòºâ³ öèêëè
// builder.Services.AddSingleton<RandomService>();
// builder.Services.AddTransient<RandomService>();
// builder.Services.AddScoped<RandomService>();
// òðàäèö³ÿ - âèêîðèñòàííÿ êëàñ³â-ðîçøèðåíü
builder.Services.AddRandom();
builder.Services.AddTimestamp();
builder.Services.AddHash();
builder.Services.AddSalt();
builder.Services.AddKdf();
builder.Services.AddSignature();
builder.Services.AddStorage();

// ñåñ³¿ - ÿê ³íñòðóìåíò çáåðåæåííÿ äàíèõ ì³æ çàïèòàìè
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(15);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Ï³äêëþ÷åííÿ êîíòåêñòó äàíèõ
String connectionString = builder.Configuration.GetConnectionString("LocalDb")
    ?? throw new KeyNotFoundException("Connection String Configuration: key not found 'LocalDb'");
// Äëÿ EF-êîíòåêñò³â º ñâ³é ìåòîä ðåºñòðàö³¿ - AddDbContext
builder.Services.AddDbContext<DataContext>(
    options => options.UseSqlServer(connectionString));


builder.Services.AddCors(
    options => options.AddDefaultPolicy( 
        policy => policy.AllowAnyOrigin().AllowAnyHeader()
));

var app = builder.Build();

var migrationTask = app
    .Services
    .CreateScope()
    .ServiceProvider
    .GetRequiredService<DataContext>()
    .Database
    .MigrateAsync();  // Apply migrations

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();
app.UseAuthorization();
app.UseSession();
app.MapStaticAssets();

app.UseAuthSession();   // äîäàòêîâèé Middleware.

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
/* Ïðèíöèï ìàðøðóòèçàö³¿ çàäàºòüñÿ øàáëîíîì ó MapControllerRoute:
 * àäðåñà (URL) ðîçä³ëÿºòüñÿ íà 3 ÷àñòèíè ïî çíàêó "/"
 * 1. Êîíòðîëåð, ÿêùî íå çàçíà÷åíèé, òî ïðèéìàºòüñÿ çà Home
 * 2. Ä³ÿ (action), çà â³äñóòíîñò³ - Index
 * 3. Îïö³îíàëüíî (?) - id
 * 
 * Ðîáîòà ìàðøðóòèçàö³¿: 
 * - çä³éñíþºòüñÿ ïîøóê êîíòðîëåðà çà íàçâîþ:
 *    [controller]Controller (çà çàìîâ÷àííÿì HomeController)
 * - ó êîíòðîëåð³ øóêàºòüñÿ ìåòîä ç íàçâîþ [action] (Index)
 * - ïåðåäàºòüñÿ âèêîíàííÿ íà íüîãî
 */

await migrationTask;
app.Run();
