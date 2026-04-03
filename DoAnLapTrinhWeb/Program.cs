using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DoAnLapTrinhWeb.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
})

.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// Fix lỗi thiếu dịch vụ gửi Email (tạo một bộ gửi Email giả cục bộ)
builder.Services.AddScoped<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, DummyEmailSender>();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// ------- BƯỚC 2: TỰ ĐỘNG TẠO ROLE VÀ ADMIN (SEED DATA) --------
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate(); // Tự động chạy Migration tạo DB và bảng nếu chưa tồn tại

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    if (!await roleManager.RoleExistsAsync("Admin")) {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }
    if (!await roleManager.RoleExistsAsync("Customer")) {
        await roleManager.CreateAsync(new IdentityRole("Customer"));
    }

    var adminAccount = await userManager.FindByEmailAsync("admin@vermeil.vn");
    if (adminAccount == null) {
        var newAdmin = new ApplicationUser { 
            UserName = "admin@vermeil.vn", 
            Email = "admin@vermeil.vn",
            EmailConfirmed = true 
        };
        await userManager.CreateAsync(newAdmin, "123"); 
        await userManager.AddToRoleAsync(newAdmin, "Admin");
    }
}
// -------------------------------------------------------------

app.Run();

public class DummyEmailSender : Microsoft.AspNetCore.Identity.UI.Services.IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        return Task.CompletedTask;
    }
}
