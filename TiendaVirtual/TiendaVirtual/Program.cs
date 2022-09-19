using Microsoft.EntityFrameworkCore;
using TiendaVirtual.Data;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();

//Aqui se especifica el tipo de base de datos que se va a utilizar en este caso SqlServer.
builder.Services.AddDbContext<DatosTienda>(o =>
{
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
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
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
