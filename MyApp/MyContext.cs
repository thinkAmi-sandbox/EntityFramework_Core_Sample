using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyApp.ClientSetNull;
using MyApp.HelloEfCore;

namespace MyApp
{
    public class MyContext : DbContext
    {
        // 既存コードを壊さないように、コンストラクタを追加
        public MyContext() {}
        
        public MyContext(DbContextOptions<MyContext> options) : base(options) {}

        private readonly DeleteBehavior _deleteBehavior;
        private readonly bool _isRequired;
        public MyContext(DbContextOptions<MyContext> options, DeleteBehavior behavior, bool isRequired) : base(options)
        {
            _deleteBehavior = behavior;
            _isRequired = isRequired;
        }
            
        public DbSet<HelloModel> HelloModels { get; set; }

        public DbSet<SingleFkUser> SingleFkUsers { get; set; }
        public DbSet<SingleFkBlog> SingleFkBlogs { get; set; }
        
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
                .UseLoggerFactory(MyLoggerFactory)
                // Databaseは、新規に "efcore" を作成する
                .UseSqlServer(@"Server=tcp:.;Database=efcore;User=sa;Password=Your_password123;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SingleFkBlog>()
                .HasOne(m => m.Author)
                .WithMany(m => m.AuthoredBlogs)
                .OnDelete(_deleteBehavior)
                .IsRequired(_isRequired);
            
            
            modelBuilder.Entity<MultiFkBlog>()
                .HasOne(m => m.Author)
                .WithMany(m => m.AuthoredBlogs)
                // .OnDelete(DeleteBehavior.Cascade)
                // .OnDelete(DeleteBehavior.SetNull)
                .OnDelete(_deleteBehavior)
                .IsRequired(_isRequired);
            
            modelBuilder.Entity<MultiFkBlog>()
                .HasOne(m => m.Contributor)
                .WithMany(m => m.ContributedToBlogs)
                // .OnDelete(DeleteBehavior.Cascade)
                // .OnDelete(DeleteBehavior.SetNull)
                .OnDelete(_deleteBehavior)
                .IsRequired(_isRequired);
        }
    }
}