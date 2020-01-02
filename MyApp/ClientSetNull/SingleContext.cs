using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MyApp.ClientSetNull
{
    public class SingleContext : DbContext
    {

        private readonly DeleteBehavior _deleteBehavior;
        private readonly bool _isRequired;
        
        // SetNull/ClientSetNullを切り替えられるよう、引数を追加したコンストラクタを用意
        public SingleContext(DbContextOptions<SingleContext> options, 
            DeleteBehavior behavior, bool isRequired) : base(options)
        {
            _deleteBehavior = behavior;
            _isRequired = isRequired;
        }

        public DbSet<SingleFkUser> SingleFkUsers { get; set; }
        public DbSet<SingleFkBlog> SingleFkBlogs { get; set; }

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
            // コンストラクタで受け取った内容で、User - Blog 間の関連を定義
            modelBuilder.Entity<SingleFkBlog>()
                .HasOne(m => m.Author)
                .WithMany(m => m.AuthoredBlogs)
                .OnDelete(_deleteBehavior)
                .IsRequired(_isRequired);
        }
    }
}