using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Seeds
{
    public class CategorySeed : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            var createdDate = new DateTime(2025, 10, 23, 7, 0, 0, DateTimeKind.Utc);
            var systemUserId = SystemUsers.SystemUserId;

            builder.HasData(
                new Category
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                    Name = "ASP.NET Core",
                    NormalizedName = "ASP.NET CORE",
                    CreatedById = systemUserId,
                    CreatedDate = createdDate,
                    IsDeleted = false
                },
                new Category
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                    Name = "Entity Framework Core",
                    NormalizedName = "ENTITY FRAMEWORK CORE",
                    CreatedById = systemUserId,
                    CreatedDate = createdDate,
                    IsDeleted = false
                },
                new Category
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                    Name = "Cloud Native DevOps",
                    NormalizedName = "CLOUD NATIVE DEVOPS",
                    CreatedById = systemUserId,
                    CreatedDate = createdDate,
                    IsDeleted = false
                },
                new Category
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                    Name = "Event-Driven Messaging",
                    NormalizedName = "EVENT-DRIVEN MESSAGING",
                    CreatedById = systemUserId,
                    CreatedDate = createdDate,
                    IsDeleted = false
                },
                new Category
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                    Name = "Observability & Monitoring",
                    NormalizedName = "OBSERVABILITY & MONITORING",
                    CreatedById = systemUserId,
                    CreatedDate = createdDate,
                    IsDeleted = false
                },
                new Category
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000006"),
                    Name = "Clean Architecture",
                    NormalizedName = "CLEAN ARCHITECTURE",
                    CreatedById = systemUserId,
                    CreatedDate = createdDate,
                    IsDeleted = false
                });
        }
    }
}
