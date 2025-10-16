# BlogApp

BlogApp; ASP.NET Core tabanlı arka uç ve Blazor Server tabanlı yönetim/son kullanıcı arayüzü içeren katmanlı bir örnek projedir.

## Projeyi Çalıştırma

### Sunucu
`src/BlogApp.API` klasöründen `dotnet run` komutunu çalıştırarak REST API'yi ayağa kaldırabilirsiniz.

### Blazor İstemci
`src/BlogApp.Client` klasöründe yeni oluşturulan Blazor Server projesi, Radzen bileşenleriyle zenginleştirilmiş iki temel sayfa içerir:

- `/` — Blog ana sayfası, öne çıkan yazıları kart yapısı ile listeler, arama ve kategori filtreleri barındırır.
- `/admin/dashboard` — Yönetim paneli, metrik kartları, trafik grafikleri ve gönderi listesi sunar.

İstemciyi çalıştırmak için ilgili klasörde `dotnet run` komutunu kullanabilirsiniz.
