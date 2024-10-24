using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Ingreso.Models
{
    public class Vehiculo
    {
        public int Id_vehiculo { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Placa { get; set; }
        public byte[] Imagen_vehiculo { get; set; } // Propiedad para la imagen del vehículo

        // Relación con Usuario
        public Usuario Usuario { get; set; } // Objeto que representa el usuario al que pertenece el vehículo
    }


}
