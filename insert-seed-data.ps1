# Script para insertar datos semilla en EcoBrotes API
# Uso: .\insert-seed-data.ps1

$baseUrl = "http://localhost:8080"
$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  EcoBrotes - Insertar Datos Semilla" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Funcion helper para hacer POST y obtener el ID retornado
function Invoke-ApiPost {
    param(
        [string]$Endpoint,
        [object]$Body
    )
    $response = Invoke-RestMethod -Uri "$baseUrl$Endpoint" -Method Post -Body ($Body | ConvertTo-Json) -ContentType "application/json"
    return $response
}

# ========================================
# 1. Insertar Zonas de Prueba
# ========================================
Write-Host "1. Insertando zonas urbanas..." -ForegroundColor Yellow

$zonas = @(
    @{ Name = "Parque Central" },
    @{ Name = "Plaza Bolivar" },
    @{ Name = "Jardin Botanico" },
    @{ Name = "Par Simon Rodriguez" },
    @{ Name = "Plaza Sucre" }
)

$zonasData = @()
foreach ($zona in $zonas) {
    try {
        $id = Invoke-ApiPost -Endpoint "/api/zonas" -Body $zona
        $zonasData += @{ Name = $zona.Name; Id = $id }
        Write-Host "  [OK] Zona creada: $($zona.Name) (ID: $id)" -ForegroundColor Green
    } catch {
        Write-Host "  [FAIL] Error al crear zona '$($zona.Name)': $_" -ForegroundColor Red
    }
}

Write-Host ""

# ========================================
# 2. Insertar Especies de Prueba
# ========================================
Write-Host "2. Insertando especies arboreas..." -ForegroundColor Yellow

$especies = @(
    @{ Name = "Ceiba"; ScientificName = "Ceiba pentandra"; MaxHeightMeters = 50.0 },
    @{ Name = "Nogal"; ScientificName = "Juglans regia"; MaxHeightMeters = 20.0 },
    @{ Name = "Eucalipto"; ScientificName = "Eucalyptus globulus"; MaxHeightMeters = 60.0 },
    @{ Name = "Pino"; ScientificName = "Pinus patula"; MaxHeightMeters = 40.0 },
    @{ Name = "Guayacan"; ScientificName = "Tabebuia chrysantha"; MaxHeightMeters = 25.0 },
    @{ Name = "Mango"; ScientificName = "Mangifera indica"; MaxHeightMeters = 20.0 }
)

$especiesData = @()
foreach ($especie in $especies) {
    try {
        $id = Invoke-ApiPost -Endpoint "/api/especies" -Body $especie
        $especiesData += @{ Name = $especie.Name; Id = $id }
        Write-Host "  [OK] Especie creada: $($especie.Name) (ID: $id)" -ForegroundColor Green
    } catch {
        Write-Host "  [FAIL] Error al crear especie '$($especie.Name)': $_" -ForegroundColor Red
    }
}

Write-Host ""

# ========================================
# 3. Insertar Jornadas de Prueba
# ========================================
Write-Host "3. Insertando jornadas de reforestacion..." -ForegroundColor Yellow

# Usar las primeras 3 zonas y 4 especies
$zona1 = $zonasData[0].Id
$zona2 = $zonasData[1].Id
$zona3 = $zonasData[2].Id

$especie1 = $especiesData[0].Id
$especie2 = $especiesData[1].Id
$especie3 = $especiesData[2].Id
$especie4 = $especiesData[3].Id

# Fechas en UTC (PostgreSQL requiere UTC para timestamp with time zone)
$jornadas = @(
    @{
        ZonaUrbanaId = $zona1
        Name = "Jornada Parque Central - Primavera"
        ScheduledDate = "2026-08-15T08:00:00Z"
        TreeMeta = 50
        VolunteerCapacity = 30
        DetalleEspecies = @(
            @{ EspecieArboreaId = $especie1; Quantity = 20 },
            @{ EspecieArboreaId = $especie2; Quantity = 30 }
        )
    },
    @{
        ZonaUrbanaId = $zona2
        Name = "Jornada Plaza Bolivar - Verano"
        ScheduledDate = "2026-09-10T07:00:00Z"
        TreeMeta = 100
        VolunteerCapacity = 50
        DetalleEspecies = @(
            @{ EspecieArboreaId = $especie3; Quantity = 40 },
            @{ EspecieArboreaId = $especie4; Quantity = 60 }
        )
    },
    @{
        ZonaUrbanaId = $zona3
        Name = "Jornada Jardin Botanico - Otono"
        ScheduledDate = "2026-10-20T08:30:00Z"
        TreeMeta = 75
        VolunteerCapacity = 40
        DetalleEspecies = @(
            @{ EspecieArboreaId = $especie1; Quantity = 25 },
            @{ EspecieArboreaId = $especie3; Quantity = 25 },
            @{ EspecieArboreaId = $especie4; Quantity = 25 }
        )
    },
    @{
        ZonaUrbanaId = $zona1
        Name = "Jornada Parque Central - Invierno"
        ScheduledDate = "2026-11-05T08:00:00Z"
        TreeMeta = 40
        VolunteerCapacity = 25
        DetalleEspecies = @(
            @{ EspecieArboreaId = $especie2; Quantity = 40 }
        )
    }
)

$jornadasCount = 0
foreach ($jornada in $jornadas) {
    try {
        $id = Invoke-ApiPost -Endpoint "/api/jornadas" -Body $jornada
        $jornadasCount++
        Write-Host "  [OK] Jornada creada: $($jornada.Name) (ID: $id)" -ForegroundColor Green
    } catch {
        Write-Host "  [FAIL] Error al crear jornada '$($jornada.Name)': $_" -ForegroundColor Red
    }
}

Write-Host ""

# ========================================
# Resumen
# ========================================
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Resumen de Datos Insertados" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "  Zonas:        $($zonasData.Count)" -ForegroundColor White
Write-Host "  Especies:     $($especiesData.Count)" -ForegroundColor White
Write-Host "  Jornadas:     $jornadasCount" -ForegroundColor White
Write-Host ""

# Mostrar IDs para referencia
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  IDs de Referencia" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "  Zonas:" -ForegroundColor Yellow
foreach ($z in $zonasData) {
    Write-Host "    - $($z.Name): $($z.Id)" -ForegroundColor Gray
}
Write-Host ""
Write-Host "  Especies:" -ForegroundColor Yellow
foreach ($e in $especiesData) {
    Write-Host "    - $($e.Name): $($e.Id)" -ForegroundColor Gray
}
Write-Host ""

Write-Host "Datos semilla insertados correctamente!" -ForegroundColor Green
Write-Host ""
