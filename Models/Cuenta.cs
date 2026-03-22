using System;

namespace UpscaleWeb.Models;

public partial class Cuenta
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int? IntentosFallidos { get; set; }

    public DateTime? BloqueadoHasta { get; set; }

    public int UsuarioDetalleId { get; set; }

    public bool RequiereCambioPassword { get; set; }

    public virtual UsuariosDetalle UsuarioDetalle { get; set; } = null!;
}
