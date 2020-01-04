using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MyApp.IsRequired
{
    public class IsRequiredBaseContext<T> : DbContext
        where T : DbContextOptions
    {
        protected bool _isRequired;
        
        public IsRequiredBaseContext(T options, bool isRequired) : base(options)
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
    }
}