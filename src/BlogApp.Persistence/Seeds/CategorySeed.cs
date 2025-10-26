using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Seeds
{
    public class CategorySeed : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            var createdDate = new DateTime(2025, 10, 23);
            var adminUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

            builder.HasData(
                new Category
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                    Name = "ASP .NET Core",
                    CreatedById = adminUserId,
                    CreatedDate = createdDate,
                    IsDeleted = false
                },
                new Category
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                    Name = "Entity Framework Core",
                    CreatedById = adminUserId,
                    CreatedDate = createdDate,
                    IsDeleted = false
                },
                new Category
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                    Name = "Docker",
                    CreatedById = adminUserId,
                    CreatedDate = createdDate,
                    IsDeleted = false
                },
                new Category
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                    Name = "RabbitMQ",
                    CreatedById = adminUserId,
                    CreatedDate = createdDate,
                    IsDeleted = false
                },
                new Category
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                    Name = "Redis",
                    CreatedById = adminUserId,
                    CreatedDate = createdDate,
                    IsDeleted = false
                },
                new Category
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000006"),
                    Name = "Clean Architecture",
                    CreatedById = adminUserId,
                    CreatedDate = createdDate,
                    IsDeleted = false
                });
        }
    }
}
