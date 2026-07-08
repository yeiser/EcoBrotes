# 📊 SonarQube/SonarCloud Analysis - EcoBrotes.NET

## 📋 Configuración del Proyecto

Este proyecto está configurado para análisis de calidad de código con **SonarCloud** (recomendado) o **SonarQube Local**.

---

## 🚀 Opción 1: SonarCloud (Recomendado - Gratis para repos públicos)

### Paso 1: Crear cuenta en SonarCloud
1. Ve a https://sonarcloud.io
2. Inicia sesión con tu cuenta GitHub (`yeiser`)
3. Acepta los permisos para acceder a tu repositorio

### Paso 2: Agregar el proyecto
1. En el dashboard de SonarCloud, haz clic en **"Add Project"**
2. Selecciona **"GitHub"** como proveedor
3. Busca y selecciona **"yeiser/EcoBrotes"**
4. Elige el método de configuración: **"Set Up Manually"**

### Paso 3: Configurar token de autenticación
1. Ve a **Your Account → Security** (https://sonarcloud.io/account/security)
2. Genera un **New Token** con el nombre "EcoBrotes"
3. **Copia el token** (no se mostrará nuevamente)

### Paso 4: Agregar el token como Secret en GitHub
1. Ve a tu repositorio: https://github.com/yeiser/EcoBrotes
2. Haz clic en **Settings → Secrets and variables → Actions**
3. Haz clic en **"New repository secret"**
4. Nombre: `SONAR_TOKEN`
5. Valor: Pega el token que copiaste en el Paso 3
6. Haz clic en **"Add secret"**

### Paso 5: Ejecutar análisis manual (opcional)

#### Instalación del Scanner
```powershell
dotnet tool install --global dotnet-sonarscanner
```

#### Ejecutar Análisis
```powershell
cd d:\Proyectos\ADN-Ceiba\NetCore8.1.0.0\EcoBrotes

# 1. Begin SonarScanner
dotnet sonarscanner begin ^
  /k:"EcoBrotes" ^
  /o:"yeiser" ^
  /d:sonar.login="TU_TOKEN_AQUI" ^
  /d:sonar.host.url="https://sonarcloud.io" ^
  /d:sonar.projectName="EcoBrotes.NET"

# 2. Build y Test con Coverage
dotnet restore EcoBrotes.sln
dotnet build EcoBrotes.sln --configuration Release
dotnet test EcoBrotes.sln ^
  --no-build ^
  --logger "trx;LogFileName=test-results.trx" ^
  /p:CollectCoverage=true ^
  /p:CoverletOutputFormat=opencover ^
  /p:CoverletOutput="./TestResults/coverage.opencover.xml"

# 3. End SonarScanner
dotnet sonarscanner end /d:sonar.login="TU_TOKEN_AQUI"
```

### Paso 6: Análisis automático con GitHub Actions
El archivo `.github/workflows/sonarcloud.yml` está configurado para:
- Ejecutar análisis en cada push a `master`
- Ejecutar análisis en cada Pull Request
- Generar reportes de cobertura de código

---

## 🖥️ Opción 2: SonarQube Local

### Paso 1: Instalar SonarQube
1. Descarga desde: https://www.sonarqube.org/downloads/
2. Sigue las instrucciones de instalación para Windows
3. Inicia el servidor: `StartSonar.bat`
4. Accede a: http://localhost:9000

### Paso 2: Crear proyecto en SonarQube
1. Inicia sesión (admin/admin por defecto)
2. Ve a **Projects → Create Project**
3. Haz clic en **"Generate Token"** en User Security
4. Copa el token y el URL de tu servidor local

### Paso 3: Configurar y ejecutar
```powershell
cd d:\Proyectos\ADN-Ceiba\NetCore8.1.0.0\EcoBrotes

# Modifica el sonar-project.properties con tus datos:
# sonar.host.url=http://localhost:9000
# sonar.login=TU_TOKEN_LOCAL

# Ejecutar análisis
dotnet sonarscanner begin /k:"EcoBrotes" /d:sonar.login="TU_TOKEN" /d:sonar.host.url="http://localhost:9000"
dotnet build EcoBrotes.sln
dotnet sonarscanner end /d:sonar.login="TU_TOKEN"
```

---

## 📊 Métricas que analiza SonarQube

### 🐛 Bugs
- Código que puede causar fallos en producción
- Errores de lógica, manejo de excepciones

### 🛡️ Vulnerabilidades
- Problemas de seguridad (SQL Injection, XSS, etc.)
- Dependencias con vulnerabilidades conocidas

### 🔒 Security Smells
- Credenciales hardcodeadas
- Algoritmos de encriptación débiles

### 📏 Code Smells
- Código duplicado
- Complejidad ciclomática alta
- Métodos demasiado largos
- Documentación insuficiente

### 📈 Coverage
- Porcentaje de líneas cubiertas por tests
- Cobertura por tipo (unit, integration)

---

## 🎯 Calidad Esperada (Quality Gates)

| Métrica | Umbral Mínimo |
|---------|---------------|
| Coverage | ≥ 80% |
| Duplicación | ≤ 3% |
| Code Smells | ≤ 20 |
| Bugs | 0 |
| Vulnerabilidades | 0 |
| Technical Debt | ≤ 1% |

---

## 📁 Archivos de Configuración

| Archivo | Propósito |
|---------|-----------|
| `sonar-project.properties` | Configuración del proyecto |
| `.github/workflows/sonarcloud.yml` | Pipeline CI/CD |
| `sonar-analysis.ps1` | Script de análisis local |

---

## 🔧 Troubleshooting

### Error: "Token is invalid"
- Verifica que el token no haya expirado
- Regenera un nuevo token en SonarCloud

### Error: "No test reports found"
- Asegúrate de instalar Coverlet: `dotnet tool install --global coverlet.console`
- Verifica que los tests se ejecuten con `--logger trx`

### Error: "Project already exists"
- Usa un projectKey diferente en sonar-project.properties

### Errores de build durante análisis
- Ejecuta `dotnet restore` antes del análisis
- Verifica que la solución compile sin errores

---

## 📞 Soporte

- Documentación SonarCloud: https://docs.sonarcloud.io/
- Documentación SonarQube: https://docs.sonarqube.org/
- Issues del proyecto: https://github.com/yeiser/EcoBrotes/issues
