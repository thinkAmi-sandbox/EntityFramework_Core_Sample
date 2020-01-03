using Microsoft.EntityFrameworkCore;

namespace MyApp.GenericInheritance
{
    public class GenericInheritanceBlogContext
        : GenericInheritanceBaseContext<DbContextOptions<GenericInheritanceBlogContext>>
    {
        public GenericInheritanceBlogContext(
            DbContextOptions<GenericInheritanceBlogContext> options, bool isRequired)
            : base(options, isRequired) {}

        public DbSet<Blog> Blogs { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>()
                .Property(blog => blog.Url)
                .IsRequired(_isRequired);
        }
    }
}