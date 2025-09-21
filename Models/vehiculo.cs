using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace parcial2.Models;

[Index("idcolor", Name = "fk_vehiculos_colores_idx")]
[Index("idmarca", Name = "fk_vehiculos_marcas_idx")]
public partial class vehiculo
{
    [Key]
    [Column(TypeName = "int(11)")]
    public int idvehiculo { get; set; }

    [Column(TypeName = "int(11)")]
    public int idcolor { get; set; }

    [Column(TypeName = "int(11)")]
    public int idmarca { get; set; }

    [Column(TypeName = "smallint(4)")]
    public short modelo { get; set; }

    [StringLength(45)]
    public string chasis { get; set; } = null!;

    [StringLength(45)]
    public string motor { get; set; } = null!;

    [StringLength(45)]
    public string nombre { get; set; } = null!;

    [Column(TypeName = "smallint(1)")]
    public short activo { get; set; }
}

public partial class vehiculoInsert {
    [Column(TypeName = "int(11)")]
    public int idcolor { get; set; }
    [Column(TypeName = "int(11)")]
    public int idmarca { get; set; }

    [Column(TypeName = "smallint(4)")]
    public short modelo { get; set; }


    [StringLength(45)]
    public string chasis { get; set; } = null!;

    [StringLength(45)]
    public string motor { get; set; } = null!;

    [StringLength(45)]
    public string nombre { get; set; } = null!;

    [Column(TypeName = "smallint(1)")]
    public short activo { get; set; }


}

public partial class vehiculoActualizar

{
    [Key]
    [Column(TypeName = "int(11)")]
    public int idvehiculo { get; set; }

    [Column(TypeName = "int(11)")]
    public int idcolor { get; set; }
    [Column(TypeName = "int(11)")]
    public int idmarca { get; set; }

    [Column(TypeName = "smallint(4)")]
    public short modelo { get; set; }


    [StringLength(45)]
    public string chasis { get; set; } = null!;

    [StringLength(45)]
    public string motor { get; set; } = null!;

    [StringLength(45)]
    public string nombre { get; set; } = null!;

    [Column(TypeName = "smallint(1)")]
    public short activo { get; set; }


}

public partial class vehiculoEliminar

{
    [Key]
    [Column(TypeName = "int(11)")]
    public int idvehiculo { get; set; }


}
