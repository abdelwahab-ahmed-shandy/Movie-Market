using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MovieMart.Models;

namespace DAL.Context
{
    public class ApplicationdbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {


        #region Entities Definition :

        public virtual DbSet<Cinema> Cinemas { get; set; }
        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<TvSeries> TvSeries { get; set; }
        public virtual DbSet<Season> Seasons { get; set; }
        public virtual DbSet<Episode> Episodes { get; set; }
        public virtual DbSet<Special> Specials { get; set; }
        public virtual DbSet<MovieSpecial> MovieSpecials { get; set; }
        public virtual DbSet<Character> Characters { get; set; }
        public virtual DbSet<CharacterMovie> CharacterMovies { get; set; }
        public virtual DbSet<CharacterTvSeries> CharacterTvSeries { get; set; }
        public virtual DbSet<CinemaMovie> CinemaMovies { get; set; }
        public virtual DbSet<Subscriber> Subscribers { get; set; }
        public virtual DbSet<SentEmail> SentEmails { get; set; }
        public virtual DbSet<AuditRecord> AuditRecords { get; set; }

        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="MovieMarketDbContext"/> class
        /// with the specified database connection options.
        /// </summary>
        /// <param name="options">Configuration settings for the database connection.</param>
        public ApplicationdbContext(DbContextOptions<ApplicationdbContext> options)
            : base(options)
        {
        }


        /// <summary>
        /// Create relationships and table settings when creating the model.
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseLazyLoadingProxies()
                    .UseSqlServer("DefaultConnection");
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Filter automatically deleted records (Soft Delete)

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseModel).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var body = Expression.Equal(
                        Expression.Property(parameter, nameof(BaseModel.IsDeleted)),
                        Expression.Constant(false));
                    var lambda = Expression.Lambda(body, parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }

            #endregion


            #region Define the relationship

            modelBuilder.Entity<AuditRecord>()
                .HasOne(a => a.User)
                .WithMany(u => u.AuditRecords)
                .HasForeignKey(a => a.UserId);

            #endregion




        }

    }

}
