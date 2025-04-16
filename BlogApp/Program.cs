using BlogApp.Data;
using BlogApp.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BlogApp.Data.Abstract; 
using BlogApp.Data.Concrete;
using AutoMapper;
using BlogApp.Services.Abstract;
using BlogApp.Services.Concrete;

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
    options.Lockout.MaxFailedAccessAttempts = 15;                     
    options.Lockout.AllowedForNewUsers = true;

    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;     
})
    .AddEntityFrameworkStores<BlogAppDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Services.ConfigureApplicationCookie(options => { /* Ayarlar */ });

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// NOT: UnitOfWork kullandýðýmýz için genellikle tek tek repository'leri kaydetmeye GEREK YOKTUR.
// Çünkü UnitOfWork zaten içinde repository'leri oluþturuyor.
// Ancak, eðer bir yerde doðrudan IBlogPostRepository'ye ihtiyaç duyarsanýz (UoW kullanmadan),
// o zaman aþaðýdaki gibi kayýtlarý da yapmanýz gerekebilir (ama genellikle UoW yeterlidir):
// builder.Services.AddScoped<IBlogPostRepository, EfCoreBlogPostRepository>();
// builder.Services.AddScoped<ICategoryRepository, EfCoreCategoryRepository>();
// builder.Services.AddScoped<ICommentRepository, EfCoreCommentRepository>(); // Yorumlar varsa
builder.Services.AddScoped<IBlogPostService, BlogPostService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICommentService, CommentService>();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true; 
    options.ExpireTimeSpan = TimeSpan.FromDays(30); 
    options.SlidingExpiration = true; 
    options.Cookie.SameSite = SameSiteMode.Lax; 

    options.LoginPath = "/Account/Login";       
    options.LogoutPath = "/Account/Logout";      
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddRazorPages();

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
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

app.Run();
