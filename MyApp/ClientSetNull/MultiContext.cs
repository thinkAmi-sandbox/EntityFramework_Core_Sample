using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyApp.ClientSetNull;
using MyApp.HelloEfCore;

namespace MyApp.ClientSetNull
{
    public class MultiContext : DbContext
    {

        private readonly DeleteBehavior _deleteBehavior;
        private readonly bool _isRequired;
        public MultiContext(DbContextOptions<MultiContext> options, DeleteBehavior behavior, bool isRequired) : base(options)
        {
            _deleteBehavior = behavior;
            _isRequired = isRequired;
        }

        public DbSet<MultiFkUser> MultiFkUsers { get; set; }
        public DbSet<MultiFkBlog> MultiFkBlogs { get; set; }

        public static readonly ILoggerFactory MyLoggerFactory
            = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information)
                    .AddConsole();
            });
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseLoggerFactory(MyLoggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MultiFkBlog>()
                .HasOne(m => m.Author)
                .WithMany(m => m.AuthoredBlogs)
                .OnDelete(_deleteBehavior)
                .IsRequired(_isRequired);
            
            modelBuilder.Entity<MultiFkBlog>()
                .HasOne(m => m.Contributor)
                .WithMany(m => m.ContributedToBlogs)
                .OnDelete(_deleteBehavior)
                .IsRequired(_isRequired);
        }
    }
}