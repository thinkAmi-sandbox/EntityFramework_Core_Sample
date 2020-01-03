using Microsoft.EntityFrameworkCore;

namespace MyApp.GenericInheritance
{
    public class GenericInheritanceUserContext
        : GenericInheritanceBaseContext<DbContextOptions<GenericInheritanceUserContext>>
    {
        public GenericInheritanceUserContext(
            DbContextOptions<GenericInheritanceUserContext> options, bool isRequired)
            : base(options, isRequired) {}

        public DbSet<User> Users { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(user => user.Name)
                .IsRequired(_isRequired);
        }
    }
}