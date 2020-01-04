using Microsoft.EntityFrameworkCore;

namespace MyApp.IsRequired
{
    public class NotNullContext : IsRequiredBaseContext<DbContextOptions<NotNullContext>>
    {
        public NotNullContext(DbContextOptions<NotNullContext> options, bool isRequired) : base(options, isRequired) {}

        public DbSet<NotNullEntity> NotNullModels { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NotNullEntity>()
                .Property(m => m.NotNullField)
                .IsRequired(_isRequired);
        }
    }
}