
using System;
using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Seeds
{
    public class CategorySeed : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            var createdDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                new Category
                {
                    Id = 1,
                    Name = "ASP .NET Core",
                    CreatedById = 1,
                    CreatedDate = createdDate,
                    IsDeleted = false
                },
                new Category
                {
                    Id = 2,
                    Name = "Entity Framework Core",
                    CreatedById = 1,
                    CreatedDate = createdDate,
                    IsDeleted = false
                },
                new Category
                {
                    Id = 3,
                    Name = "Docker",
                    CreatedById = 1,
                    CreatedDate = createdDate,
                    IsDeleted = false
                },
                new Category
                {
                    Id = 4,
                    Name = "RabbitMQ",
                    CreatedById = 1,
                    CreatedDate = createdDate,
                    IsDeleted = false
                },
                new Category
                {
                    Id = 5,
                    Name = "Redis",
                    CreatedById = 1,
                    CreatedDate = createdDate,
                    IsDeleted = false
                },
                new Category
                {
                    Id = 6,
                    Name = "Clean Architecture",
                    CreatedById = 1,
                    CreatedDate = createdDate,
                    IsDeleted = false
                });
        }
    }
}
