using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Configurations.Seeds
{
    public class CategorySeed : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasData(
                new Category
                {
                    Id = 1,
                    Name = "ASP .NET Core",
                    CreatedById = 1,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false
                },
                new Category
                {
                    Id = 2,
                    Name = "Entity Framework Core",
                    CreatedById = 1,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false
                },
                new Category
                {
                    Id = 3,
                    Name = "Docker",
                    CreatedById = 1,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false
                },
                new Category
                {
                    Id = 4,
                    Name = "RabbitMQ",
                    CreatedById = 1,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false
                },
                new Category
                {
                    Id = 5,
                    Name = "Redis",
                    CreatedById = 1,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false
                },
                 new Category
                 {
                     Id = 6,
                     Name = "Clean Architecture",
                     CreatedById = 1,
                     CreatedDate = DateTime.Now,
                     IsDeleted = false
                 });
        }
    }
}
