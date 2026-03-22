# 🚀 UpscaleWeb - Prueba Técnica: Programador Web

Este repositorio contiene la implementación técnica para el proceso de selección de **Upscale**. El objetivo es demostrar habilidades sólidas en desarrollo Web utilizando .NET 8, C#, Bootstrap 5 y Entity Framework Core.

## 🖼️ Vista Previa del Proyecto

<div align="center">
  <h3>Pantalla de Bienvenida</h3>
  <img src="wwwroot/img/pages/Screenshot from 2026-03-21 20-17-01.png" width="800" alt="Bienvenida">
  <br><br>
  <h3>Inicio de Sesión</h3>
  <img src="wwwroot/img/pages/Screenshot from 2026-03-21 20-17-04.png" width="800" alt="Login">
  <br><br>
  <h3>Perfil de Usuario / Gestión</h3>
  <img src="wwwroot/img/pages/Screenshot from 2026-03-21 20-17-14.png" width="800" alt="Perfil">
</div>

---

---

## 🛠️ Instrucciones de Instalación y Despliegue

Sigue estos pasos para levantar el proyecto en tu entorno local:

### 1. Requisitos Previos
*   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) instalado.
*   [SQL Server](https://www.microsoft.com/es-es/sql-server/sql-server-downloads) Express o Developer Edition.
*   [Entity Framework Core Tools](https://learn.microsoft.com/es-es/ef/core/cli/dotnet) (`dotnet tool install --global dotnet-ef`).

### 2. Configuración de Base de Datos
1.  Abre el archivo `appsettings.json` y ajusta la cadena de conexión `DefaultConnection` con tus credenciales de SQL Server local.
2.  Ejecuta las migraciones para crear la estructura de tablas:
    ```bash
    dotnet ef database update
    ```

### 3. Ejecución del Proyecto
Desde la raíz de la carpeta del proyecto, ejecuta:
```bash
dotnet run
```
La aplicación estará disponible en `https://localhost:5001` (o el puerto configurado por defecto).

### 4. Credenciales de Acceso (Admin por Defecto)
El sistema incluye un usuario administrador inicial para facilitar la evaluación:
*   **Usuario:** `admin`
*   **Contraseña:** `Upscale2026`

_Nota: Si prefieres no usar migraciones, puedes ejecutar el script completo `add_col.sql` directamente en tu servidor SQL para crear la estructura e inyectar el administrador._

---

## 🏗️ Funcionalidades del Sistema (Módulos Soportados)

El sistema ha sido diseñado como un MVP robusto para la gestión de usuarios y seguridad:

*   **🛡️ Autenticación Multimodal:** Inicio de sesión flexible mediante **Nombre de Usuario** o **Documento de Identidad (DNI/CE)**.
*   **👥 Gestión de Roles:** Soporte para roles diferenciados (**Administrador** y **Usuario**).
    *   **Administradores:** Acceso al panel de gestión, creación y edición masiva de colaboradores.
    *   **Usuarios:** Acceso restringido a su perfil personal y cambio de credenciales.
*   **♻️ Gestión Integral de Perfiles:**
    *   Visualización de ficha de colaborador (Card de Identidad).
    *   Actualización de datos personales y laborales.
    *   Edición de **contactos secundarios** (email y teléfono) con validación en tiempo real.
*   **🧩 Seguridad Proactiva:**
    *   Reglas estrictas de contraseña (mínimo 3 números, 3 letras, 1 mayúscula y 1 carácter especial).
    *   Bloqueo temporal de cuenta tras **5 intentos fallidos**.
    *   Forzado de cambio de contraseña tras reseteo administrativo.
*   **🔑 Recuperación de Cuenta:** Sistema de validación de identidad triple (Nombre completo, Documento, ID Usuario) para el reinicio seguro de credenciales.

---

## ⚙️ Tecnologías & Enfoque Técnico

- **Arquitectura:** Patrón MVC (Model-View-Controller) sobre .NET 8.
- **Seguridad:** Hashing de contraseñas con **BCrypt** y autenticación basada en **Cookies**.
- **Frontend Panorámico:** Diseño premium con ilustración corporativa fluida, adaptado 100% a móviles, tablets y escritorio mediante **Bootstrap 5**.

---

## 📤 Entrega del Proyecto

Para la revisión de esta prueba, se han cumplido los siguientes puntos:

- **Explicación del Desarrollo:** Al responder el correo, se incluye una descripción corta del trabajo realizado: qué se hizo, cómo se abordó el problema y qué herramientas se utilizaron.
- **Grabación de Pantalla:** Se adjunta el enlace (YouTube, Loom, etc.) donde se muestra el funcionamiento del sistema, explicando el enfoque y las decisiones técnicas.
- **Repositorio Público:** El enlace a este repositorio de GitHub se comparte en la entrega oficial.

---

_Desarrollado con ❤️ para el equipo de Selección de Upscale._
