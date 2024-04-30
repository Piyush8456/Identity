using Custom_Identity.Data;
using Custom_Identity.Models;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddControllersWithViews();

builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    options.DisableColors = false;
    options.TimestampFormat = "[HH:mm:ss] ";
    options.LogToStandardErrorThreshold = LogLevel.Information; 
});

builder.Services.AddDbContextPool<ApplicationDbContext>(
    options => options.UseSqlServer(ConnectionString)
);

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("EditRolePolicy",
//        policy => policy.RequireClaim("Edit Role","true"));
//    options.AddPolicy("DeleteRolePolicy",
//    policy => policy.RequireClaim("Delete Role"));
//    options.AddPolicy("AddRolePolicy",
//    policy => policy.RequireClaim("Add Role"));
//});

//builder.Services.AddAuthorization(options =>
//{
//    foreach (var claim in ClaimsStore.AllClaims)
//    {
//        options.AddPolicy($"Claim-{claim.Type}-{claim.Value}", policy =>
//        {
//            policy.RequireClaim(claim.Type, claim.Value);
//        });
//    }
//});
builder.Services.AddAuthorization(Option =>
{
    Option.AddPolicy("EditRolePolicy",
                policy => policy.RequireClaim("Edit Role"));
});
builder.Services.AddAuthorization(Option =>
{
    Option.AddPolicy("CreateRolePolicy",
                policy => policy.RequireClaim("Create Role"));
});
builder.Services.AddAuthorization(Option =>
{
    Option.AddPolicy("ReadOnlyRolePolicy",
                policy => policy.RequireClaim("ReadOnly Role"));
});





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

    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseStatusCodePagesWithRedirects("/Error/Unauthorized");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
