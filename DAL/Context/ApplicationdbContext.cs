using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
                    .UseSqlServer("YourConnectionString");

            }
        }



        /// <summary>
        /// Configures the model for the database context, including global filters, initial data seeding, relationships, and indexes.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureGlobalFilters(modelBuilder);
            ConfigureRelationships(modelBuilder);
            ConfigureIndexes(modelBuilder);
        }



        #region Private Configuration Methods
        private void ConfigureGlobalFilters(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseModel).IsAssignableFrom(entityType.ClrType) &&
                    entityType.BaseType == null)
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var body = Expression.Equal(
                        Expression.Property(parameter, nameof(BaseModel.IsDeleted)),
                        Expression.Constant(false));
                    var lambda = Expression.Lambda(body, parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
        }

        private void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            // AuditRecord ↔ User
            modelBuilder.Entity<AuditRecord>()
                .HasOne(a => a.User)
                .WithMany(u => u.AuditRecords)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Movie ↔ Category
            modelBuilder.Entity<Movie>()
                .HasOne(m => m.Category)
                .WithMany(c => c.Movies)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Season ↔ TvSeries
            modelBuilder.Entity<Season>()
                .HasOne(s => s.TvSeries)
                .WithMany(t => t.Seasons)
                .HasForeignKey(s => s.TvSeriesId)
                .OnDelete(DeleteBehavior.Cascade);

            // Character ↔ Movie (Many-to-Many)
            modelBuilder.Entity<CharacterMovie>()
                .HasKey(cm => new { cm.CharacterId, cm.MovieId });

            // Character ↔ TvSeries (Many-to-Many)
            modelBuilder.Entity<CharacterTvSeries>()
                .HasKey(ct => new { ct.CharacterId, ct.TvSeriesId });
        }

        private void ConfigureIndexes(ModelBuilder modelBuilder)
        {
            // Episode
            modelBuilder.Entity<Episode>()
                .Property(e => e.Rating)
                .HasPrecision(5, 2);

            // Special
            modelBuilder.Entity<Special>()
                .Property(s => s.DiscountPercentage)
                .HasPrecision(5, 2);

            // CharacterMovie
            modelBuilder.Entity<CharacterMovie>()
                .HasIndex(cm => cm.CharacterId);
            modelBuilder.Entity<CharacterMovie>()
                .HasIndex(cm => cm.MovieId);

            // CinemaMovie
            modelBuilder.Entity<CinemaMovie>()
                .HasIndex(cm => new { cm.CinemaId, cm.MovieId, cm.ShowTime })
                .IsUnique();

            // Movie
            modelBuilder.Entity<Movie>()
                .HasIndex(m => m.Title);

            // User
            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }



        #endregion



    }

}
