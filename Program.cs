using ConsultoriaApp.Data;
using ConsultoriaApp.Models;
using ConsultoriaApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Base de datos ────────────────────────────────────────────
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Identity ─────────────────────────────────────────────────
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();                    // ← esta línea es clave

// ── Servicios ────────────────────────────────────────────────
builder.Services.AddScoped<IProyectoService, ProyectoService>();
builder.Services.AddScoped<RegistroTiempoService>();

// ── MVC + Razor Pages ────────────────────────────────────────
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();   // ← necesario para Identity UI

var app = builder.Build();

// ── Seed roles y usuario admin ───────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var rol in new[] { "Administrador", "Lider", "Consultor" })
        if (!await roleManager.RoleExistsAsync(rol))
            await roleManager.CreateAsync(new IdentityRole(rol));

    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    if (await userMgr.FindByEmailAsync("admin@consultoria.com") == null)
    {
        var admin = new ApplicationUser
        {
            UserName = "admin@consultoria.com",
            Email = "admin@consultoria.com",
            FullName = "Administrador del Sistema",
            Rol = "Administrador",
            EmailConfirmed = true
        };
        await userMgr.CreateAsync(admin, "Admin@1234!");
        await userMgr.AddToRoleAsync(admin, "Administrador");
    }
}

// ── Pipeline ─────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
    app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();    // ← antes de UseAuthorization
app.UseAuthorization();

// ── Rutas ────────────────────────────────────────────────────
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Proyectos}/{action=Index}/{id?}");

app.MapRazorPages();        // ← registra las rutas de Identity UI

app.Run();