# Script para insertar jornadas de prueba en EcoBrotes API
# Las zonas y especies ya existen desde la ejecucion anterior

$baseUrl = "http://localhost:8080"
$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  EcoBrotes - Insertar Jornadas" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# IDs de datos existentes (de ejecucion anterior)
$zona1 = "9fdcc19b-e395-44fb-af2f-5731806823af"  # Parque Central
$zona2 = "4c9c08ae-eca0-4878-98ad-5e20c3691a2b"  # Plaza Bolivar
$zona3 = "e39b4571-08ee-486c-b353-1a8cfba56312"  # Jardin Botanico

$especie1 = "fe9484b4-d593-4d08-8aa6-06660521a555"  # Ceiba
$especie2 = "a0933a04-325e-47b3-bc9e-d4e2415661f6"  # Nogal
$especie3 = "72905dda-1882-4a05-853e-6e9b2b18c118"  # Eucalipto
$especie4 = "904d22ad-507a-45e1-8d06-065385559247"  # Pino

Write-Host "Zonas existentes:" -ForegroundColor Yellow
Write-Host "  - Parque Central: $zona1" -ForegroundColor Gray
Write-Host "  - Plaza Bolivar: $zona2" -ForegroundColor Gray
Write-Host "  - Jardin Botanico: $zona3" -ForegroundColor Gray
Write-Host ""
Write-Host "Especies existentes:" -ForegroundColor Yellow
Write-Host "  - Ceiba: $especie1" -ForegroundColor Gray
Write-Host "  - Nogal: $especie2" -ForegroundColor Gray
Write-Host "  - Eucalipto: $especie3" -ForegroundColor Gray
Write-Host "  - Pino: $especie4" -ForegroundColor Gray
Write-Host ""

# Funcion helper para hacer POST
function Invoke-ApiPost {
    param(
        [string]$Endpoint,
        [object]$Body
    )
    try {
        $jsonBody = $Body | ConvertTo-Json -Depth 10
        Write-Host "  DEBUG: Sending JSON: $jsonBody" -ForegroundColor DarkGray
        $response = Invoke-RestMethod -Uri "$baseUrl$Endpoint" -Method Post -Body $jsonBody -ContentType "application/json"
        return @{ Success = $true; Data = $response }
    } catch {
        $errorMsg = $_.Exception.Message
        if ($_.Exception.Response) {
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $reader.BaseStream.Position = 0
            $errorResponse = $reader.ReadToEnd()
            $errorMsg = $errorResponse
        }
        return @{ Success = $false; Error = $errorMsg }
    }
}

# ========================================
# Insertar Jornadas
# ========================================
Write-Host "Insertando jornadas..." -ForegroundColor Yellow
Write-Host ""

$jornadas = @(
    @{
        ZonaUrbanaId = $zona1
        Name = "Jornada Parque Central - Primavera"
        ScheduledDate = "2026-08-15T08:00:00+00:00"
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
        ScheduledDate = "2026-09-10T07:00:00+00:00"
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
        ScheduledDate = "2026-10-20T08:30:00+00:00"
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
        ScheduledDate = "2026-11-05T08:00:00+00:00"
        TreeMeta = 40
        VolunteerCapacity = 25
        DetalleEspecies = @(
            @{ EspecieArboreaId = $especie2; Quantity = 40 }
        )
    }
)

$jornadasCount = 0
foreach ($jornada in $jornadas) {
    Write-Host "Creando: $($jornada.Name)..." -ForegroundColor White
    $result = Invoke-ApiPost -Endpoint "/api/jornadas" -Body $jornada
    if ($result.Success) {
        $jornadasCount++
        Write-Host "  [OK] ID: $($result.Data)" -ForegroundColor Green
    } else {
        Write-Host "  [FAIL] Error: $($result.Error)" -ForegroundColor Red
    }
    Write-Host ""
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Resumen" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "  Jornadas creadas: $jornadasCount" -ForegroundColor White
Write-Host ""
Write-Host "Listo!" -ForegroundColor Green
