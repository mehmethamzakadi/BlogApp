// k6 Load Testing Script - BlogApp Performance Test
// Kullanım: k6 run performance-test.js
// Detaylı rapor: k6 run --out json=test-results.json performance-test.js

import http from 'k6/http';
import { check, sleep, group } from 'k6';
import { Rate, Trend, Counter } from 'k6/metrics';
import { htmlReport } from 'https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js';

// Custom Metrics
const errorRate = new Rate('errors');
const postListDuration = new Trend('post_list_duration');
const categoryListDuration = new Trend('category_list_duration');
const postDetailDuration = new Trend('post_detail_duration');
const totalRequests = new Counter('total_requests');

// Test Configuration
export const options = {
  stages: [
    { duration: '2m', target: 50 },    // Warm-up: 50 kullanıcı
    { duration: '3m', target: 50 },    // Stable: 50 kullanıcı
    { duration: '2m', target: 200 },   // Ramp-up: 200 kullanıcı
    { duration: '5m', target: 200 },   // Stable: 200 kullanıcı
    { duration: '2m', target: 500 },   // Ramp-up: 500 kullanıcı
    { duration: '5m', target: 500 },   // Stable: 500 kullanıcı
    { duration: '2m', target: 1000 },  // Stress: 1000 kullanıcı
    { duration: '3m', target: 1000 },  // Stable: 1000 kullanıcı
    { duration: '2m', target: 2000 },  // Peak: 2000 kullanıcı
    { duration: '3m', target: 2000 },  // Stable: 2000 kullanıcı
    { duration: '3m', target: 0 },     // Cool-down
  ],
  thresholds: {
    'http_req_duration': ['p(95)<500', 'p(99)<1000'],
    'http_req_failed': ['rate<0.01'],
    'errors': ['rate<0.05'],
    'post_list_duration': ['p(95)<300'],
    'category_list_duration': ['p(95)<200'],
    'post_detail_duration': ['p(95)<400'],
  },
};

const BASE_URL = __ENV.API_URL || 'http://localhost:6060/api';

// Test senaryoları
export default function () {
  totalRequests.add(1);

  // Senaryo 1: Ana sayfa yükleme (80% kullanıcı)
  if (Math.random() < 0.8) {
    group('Homepage Load', function () {
      // Post listesi
      const postRes = http.get(`${BASE_URL}/post?page=1&pageSize=10`);
      postListDuration.add(postRes.timings.duration);
      
      const postCheck = check(postRes, {
        'post list status 200': (r) => r.status === 200,
        'post list has data': (r) => JSON.parse(r.body).items.length > 0,
        'post list < 500ms': (r) => r.timings.duration < 500,
      });
      errorRate.add(!postCheck);

      // Kategori listesi
      const catRes = http.get(`${BASE_URL}/category`);
      categoryListDuration.add(catRes.timings.duration);
      
      const catCheck = check(catRes, {
        'category list status 200': (r) => r.status === 200,
        'category list < 300ms': (r) => r.timings.duration < 300,
      });
      errorRate.add(!catCheck);
    });
  }

  // Senaryo 2: Post detay görüntüleme (15% kullanıcı)
  else if (Math.random() < 0.95) {
    group('Post Detail View', function () {
      // Önce liste al
      const listRes = http.get(`${BASE_URL}/post?page=1&pageSize=5`);
      
      if (listRes.status === 200) {
        const posts = JSON.parse(listRes.body).items;
        if (posts && posts.length > 0) {
          // Rastgele bir post seç
          const randomPost = posts[Math.floor(Math.random() * posts.length)];
          const detailRes = http.get(`${BASE_URL}/post/${randomPost.id}`);
          postDetailDuration.add(detailRes.timings.duration);
          
          const detailCheck = check(detailRes, {
            'post detail status 200': (r) => r.status === 200,
            'post detail has title': (r) => JSON.parse(r.body).title !== undefined,
            'post detail < 600ms': (r) => r.timings.duration < 600,
          });
          errorRate.add(!detailCheck);
        }
      }
    });
  }

  // Senaryo 3: Sayfalama (5% kullanıcı)
  else {
    group('Pagination', function () {
      const page = Math.floor(Math.random() * 5) + 1;
      const paginationRes = http.get(`${BASE_URL}/post?page=${page}&pageSize=10`);
      
      check(paginationRes, {
        'pagination status 200': (r) => r.status === 200,
        'pagination < 500ms': (r) => r.timings.duration < 500,
      });
    });
  }

  sleep(Math.random() * 2 + 1); // 1-3 saniye arası rastgele bekleme
}

export function handleSummary(data) {
  return {
    'performance-summary.json': JSON.stringify(data, null, 2),
    'performance-report.html': htmlReport(data),
    stdout: textSummary(data),
  };
}

function textSummary(data) {
  const metrics = data.metrics;
  
  return `
╔════════════════════════════════════════════════════════════════╗
║           BlogApp Performance Test Results                     ║
╚════════════════════════════════════════════════════════════════╝

📊 GENEL İSTATİSTİKLER
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Toplam İstek Sayısı    : ${metrics.http_reqs.values.count}
Başarısız İstek Oranı  : ${(metrics.http_req_failed.values.rate * 100).toFixed(2)}%
Hata Oranı             : ${(metrics.errors.values.rate * 100).toFixed(2)}%
Throughput             : ${metrics.http_reqs.values.rate.toFixed(2)} req/s

⏱️  YANIT SÜRELERİ (Genel)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Ortalama               : ${metrics.http_req_duration.values.avg.toFixed(2)}ms
Min                    : ${metrics.http_req_duration.values.min.toFixed(2)}ms
Max                    : ${metrics.http_req_duration.values.max.toFixed(2)}ms
P50 (Median)           : ${metrics.http_req_duration.values.med.toFixed(2)}ms
P90                    : ${metrics.http_req_duration.values['p(90)'].toFixed(2)}ms
P95                    : ${metrics.http_req_duration.values['p(95)'].toFixed(2)}ms
P99                    : ${metrics.http_req_duration.values['p(99)'].toFixed(2)}ms

📝 ENDPOINT BAZLI PERFORMANS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Post List P95          : ${metrics.post_list_duration.values['p(95)'].toFixed(2)}ms
Category List P95      : ${metrics.category_list_duration.values['p(95)'].toFixed(2)}ms
Post Detail P95        : ${metrics.post_detail_duration.values['p(95)'].toFixed(2)}ms

✅ BAŞARI KRİTERLERİ
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
P95 < 500ms            : ${metrics.http_req_duration.values['p(95)'] < 500 ? '✅ BAŞARILI' : '❌ BAŞARISIZ'}
P99 < 1000ms           : ${metrics.http_req_duration.values['p(99)'] < 1000 ? '✅ BAŞARILI' : '❌ BAŞARISIZ'}
Hata Oranı < %1        : ${metrics.http_req_failed.values.rate < 0.01 ? '✅ BAŞARILI' : '❌ BAŞARISIZ'}

📈 SONUÇ
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
${getSummaryResult(metrics)}

💡 Detaylı HTML rapor: performance-report.html
💾 JSON veri: performance-summary.json

`;
}

function getSummaryResult(metrics) {
  const p95 = metrics.http_req_duration.values['p(95)'];
  const p99 = metrics.http_req_duration.values['p(99)'];
  const errorRate = metrics.http_req_failed.values.rate;
  const throughput = metrics.http_reqs.values.rate;

  if (p95 < 300 && p99 < 800 && errorRate < 0.005 && throughput > 500) {
    return '🎉 MÜKEMMEL! Sistem yüksek performans gösteriyor.';
  } else if (p95 < 500 && p99 < 1000 && errorRate < 0.01 && throughput > 300) {
    return '✅ İYİ! Sistem hedeflenen performansı sağlıyor.';
  } else if (p95 < 1000 && errorRate < 0.05) {
    return '⚠️  ORTA! Sistem çalışıyor ama optimizasyon gerekebilir.';
  } else {
    return '❌ ZAYIF! Sistem performans sorunları yaşıyor.';
  }
}
