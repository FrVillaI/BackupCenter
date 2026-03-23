# BackupCenter

Sistema integral de gestión de respaldos empresariales compuesto por:

- **Backend (API REST)** desarrollado en .NET 8  
- **Frontend (SPA)** desarrollado en Angular  

Permite administrar empresas, ejecutar respaldos manuales/automáticos y visualizar información desde un dashboard.

---

## Arquitectura General

[ Angular Frontend ]
│
▼
[ .NET 8 Web API ]
│
▼
[ SQLite Database ]
│
▼
[ Sistema de Archivos (Backups .zip) ]


---

## Componentes del Proyecto

### Backend – BackupCenter API

Microservicio encargado de:

- Gestión de usuarios y autenticación
- Configuración de empresas
- Ejecución de respaldos
- Scheduler automático
- Registro de logs e historial

 Ubicación:
 
/backend

---

### Frontend – BackupCenter SPA

Aplicación web que permite:

- Login de usuarios
- Visualización de dashboard
- Consumo de la API
- Interacción con respaldos

 Ubicación:

/frontend

---

## Tecnologías utilizadas

### Backend

- .NET 8  
- Entity Framework Core  
- SQLite  
- BCrypt  

### Frontend

- Angular 18  
- TypeScript  
- Angular CLI  

---

## Requisitos previos

- Node.js (v18 o superior)  
- Angular CLI  
- .NET SDK 8  
- Git  

---

## Ejecución del proyecto

1. Clonar repositorio

    git clone <url-del-repositorio>
    cd BackupCenter

2. Ejecutar Backend
    cd backend
    dotnet restore
    dotnet ef database update
    dotnet run --project BackupCenter.API

  API disponible en:

    http://localhost:5000

  Swagger:

    http://localhost:5000/swagger

3. Ejecutar Frontend
    cd frontend
    npm install
    ng serve

Aplicación disponible en:

    http://localhost:4200

## Configuración importante

Backend
Ruta DBF:

  C:\Fenix\AdsFenix\Datos_Pro\EMPRESAS.dbf

Carpeta de backups:

  C:\Fenix\Backups
