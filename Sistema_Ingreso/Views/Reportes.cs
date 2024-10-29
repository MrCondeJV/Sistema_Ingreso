using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Sistema_Ingreso.Models;


namespace Sistema_Ingreso
{
    public partial class Reportes : UserControl
    {
        private MySqlConnection conexion;

        public Reportes()
        {
            InitializeComponent();
            // Inicializa la conexión a la base de datos
            Conexion conexion = new Conexion();
            CargarAsistencias();
        }

        private void CargarAsistencias(string filtro = "")
        {
            List<Asistencia> asistencias = new List<Asistencia>();

            try
            {
                Conexion conexion = new Conexion();
                using (MySqlConnection connection = conexion.ObtenerConexion())
                {
                    // Modificación en la consulta SQL para incluir el filtro
                    string query = @"SELECT 
                                a.id, 
                                CONCAT(u.nombre, ' ', u.apellido) AS UsuarioNombre, 
                                v.placa AS VehiculoPlaca, 
                                mi.descripcion AS ModoIngresoDescripcion, 
                                a.fecha_hora, 
                                a.tipo 
                             FROM asistencias a
                             JOIN Usuarios u ON a.usuario_id = u.id
                             JOIN Vehiculos v ON a.vehiculo_id = v.id
                             JOIN modos_ingreso mi ON a.modo_ingreso_id = mi.id
                             WHERE (@filtro = '' OR 
                                    u.nombre LIKE @filtro OR 
                                    u.documento LIKE @filtro OR 
                                    u.grado LIKE @filtro OR 
                                    DATE(a.fecha_hora) = @fecha)
                             ORDER BY a.fecha_hora DESC;";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        conexion.Conectar();

                        // Parámetro del filtro con comodín
                        command.Parameters.AddWithValue("@filtro", "%" + filtro + "%");

                        // Verificar si el filtro es una fecha válida
                        if (DateTime.TryParse(filtro, out DateTime fechaFiltro))
                        {
                            command.Parameters.AddWithValue("@fecha", fechaFiltro.ToString("yyyy-MM-dd"));
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@fecha", DBNull.Value);
                        }

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Asistencia asistencia = new Asistencia
                                {
                                    Id = reader.GetInt32("id"),
                                    UsuarioNombre = reader["UsuarioNombre"].ToString(),
                                    VehiculoPlaca = reader["VehiculoPlaca"].ToString(),
                                    ModoIngresoDescripcion = reader["ModoIngresoDescripcion"].ToString(),
                                    FechaHora = reader.GetDateTime("fecha_hora"),
                                    Tipo = reader["tipo"].ToString()
                                };

                                asistencias.Add(asistencia);
                            }
                        }

                        conexion.Cerrar();
                    }

                    // Configurar columnas del DataGridView si es la primera carga
                    if (dgvRegistros.Columns.Count == 0)
                    {
                        dgvRegistros.AutoGenerateColumns = false;
                        dgvRegistros.Columns.Clear();

                        dgvRegistros.Columns.Add(new DataGridViewTextBoxColumn
                        {
                            DataPropertyName = "Id",
                            HeaderText = "ID"
                        });

                        dgvRegistros.Columns.Add(new DataGridViewTextBoxColumn
                        {
                            DataPropertyName = "UsuarioNombre",
                            HeaderText = "Nombre del Usuario"
                        });

                        dgvRegistros.Columns.Add(new DataGridViewTextBoxColumn
                        {
                            DataPropertyName = "VehiculoPlaca",
                            HeaderText = "Placa del Vehículo"
                        });

                        dgvRegistros.Columns.Add(new DataGridViewTextBoxColumn
                        {
                            DataPropertyName = "ModoIngresoDescripcion",
                            HeaderText = "Modo de Ingreso"
                        });

                        dgvRegistros.Columns.Add(new DataGridViewTextBoxColumn
                        {
                            DataPropertyName = "FechaHora",
                            HeaderText = "Fecha y Hora"
                        });

                        dgvRegistros.Columns.Add(new DataGridViewTextBoxColumn
                        {
                            DataPropertyName = "Tipo",
                            HeaderText = "Tipo"
                        });
                    }

                    // Asignar la lista de asistencias al DataGridView
                    dgvRegistros.DataSource = asistencias;
                }
            }
            catch (MySqlException sqlEx)
            {
                MessageBox.Show($"Error de base de datos: {sqlEx.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar asistencias: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Evento de búsqueda
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarAsistencias(txtBuscar.Text);
        }

        // Alternativamente, para búsqueda en tiempo real
        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            CargarAsistencias(txtBuscar.Text);
        }


        // Método para crear una columna de botón
        private DataGridViewButtonColumn CreateActionButtonColumn()
        {
            DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn
            {
                HeaderText = "Acción",
                Text = "Imprimir",
                UseColumnTextForButtonValue = true,
                Name = "Imprimir"
            };
            return buttonColumn;
        }


    }
}