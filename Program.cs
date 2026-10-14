using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HotelManager.Data;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDefaultIdentity<IdentityUser>(options => 
    {
        options.SignIn.RequireConfirmedAccount = false;
        //user admin/ Pass admin
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 4;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();
// --- DATA SEEDER: Create Roles and Admin Account ---
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // 1. Create the Admin and Client roles
    string[] roleNames = { "Admin", "Client" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
    // Create Admin account where UserName and Email perfectly match
    string adminEmail = "boss@hotel.com";
    string adminPassword = "admin"; 
    
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        var newAdmin = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        
        var result = await userManager.CreateAsync(newAdmin, adminPassword);
        if (result.Succeeded)
        {
            // Assign Admin role
            await userManager.AddToRoleAsync(newAdmin, "Admin");
        }
    }
    // Create Client
    string clientEmail = "client@hotel.com";
    string clientPassword = "admin"; 
    
    var clientUser = await userManager.FindByEmailAsync(clientEmail);
    if (clientUser == null)
    {
        var newClient = new IdentityUser
        {
            UserName = clientEmail,
            Email = clientEmail,
            EmailConfirmed = true
        };
        
        var result = await userManager.CreateAsync(newClient, clientPassword);
        if (result.Succeeded)
        {
            // Assign strictly to the Client role
            await userManager.AddToRoleAsync(newClient, "Client");
        }
    }
}
app.Run();
