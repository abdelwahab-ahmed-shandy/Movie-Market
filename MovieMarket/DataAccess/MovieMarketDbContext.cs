using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MovieMart.Models;
using Microsoft.EntityFrameworkCore;
using MovieMarket.Models.ViewModels;

namespace MovieMarket.DataAccess
{
    public class MovieMarketDbContext : IdentityDbContext<ApplicationUser>
    {
        #region Entities definition :

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<TvSeries> TvSeries { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<CharacterMovie> CharacterMovies { get; set; }
        public DbSet<CharacterTvSeries> CharacterTvSeries { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieMarketDbContext"/> class
        /// with the specified database connection options.
        /// </summary>
        /// <param name="options">Configuration settings for the database connection.</param>
        public MovieMarketDbContext(DbContextOptions<MovieMarketDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Create relationships and table settings when creating the model.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call the default configuration for the data model in the Entity Framework
            // This ensures that any basic configuration is applied before any additional customization is applied
            base.OnModelCreating(modelBuilder);

            #region Many To Many In Table :
            // Many-to-Many: Character <-> Movie
            modelBuilder.Entity<CharacterMovie>()
                .HasKey(cm => new { cm.CharacterId, cm.MovieId });

            modelBuilder.Entity<CharacterMovie>()
                .HasOne(cm => cm.Character)
                .WithMany(c => c.CharacterMovies)
                .HasForeignKey(cm => cm.CharacterId);

            modelBuilder.Entity<CharacterMovie>()
                .HasOne(cm => cm.Movie)
                .WithMany(m => m.CharacterMovies)
                .HasForeignKey(cm => cm.MovieId);

            // Many-to-Many: Character <-> TvSeries
            modelBuilder.Entity<CharacterTvSeries>()
                .HasKey(ct => new { ct.CharacterId, ct.TvSeriesId });

            modelBuilder.Entity<CharacterTvSeries>()
                .HasOne(ct => ct.Character)
                .WithMany(c => c.CharacterTvSeries)
                .HasForeignKey(ct => ct.CharacterId);

            modelBuilder.Entity<CharacterTvSeries>()
                .HasOne(ct => ct.TvSeries)
                .WithMany(ts => ts.Characters)
                .HasForeignKey(ct => ct.TvSeriesId);
            #endregion

            #region One To Many :
            // One-to-Many: TvSeries <-> Season
            modelBuilder.Entity<Season>()
                .HasOne(s => s.TvSeries)
                .WithMany(ts => ts.Seasons)
                .HasForeignKey(s => s.TvSeriesId);

            // One-to-Many: Season <-> Episode
            modelBuilder.Entity<Episode>()
                .HasOne(e => e.Season)
                .WithMany(s => s.Episodes)
                .HasForeignKey(e => e.SeasonId);

            // One-to-Many: Movie <-> Category
            modelBuilder.Entity<Movie>()
                .HasOne(m => m.Category)
                .WithMany(c => c.Movies)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Seed Data In Table :
            //  Add anime categories (Genres)
            modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Shonen", Description = "Anime filled with action and adventure." },
            new Category { Id = 2, Name = "Seinen", Description = "Anime for mature audiences." },
            new Category { Id = 3, Name = "Fantasy", Description = "Anime with magical and supernatural elements." },
            new Category { Id = 4, Name = "Sci-Fi", Description = "Futuristic anime with advanced technology." }
            );

            // Add anime series
            modelBuilder.Entity<TvSeries>().HasData(
            new TvSeries
            {
                Id = 1,
                Title = "Naruto",
                Description = "A young ninja seeks recognition and dreams of becoming the Hokage.",
                Author = "Masashi Kishimoto",
                ImgUrl = "naruto.jpg",
                ReleaseYear = 2002,
                Rating = 8.3
            },
            new TvSeries
            {
                Id = 2,
                Title = "Attack on Titan",
                Description = "Humanity fights for survival against terrifying Titans.",
                Author = "Hajime Isayama",
                ImgUrl = "aot.jpg",
                ReleaseYear = 2013,
                Rating = 9.1
            },
            new TvSeries
            {
                Id = 3,
                Title = "One Piece",
                Description = "Monkey D. Luffy sets sail to find the legendary One Piece treasure.",
                Author = "Eiichiro Oda",
                ImgUrl = "onepiece.jpg",
                ReleaseYear = 1999,
                Rating = 9.0
            }
            );

            // Add anime movies
            modelBuilder.Entity<Movie>().HasData(
            new Movie
            {
                Id = 1,
                Title = "Your Name",
                Description = "A heartwarming story of two teenagers swapping bodies across time.",
                Author = "Makoto Shinkai",
                ImgUrl = "yourname.jpg",
                Price = 12.99,
                Duration = TimeSpan.FromMinutes(106),
                StartDate = new DateTime(2016, 8, 26),
                ReleaseYear = 2016,
                Rating = 8.8,
                CategoryId = 3
            },
            new Movie
            {
                Id = 2,
                Title = "Spirited Away",
                Description = "A young girl must navigate a magical world to save her parents.",
                Author = "Hayao Miyazaki",
                ImgUrl = "spiritedaway.jpg",
                Price = 14.99,
                Duration = TimeSpan.FromMinutes(125),
                StartDate = new DateTime(2001, 7, 20),
                ReleaseYear = 2001,
                Rating = 8.9,
                CategoryId = 3 // Fantasy
            },
            new Movie
            {
                Id = 3,
                Title = "Akira",
                Description = "A cyberpunk masterpiece exploring a dystopian Tokyo.",
                Author = "Katsuhiro Otomo",
                ImgUrl = "akira.jpg",
                Price = 10.99,
                Duration = TimeSpan.FromMinutes(124),
                StartDate = new DateTime(1988, 7, 16),
                ReleaseYear = 1988,
                Rating = 8.1,
                CategoryId = 4 // Sci-Fi
            }
           );

            // Add anime characters
            modelBuilder.Entity<Character>().HasData(
            new Character { Id = 1, Name = "Naruto Uzumaki", Description = "The protagonist of Naruto, dreams of becoming Hokage." },
            new Character { Id = 2, Name = "Sasuke Uchiha", Description = "Naruto’s rival, seeking revenge against his brother Itachi." },
            new Character { Id = 3, Name = "Levi Ackerman", Description = "Captain of the Survey Corps, known for his unmatched combat skills." },
            new Character { Id = 4, Name = "Eren Yeager", Description = "Main protagonist of Attack on Titan, fights against the Titans." },
            new Character { Id = 5, Name = "Mitsuha Miyamizu", Description = "A teenage girl who swaps bodies with a boy from Tokyo in 'Your Name'." }
            );

            //  Add Many-to-Many relationship between characters and series
            modelBuilder.Entity<CharacterTvSeries>().HasData(
            new CharacterTvSeries { CharacterId = 1, TvSeriesId = 1 }, // Naruto in Naruto
            new CharacterTvSeries { CharacterId = 2, TvSeriesId = 1 }, // Sasuke in Naruto
            new CharacterTvSeries { CharacterId = 3, TvSeriesId = 2 }, // Levi in ​​Attack on Titan
            new CharacterTvSeries { CharacterId = 4, TvSeriesId = 2 } // Eren in Attack on Titan
            );

            //  Add a Many-to-Many relationship between characters and movies
            modelBuilder.Entity<CharacterMovie>().HasData(
            new CharacterMovie { CharacterId = 5, MovieId = 1 } // Mitsuha in Your Name
            );

            // Add season data for each series
            modelBuilder.Entity<Season>().HasData(
            // Naruto seasons
            new Season { Id = 1, TvSeriesId = 1, SeasonNumber = 1, Title = "Naruto - Season 1", ReleaseYear = 2002 },
            new Season { Id = 2, TvSeriesId = 1, SeasonNumber = 2, Title = "Naruto - Season 2", ReleaseYear = 2003 },
            new Season { Id = 3, TvSeriesId = 1, SeasonNumber = 3, Title = "Naruto - Season 3", ReleaseYear = 2004 },

            // Attack on Titan seasons
            new Season { Id = 4, TvSeriesId = 2, SeasonNumber = 1, Title = "Attack on Titan - Season 1", ReleaseYear = 2013 },
             new Season { Id = 5, TvSeriesId = 2, SeasonNumber = 2, Title = "Attack on Titan - Season 2", ReleaseYear = 2017 },
             new Season { Id = 6, TvSeriesId = 2, SeasonNumber = 3, Title = "Attack on Titan - Season 3", ReleaseYear = 2018 }
             );

            //  Add Episode Data (Episodes)
            modelBuilder.Entity<Episode>().HasData(
            //  Naruto Episodes - Season 1
            new Episode { Id = 1, SeasonId = 1, EpisodeNumber = 1, Title = "Enter: Naruto Uzumaki!", Duration = TimeSpan.FromMinutes(23) },
            new Episode { Id = 2, SeasonId = 1, EpisodeNumber = 2, Title = "My Name is Konohamaru!", Duration = TimeSpan.FromMinutes(23) },
            new Episode { Id = 3, SeasonId = 1, EpisodeNumber = 3, Title = "Sasuke and Sakura: Friends or Foes?", Duration = TimeSpan.FromMinutes(23) },

            //  Attack on Titan Episodes - Season 1
            new Episode { Id = 4, SeasonId = 2, EpisodeNumber = 1, Title = "To You, in 2000 Years: The Fall of Shiganshina", Duration = TimeSpan.FromMinutes(25) },
             new Episode { Id = 5, SeasonId = 2, EpisodeNumber = 2, Title = "That Day: The Fall of Shiganshina, Part 2", Duration = TimeSpan.FromMinutes(25) },
             new Episode { Id = 6, SeasonId = 2, EpisodeNumber = 3, Title = "A Dim Light Amid Despair: Humanity's Comeback", Duration = TimeSpan.FromMinutes(25) },

             // One Piece Episodes - East Blue Arc
             new Episode
             {
                 Id = 7,
                 SeasonId = 3,
                 EpisodeNumber = 1,
                 Title = "I'm Luffy! The Man Who's Gonna Be King of the Pirates!",
                 Duration = TimeSpan.FromMinutes(24)
             },

             new Episode { Id = 8, SeasonId = 3, EpisodeNumber = 2, Title = "Enter the Great Swordsman! Pirate Hunter Roronoa Zoro", Duration = TimeSpan.FromMinutes(24) },
                 new Episode { Id = 9, SeasonId = 3, EpisodeNumber = 3, Title = "Morgan vs. Luffy! Who's This Beautiful Young Girl?", Duration = TimeSpan.FromMinutes(24) }

             );
            #endregion
        }
        public DbSet<MovieMarket.Models.ViewModels.RegisterVM> RegisterVM { get; set; } = default!;
        public DbSet<MovieMarket.Models.ViewModels.LoginVM> LoginVM { get; set; } = default!;
        public DbSet<MovieMarket.Models.ViewModels.ManageProfileVM> ManageProfileVM { get; set; } = default!;





    }
}
