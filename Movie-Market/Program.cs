using BLL.Interfaces;
using BLL.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

namespace Movie_Market
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();


            #region Register repository services with Dependency Injection (Scoped Lifetime) 

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IAuditService, AuditService>();

            #endregion



            #region Email Sender
            builder.Services.AddTransient<IEmailService, EmailService>();
            #endregion



            #region Configure authentication services in the application

            builder.Services.AddAuthentication(options =>
            {
                // Specify the default authentication system using cookies
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                // Specify Google as the authentication method when attempting to log in (when attempting to log in)
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            // Add authentication using cookies to store session data after login
            .AddCookie()
            // Add authentication via Google OAuth
            .AddGoogle(googleOptions =>
            {
                // Set the client ID for Google authentication from the application settings
                googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];

                // Set the client secret key for Google authentication from the application settings 
                googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
            });

            #endregion



            #region Confige Stripe Setting
            //builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
            //StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];
            #endregion


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();


            #region Authentication & Authorization
            // Enable Authentication
            // Ensures that incoming requests pass user identity verification before accessing protected resources           
            app.UseAuthentication();

            // Enable Authorization
            // Determines whether the authenticated user has the required permissions to access certain resources           
            app.UseAuthorization();

            #endregion

            app.UseStaticFiles();
            app.MapStaticAssets();


            #region Setting up top-level routes

            // Enable routing requests to the appropriate paths
            app.UseRouting();

            // Route for the "Admin" area
            app.MapControllerRoute(
            name: "Admin",
            pattern: "{area:exists}/{controller=Home}/{action=Dashboard}/{id?}"
            );

            // Route for the "Customer" area
            app.MapControllerRoute(
            name: "Customer",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
            );

            // Default Route
            // If no area is specified, the area will be assumed to be "Customer"
            app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}",
            defaults: new { area = "Customer" }
            );

            #endregion



            app.Run();

        }
    }
}
