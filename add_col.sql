-- =============================================
-- Script de Creación de Base de Datos y Semilla
-- Proyecto: UpscaleWeb - Prueba Técnica
-- =============================================

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

-- 1. Crear Tabla de Detalles de Usuario
IF OBJECT_ID(N'[UsuariosDetalle]') IS NULL
BEGIN
    CREATE TABLE [UsuariosDetalle] (
        [UsuarioId] int NOT NULL IDENTITY,
        [Estado] bit NULL DEFAULT 1,
        [NombresCompletoCabecera] nvarchar(150) NULL,
        [Rol] nvarchar(100) NULL,
        [Entidad] nvarchar(150) NULL,
        [Nombres] nvarchar(100) NULL,
        [PrimerApellido] nvarchar(100) NULL,
        [SegundoApellido] nvarchar(100) NULL,
        [TipoDocumento] nvarchar(50) NULL,
        [NumeroDocumento] nvarchar(20) NULL,
        [FechaNacimiento] date NULL,
        [Nacionalidad] nvarchar(100) NULL,
        [Sexo] char(1) NULL,
        [EmailPrincipal] nvarchar(255) NULL,
        [EmailSecundario] nvarchar(255) NULL,
        [TelefonoSecundario] nvarchar(20) NULL,
        [TelefonoMovil] nvarchar(20) NULL,
        [TipoContratacion] nvarchar(50) NULL,
        [FechaContratacion] date NULL,
        [FotoUrl] nvarchar(500) NULL,
        CONSTRAINT [PK_UsuariosDetalle] PRIMARY KEY ([UsuarioId])
    );
END;
GO

-- 2. Crear Tabla de Cuentas de Acceso
IF OBJECT_ID(N'[Cuentas]') IS NULL
BEGIN
    CREATE TABLE [Cuentas] (
        [Id] int NOT NULL IDENTITY,
        [Username] nvarchar(50) NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [IntentosFallidos] int NULL DEFAULT 0,
        [BloqueadoHasta] datetime NULL,
        [UsuarioDetalleId] int NOT NULL,
        [RequiereCambioPassword] bit NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Cuentas] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Cuentas_UsuariosDetalle] FOREIGN KEY ([UsuarioDetalleId]) REFERENCES [UsuariosDetalle] ([UsuarioId]) ON DELETE CASCADE
    );
END;
GO

-- 3. Índices de Rendimiento y Unicidad
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ_Cuentas_Username')
    CREATE UNIQUE INDEX [UQ_Cuentas_Username] ON [Cuentas] ([Username]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ_UsuariosDetalle_DNI' AND object_id = OBJECT_ID('[UsuariosDetalle]'))
    CREATE UNIQUE INDEX [UQ_UsuariosDetalle_DNI] ON [UsuariosDetalle] ([NumeroDocumento]) WHERE [NumeroDocumento] IS NOT NULL;
GO

-- 4. INSERTAR USUARIO ADMINISTRADOR POR DEFECTO
-- Contraseña por defecto: Upscale2026 (BCrypt Hashed)
IF NOT EXISTS (SELECT 1 FROM [Cuentas] WHERE [Username] = 'admin')
BEGIN
    -- Primero creamos el detalle
    INSERT INTO [UsuariosDetalle] (
        [Estado], [NombresCompletoCabecera], [Rol], [Entidad], [Nombres], 
        [PrimerApellido], [SegundoApellido], [TipoDocumento], [NumeroDocumento], 
        [EmailPrincipal], [Nacionalidad], [Sexo]
    )
    VALUES (
        1, 'Administrador del Sistema', 'Administrador', 'Upscale HQ', 'Admin', 
        'Upscale', 'System', 'DNI', '00000000', 
        'admin@upscale.com.pe', 'Peruana', 'M'
    );

    -- Obtenemos el ID y creamos la cuenta
    DECLARE @AdminId int = SCOPE_IDENTITY();

    INSERT INTO [Cuentas] ([Username], [PasswordHash], [UsuarioDetalleId], [RequiereCambioPassword])
    VALUES ('admin', '$2a$11$R9h/lIPzHZluvJum65wLUOg9m30JtTjUp4W6U39O6X19p9XG6O5pG', @AdminId, 0);
END;
GO

COMMIT;
GO
