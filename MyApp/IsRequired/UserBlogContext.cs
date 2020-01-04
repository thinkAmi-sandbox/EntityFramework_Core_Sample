using Microsoft.EntityFrameworkCore;

namespace MyApp.IsRequired
{
    public class UserBlogContext : IsRequiredBaseContext<DbContextOptions<UserBlogContext>>
    {
        public UserBlogContext(DbContextOptions<UserBlogContext> options, bool isRequired) 
            : base(options, isRequired) {}

        public DbSet<User> NotNullUsers { get; set; }
        public DbSet<Blog> NotNullBlogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>()
                .HasOne(m => m.Author)
                .WithMany(m => m.AuthoredBlogs)
                .IsRequired(_isRequired);
            
            modelBuilder.Entity<Blog>()
                .HasOne(m => m.Contributor)
                .WithMany(m => m.ContributedToBlogs)
                .IsRequired(_isRequired);
        }
    }
}