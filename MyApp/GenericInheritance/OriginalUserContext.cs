using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MyApp.GenericInheritance
{
    public class OriginalUserContext : DbContext
    {
        protected bool _isRequired;
        
        public OriginalUserContext(DbContextOptions<OriginalUserContext> options, bool isRequired)
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
            modelBuilder.Entity<User>()
                .Property(user => user.Name )
                .IsRequired(_isRequired);
        }
    }
}