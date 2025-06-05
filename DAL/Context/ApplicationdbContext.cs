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
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }

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


            #region Seed Data In Table
            // Add anime categories (Genres)
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = SeedDataConstants.ShonenCategoryId,
                    Name = "Shonen",
                    Description = "Anime filled with action and adventure.",
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                },
                new Category
                {
                    Id = SeedDataConstants.SeinenCategoryId,
                    Name = "Seinen",
                    Description = "Anime for mature audiences.",
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                },
                new Category
                {
                    Id = SeedDataConstants.FantasyCategoryId,
                    Name = "Fantasy",
                    Description = "Anime with magical and supernatural elements.",
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                },
                new Category
                {
                    Id = SeedDataConstants.SciFiCategoryId,
                    Name = "Sci-Fi",
                    Description = "Futuristic anime with advanced technology.",
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                }
            );

            // Add anime series
            modelBuilder.Entity<TvSeries>().HasData(
                new TvSeries
                {
                    Id = SeedDataConstants.NarutoSeriesId,
                    Title = "Naruto",
                    Description = "A young ninja seeks recognition and dreams of becoming the Hokage.",
                    Author = "Masashi Kishimoto",
                    ImgUrl = "naruto.jpg",
                    ReleaseYear = 2002,
                    Rating = 8.3,
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                },
                new TvSeries
                {
                    Id = SeedDataConstants.AttackSeriesId,
                    Title = "Attack on Titan",
                    Description = "Humanity fights for survival against terrifying Titans.",
                    Author = "Hajime Isayama",
                    ImgUrl = "aot.jpg",
                    ReleaseYear = 2013,
                    Rating = 9.1,
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                },
                new TvSeries
                {
                    Id = SeedDataConstants.OnePieceSeriesId,
                    Title = "One Piece",
                    Description = "Monkey D. Luffy sets sail to find the legendary One Piece treasure.",
                    Author = "Eiichiro Oda",
                    ImgUrl = "onepiece.jpg",
                    ReleaseYear = 1999,
                    Rating = 9.0,
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                }
            );

            // Add anime movies
            modelBuilder.Entity<Movie>().HasData(
                new Movie
                {
                    Id = SeedDataConstants.YourNameMovieId,
                    Title = "Your Name",
                    Description = "A heartwarming story of two teenagers swapping bodies across time.",
                    Author = "Makoto Shinkai",
                    ImgUrl = "yourname.jpg",
                    Price = 12.99,
                    Duration = TimeSpan.FromMinutes(106),
                    StartDate = new DateTime(2016, 8, 26),
                    ReleaseYear = 2016,
                    Rating = 8.8,
                    CategoryId = SeedDataConstants.FantasyCategoryId,
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                },
                new Movie
                {
                    Id = SeedDataConstants.SpiritedAwayId,
                    Title = "Spirited Away",
                    Description = "A young girl must navigate a magical world to save her parents.",
                    Author = "Hayao Miyazaki",
                    ImgUrl = "spiritedaway.jpg",
                    Price = 14.99,
                    Duration = TimeSpan.FromMinutes(125),
                    StartDate = new DateTime(2001, 7, 20),
                    ReleaseYear = 2001,
                    Rating = 8.9,
                    CategoryId = SeedDataConstants.FantasyCategoryId,
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                },
                new Movie
                {
                    Id = SeedDataConstants.AkiraId,
                    Title = "Akira",
                    Description = "A cyberpunk masterpiece exploring a dystopian Tokyo.",
                    Author = "Katsuhiro Otomo",
                    ImgUrl = "akira.jpg",
                    Price = 10.99,
                    Duration = TimeSpan.FromMinutes(124),
                    StartDate = new DateTime(1988, 7, 16),
                    ReleaseYear = 1988,
                    Rating = 8.1,
                    CategoryId = SeedDataConstants.SciFiCategoryId,
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                }
            );

            // Add anime characters
            modelBuilder.Entity<Character>().HasData(
                new Character
                {
                    Id = SeedDataConstants.NarutoCharacterId,
                    Name = "Naruto Uzumaki",
                    Description = "The protagonist of Naruto, dreams of becoming Hokage.",
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                },
                new Character
                {
                    Id = SeedDataConstants.SasukeCharacterId,
                    Name = "Sasuke Uchiha",
                    Description = "Naruto's rival, seeking revenge against his brother Itachi.",
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                },
                new Character
                {
                    Id = SeedDataConstants.LeviCharacterId,
                    Name = "Levi Ackerman",
                    Description = "Captain of the Survey Corps, known for his unmatched combat skills.",
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                },
                new Character
                {
                    Id = SeedDataConstants.ErenCharacterId,
                    Name = "Eren Yeager",
                    Description = "Main protagonist of Attack on Titan, fights against the Titans.",
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                },
                new Character
                {
                    Id = SeedDataConstants.MitsuhaCharacterId,
                    Name = "Mitsuha Miyamizu",
                    Description = "A teenage girl who swaps bodies with a boy from Tokyo in 'Your Name'.",
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                }
            );

            // Add Many-to-Many relationship between characters and series
            modelBuilder.Entity<CharacterTvSeries>().HasData(
                new CharacterTvSeries
                {
                    Id = SeedDataConstants.NarutoCharacterTvSeriesId,
                    CharacterId = SeedDataConstants.NarutoCharacterId,
                    TvSeriesId = SeedDataConstants.NarutoSeriesId,
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                },
                new CharacterTvSeries
                {
                    Id = SeedDataConstants.SasukeCharacterTvSeriesId,
                    CharacterId = SeedDataConstants.SasukeCharacterId,
                    TvSeriesId = SeedDataConstants.NarutoSeriesId,
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                },
                new CharacterTvSeries
                {
                    Id = SeedDataConstants.LeviCharacterTvSeriesId,
                    CharacterId = SeedDataConstants.LeviCharacterId,
                    TvSeriesId = SeedDataConstants.AttackSeriesId,
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                },
                new CharacterTvSeries
                {
                    Id = SeedDataConstants.ErenCharacterTvSeriesId,
                    CharacterId = SeedDataConstants.ErenCharacterId,
                    TvSeriesId = SeedDataConstants.AttackSeriesId,
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                }
            );

            // Add season data for each series
            modelBuilder.Entity<Season>().HasData(
                new Season
                {
                    Id = SeedDataConstants.NarutoSeason1Id,
                    TvSeriesId = SeedDataConstants.NarutoSeriesId,
                    SeasonNumber = 1,
                    Title = "Naruto - Season 1",
                    ReleaseYear = 2002,
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                },
                new Season
                {
                    Id = SeedDataConstants.NarutoSeason2Id,
                    TvSeriesId = SeedDataConstants.NarutoSeriesId,
                    SeasonNumber = 2,
                    Title = "Naruto - Season 2",
                    ReleaseYear = 2003,
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                },
                new Season
                {
                    Id = SeedDataConstants.AttackSeason1Id,
                    TvSeriesId = SeedDataConstants.AttackSeriesId,
                    SeasonNumber = 1,
                    Title = "Attack on Titan - Season 1",
                    ReleaseYear = 2013,
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                }
            );

            // Add Episode Data
            modelBuilder.Entity<Episode>().HasData(
                new Episode
                {
                    Id = SeedDataConstants.NarutoEpisode1Id,
                    SeasonId = SeedDataConstants.NarutoSeason1Id,
                    EpisodeNumber = 1,
                    Title = "Enter: Naruto Uzumaki!",
                    Duration = TimeSpan.FromMinutes(23),
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                },
                new Episode
                {
                    Id = SeedDataConstants.NarutoEpisode2Id,
                    SeasonId = SeedDataConstants.NarutoSeason1Id,
                    EpisodeNumber = 2,
                    Title = "My Name is Konohamaru!",
                    Duration = TimeSpan.FromMinutes(23),
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                },
                new Episode
                {
                    Id = SeedDataConstants.AttackEpisode1Id,
                    SeasonId = SeedDataConstants.AttackSeason1Id,
                    EpisodeNumber = 1,
                    Title = "To You, in 2000 Years: The Fall of Shiganshina",
                    Duration = TimeSpan.FromMinutes(25),
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                }
            );

            // Add Cinemas
            modelBuilder.Entity<Cinema>().HasData(
                new Cinema
                {
                    Id = SeedDataConstants.Cinema1Id,
                    Name = "IMAX Theater",
                    Description = "A premium large-screen cinema experience.",
                    Location = "Downtown",
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                },
                new Cinema
                {
                    Id = SeedDataConstants.Cinema2Id,
                    Name = "Grand Cineplex",
                    Description = "A modern cinema with multiple screening rooms.",
                    Location = "City Center",
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                }
            );

            // Add CinemaMovies
            modelBuilder.Entity<CinemaMovie>().HasData(
                new CinemaMovie
                {
                    Id = SeedDataConstants.CinemaMovie1Id,
                    CinemaId = SeedDataConstants.Cinema1Id,
                    MovieId = SeedDataConstants.YourNameMovieId,
                    ShowTime = new DateTime(2023, 1, 2, 18, 0, 0),
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                },
                new CinemaMovie
                {
                    Id = SeedDataConstants.CinemaMovie2Id,
                    CinemaId = SeedDataConstants.Cinema2Id,
                    MovieId = SeedDataConstants.SpiritedAwayId,
                    ShowTime = new DateTime(2023, 1, 3, 19, 30, 0),
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                },
                new CinemaMovie
                {
                    Id = SeedDataConstants.CinemaMovie3Id,
                    CinemaId = SeedDataConstants.Cinema1Id,
                    MovieId = SeedDataConstants.AkiraId,
                    ShowTime = new DateTime(2023, 1, 4, 21, 0, 0),
                    CreatedDateUtc = SeedDataConstants.SeedDate,
                    LastAction = "Seed Data"
                }
            );
            #endregion


        }


        #region Seed Data Constants
        public static class SeedDataConstants
        {
            // Categories
            public static readonly Guid ShonenCategoryId = Guid.Parse("00000000-0000-0000-0003-000000000001");
            public static readonly Guid SeinenCategoryId = Guid.Parse("00000000-0000-0000-0002-000000000002");
            public static readonly Guid FantasyCategoryId = Guid.Parse("00000000-0000-0000-0020-000000000003");
            public static readonly Guid SciFiCategoryId = Guid.Parse("00000000-0000-0000-0030-000000000004");

            // TV Series
            public static readonly Guid NarutoSeriesId = Guid.Parse("00000000-0000-0000-0000-000000000101");
            public static readonly Guid AttackSeriesId = Guid.Parse("00000000-0000-0000-0000-000000000102");
            public static readonly Guid OnePieceSeriesId = Guid.Parse("00000000-0000-0000-0000-000000000103");

            // Movies
            public static readonly Guid YourNameMovieId = Guid.Parse("00000000-0000-0000-0000-000000000201");
            public static readonly Guid SpiritedAwayId = Guid.Parse("00000000-0000-0000-0000-000000000202");
            public static readonly Guid AkiraId = Guid.Parse("00000000-0000-0000-0000-000000000203");

            // Characters
            public static readonly Guid NarutoCharacterId = Guid.Parse("00000000-0000-0000-0000-000000000301");
            public static readonly Guid SasukeCharacterId = Guid.Parse("00000000-0000-0000-0000-000000000302");
            public static readonly Guid LeviCharacterId = Guid.Parse("00000000-0000-0000-0000-000000000303");
            public static readonly Guid ErenCharacterId = Guid.Parse("00000000-0000-0000-0000-000000000304");
            public static readonly Guid MitsuhaCharacterId = Guid.Parse("00000000-0000-0000-0000-000000000305");

            // CharacterTvSeries
            public static readonly Guid NarutoCharacterTvSeriesId = Guid.Parse("00000000-0000-0000-0000-000000000401");
            public static readonly Guid SasukeCharacterTvSeriesId = Guid.Parse("00000000-0000-0000-0000-000000000402");
            public static readonly Guid LeviCharacterTvSeriesId = Guid.Parse("00000000-0000-0000-0000-000000000403");
            public static readonly Guid ErenCharacterTvSeriesId = Guid.Parse("00000000-0000-0000-0000-000000000404");

            // Seasons
            public static readonly Guid NarutoSeason1Id = Guid.Parse("00000000-0000-0000-0000-000000000501");
            public static readonly Guid NarutoSeason2Id = Guid.Parse("00000000-0000-0000-0000-000000000502");
            public static readonly Guid AttackSeason1Id = Guid.Parse("00000000-0000-0000-0000-000000000503");

            // Episodes
            public static readonly Guid NarutoEpisode1Id = Guid.Parse("00000000-0000-0000-0000-000000000601");
            public static readonly Guid NarutoEpisode2Id = Guid.Parse("00000000-0000-0000-0000-000000000602");
            public static readonly Guid AttackEpisode1Id = Guid.Parse("00000000-0000-0000-0000-000000000603");

            // Cinemas
            public static readonly Guid Cinema1Id = Guid.Parse("00000000-0000-0000-0000-000000000701");
            public static readonly Guid Cinema2Id = Guid.Parse("00000000-0000-0000-0000-000000000702");

            // CinemaMovies
            public static readonly Guid CinemaMovie1Id = Guid.Parse("00000000-0000-0000-0000-000000000801");
            public static readonly Guid CinemaMovie2Id = Guid.Parse("00000000-0000-0000-0000-000000000802");
            public static readonly Guid CinemaMovie3Id = Guid.Parse("00000000-0000-0000-0000-000000000803");

            // Fixed Dates
            public static readonly DateTime SeedDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }
        #endregion


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
