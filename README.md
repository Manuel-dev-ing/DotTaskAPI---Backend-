# 📌 DotTask – Backend (ASP.NET Core)

DotTask es un sistema colaborativo de administración de tareas que permite a equipos organizar proyectos, asignar tareas y colaborar en tiempo real.  
Este repositorio corresponde al **backend**, desarrollado con **ASP.NET Core** y **Entity Framework Core**, siguiendo buenas prácticas como el **Patrón Repository** y principios de arquitectura limpia.

## ⚙️ Funcionalidades principales

- 🔐 **Autenticación con JSON Web Tokens (JWT)**  
- 📧 **Restablecimiento de contraseña por correo electrónico**  
- 🧩 **Creación y gestión de proyectos y tareas**  
- 🔄 **Cambio de estado de tareas** con historial de modificaciones  
- 🧑‍🤝‍🧑 **Agregar colaboradores a proyectos** con control de permisos  
- 📝 **Notas por tarea** para mayor seguimiento  
- 🧠 **Sistema de roles** (Administrador y Colaborador)  
- 🖱️ **Gestión visual de tareas estilo Kanban** con soporte para Drag and Drop *(soportado en frontend)*  

### 🔧 Backend
- **ASP.NET Core** – Framework principal para la API REST.  
- **Entity Framework Core** – ORM para el manejo de datos.  
- **SQL Server** – Base de datos relacional.  
- **Patrón Repository** – Separación de la lógica de acceso a datos.  
- **Sistema de envío de correos** – Para recuperación de contraseña.  

### 🧪 Herramientas
- **Postman** – Pruebas de endpoints de la API.  

## 🚀 Instalación y ejecución local (Backend)

### 1. Requisitos previos
- [Visual Studio 2022 o superior](https://visualstudio.microsoft.com/) con soporte para **ASP.NET Core**.  
- [.NET 8 SDK o superior](https://dotnet.microsoft.com/en-us/download).  
- [SQL Server](https://www.microsoft.com/es-es/sql-server/sql-server-downloads) (puede ser Express).  
- [Postman](https://www.postman.com/) (opcional, para probar la API).

### 2. Clonar el repositorio
```bash
  git clone https://github.com/tu-usuario/dottask-backend.git
```
Abrir la solución .sln en Visual Studio.

### 3. Configurar la cadena de conexión
En el archivo appsettings.json, configura la conexión a tu base de datos SQL Server:
```
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=DotTaskDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
```
### 4. Restaurar dependencias
Visual Studio restaurará los paquetes NuGet automáticamente, pero también puedes hacerlo manualmente:
```
  dotnet restore
```
### 5. Migraciones y base de datos
Si aún no existe la base de datos, aplica las migraciones con:
```
  dotnet ef database update

```
### 6. Ejecutar el proyecto
Presiona F5 en Visual Studio o ejecuta:
```
  dotnet run
```
## Licencia

 DotTask – Backend es [MIT licensed](./LICENSE).

## Contacto
**Nombre:** Manuel Tamayo Montero.

**Correo:** manueltamayo9765@gmail.com


