
using BlogApp.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Configurations
{
    /// <summary>
    /// Tüm BaseEntity türevleri için ortak yapılandırma
    /// </summary>
    public class BaseConfiguraiton<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            // Primary key
            builder.HasKey(x => x.Id);
            
            // ✅ Concurrency token - Optimistic locking
            builder.Property(x => x.RowVersion)
                .IsRowVersion()
                .HasColumnName("RowVersion");
            
            // Soft delete filter
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
