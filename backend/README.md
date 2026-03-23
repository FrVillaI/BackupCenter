# Microservicio de Respaldos (Backup Center)

## Resumen

Microservicio desarrollado en **.NET 8** que permite gestionar respaldos automáticos y manuales de carpetas empresariales.

### Funcionalidades principales:

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

* API REST para control y ejecución
* Base preparada para integración con frontend (dashboard)

---

## Estructura del Proyecto

```
BackupCenter/
│
├── BackupCenter.API              # API principal (controladores)
├── BackupCenter.Application      # Lógica de negocio (servicios)
├── BackupCenter.Domain           # Entidades
├── BackupCenter.Data             # DbContext + SQLite + migraciones
├── BackupCenter.Infrastructure   # Servicios internos (scheduler, etc.)
```

---

## Configuración importante

### Archivo DBF (origen de datos)

Ruta esperada:

```
C:\Fenix\AdsFenix\Datos_Pro\EMPRESAS.dbf
```

---

### Carpeta de respaldos

Ruta por defecto:

```
C:\Fenix\Backups
```

> Nota: Esta ruta está definida en código (`BackupService`).

---

## Base de Datos

* Motor: **SQLite**
* Archivo generado automáticamente:

```
BackupCenter.db
```

### Tablas principales:

* `Usuarios`
* `Empresas`
* `Backups`
* `Logs`

---

## Autenticación

Usuarios iniciales generados automáticamente:

| Usuario | Password | Rol     |
| ------- | -------- | ------- |
| ads     | password | ADMIN   |
| gerente | password | GERENTE |

>  Passwords encriptados con BCrypt

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

```
http://localhost:5000
```

Swagger:

```
http://localhost:5000/swagger
```

---

## Scheduler (Backups Automáticos)

El sistema incluye un servicio en segundo plano que:

* Revisa cada 60 segundos
* Compara la hora actual con `HoraProgramada`
* Ejecuta el backup automáticamente si coincide

Formato requerido:

```
HH:mm
Ejemplo: 22:30
```

---

##  Flujo de Backup

1. Obtiene ruta origen desde DB (`Empresas.RutaOrigen`)
2. Valida existencia y permisos
3. Copia archivos a carpeta temporal (staging)
4. Genera archivo `.zip`
5. Calcula hash SHA-256
6. Guarda registro en:

   * Tabla `Backups`
   * Tabla `Logs`

---

##  Consideraciones importantes

###  Rutas

* Las rutas deben existir físicamente
* Deben tener permisos de lectura/escritura

---

###  Límite de tamaño

* Máximo permitido: **50 GB por respaldo**

---

###  Errores comunes

* Ruta inexistente → `DirectoryNotFoundException`
* Permisos insuficientes → `UnauthorizedAccessException`
* Espacio insuficiente → fallo en compresión

---


##  Estado del Proyecto

 Funcional para pruebas
 Arquitectura en capas implementada
 Aún no optimizado para producción
