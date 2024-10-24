using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Sistema_Ingreso.Controller
{
    using System;
    using MySql.Data.MySqlClient;
    using Sistema_Ingreso.Models;

    public class AsistenciaController
    {
        private Conexion conexion;

        public AsistenciaController()
        {
            conexion = new Conexion();
        }

        // Método para registrar una nueva asistencia
        public void RegistrarAsistencia(Asistencia asistencia)
        {
            string query = "INSERT INTO asistencias (usuario_id, vehiculo_id, modo_ingreso_id, fecha_hora, tipo) " +
                           "VALUES (@usuario_id, @vehiculo_id, @modo_ingreso_id, @fecha_hora, @tipo)";
            MySqlCommand cmd = new MySqlCommand(query, conexion.ObtenerConexion());
            cmd.Parameters.AddWithValue("@usuario_id", asistencia.UsuarioId);
            cmd.Parameters.AddWithValue("@vehiculo_id", asistencia.VehiculoId);
            cmd.Parameters.AddWithValue("@modo_ingreso_id", asistencia.ModoIngresoId);
            cmd.Parameters.AddWithValue("@fecha_hora", asistencia.FechaHora);
            cmd.Parameters.AddWithValue("@tipo", asistencia.Tipo);

            conexion.Conectar();
            cmd.ExecuteNonQuery();
            conexion.Cerrar();
        }
    }

}
