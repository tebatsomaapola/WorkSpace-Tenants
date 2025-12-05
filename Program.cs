using System.Text;
using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WorkspaceTenants.Middleware;
using WorkspaceTenants.Common;
using WorkspaceTenants.Models;
using WorkspaceTenants.Data;
using WorkspaceTenants.Infrastructure;
using WorkspaceTenants.Services;
using WorkspaceTenants.Seed;
using WorkspaceTenants;

var builder = WebApplication.CreateBuilder(args);

// MVC, API and Swagger
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Entity Framework 
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Pricing rules 
builder.Services.AddScoped<IPricingRule>(sp => new PeakHoursRule());
builder.Services.AddScoped<IPricingRule>(sp => new LongBookingDiscountRule());
builder.Services.AddScoped<IPricingRule>(sp =>
    new TenantDiscountRule(tenantId => /* demo */ 0.05m));
builder.Services.AddScoped<PricingService>();

// App services
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();

// JWT
var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, ValidateAudience = true, ValidateIssuerSigningKey = true, ValidateLifetime = true,
            ValidIssuer = jwtIssuer, ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthRoles.TenantAdmin, p => p.RequireRole(AuthRoles.TenantAdmin));
    options.AddPolicy(AuthRoles.Member, p => p.RequireRole(AuthRoles.Member));
    options.AddPolicy(AuthRoles.ReadOnly, p => p.RequireRole(AuthRoles.ReadOnly));
});

var app = builder.Build();

// Migrate and seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    SeedData.Seed(db);
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<TenantContextMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
