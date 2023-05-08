﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCubosExamenFGG.Models
{
    public class CompraCubo
    {
        public int IdPedido { get; set; }

        public int IdCubo { get; set; }

        public int IdUsuario { get; set; }

        public DateTime FechaPedido { get; set; }
    }
}
