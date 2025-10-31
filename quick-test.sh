#!/bin/bash
# BlogApp Quick Load Test Script
# Linux/macOS için hızlı test başlatma script'i

set -e

echo "========================================"
echo "  BlogApp Load Test Başlatılıyor"
echo "========================================"
echo ""

# k6 kurulu mu kontrol et
if ! command -v k6 &> /dev/null; then
    echo "[HATA] k6 bulunamadı!"
    echo ""
    echo "k6'yi yüklemek için:"
    echo "  macOS: brew install k6"
    echo "  Linux: https://k6.io/docs/getting-started/installation/"
    echo ""
    exit 1
fi

# API çalışıyor mu kontrol et
echo "[1/4] API bağlantısı kontrol ediliyor..."
if ! curl -s http://localhost:6060/api/category > /dev/null 2>&1; then
    echo "[HATA] API'ye bağlanılamıyor!"
    echo ""
    echo "API'yi başlatmak için:"
    echo "  cd src/BlogApp.API"
    echo "  dotnet run --urls http://localhost:6060"
    echo ""
    echo "veya Docker ile:"
    echo "  docker compose -f docker-compose.yml -f docker-compose.local.yml up -d"
    echo ""
    exit 1
fi
echo "[OK] API çalışıyor"

# Test türünü seç
echo ""
echo "========================================"
echo "  Test Türü Seçin"
echo "========================================"
echo ""
echo "1. Hızlı Test (5 dakika, 50-200 kullanıcı)"
echo "2. Orta Test (15 dakika, 50-500 kullanıcı)"
echo "3. Tam Test (33 dakika, 50-2000 kullanıcı)"
echo "4. Stress Test (10 dakika, 1000 kullanıcı sabit)"
echo "5. Spike Test (7 dakika, ani yük artışı)"
echo ""
read -p "Seçiminiz (1-5): " choice

case $choice in
    1)
        echo ""
        echo "[2/4] Hızlı test hazırlanıyor..."
        echo "[3/4] Test başlatılıyor (5 dakika)..."
        k6 run --vus 200 --duration 5m performance-test.js
        ;;
    2)
        echo ""
        echo "[2/4] Orta test hazırlanıyor..."
        echo "[3/4] Test başlatılıyor (15 dakika)..."
        k6 run --stages "2m:50,3m:50,2m:200,5m:200,2m:500,3m:500,2m:0" performance-test.js
        ;;
    3)
        echo ""
        echo "[2/4] Tam test hazırlanıyor..."
        echo "[3/4] Test başlatılıyor (33 dakika)..."
        k6 run performance-test.js
        ;;
    4)
        echo ""
        echo "[2/4] Stress test hazırlanıyor..."
        echo "[3/4] Test başlatılıyor (10 dakika)..."
        k6 run --vus 1000 --duration 10m performance-test.js
        ;;
    5)
        echo ""
        echo "[2/4] Spike test hazırlanıyor..."
        echo "[3/4] Test başlatılıyor (7 dakika)..."
        k6 run --stages "1m:100,30s:2000,3m:2000,1m:100,1m:0" performance-test.js
        ;;
    *)
        echo "Geçersiz seçim!"
        exit 1
        ;;
esac

echo ""
echo "[4/4] Test tamamlandı!"
echo ""
echo "========================================"
echo "  Sonuçlar"
echo "========================================"
echo ""
echo "HTML Rapor: performance-report.html"
echo "JSON Veri : performance-summary.json"
echo ""
echo "HTML raporunu açmak için:"
if [[ "$OSTYPE" == "darwin"* ]]; then
    echo "  open performance-report.html"
else
    echo "  xdg-open performance-report.html"
fi
echo ""
