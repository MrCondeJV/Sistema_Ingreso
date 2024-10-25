using iTextSharp.text.pdf;
using iTextSharp.text;
using MySql.Data.MySqlClient;
using Sistema_Ingreso.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient; // Cambia esto si usas otro tipo de base de datos
using System.IO;
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
            List<Usuario> usuarios = new List<Usuario>();

            try
            {
                Conexion conexion = new Conexion();
                using (MySqlConnection connection = conexion.ObtenerConexion())
                {
                    string query = "SELECT id, documento, nombre, apellido, grado, unidad FROM Usuarios";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        conexion.Conectar();

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                usuarios.Add(new Usuario
                                {
                                    Id = reader.GetInt32("id"),
                                    Documento = reader["documento"].ToString(),
                                    Nombre = reader["nombre"].ToString(),
                                    Apellido = reader["apellido"].ToString(),
                                    Grado = reader["grado"].ToString(),
                                    Unidad = reader["unidad"].ToString()
                                });
                            }
                        }

                        conexion.Cerrar();
                    }

                    // Configura el DataGridView para no generar columnas automáticamente
                    dataGridViewUsuarios.AutoGenerateColumns = false;

                    // Limpia cualquier columna existente
                    dataGridViewUsuarios.Columns.Clear();

                    // Define las columnas manualmente
                    dataGridViewUsuarios.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "Id",
                        HeaderText = "ID"
                    });

                    dataGridViewUsuarios.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "Documento",
                        HeaderText = "Documento"
                    });

                    dataGridViewUsuarios.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "Nombre",
                        HeaderText = "Nombre"
                    });

                    dataGridViewUsuarios.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "Apellido",
                        HeaderText = "Apellido"
                    });

                    dataGridViewUsuarios.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "Grado",
                        HeaderText = "Grado"
                    });

                    dataGridViewUsuarios.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "Unidad",
                        HeaderText = "Unidad"
                    });

                    // Asignar la lista de usuarios al DataGridView
                    dataGridViewUsuarios.DataSource = usuarios;

                    // Agregar la columna de botones
                    dataGridViewUsuarios.Columns.Add(CreatePrintButtonColumn());
                }
            }
            catch (MySqlException sqlEx)
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

        public void GenerarCarnetPDF(Models.Usuario usuario)
        {
            // Ruta donde se guardará el PDF
            string rutaPDF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{usuario.Documento}_Carnet.pdf");

            // Tamaño personalizado para la tarjeta de crédito
            var tamañoTarjeta = new iTextSharp.text.Rectangle(242.64f, 153.54f); // 85.6 mm x 53.98 mm
            Document documento = new Document(tamañoTarjeta, 0, 0, 0, 0); // Sin márgenes

            try
            {
                PdfWriter writer = PdfWriter.GetInstance(documento, new FileStream(rutaPDF, FileMode.Create));
                documento.Open();

                // Añadir la imagen del usuario si existe
                if (usuario.Imagen != null)
                {
                    try
                    {
                        iTextSharp.text.Image fotoUsuario = iTextSharp.text.Image.GetInstance(usuario.Imagen);
                        fotoUsuario.ScaleToFit(50f, 70f); // Ajustar el tamaño de la imagen
                        fotoUsuario.Alignment = Element.ALIGN_CENTER;
                        documento.Add(fotoUsuario);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al cargar la imagen del usuario: {ex.Message}");
                    }
                }

                // Espacio entre la foto y el texto
                documento.Add(new Paragraph("\n"));

                // Añadir la información del usuario
                documento.Add(new Paragraph($"Documento: {usuario.Documento}", FontFactory.GetFont(FontFactory.HELVETICA, 8)));
                documento.Add(new Paragraph($"Nombre: {usuario.Nombre} {usuario.Apellido}", FontFactory.GetFont(FontFactory.HELVETICA, 8)));
                documento.Add(new Paragraph($"Grado: {usuario.Grado}", FontFactory.GetFont(FontFactory.HELVETICA, 8)));
                documento.Add(new Paragraph($"Unidad: {usuario.Unidad}", FontFactory.GetFont(FontFactory.HELVETICA, 8)));

                // Añadir espacio antes del código de barras
                documento.Add(new Paragraph("\n"));

                // Generar código de barras si existe
                if (usuario.CarnetCodigoBarras != null)
                {
                    try
                    {
                        iTextSharp.text.Image codigoBarras = iTextSharp.text.Image.GetInstance(usuario.CarnetCodigoBarras);
                        codigoBarras.ScaleToFit(150f, 30f); // Ajustar el tamaño del código de barras
                        codigoBarras.Alignment = Element.ALIGN_CENTER;
                        documento.Add(codigoBarras);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al cargar el código de barras: {ex.Message}");
                    }
                }

                // Cierra el documento
                documento.Close();
                MessageBox.Show("PDF de carnet generado exitosamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el PDF: " + ex.Message);
            }
        }

    }

}
