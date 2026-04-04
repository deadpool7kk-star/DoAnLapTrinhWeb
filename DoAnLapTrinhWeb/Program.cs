using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DoAnLapTrinhWeb.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

// Fix lỗi thiếu dịch vụ gửi Email (tạo một bộ gửi Email giả cục bộ)
builder.Services.AddScoped<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, DummyEmailSender>();
builder.Services.AddTransient<Microsoft.AspNetCore.Authentication.IClaimsTransformation, AdminAccountTransformer>();

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

    var adminEmail = "Triadmin@gmail.com";
    var adminPassword = "Aa@123";

    var adminAccount = await userManager.FindByEmailAsync(adminEmail);
    if (adminAccount == null) {
        adminAccount = new ApplicationUser { 
            UserName = adminEmail, 
            Email = adminEmail,
            EmailConfirmed = true,
            FullName = "Administrator" // Gán FullName tránh lỗi null nếu migration yêu cầu
        };
        await userManager.CreateAsync(adminAccount, adminPassword); 
    }

    // Đảm bảo Role Admin tồn tại
    if (!await roleManager.RoleExistsAsync("Admin")) {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // ÉP QUYỀN: Xóa role hiện có và add lại để chắc chắn
    if (adminAccount != null) {
        var roles = await userManager.GetRolesAsync(adminAccount);
        if (!roles.Contains("Admin")) {
            await userManager.AddToRoleAsync(adminAccount, "Admin");
        }
    }

    // ------- TỰ ĐỘNG TẠO DANH MỤC MẪU --------
    if (!context.Categories.Any())
    {
        context.Categories.AddRange(new List<Category>
        {
            new Category { Name = "Khai vị" },
            new Category { Name = "Món chính" },
            new Category { Name = "Tráng miệng" },
            new Category { Name = "Đồ uống" }
        });
        await context.SaveChangesAsync();
    }
}
// -------------------------------------------------------------

app.Run();

public class AdminAccountTransformer : Microsoft.AspNetCore.Authentication.IClaimsTransformation
{
    public Task<System.Security.Claims.ClaimsPrincipal> TransformAsync(System.Security.Claims.ClaimsPrincipal principal)
    {
        var identity = principal.Identity as System.Security.Claims.ClaimsIdentity;
        if (identity != null && identity.IsAuthenticated && identity.Name != null && identity.Name.Equals("Triadmin@gmail.com", StringComparison.OrdinalIgnoreCase))
        {
            if (!principal.IsInRole("Admin"))
            {
                identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "Admin"));
            }
        }
        return Task.FromResult(principal);
    }
}
public class DummyEmailSender : Microsoft.AspNetCore.Identity.UI.Services.IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        return Task.CompletedTask;
    }
}
