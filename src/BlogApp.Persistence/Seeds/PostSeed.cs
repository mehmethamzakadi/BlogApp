using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Seeds;

public class PostSeed : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasData(
            new Post
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000001"),
                Title = ".NET 9 Minimal API'lerinde Observability Entegrasyonu",
                Summary = "OpenTelemetry ve Aspire ile ASP.NET Core minimal API projelerinde uçtan uca gözlemlenebilirlik altyapısını kurma rehberi.",
                Body = """
OpenTelemetry Collector, distributed tracing ve yapılandırılmış logging birleştiğinde minimal API'ler üretim ortamında şeffaf hâle geliyor.
Bu rehberde ActivitySource, Meter ve TraceId bağlamlarını nasıl kodladığımızı adım adım ele alıyoruz.
Ayrıca Aspire Dashboard ile gecikme ve hata oranlarını anlık izlemenin püf noktalarını paylaşıyoruz.
""",
                Thumbnail = "https://miro.medium.com/v2/resize:fit:1286/format:webp/1*chhJLW0ApPDHqmVPRBBUtQ.png",
                IsPublished = true,
                CategoryId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                CreatedById = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                CreatedDate = new DateTime(2025, 9, 28, 8, 30, 0, DateTimeKind.Utc),
                IsDeleted = false
            },
            new Post
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000002"),
                Title = "EF Core 9 ile Çoklu Tenant Mimarisinde Performans İpuçları",
                Summary = "Tenant bazlı filtreleme, connection pooling ve caching stratejileriyle çoklu tenant SaaS uygulamalarında EF Core'u hızlandırma.",
                Body = """
SaaS uygulamalarında sorgu optimizasyonu tenant bazında indeksleme ile başlıyor.
Model seeding, global query filter'lar ve concurrency token'ları üzerinden performans analizleri paylaşıyoruz.
Ayrıca Npgsql provider'ı ile partitioned table senaryolarını örneklendiriyoruz.
""",
                Thumbnail = "https://miro.medium.com/0*V56TEDMUsms9XLBY.jpg",
                IsPublished = true,
                CategoryId = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                CreatedById = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                CreatedDate = new DateTime(2025, 10, 2, 9, 15, 0, DateTimeKind.Utc),
                IsDeleted = false
            },
            new Post
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000003"),
                Title = "GitOps ile Kubernetes Üzerinde Sürekli Teslimat",
                Summary = "FluxCD ve ArgoCD karşılaştırmasıyla GitOps pipeline'larını teknoloji blogu projelerine uyarlama.",
                Body = """
GitOps, manifest kaynağını tek gerçeğin kaynağına dönüştürerek roll-forward ve roll-back süreçlerini sadeleştiriyor.
FluxCD ile progressive delivery, ArgoCD ile health check politika tanımlarını örnek YAML dosyalarıyla açıklıyoruz.
Pipeline gözlemlenebilirliği için Prometheus ve Grafana entegrasyonlarını da ekliyoruz.
""",
                Thumbnail = "https://miro.medium.com/v2/resize:fit:4800/format:webp/1*9P6wnky3C9xMwaBAElALLQ.png",
                IsPublished = true,
                CategoryId = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                CreatedById = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                CreatedDate = new DateTime(2025, 10, 5, 10, 45, 0, DateTimeKind.Utc),
                IsDeleted = false
            },
            new Post
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000004"),
                Title = "Event-Driven Mimarilerde RabbitMQ mu Kafka mı?",
                Summary = "Etkinlik yönelimli servisler için mesajlaşma altyapısı seçerken throughput, gecikme ve gözlemlenebilirlik kriterlerinin karşılaştırılması.",
                Body = """
Mesajlaşma altyapısı seçiminde gereksinimleri segmentlere ayırmak kritik.
RabbitMQ routing esnekliği sağlar; Kafka ise sıralı event log ile akış analitiğine güç katar.
Makale boyunca tüketici grupları, dead-letter stratejileri ve metrik takip yöntemlerini detaylandırıyoruz.
""",
                Thumbnail = "/media/posts/event-driven-rabbitmq-vs-kafka.png",
                IsPublished = false,
                CategoryId = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                CreatedById = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                CreatedDate = new DateTime(2025, 10, 12, 15, 0, 0, DateTimeKind.Utc),
                IsDeleted = false
            },
            new Post
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000005"),
                Title = "OpenTelemetry ile Katmanlı Gözlemlenebilirlik",
                Summary = "Teknoloji blogu altyapısı için tracing, metrics ve logging verilerini aynı veri gölünde birleştirme stratejileri.",
                Body = """
Tracing zincirleri, metrik korelasyonları ve yapılandırılmış log'lar aynı veri modelinde buluştuğunda kök neden analizi hızlanıyor.
Bu makalede collector konfigürasyonlarını, OTLP protokolünü ve Prometheus remote write senaryolarını harmanlıyoruz.
Ek olarak, kullanıcı segmenti bazlı alert kurallarına dair pratik şablonlar sunuyoruz.
""",
                Thumbnail = "https://miro.medium.com/v2/resize:fit:1400/format:webp/1*zHc9d823Uol9SSj8s_uBug.png",
                IsPublished = true,
                CategoryId = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                CreatedById = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                CreatedDate = new DateTime(2025, 10, 15, 11, 20, 0, DateTimeKind.Utc),
                IsDeleted = false
            });
    }
}
