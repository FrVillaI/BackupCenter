# Backup Center - Backend

## Descripción

**Backup Center** es un microservicio desarrollado en **.NET** 8 que gestiona respaldos automáticos y manuales de carpetas empresariales, con control de acceso, auditoría y programación de tareas.

Está diseñado bajo una **arquitectura en capas (Clean Architecture simplificada)** para facilitar mantenimiento, escalabilidad y comprensión del código.

### Funcionalidades

* Autenticación mediante **JWT**
* Gestión de usuarios con roles:
  * ADMIN
  * GERENTE

* Gestión de empresas y configuración de backups
* Importación de configuraciones desde archivo `EMPRESAS.dbf` (modo solo lectura)

* Almacenamiento en base de datos **SQLite**
  * Usuarios
  * Empresas
  * Logs
  * Historial de backups

* Autenticación básica con usuarios preconfigurados:
  * `ads` (ADMIN)
  * `gerente` (GERENTE)

* Ejecución de respaldos:
  * Copia de carpetas
  * Compresión en formato `.zip`
  * Generación de hash SHA-256
  * Registro automático en logs

* API REST con Swagger para control y ejecución
* Base preparada para integración con frontend (dashboard)

---

## Arquitectura
El proyecto sigue una separación clara de responsabilidades:

```
BackupCenter/
│
├── BackupCenter.Api             → Exposición HTTP (Controllers, Middleware)
├── BackupCenter.Application     → Lógica de negocio (Services, DTOs)
├── BackupCenter.Domain          → Entidades del dominio
├── BackupCenter.Data            → Acceso a datos (DbContext, EF Core)
├── BackupCenter.Infrastructure  → Servicios internos (Scheduler, Background Jobs)
```

## Flujo general

```
Controller → Service → DbContext → SQLite
```

---

## Configuración 

## Archivo appsettings.json

```
{
  "ConnectionStrings": {
    // Ruta de la base de datos SQLite
    "DefaultConnection": "Data Source=BackupCenter.db"
  },
  "JwtConfig": {
    // Clave secreta para firmar tokens (DEBE ser segura en producción)
    "SecretKey": "SUPER_SECRET_KEY_LARGA_Y_SEGURA_123456",

    // Emisor del token
    "Issuer": "BackupCenter",

    // Destinatario del token
    "Audience": "BackupCenterClient"
  },
  "BackupSettings": {
    // Ruta donde se almacenan los backups generados
    "RootPath": "C:\\BackupCenter\\Backups"
  }
}

```

### Archivo DBF (origen de datos)

Ruta esperada:

```
C:\BackupCenter\EMPRESAS.dbf
```

---

## Base de Datos

* Motor: **SQLite**
* ORM: Entity Framework Core
* Archivo generado automáticamente:

```
BackupCenter.db
```

---

### Tablas principales:

* `Usuarios` -> `Usuarios del sistema`
* `Empresas` -> `Configuración de backups`
* `Backups`  -> `Historial de respaldos`
* `Logs`     -> `Auditoría del sistema`

---

## Autenticación

Se utiliza **JWT (Json Web Token).**

Usuarios iniciales generados automáticamente **por defecto:**

| Usuario | Password | Rol     |
| ------- | -------- | ------- |
| ads     | password | ADMIN   |
| gerente | password | GERENTE |

>  Las contraseñas se almacenan usando **BCrypt**

---

## Scheduler (Backups Automáticos)

Implementado mediante:
* BackgroundService
* Clase: BackupSchedulerService

Comportamiento:
* Ejecuta cada 60 segundos
* Evalúa:
    * HoraProgramada
    * FrecuenciaHoras
* Evita duplicados por día usando memoria (ConcurrentDictionary)

---

##  Flujo de Backup

1. Validación de empresa activa
2. Valida de ruta origen
3. Copia archivos a carpeta temporal (staging)
4. Compresión archivo `.zip`
5. Generación de hash **SHA-256**
6. Guarda registro en:

   * Tabla `Backups`
   * Tabla `Logs`

---

## Endpoints principales

## Auth

```
POST /api/auth/login
```

## Backups

```
POST /api/backups/{empresaId}
```

## Empresas

```
GET    /api/empresas
PUT    /api/empresas/{id}/toggle-activa
PUT    /api/empresas/{id}/config
```

## Importación DBF

```
POST /api/importdbf?path=...
```
---

## Manejo de archivos

## Carpeta de backups

```
C:\BackupCenter\Backups
```

Estructura generada: 

```
/Empresa_X/
   ├── Backup_20260328_220000.zip
   ├── Backup_20260328_220000.zip.sha256
```

---

##  Consideraciones importantes

###  Rutas

* Las rutas deben existir físicamente
* Empresa debe estar activa
---

###  Límite de tamaño

* Tamaño maximo por backup: **50 GB por respaldo**

---

## Manejo de concurrencia

* Lock por empresa usando:

```
ConcurrentDictionary<int, SemaphoreSlim>
```

###  Errores comunes

* Ruta inexistente → `DirectoryNotFoundException`
* Permisos insuficientes → `UnauthorizedAccessException`
* Espacio insuficiente → fallo en compresión

---

##  Ejecución del Proyecto

### 1. Restaurar dependencias

```bash
dotnet restore
```

### 2. Aplicar migraciones (si es necesario)

```bash
dotnet ef database update
```

### 3. Ejecutar la API

```bash
dotnet run --project BackupCenter.API
```

---

## Acceso

Una vez ejecutado:

**API**

```
http://localhost:5000
```

**Swagger**

```
http://localhost:5000/swagger
```

---

##  Mejoras futuras

* Encriptación de base de datos SQLite
* Encriptación de archivos .zip
* Dashboard avanzado
* Almacenamiento en nube
* Notificaciones de errores

---