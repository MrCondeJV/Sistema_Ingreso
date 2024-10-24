using MySql.Data.MySqlClient;
using Sistema_Ingreso.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient; // Cambia esto si usas otro tipo de base de datos
using System.Windows.Forms;

namespace Sistema_Ingreso.Views
{
    public partial class ListaUsuarios : Form
    {
        private Conexion Conexion;

        public ListaUsuarios()
        {
            Conexion = new Conexion();
            InitializeComponent();
            CargarUsuarios();
        }

        private void CargarUsuarios()
        {
            // Crear una lista para almacenar usuarios
            List<Usuario> usuarios = new List<Usuario>();

            // Conectar a la base de datos y ejecutar la consulta
            try
            {
                // Cambiar SqlConnection por MySqlConnection
                using (MySqlConnection connection = new MySqlConnection(Conexion.ObtenerConexion()))
                {
                    string query = "SELECT documento, nombre, apellido, grado, unidad FROM Usuarios"; // Ajusta la consulta según tu tabla

                    using (MySqlCommand command = new MySqlCommand(query, connection)) // Cambiar SqlCommand por MySqlCommand
                    {
                        connection.Open();

                        using (MySqlDataReader reader = command.ExecuteReader()) // Cambiar SqlDataReader por MySqlDataReader
                        {
                            while (reader.Read())
                            {
                                usuarios.Add(new Usuario
                                {
                                    Documento = reader["documento"].ToString(),
                                    Nombre = reader["nombre"].ToString(),
                                    Apellido = reader["apellido"].ToString(),
                                    Grado = reader["grado"].ToString(),
                                    Unidad = reader["unidad"].ToString()
                                });
                            }
                        }
                    }

                    // Asignar la lista de usuarios al DataGridView
                    dataGridViewUsuarios.DataSource = usuarios;

                    // Agregar columna de botones al DataGridView
                    dataGridViewUsuarios.Columns.Add(CreatePrintButtonColumn());
                }
            }
            catch (MySqlException sqlEx) // Cambiar SqlException por MySqlException
            {
                MessageBox.Show($"Error de base de datos: {sqlEx.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar usuarios: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private DataGridViewButtonColumn CreatePrintButtonColumn()
        {
            var printButtonColumn = new DataGridViewButtonColumn
            {
                HeaderText = "Imprimir Carnet",
                Text = "Imprimir",
                UseColumnTextForButtonValue = true,
                Name = "ImprimirCarnet" // Asigna un nombre a la columna
            };
            return printButtonColumn;
        }

        private void dataGridViewUsuarios_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verifica si la celda clickeada es un botón
            if (e.RowIndex >= 0 && e.ColumnIndex == dataGridViewUsuarios.Columns["ImprimirCarnet"].Index)
            {
                // Obtener el usuario correspondiente
                var usuario = (Usuario)dataGridViewUsuarios.Rows[e.RowIndex].DataBoundItem;

                // Llamar al método para generar el carnet
                GenerarCarnetPDF(usuario);
            }
        }

        private void GenerarCarnetPDF(Usuario usuario)
        {
            // Implementación del método para generar el PDF (puedes usar el método que ya creaste)
            MessageBox.Show($"Generando carnet para {usuario.Nombre} {usuario.Apellido}");
        }
    }
}
