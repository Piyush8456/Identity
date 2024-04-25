using Custom_Identity.Data;
using Custom_Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

var builder = WebApplication.CreateBuilder(args);

var ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddControllersWithViews();

builder.Logging.ClearProviders(); 
builder.Logging.AddConsole(options =>
{
    options.DisableColors = false; 
    options.TimestampFormat = "[HH:mm:ss] "; 
    options.LogToStandardErrorThreshold = LogLevel.Information; // Show logs of Information level or higher
});
builder.Services.AddDbContextPool<ApplicationDbContext>(
    options => options.UseSqlServer(ConnectionString)
);
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
    options =>
    {
        //options.Password.RequiredUniqueChars = 0;
        //options.Password.RequireUppercase = false;
        //options.Password.RequiredLength = 8;
        //options.Password.RequireNonAlphanumeric = false;
        //options.Password.RequireLowercase = false;
    }
)
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

var app = builder.Build();

// Seed roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roleNames = { "Admin", "Manager", "Member" };

    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    //    var adminuser = await usermanager.findbyemailasync("admin@example.com");

    //    if (adminuser != null)
    //    {
    //        await usermanager.addtoroleasync(adminuser, "admin");
    //    }
    //}

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
}
