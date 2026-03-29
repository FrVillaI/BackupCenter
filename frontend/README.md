# BackupCenter Frontend

Frontend del sistema **BackupCenter**, una aplicación SPA desarrollada con **Angular 18** para la gestión de respaldos empresariales.

Este cliente consume una API en .NET y permite administrar empresas, ejecutar respaldos manuales o masivos y configurar automatización.
---

## Descripción general

El sistema está diseñado para:

* Gestionar múltiples empresas y sus rutas de respaldo
* Ejecutar backups manuales y masivos
* Configurar frecuencia y horarios de respaldo
* Importar configuraciones desde archivos DBF
* Validar integridad mediante hash

---

##  Características principales

## Autenticación

* Login tradicional (/login)
* Modal de autenticación para acciones críticas (backup manual)
* Integración con JWT

## Dashboard interactivo

Separación de empresas:

* Activas
* Inactivas
* Expansión dinámica de tarjetas
* Edición de configuración en tiempo real

## Gestión de respaldos

* Backup individual por empresa
* Backup masivo (con progreso visual)
* Indicadores de estado (loading por empresa)
* Visualización de:
    * archivo .zip
    * hash SHA-256
    * ruta de almacenamiento

## Importación de datos
* Importación desde archivo DBF
* Recarga automática de empresas tras importación

## Sistema de notificaciones
* Toasts para feedback del usuario
* Estados visuales (loading, disabled, progreso)
---

## Arquitectura

Componentes principales

```
DashboardComponent
├── Gestión de empresas
├── Backup individual / masivo
├── Estado UI (selección, loading, progreso)
└── Integración con modal de login

LoginComponent
└── Autenticación principal

LoginModalComponent
└── Autenticación contextual (acciones sensibles)
```

---

## Servicios

```
AuthService
├── Login / Logout
├── Manejo de JWT
└── Estado de sesión

BackupService
├── Ejecutar backups
└── Listar respaldos

EmpresaService
├── Obtener empresas
└── Activar / desactivar
```

---

## Flujo de autenticación

```
LoginComponent → AuthService → localStorage
                           ↓
                    AuthInterceptor
                           ↓
                        Backend
```

---

## Estructura del proyecto

```
src/
├── app/
│   ├── dashboard/       # Vista principal
│   ├── login/           # Login tradicional
│   ├── login-modal/     # Modal de autenticación
│   ├── services/        # Servicios HTTP
│   └── app.routes.ts    # Rutas
```

---

##  Requisitos previos

- Node.js (versión 18 o superior recomendada)
- Angular CLI instalado globalmente:

```
  npm install -g @angular/cli
```

---

##   Instalación y ejecución
Clonar el repositorio:

```
cd frontend
```

---

## Instalar dependencias:

```
npm install
```

---

## Configurar la URL de la API 

Iniciar servidor de desarrollo:

```
ng serve
```

Acceder a:

```
http://localhost:4200/
```

---

## Seguridad

* Uso de JWT para autenticación
* Token almacenado en localStorage
* Interceptor HTTP para adjuntar token automáticamente

## Consideraciones:

* No se valida expiración del token en frontend
* localStorage es vulnerable a XSS (mejorable con cookies HttpOnly)

---

## Tecnologías utilizadas

* Angular 18
* TypeScript
* RxJS
* Angular Standalone Components
* HTTP Client

## Próximas mejoras

* Guards reales de autenticación
* Manejo de expiración JWT
* Refactor a arquitectura por capas
* Control de concurrencia en procesos
* UI/UX más robusta (sin alert)