using AutoMapper;
using BLL.Utilities;
using DAL.Repositories;
using DAL.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc.Razor;
using Stripe;

namespace Movie_Market
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);



            #region Add services to the container

            builder.Services.AddControllersWithViews()
                .AddRazorOptions(options =>
                {
                    options.AreaViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
                    options.AreaViewLocationFormats.Add("/Areas/{2}/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
                    options.ViewLocationFormats.Add("/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
                    options.ViewLocationFormats.Add("/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
                });

            #endregion


            #region Register ApplicationDbContext with Dependency Injection 
            // Configured to use SQL Server with the connection string from app settings.
            builder.Services.AddDbContext<ApplicationdbContext>(option =>
                        option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            #endregion


            #region Locked Out
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            });
            #endregion


            #region Register Services

            // Repository Services
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository > ();

            builder.Services.AddScoped<IFileService, BLL.Services.Implementations.FileService>();
            builder.Services.AddTransient<IEmailService, EmailService>();
            builder.Services.AddScoped<IAuditService, AuditService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IMovieService, MovieService>();
            builder.Services.AddScoped<ICinemaService, CinemaService>();
            builder.Services.AddScoped<ICharacterService, CharacterService>();
            builder.Services.AddScoped<ISpecialService, SpecialService>();
            builder.Services.AddScoped<ITvSeriesService, TvSeriesService>();
            builder.Services.AddScoped<ISeasonService, SeasonService>();
            builder.Services.AddScoped<IEpisodeService, EpisodeService>();
            builder.Services.AddScoped<ICharacterTvSeriesService, CharacterTvSeriesService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<IProfileService, ProfileService>();
            builder.Services.AddScoped<IUserService, UserService>();


            builder.Services.AddScoped<ISearchService, SearchService>();
            builder.Services.AddScoped<ISubscriberService, SubscriberService>();
            builder.Services.AddScoped<INewsletterService, NewsletterService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            builder.Services.AddMemoryCache();


            #endregion


            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
            });


            #region Permissions Policy

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("BlockUserPolicy", policy =>
                    policy.RequireAssertion(context =>
                    {
                        var httpContext = context.Resource as HttpContext;
                        var userId = httpContext.Request.RouteValues["id"]?.ToString();

                        if (httpContext.User.Identity.Name == userId)
                            return false;

                        if (httpContext.User.IsInRole("SuperAdmin"))
                            return true;

                        var userService = httpContext.RequestServices.GetRequiredService<IUserService>();
                        var user = userService.GetUserDetailsAsync(Guid.Parse(userId)).GetAwaiter().GetResult();

                        return user?.UserType == "Customer";
                    }));

                options.AddPolicy("ChangeRolePolicy", policy =>
                    policy.RequireRole("SuperAdmin"));
            });

            #endregion


            #region Identity Configuration
            builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-.@";
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
            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];
            #endregion


            #region Configure the HTTP request 

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/GloubalUsing/Common/Error");
                app.UseStatusCodePagesWithReExecute("/GloubalUsing/Common/Error", "?statusCode={0}");

                app.UseHsts();

                app.Use(async (context, next) =>
                {
                    await next();
                    if (context.Response.StatusCode >= 400)
                    {
                        context.Items["originalPath"] = context.Request.Path;
                    }
                });
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            #endregion


            #region Setting up top-level routes

            app.UseRouting();

            app.MapControllerRoute(
            name: "Admin",
            pattern: "{area:exists}/{controller=Home}/{action=Dashboard}/{id?}"
            );

            app.MapControllerRoute(
            name: "Customer",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
            );


            app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}",
            defaults: new { area = "Customer" }
            );

            app.MapAreaControllerRoute(
                name: "Identity",
                areaName: "Identity",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.MapAreaControllerRoute(
                name: "GloubalUsing",
                areaName: "GloubalUsing",
                pattern: "GloubalUsing/{controller=Common}/{action=Error}/{id?}");

            app.MapFallbackToController("NotFound", "Common", "GloubalUsing");

            #endregion


            app.Run();

        }
    }
}
