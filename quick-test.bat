@echo off
REM BlogApp Quick Load Test Script
REM Windows için hızlı test başlatma script'i

echo ========================================
echo   BlogApp Load Test Başlatılıyor
echo ========================================
echo.

REM k6 kurulu mu kontrol et
where k6 >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo [HATA] k6 bulunamadi!
    echo.
    echo k6'yi yuklemek icin:
    echo   choco install k6
    echo.
    pause
    exit /b 1
)

REM API çalışıyor mu kontrol et
echo [1/4] API baglantisi kontrol ediliyor...
curl -s http://localhost:6060/api/category >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo [HATA] API'ye baglanilamiyor!
    echo.
    echo API'yi baslatmak icin:
    echo   cd src\BlogApp.API
    echo   dotnet run --urls http://localhost:6060
    echo.
    echo veya Docker ile:
    echo   docker compose -f docker-compose.yml -f docker-compose.local.yml up -d
    echo.
    pause
    exit /b 1
)
echo [OK] API calisiyor

REM Test türünü seç
echo.
echo ========================================
echo   Test Turu Secin
echo ========================================
echo.
echo 1. Hizli Test (5 dakika, 50-200 kullanici)
echo 2. Orta Test (15 dakika, 50-500 kullanici)
echo 3. Tam Test (33 dakika, 50-2000 kullanici)
echo 4. Stress Test (10 dakika, 1000 kullanici sabit)
echo 5. Spike Test (7 dakika, ani yuk artisi)
echo.
set /p choice="Seciminiz (1-5): "

if "%choice%"=="1" goto quick
if "%choice%"=="2" goto medium
if "%choice%"=="3" goto full
if "%choice%"=="4" goto stress
if "%choice%"=="5" goto spike
echo Gecersiz secim!
pause
exit /b 1

:quick
echo.
echo [2/4] Hizli test hazirlaniyor...
echo [3/4] Test baslatiliyor (5 dakika)...
k6 run --vus 200 --duration 5m performance-test.js
goto end

:medium
echo.
echo [2/4] Orta test hazirlaniyor...
echo [3/4] Test baslatiliyor (15 dakika)...
k6 run --stages "2m:50,3m:50,2m:200,5m:200,2m:500,3m:500,2m:0" performance-test.js
goto end

:full
echo.
echo [2/4] Tam test hazirlaniyor...
echo [3/4] Test baslatiliyor (33 dakika)...
k6 run performance-test.js
goto end

:stress
echo.
echo [2/4] Stress test hazirlaniyor...
echo [3/4] Test baslatiliyor (10 dakika)...
k6 run --vus 1000 --duration 10m performance-test.js
goto end

:spike
echo.
echo [2/4] Spike test hazirlaniyor...
echo [3/4] Test baslatiliyor (7 dakika)...
k6 run --stages "1m:100,30s:2000,3m:2000,1m:100,1m:0" performance-test.js
goto end

:end
echo.
echo [4/4] Test tamamlandi!
echo.
echo ========================================
echo   Sonuclar
echo ========================================
echo.
echo HTML Rapor: performance-report.html
echo JSON Veri : performance-summary.json
echo.
echo HTML raporunu acmak icin:
echo   start performance-report.html
echo.
pause
