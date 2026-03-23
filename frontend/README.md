# BackupCenter Frontend

Frontend para el sistema BackupCenter.  
Aplicación de una sola página (SPA) desarrollada con **Angular** que consume la API de BackupCenter. Incluye autenticación, rutas protegidas y una estructura modular lista para escalar.

> Generado con Angular CLI versión 18.2.21.

---

##  Características

-  **Login** – Autenticación de usuarios.
-  **Dashboard protegido** – Rutas con guards para control de acceso.
-  **Servicios** – Comunicación con el backend mediante servicios centralizados.
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

Navegar a http://localhost:4200/. La aplicación se recargará automáticamente al realizar cambios.

---

## Estructura del proyecto

```
src/
├── app/
│   ├── services/        # Servicios para API y autenticación
│   └── app.module.ts    # Módulo principal de la aplicación
└── environments/        # Configuración por entorno (development, production)
```

---

## Seguridad
Se utilizan guards para proteger rutas como el dashboard.

El token de autenticación se almacena en localStorage (puede ajustarse según necesidades de seguridad).

---

## Tecnologías utilizadas
Angular 18
TypeScript
Angular CLI