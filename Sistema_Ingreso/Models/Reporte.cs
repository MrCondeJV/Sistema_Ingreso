using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Ingreso.Models
{
    public class Reporte
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int? VehiculoId { get; set; }
        public int TotalAsistencias { get; set; }
        public int TotalEntradas { get; set; }
        public int TotalSalidas { get; set; }
        public DateTime FechaInicial { get; set; }
        public DateTime FechaFinal { get; set; }

        // Relaciones
        public Usuario Usuario { get; set; }
        public Vehiculo Vehiculo { get; set; }
    }

}
