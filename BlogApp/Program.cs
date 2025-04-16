using BlogApp.Data; // namespace'i kontrol edin
using BlogApp.Entities; // namespace'i kontrol edin
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BlogApp.Data.Abstract; // namespace'i kontrol edin
 // namespace'i kontrol edin (varsa)
using BlogApp.Services.Abstract; // namespace'i kontrol edin
using BlogApp.Services.Concrete;
using BlogApp.Data.Concrete; // namespace'i kontrol edin

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json.");

builder.Services.AddDbContext<BlogAppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5; 
    options.Lockout.AllowedForNewUsers = true;

    // Kullanıcı Ayarları
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    // Oturum Açma Ayarları
    options.SignIn.RequireConfirmedAccount = false; 
})
    .AddEntityFrameworkStores<BlogAppDbContext>()
    .AddDefaultTokenProviders() 
    .AddDefaultUI(); 

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
  
    options.LoginPath = "/Identity/Account/Login"; // Identity Area kullanıyorsanız path böyle olmalı
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    // Eğer Identity Area kullanmıyorsanız /Account/Login vb. olabilir. Scaffolding genellikle Area kullanır.
});


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


builder.Services.AddScoped<IBlogPostService, BlogPostService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICommentService, CommentService>();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); 


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) // Development kontrolü düzeltildi
{
    //app.UseMigrationsEndPoint(); // Db Hataları için
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
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

app.Run();