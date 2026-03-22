using System;
using Microsoft.EntityFrameworkCore;

namespace UpscaleWeb.Models;

public partial class UpscalePruebaContext : DbContext
{
    public UpscalePruebaContext()
    {
    }

    public UpscalePruebaContext(DbContextOptions<UpscalePruebaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cuenta> Cuentas { get; set; }

    public virtual DbSet<UsuariosDetalle> UsuariosDetalles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cuenta>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cuentas__3214EC0773F8C68F");

            entity.HasIndex(e => e.Username, "UQ__Cuentas__536C85E4108580FE").IsUnique();

            entity.Property(e => e.BloqueadoHasta).HasColumnType("datetime");
            entity.Property(e => e.IntentosFallidos).HasDefaultValue(0);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.Property(e => e.RequiereCambioPassword).HasDefaultValue(false);
            
            entity.HasOne(d => d.UsuarioDetalle).WithMany(p => p.Cuenta)
                .HasForeignKey(d => d.UsuarioDetalleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cuentas_UsuariosDetalle");
        });

        modelBuilder.Entity<UsuariosDetalle>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK__Usuarios__2B3DE7B8739602B1");

            entity.ToTable("UsuariosDetalle");

            entity.HasIndex(e => e.NumeroDocumento, "UQ__Usuarios__A4202588BABC2E16").IsUnique();

            entity.Property(e => e.EmailPrincipal).HasMaxLength(255);
            entity.Property(e => e.EmailSecundario).HasMaxLength(255);
            entity.Property(e => e.TelefonoSecundario).HasMaxLength(20);
            entity.Property(e => e.Entidad).HasMaxLength(150);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Nacionalidad).HasMaxLength(100);
            entity.Property(e => e.Nombres).HasMaxLength(100);
            entity.Property(e => e.NombresCompletoCabecera).HasMaxLength(150);
            entity.Property(e => e.NumeroDocumento).HasMaxLength(20);
            entity.Property(e => e.PrimerApellido).HasMaxLength(100);
            entity.Property(e => e.Rol).HasMaxLength(100);
            entity.Property(e => e.SegundoApellido).HasMaxLength(100);
            entity.Property(e => e.Sexo)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TelefonoMovil).HasMaxLength(20);
            entity.Property(e => e.TipoContratacion).HasMaxLength(50);
            entity.Property(e => e.TipoDocumento).HasMaxLength(50);
            entity.Property(e => e.FotoUrl).HasMaxLength(500);
        });

    }
}
