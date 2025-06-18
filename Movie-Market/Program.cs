using BLL.Services.Implementations;
using AutoMapper;
using BLL.Services.Implementations.Admin;
using BLL.Services.Implementations.Customer;
using BLL.Services.Interfaces.Admin;
using BLL.Services.Interfaces.Customer;
using DAL.Repositories;
using DAL.Repositories.IRepositories;

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


            #region Register Services

            // Admin Services
            builder.Services.AddScoped<IAdminCategoryService, AdminCategoryService>();
            builder.Services.AddScoped<IAdminMovieService, AdminMovieService>();
            builder.Services.AddScoped<IAdminCinemaService, AdminCinemaService>();
            builder.Services.AddScoped<IAdminCharacterService, AdminCharacterService>();
            builder.Services.AddScoped<IAdminSpecialService, AdminSpecialService>();
            builder.Services.AddScoped<IAdminTvSeriesService, AdminTvSeriesService>();
            builder.Services.AddScoped<IAdminSeasonService, AdminSeasonService>();
            builder.Services.AddScoped<IAdminEpisodeService, AdminEpisodeService>();
            builder.Services.AddScoped<IAdminCharacterTvSeriesService, AdminCharacterTvSeriesService>();
            builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();

            // Customer Services
            builder.Services.AddScoped<ICustomerCategoryService, CustomerCategoryService>();
            builder.Services.AddScoped<ICustomerMovieService, CustomerMovieService>();
            builder.Services.AddScoped<ICustomerCinemaService, CustomerCinemaService>();
            builder.Services.AddScoped<ICustomerTvSeriesService, CustomerTvSeriesService>();
            builder.Services.AddScoped<ICustomerCharacterService, CustomerCharacterService>();
            builder.Services.AddScoped<ICustomerSpecialService, CustomerSpecialService>();
            builder.Services.AddScoped<ICustomerSeasonService, CustomerSeasonService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();

            // Repository Services
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddTransient<IEmailService, EmailService>();
            builder.Services.AddScoped<IAuditService, AuditService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<ISearchService, SearchService>();
            builder.Services.AddScoped<ISubscriberService, SubscriberService>();
            builder.Services.AddScoped<INewsletterService, NewsletterService>();
            builder.Services.AddMemoryCache();


            #endregion


            #region Identity Configuration
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
            #endregion



            #region Authentication Configuration

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"]
                    ?? throw new InvalidOperationException("Google ClientId is not configured.");

                googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]
                    ?? throw new InvalidOperationException("Google ClientSecret is not configured.");
            });


            builder.Services.AddHttpContextAccessor();

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
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

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
