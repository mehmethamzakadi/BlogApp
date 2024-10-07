using BlogApp.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Configurations
{
    public class BaseConfiguraiton<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
    {
        public void Configure(EntityTypeBuilder<TEntity> builder)
        {
            //isDeleted == true olan kayıtları filtreler.
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
