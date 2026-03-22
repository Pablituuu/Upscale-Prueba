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

### 🐳 Opción 1: Docker Compose (Recomendado)
Levanta todo el entorno (Base de Datos + App) con un solo comando:
1.  Asegúrate de tener **Docker** y **Docker Compose** instalados.
2.  En la raíz del proyecto, ejecuta:
    ```bash
    docker-compose up --build -d
    ```
3.  La aplicación estará disponible en `http://localhost:8080`.
4.  Para crear la estructura inicial, ejecuta el script `add_col.sql` en el servidor SQL (`localhost:1433`) desde DBeaver.

### 💻 Opción 2: Ejecución Local (.NET SDK)
1.  **Requisitos:** .NET 8 SDK y SQL Server local.
2.  **Seguridad:** Para no exponer credenciales en GitHub, configura tu conexión local mediante **User Secrets**:
    ```bash
    dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=UpscaleDB;User Id=sa;Password=TU_PASSWORD;TrustServerCertificate=True"
    ```
3.  **Base de Datos:**
    *   Ejecuta las migraciones: `dotnet ef database update`
    *   O ejecuta el script `add_col.sql` para tener al usuario `admin` de prueba.
4.  **Ejecución:**
    ```bash
    dotnet run
    ```
    La app estará en `https://localhost:5001`.

### 🔑 Credenciales de Acceso (Admin)
*   **Usuario:** `admin`
*   **Contraseña:** `Upscale2026`
*   *Nota: Estas credenciales se inyectan mediante el script `add_col.sql`.*


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
