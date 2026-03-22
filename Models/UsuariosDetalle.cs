using System;
using System.Collections.Generic;

namespace UpscaleWeb.Models;

public partial class UsuariosDetalle
{
    public int UsuarioId { get; set; }

    public bool? Estado { get; set; }

    public string? NombresCompletoCabecera { get; set; }

    public string? Rol { get; set; }

    public string? Entidad { get; set; }

    public string? Nombres { get; set; }

    public string? PrimerApellido { get; set; }

    public string? SegundoApellido { get; set; }

    public string? TipoDocumento { get; set; }

    public string? NumeroDocumento { get; set; }

    public DateOnly? FechaNacimiento { get; set; }

    public string? Nacionalidad { get; set; }

    public string? Sexo { get; set; }

    public string? EmailPrincipal { get; set; }

    public string? EmailSecundario { get; set; }
    public string? TelefonoSecundario { get; set; }
    public string? TelefonoMovil { get; set; }

    public string? TipoContratacion { get; set; }

    public DateOnly? FechaContratacion { get; set; }

    public string? FotoUrl { get; set; }

    public virtual ICollection<Cuenta> Cuenta { get; set; } = new List<Cuenta>();
}
