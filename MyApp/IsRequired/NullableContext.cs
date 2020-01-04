using Microsoft.EntityFrameworkCore;

namespace MyApp.IsRequired
{
    public class NullableContext : IsRequiredBaseContext<DbContextOptions<NullableContext>>
    {
        public NullableContext(DbContextOptions<NullableContext> options, bool isRequired) 
            : base(options, isRequired) {}

        public DbSet<NullableEntity> NullableModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NullableEntity>()
                .Property(m => m.NullableField)
                .IsRequired(_isRequired);
        }
    }
}