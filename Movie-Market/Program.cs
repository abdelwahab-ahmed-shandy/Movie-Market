
namespace Movie_Market
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Add services to the container
            builder.Services.AddControllersWithViews();
            #endregion


            #region Register ApplicationDbContext with Dependency Injection 
            // Configured to use SQL Server with the connection string from app settings.
            builder.Services.AddDbContext<ApplicationdbContext>(option =>
                        option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            #endregion


            #region Register repository services with Dependency Injection (Scoped Lifetime) 

            builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;

                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationdbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddTransient<IEmailService, EmailService>();
            builder.Services.AddScoped<IAuditService, AuditService>();
            builder.Services.AddScoped<IAccountService, AccountService>();

            builder.Services.AddHttpContextAccessor();

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
                googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"]
                    ?? throw new InvalidOperationException("Google ClientId is not configured.");

                // Set the client secret key for Google authentication from the application settings 
                googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]
                    ?? throw new InvalidOperationException("Google ClientSecret is not configured.");
            });

            #endregion



            #region Confige Stripe Setting
            //builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
            //StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];
            #endregion



            #region Configure the HTTP request 

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            #endregion


            #region Authentication & Authorization
            // Enable Authentication
            // Ensures that incoming requests pass user identity verification before accessing protected resources           
            app.UseAuthentication();

            // Enable Authorization
            // Determines whether the authenticated user has the required permissions to access certain resources           
            app.UseAuthorization();

            app.MapStaticAssets();
            app.UseStaticFiles();

            #endregion


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
