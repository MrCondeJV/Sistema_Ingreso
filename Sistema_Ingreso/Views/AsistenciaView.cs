using MySql.Data.MySqlClient;
using Sistema_Ingreso.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_Ingreso
{
    using System.IO;
    using System.Timers; // Importar la librería de Timers

    public partial class AsistenciaView : UserControl
    {
        private Conexion conexion;
        private System.Timers.Timer timer; // Añadimos un timer para el borrado automático

        public AsistenciaView()
        {
            InitializeComponent();
            conexion = new Conexion();

            // Inicializar el timer
            timer = new System.Timers.Timer(6000); // Configurar el timer para 6 segundos (6000 milisegundos)
            timer.Elapsed += Timer_Elapsed; // Suscribirse al evento que se dispara cuando el tiempo expira
            timer.AutoReset = false; // Asegurarse de que el timer solo se ejecute una vez por activación

            // Suscribirse al evento KeyPress del TextBox txtCodigo
            txtCodigo.KeyPress += new KeyPressEventHandler(txtCodigo_KeyPress);
        }

        // Evento Load del UserControl para hacer focus al TextBox
        private void Asistencia_Load(object sender, EventArgs e)
        {
            // Hacer foco en el TextBox para que esté listo para recibir la entrada
            txtCodigo.Focus();
        }

        // Método para capturar el evento KeyPress del TextBox
        private void txtCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Detectar si se presionó Enter (el lector de código de barras suele enviar un Enter al final)
            if (e.KeyChar == (char)Keys.Enter)
            {
                // Obtener el código escaneado desde el TextBox
                string documento = txtCodigo.Text.Trim();

                // Procesar el código escaneado
                if (!string.IsNullOrEmpty(documento))
                {
                    BuscarUsuarioPorDocumento(documento);
                }

                // Limpiar el TextBox después de procesar
                txtCodigo.Clear();
            }
        }

        private void BuscarUsuarioPorDocumento(string documento)
        {
            Usuario usuario = ObtenerUsuarioPorDocumento(documento);

            if (usuario != null)
            {
                // Mostrar la información del usuario en los labels
                lblDocumento.Text = usuario.Documento;
                lblNombre.Text = usuario.Nombre;
                lblApellido.Text = usuario.Apellido;
                lblGrado.Text = usuario.Grado;
                lblUnidad.Text = usuario.Unidad;

                // Verificar si hay una imagen para el usuario y mostrarla
                if (usuario.Imagen != null && usuario.Imagen.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream(usuario.Imagen))
                    {
                        pctbFotoUsuario.Image = Image.FromStream(ms); // Mostrar la imagen del usuario
                    }
                }
                else
                {
                    pctbFotoUsuario.Image = null; // Si no hay imagen, limpiar el PictureBox
                }

                // Verificar si el usuario tiene información del vehículo
                if (usuario.Vehiculo != null)
                {
                    lblMarca.Text = usuario.Vehiculo.Marca;
                    lblModelo.Text = usuario.Vehiculo.Modelo;
                    lblPlaca.Text = usuario.Vehiculo.Placa;

                    // Verificar si hay una imagen para el vehículo y mostrarla
                    if (usuario.Vehiculo.Imagen_vehiculo != null && usuario.Vehiculo.Imagen_vehiculo.Length > 0)
                    {
                        using (MemoryStream ms = new MemoryStream(usuario.Vehiculo.Imagen_vehiculo))
                        {
                            pctbFotoVehiculo.Image = Image.FromStream(ms); // Mostrar la imagen del vehículo
                        }
                    }
                    else
                    {
                        pctbFotoVehiculo.Image = null; // Si no hay imagen, limpiar el PictureBox
                    }

                    // Registrar asistencia con el ID del vehículo
                    RegistrarAsistencia(usuario.Id, usuario.Vehiculo.Id_vehiculo);
                }
                else
                {
                    // Si no hay vehículo asociado, limpiar los labels o mostrar un mensaje
                    lblMarca.Text = "Sin vehículo";
                    lblModelo.Text = "";
                    lblPlaca.Text = "";
                    pctbFotoVehiculo.Image = null; // Limpiar la imagen del vehículo si no hay

                    // Registrar asistencia sin vehículo
                    RegistrarAsistencia(usuario.Id, 0); // Pasar 0 como Id del vehículo
                }

                // Iniciar el temporizador para limpiar después de 6 segundos
                timer.Start();
            }
            else
            {
                MessageBox.Show("Usuario no encontrado.");
            }
        }






        // Evento que se dispara cuando el temporizador alcanza los 6 segundos
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Limpiar los campos en el hilo principal (UI)
            this.Invoke(new Action(() =>
            {
                LimpiarCampos();
            }));
        }

        // Método para limpiar todos los labels y el TextBox
        private void LimpiarCampos()
        {
            lblDocumento.Text = string.Empty;
            lblNombre.Text = string.Empty;
            lblApellido.Text = string.Empty;
            lblGrado.Text = string.Empty;
            lblUnidad.Text = string.Empty;
            lblMarca.Text = string.Empty;
            lblModelo.Text = string.Empty;
            lblPlaca.Text = string.Empty;
            txtCodigo.Clear();

            // Limpiar las imágenes de los PictureBox
            pctbFotoUsuario.Image = null; // Limpia la imagen del usuario
            pctbFotoVehiculo.Image = null; // Limpia la imagen del vehículo

        }

        public Usuario ObtenerUsuarioPorDocumento(string documento)
        {
            // Actualizamos la consulta para incluir los datos del vehículo
            string query = @"SELECT u.id AS usuario_id, u.documento, u.nombre, u.apellido, u.grado, u.unidad, u.imagen,
       v.id AS vehiculo_id, v.marca, v.modelo, v.placa, v.imagen_vehiculo
FROM usuarios u
LEFT JOIN vehiculos v ON u.id = v.usuario_id
WHERE u.documento = @documento;
";

            Usuario usuario = null;

            try
            {
                // Abrir la conexión a la base de datos
                conexion.Conectar();

                using (MySqlCommand cmd = new MySqlCommand(query, conexion.ObtenerConexion()))
                {
                    // Añadir el parámetro del documento
                    cmd.Parameters.AddWithValue("@documento", documento);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Si se encuentra el usuario, llenar el objeto Usuario con los datos
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                Id = reader.GetInt32("usuario_id"),
                                Documento = reader["documento"].ToString(), // Ahora sí existe esta columna
                                Nombre = reader["nombre"].ToString(),
                                Apellido = reader["apellido"].ToString(),
                                Grado = reader["grado"].ToString(),
                                Unidad = reader["unidad"].ToString(),
                                Imagen = reader["imagen"] as byte[],

                                Vehiculo = new Vehiculo // Creamos y llenamos el objeto Vehiculo
                                {
                                    Id_vehiculo = reader.IsDBNull(reader.GetOrdinal("vehiculo_id")) ? 0 : reader.GetInt32("vehiculo_id"), // Aquí obtienes el id del vehículo
                                    Marca = reader["marca"].ToString(),
                                    Modelo = reader["modelo"].ToString(),
                                    Placa = reader["placa"].ToString(),
                                    Imagen_vehiculo = reader["imagen_vehiculo"] as byte[]
                                }
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener el usuario y el vehículo: " + ex.Message);
            }
            finally
            {
                // Asegurarse de que la conexión siempre se cierre
                conexion.Cerrar();
            }

            return usuario;
        }


        private void RegistrarAsistencia(int usuarioId, int vehiculoId)
        {
            // Primero, verifica el último registro del usuario
            int tipoIngresoId = ObtenerUltimoRegistroModoIngreso(usuarioId); // Entrada/Salida

            // Determinar el nuevo tipo de ingreso
            if (tipoIngresoId == 1) // Último registro fue una entrada
            {
                tipoIngresoId = 2; // Nuevo registro será una salida
            }
            else // Último registro fue una salida o no hay registros
            {
                tipoIngresoId = 1; // Nuevo registro será una entrada
            }

            // Determinar el modo de ingreso
            int modoIngresoId = (vehiculoId > 0) ? 1 : 2; // 1: Vehículo, 2: Peatón

            // Ahora, registrar el nuevo ingreso/salida
            string query = "INSERT INTO asistencias (usuario_id, vehiculo_id, modo_ingreso_id, fecha_hora, tipo) " +
                           "VALUES (@usuarioId, @vehiculoId, @modoIngresoId, @fechaHora, @tipo)";

            try
            {
                conexion.Conectar();
                using (MySqlCommand cmd = new MySqlCommand(query, conexion.ObtenerConexion()))
                {
                    cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                    cmd.Parameters.AddWithValue("@vehiculoId", vehiculoId > 0 ? vehiculoId : (object)DBNull.Value); // Permitir null si no hay vehículo
                    cmd.Parameters.AddWithValue("@modoIngresoId", modoIngresoId); // 1: Vehículo, 2: Peatón
                    cmd.Parameters.AddWithValue("@fechaHora", DateTime.Now);
                    cmd.Parameters.AddWithValue("@tipo", tipoIngresoId == 1 ? "entrada" : "salida"); // Entrada o salida

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar la asistencia: " + ex.Message);
            }
            finally
            {
                conexion.Cerrar();
            }
        }


        private int ObtenerUltimoRegistroModoIngreso(int usuarioId)
        {
            int modoIngresoId = 0; // 0 si no hay registros
            string query = "SELECT modo_ingreso_id FROM asistencias WHERE usuario_id = @usuarioId ORDER BY fecha_hora DESC LIMIT 1";

            try
            {
                conexion.Conectar();
                using (MySqlCommand cmd = new MySqlCommand(query, conexion.ObtenerConexion()))
                {
                    cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            modoIngresoId = reader.GetInt32("modo_ingreso_id");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener el último registro: " + ex.Message);
            }
            finally
            {
                conexion.Cerrar();
            }

            return modoIngresoId;
        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }


}


