using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Sistema_Ingreso.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using MySql.Data.MySqlClient;
    using Sistema_Ingreso.Models;

    public class UsuarioController
    {
        private Conexion conexion;

        public UsuarioController()
        {
            conexion = new Conexion();
        }

        // Método para guardar un usuario y su vehículo en la base de datos
        public bool GuardarUsuario(Usuario usuario)
        {
            string queryUsuario = "INSERT INTO usuarios (documento, nombre, apellido, grado, unidad, imagen, carnet_codigo_barras) " +
                                  "VALUES (@documento, @nombre, @apellido, @grado, @unidad, @imagen, @carnet_codigo_barras);" +
                                  "SELECT LAST_INSERT_ID();"; // Para obtener el ID del usuario recién insertado

            string queryVehiculo = "INSERT INTO vehiculos (usuario_id, marca, modelo, placa, imagen_vehiculo) " +
                                   "VALUES (@usuario_id, @marca, @modelo, @placa, @imagen_vehiculo)";

            try
            {
                conexion.Conectar();

                // Guardar el usuario primero
                int usuarioId;
                using (MySqlCommand cmd = new MySqlCommand(queryUsuario, conexion.ObtenerConexion()))
                {
                    cmd.Parameters.AddWithValue("@documento", usuario.Documento);
                    cmd.Parameters.AddWithValue("@nombre", usuario.Nombre);
                    cmd.Parameters.AddWithValue("@apellido", usuario.Apellido);
                    cmd.Parameters.AddWithValue("@grado", usuario.Grado);
                    cmd.Parameters.AddWithValue("@unidad", usuario.Unidad);
                    cmd.Parameters.AddWithValue("@imagen", usuario.Imagen);
                    cmd.Parameters.AddWithValue("@carnet_codigo_barras", usuario.CarnetCodigoBarras);

                    usuarioId = Convert.ToInt32(cmd.ExecuteScalar()); // Obtener el ID del usuario
                }

                // Guardar el vehículo asociado al usuario
                using (MySqlCommand cmd = new MySqlCommand(queryVehiculo, conexion.ObtenerConexion()))
                {
                    cmd.Parameters.AddWithValue("@usuario_id", usuarioId);
                    cmd.Parameters.AddWithValue("@marca", usuario.Vehiculo.Marca);
                    cmd.Parameters.AddWithValue("@modelo", usuario.Vehiculo.Modelo);
                    cmd.Parameters.AddWithValue("@placa", usuario.Vehiculo.Placa);
                    cmd.Parameters.AddWithValue("@imagen_vehiculo", usuario.Vehiculo.Imagen_vehiculo); // Guardar la imagen del vehículo

                    cmd.ExecuteNonQuery(); // Ejecutar la inserción del vehículo
                }

                return true; // Usuario y vehículo guardados correctamente
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar el usuario: " + ex.Message);
                return false; // Hubo un error al guardar
            }
            finally
            {
                conexion.Cerrar(); // Asegura que la conexión se cierre
            }
        }




        // Método para obtener todos los usuarios
        public List<Usuario> ObtenerUsuarios()
        {
            List<Usuario> usuarios = new List<Usuario>();
            string query = "SELECT * FROM usuarios";
            MySqlCommand cmd = new MySqlCommand(query, conexion.ObtenerConexion());
            conexion.Conectar();
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Usuario usuario = new Usuario
                {
                    Id = reader.GetInt32("id"),
                    Documento = reader.GetString("documento"),
                    Nombre = reader.GetString("nombre"),
                    Apellido = reader.GetString("apellido"),
                    Grado = reader.GetString("grado"),
                    Unidad = reader.GetString("unidad"),
                    FechaRegistro = reader.GetDateTime("fecha_registro")
                };
                usuarios.Add(usuario);
            }

            conexion.Cerrar();
            return usuarios;
        }

        // Método para insertar un nuevo usuario
     
    }

}
