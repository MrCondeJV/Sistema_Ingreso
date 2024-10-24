using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Ingreso.Models
{
    public class Usuario
    {
        
            public int Id { get; set; }
            public string Documento { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Grado { get; set; }
            public string Unidad { get; set; }
            public byte[] Imagen { get; set; }
            public byte[] CarnetCodigoBarras { get; set; }
        public Vehiculo Vehiculo { get; set; } // Propiedad para el vehículo
        public DateTime FechaRegistro { get; set; }
        

    }
}
