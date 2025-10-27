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
/* ������� ������������� �������� �������� � MapControllerRoute:
 * ������ (URL) ����������� �� 3 ������� �� ����� "/"
 * 1. ���������, ���� �� ����������, �� ���������� �� Home
 * 2. ĳ� (action), �� ��������� - Index
 * 3. ����������� (?) - id
 * 
 * ������ �������������: 
 * - ����������� ����� ���������� �� ������:
 *    [controller]Controller (�� ����������� HomeController)
 * - � ��������� �������� ����� � ������ [action] (Index)
 * - ���������� ��������� �� �����
 */

app.Run();
