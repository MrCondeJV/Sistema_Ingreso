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


      
        public string UsuarioNombre { get; set; } // Nueva propiedad para el nombre del usuario
        public string VehiculoPlaca { get; set; } // Nueva propiedad para la placa del vehículo
        public string ModoIngresoDescripcion { get; set; } // Nueva propiedad para la descripción del modo de ingreso
     

        // Relaciones
        public Usuario Usuario { get; set; }
        public Vehiculo Vehiculo { get; set; }
        public ModoIngreso ModoIngreso { get; set; }
    }

}
