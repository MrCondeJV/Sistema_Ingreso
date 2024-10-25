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

        private void CargarAsistencias()
        {
            List<Asistencia> asistencias = new List<Asistencia>();

            try
            {
                Conexion conexion = new Conexion();
                using (MySqlConnection connection = conexion.ObtenerConexion())
                {
                    string query = @"SELECT 
                                a.id, 
                                u.nombre AS UsuarioNombre, 
                                v.placa AS VehiculoPlaca, 
                                mi.descripcion AS ModoIngresoDescripcion, 
                                a.fecha_hora, 
                                a.tipo 
                             FROM asistencias a
                             JOIN Usuarios u ON a.usuario_id = u.id
                             JOIN Vehiculos v ON a.vehiculo_id = v.id
                             JOIN modos_ingreso mi ON a.modo_ingreso_id = mi.id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        conexion.Conectar();

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Crear una nueva variable local de Asistencia
                                Asistencia asistencia = new Asistencia
                                {
                                    Id = reader.GetInt32("id"),
                                    UsuarioNombre = reader["UsuarioNombre"].ToString(),
                                    VehiculoPlaca = reader["VehiculoPlaca"].ToString(),
                                    ModoIngresoDescripcion = reader["ModoIngresoDescripcion"].ToString(),
                                    FechaHora = reader.GetDateTime("fecha_hora"),
                                    Tipo = reader["tipo"].ToString()
                                };

                                // Agregar la asistencia a la lista
                                asistencias.Add(asistencia);
                            }
                        }

                        conexion.Cerrar();
                    }

                    // Configura el DataGridView para no generar columnas automáticamente
                    dgvRegistros.AutoGenerateColumns = false;

                    // Limpia cualquier columna existente
                    dgvRegistros.Columns.Clear();

                    // Define las columnas manualmente
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

                    // Asignar la lista de asistencias al DataGridView
                    dgvRegistros.DataSource = asistencias;

                    // Agregar la columna de botones
                    dgvRegistros.Columns.Add(CreateActionButtonColumn());
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
