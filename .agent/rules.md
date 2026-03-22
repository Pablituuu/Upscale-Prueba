# REGLAS DEL PROYECTO: UPSCALEWEB - PRUEBA TÉCNICA

Estas reglas son de lectura obligatoria al iniciar cualquier sesión o chat en este proyecto.

1.  **TIPADO FUERTE (C# / TS):** Queda terminantemente prohibido el uso de `any` o cualquier tipo de dato débil/genérico que oculte la estructura real del objeto.
2.  **SEGURIDAD Y SECRETOS:** No exponer llaves, contraseñas o cadenas de conexión en el código fuente. Utilizar `Secret Manager` (`dotnet user-secrets`) o variables de entorno. Asegurarse de que `appsettings.json` o secretos no se suban con datos sensibles.
3.  **VERIFICACIÓN DE ERRORES:** Antes de finalizar cualquier tarea, se deben revisar todos los archivos modificados para asegurar que no hay errores de sintaxis, falta de puntos y comas, o dependencias no resueltas.
4.  **ESTILOS SEPARADOS (CSS):** No utilizar bloques `<style>` internos ni atributos `style="..."` inline. Usar clases de utilidad de Bootstrap o archivos `.css` externos vinculados. 
5.  **DISEÑO VISUAL:** Todo el diseño de la interfaz de usuario debe realizarse utilizando **BOOTSTRAP 5** (o superior). 
6.  **RESPONSIVE DESIGN:** Es obligatorio que todas las vistas sean totalmente responsivas, adaptándose perfectamente a dispositivos móviles, tablets y escritorio.
7.  **CONSISTENCIA VISUAL:** Todos los elementos de interfaz (Toasts, Modales, Alertas) deben mantener una línea visual única basada en el sistema de diseño del proyecto (Rojo Institucional y Azul Activo). Está prohibido mezclar estilos visuales distintos para funcionalidades similares.
8.  **EJECUCIÓN REAL:** Queda estrictamente prohibido afirmar que se ha realizado un cambio en un archivo sin haber llamado exitosamente a la herramienta de edición correspondiente (`write_to_file`, `replace_file_content`, etc.). El asistente debe validar siempre el éxito de la herramienta antes de notificar al usuario.

---
**Contexto de la Prueba:**
*   Proyecto: Sistema de Autenticación y Gestión de Perfiles (Entidad Bancaria/Financiera).
*   Tecnologías: ASP.NET Core MVC (.NET 8), SQL Server, BCrypt para hashing.
*   Flujo: Login con bloqueo tras 3 intentos fallidos y redirección a Perfil.
