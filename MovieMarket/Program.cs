using MovieMarket.DataAccess;
using MovieMart.Models;
using MovieMart.Repositories.IRepositories;
using MovieMart.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using MovieMarket.Repositories.IRepositories;
using MovieMarket.Repositories;
using MovieMarket.Services;
using System.Configuration;

namespace MovieMarket
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add MVC services to the application, allowing the use of the Model-View-Controller architecture
            builder.Services.AddControllersWithViews();

            // Add support for Razor Pages, allowing the creation of dynamic web pages using Razor Syntax
            builder.Services.AddRazorPages();

            // Register MovieMarketDbContext with Dependency Injection 
            // Configured to use SQL Server with the connection string from app settings.
            builder.Services.AddDbContext<MovieMarketDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            // Add identity services to the application
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(option =>
            {
                // Login settings:
                option.SignIn.RequireConfirmedEmail = true;

                // Password requirements:
                option.Password.RequireUppercase = true;
                option.Password.RequireNonAlphanumeric = true;
                option.Password.RequiredLength = 8;

                // User requirements:
                option.User.RequireUniqueEmail = true;

            }
            )
            // Bind the identity to the database using Entity Framework
            .AddEntityFrameworkStores<MovieMarketDbContext>()
            // Add default token providers to support operations like password reset and email confirmation
            .AddDefaultTokenProviders();


            // Register repository services with Dependency Injection (Scoped Lifetime) 
            // This ensures that a new instance is created per request, improving efficiency 
            // while maintaining consistency within a request's lifecycle.
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
            builder.Services.AddScoped<IEpisodeRepository, EpisodeRepository>();
            builder.Services.AddScoped<IMovieRepository, MovieRepository>();
            builder.Services.AddScoped<ISeasonRepository, SeasonRepository>();
            builder.Services.AddScoped<ITvSeriesRepository, TvSeriesRepository>();
            builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();

            // Email Sender .
            builder.Services.AddTransient<IEmailSender, EmailSender>();

            // Configure authentication services in the application
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




            var app = builder.Build();

            // Configure the HTTP Request Pipeline
            if (!app.Environment.IsDevelopment())
            {
                // Redirect errors to a dedicated page
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // Enable HSTS to improve security in production environments
                app.UseHsts();
            }

            // Force HTTPS on all requests
            app.UseHttpsRedirection();

            // Enable loading of static files such as CSS and JavaScript
            app.UseStaticFiles();

            // Enable routing requests to the appropriate paths
            app.UseRouting();

            // Enable Authentication
            // Ensures that incoming requests pass user identity verification before accessing protected resources           
            app.UseAuthentication();

            // Enable Authorization
            // Determines whether the authenticated user has the required permissions to access certain resources           
            app.UseAuthorization();

            // Set paths for Razor pages within the application and enable routing for them
            app.MapRazorPages();

            // Configuring endpoint routing for different application areas
            app.UseEndpoints(endpoints =>
            {
                // Route for the #Admin area
                endpoints.MapControllerRoute(
                name: "Admin",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );

                // Route for the #Customer area
                endpoints.MapControllerRoute(
                name: "Customer",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );

                // Set the default path for the application
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}",
                    defaults: new { area = "Customer" } // Define the default region
                );

            });

            // Run the application
            app.Run();
        }
    }
}
