using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TiendaVirtual.Ayudadores;
using TiendaVirtual.Data;
using TiendaVirtual.Data.Entities;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();

//Aqui se especifica el tipo de base de datos que se va a utilizar en este caso SqlServer.
builder.Services.AddDbContext<DatosTienda>(o =>
{
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<Usuario, IdentityRole>(cfg =>
{
    cfg.User.RequireUniqueEmail = true;
    cfg.Password.RequireDigit = false;
    cfg.Password.RequiredUniqueChars = 0;
    cfg.Password.RequireLowercase = false;
    cfg.Password.RequireNonAlphanumeric = false;
    cfg.Password.RequireUppercase = false;
}).AddEntityFrameworkStores<DatosTienda>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Cuenta/NoAutorizado";
    options.AccessDeniedPath = "/Cuenta/NoAutorizado";
});


//Este transient lo que me permite es crear la base de datos una unica vez al ejecutar el programa.
builder.Services.AddTransient<PrincipalDb>();

//Este scoped lo que me ayuda es que inyecta lo que le estoy pasando cada que se llama y
//se destruye cuando se utiliza.
builder.Services.AddScoped<IayudasUsuario, AyudasUsuario>();

var app = builder.Build();
principalData();

//Con este metodo estoy inyectando la conexión a la base de datos con el principalDb
void principalData()
{
   IServiceScopeFactory? scopeFactory = app.Services.GetService<IServiceScopeFactory>();

    using (IServiceScope? scope = scopeFactory.CreateScope())
    {
        PrincipalDb? service = scope.ServiceProvider.GetService<PrincipalDb>();
        service.PrincipalAsync().Wait();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseStatusCodePagesWithReExecute("/error/{0}");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
