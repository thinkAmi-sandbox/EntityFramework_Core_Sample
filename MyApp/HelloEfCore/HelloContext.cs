using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MyApp.HelloEfCore
{
    public class HelloContext: DbContext
    {
        public DbSet<HelloModel> HelloModels { get; set; }
        
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
                .UseLoggerFactory(MyLoggerFactory)
                // Databaseは、新規に "efcore" を作成する
                .UseSqlServer(@"Server=tcp:.;Database=efcore;User=sa;Password=Your_password123;");
        }
    }
}