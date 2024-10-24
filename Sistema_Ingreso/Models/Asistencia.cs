using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Ingreso.Models
{
    public class Asistencia
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int? VehiculoId { get; set; }  // Puede ser nulo si el usuario ingresa a pie
        public int ModoIngresoId { get; set; }
        public DateTime FechaHora { get; set; }
        public string Tipo { get; set; }  // "Entrada" o "Salida"

        // Relaciones
        public Usuario Usuario { get; set; }
        public Vehiculo Vehiculo { get; set; }
        public ModoIngreso ModoIngreso { get; set; }
    }

}
