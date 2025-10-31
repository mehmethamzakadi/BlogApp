using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Seeds;

public class CommentSeed : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        // Comment seed'leri kaldırıldı - Comment artık aggregate boundary içinde
        // Seed data'lar Post aggregate üzerinden eklenmeli
    }
}
