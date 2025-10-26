using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Seeds;

public class CommentSeed : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        var seedDate = new DateTime(2025, 10, 20, 9, 0, 0, DateTimeKind.Utc);
        var systemUserId = SystemUsers.SystemUserId;

        builder.HasData(
            new Comment
            {
                Id = Guid.Parse("60000000-0000-0000-0000-000000000001"),
                PostId = Guid.Parse("40000000-0000-0000-0000-000000000001"),
                Content = "Minimal API'lerde TraceId propagasyonu için örnek kod parçalarını uyguladım, Aspire Dashboard harika çalıştı!",
                CommentOwnerMail = "techreader@blogapp.dev",
                IsPublished = true,
                CreatedById = systemUserId,
                CreatedDate = seedDate,
                IsDeleted = false
            },
            new Comment
            {
                Id = Guid.Parse("60000000-0000-0000-0000-000000000002"),
                ParentId = Guid.Parse("60000000-0000-0000-0000-000000000001"),
                PostId = Guid.Parse("40000000-0000-0000-0000-000000000001"),
                Content = "Trace örneklerinin yanına log seviyeleri ekleyince sorun tespiti çok hızlandı, teşekkürler!",
                CommentOwnerMail = "editor@blogapp.dev",
                IsPublished = true,
                CreatedById = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                CreatedDate = seedDate.AddMinutes(12),
                IsDeleted = false
            },
            new Comment
            {
                Id = Guid.Parse("60000000-0000-0000-0000-000000000003"),
                PostId = Guid.Parse("40000000-0000-0000-0000-000000000004"),
                Content = "Kafka bölümündeki partition senaryosunu canlı görmek isterim, yayınlandığında haber verir misiniz?",
                CommentOwnerMail = "community@blogapp.dev",
                IsPublished = false,
                CreatedById = systemUserId,
                CreatedDate = seedDate.AddMinutes(40),
                IsDeleted = false
            });
    }
}
