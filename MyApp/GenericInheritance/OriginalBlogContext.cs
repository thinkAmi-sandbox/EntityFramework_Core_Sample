using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MyApp.GenericInheritance
{
    public class OriginalBlogContext : DbContext
    {
        protected bool _isRequired;
        
        public OriginalBlogContext(DbContextOptions<OriginalBlogContext> options, bool isRequired)
            : base(options)
        {
            _isRequired = isRequired;
        }

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
            modelBuilder.Entity<Blog>()
                .Property(blog => blog.Url)
                .IsRequired(_isRequired);
        }
    }
}