using iTextSharp.text.pdf;
using iTextSharp.text;
using MySql.Data.MySqlClient;
using Sistema_Ingreso.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ZXing; // Asegúrate de agregar esta línea para el código de barras

namespace Sistema_Ingreso.Views
{
    public partial class ListaUsuarios : Form
    {
        private Conexion Conexion;
        private List<Usuario> usuarios; // Guardar la lista de usuarios
        private BindingSource bindingSource; // BindingSource para el DataGridView

        public ListaUsuarios()
        {
            Conexion = new Conexion();
            InitializeComponent();
            bindingSource = new BindingSource(); // Inicializa el BindingSource
            CargarUsuarios();
            dataGridViewUsuarios.CellContentClick += dataGridViewUsuarios_CellContentClick;
        }

        private void CargarUsuarios()
        {
            usuarios = new List<Usuario>();

            try
            {
                using (MySqlConnection connection = Conexion.ObtenerConexion())
                {
                    string query = "SELECT id, documento, nombre, apellido, grado, unidad, imagen FROM Usuarios";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        Conexion.Conectar();

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
                                    Unidad = reader["unidad"].ToString(),
                                    Imagen = reader["imagen"] as byte[] // Asegúrate de que la imagen se esté cargando correctamente
                                });
                            }
                        }

                        Conexion.Cerrar();
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

                    // Asignar la lista de usuarios al BindingSource
                    bindingSource.DataSource = usuarios;
                    dataGridViewUsuarios.DataSource = bindingSource; // Asigna el BindingSource al DataGridView

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
            if (e.RowIndex >= 0 && e.ColumnIndex == dataGridViewUsuarios.Columns["ImprimirCarnet"].Index)
            {
                var usuario = (Usuario)dataGridViewUsuarios.Rows[e.RowIndex].DataBoundItem;

                // Verificación adicional de datos necesarios
                if (usuario != null)
                {
                    GenerarCarnetPNG(usuario); // Cambiado a PNG
                }
                else
                {
                    MessageBox.Show("Error: El usuario seleccionado es nulo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void GenerarCarnetPNG(Models.Usuario usuarioSeleccionado)
        {
            // Ruta donde se guardará el PNG
            string rutaPNG = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{usuarioSeleccionado.Documento}_Carnet.png");

            // Tamaño personalizado para una tarjeta
            int anchoTarjeta = 400; // Ajusta según tus necesidades
            int altoTarjeta = 600; // Aumentado para hacer el carnet más vertical

            using (var bitmap = new System.Drawing.Bitmap(anchoTarjeta, altoTarjeta))
            using (var graphics = System.Drawing.Graphics.FromImage(bitmap))
            {
                graphics.Clear(System.Drawing.Color.White); // Fondo blanco

                // Establecer la fuente para el texto
                var font = new System.Drawing.Font("Arial", 14); // Aumenta el tamaño de la fuente
                var brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);

                // Dibuja la imagen del usuario en la parte superior
                if (usuarioSeleccionado.Imagen != null)
                {
                    using (var imagenUsuario = ConvertirBytesAImagen(usuarioSeleccionado.Imagen))
                    {
                        graphics.DrawImage(imagenUsuario, (anchoTarjeta - 150) / 2, 10, 100, 150); // Dibuja la imagen centrada
                    }
                }

                // Dibuja la información del usuario debajo de la imagen
                graphics.DrawString($"Documento: {usuarioSeleccionado.Documento}", font, brush, 10, 170);
                graphics.DrawString($"Nombre: {usuarioSeleccionado.Nombre} {usuarioSeleccionado.Apellido}", font, brush, 10, 210);
                graphics.DrawString($"Grado: {usuarioSeleccionado.Grado}", font, brush, 10, 250);
                graphics.DrawString($"Unidad: {usuarioSeleccionado.Unidad}", font, brush, 10, 290);

                // Generar el código de barras
                var codigoBarras = GenerarCodigoDeBarras(usuarioSeleccionado.Documento);
                graphics.DrawImage(codigoBarras, (anchoTarjeta - 200) / 2, 320, 200, 100); // Dibuja el código de barras centrado

                // Guardar la tarjeta como PNG
                bitmap.Save(rutaPNG, System.Drawing.Imaging.ImageFormat.Png);
            }

            MessageBox.Show("PNG de carnet generado exitosamente en el escritorio.");
        }

        // Método para generar código de barras
        public Bitmap GenerarCodigoDeBarras(string texto)
        {
            var writer = new BarcodeWriter
            {
                Format = ZXing.BarcodeFormat.CODE_128,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 200,
                    Height = 100
                }
            };
            return writer.Write(texto);
        }

        // Método para convertir byte[] a System.Drawing.Image
        public System.Drawing.Image ConvertirBytesAImagen(byte[] imagenBytes)
        {
            using (var ms = new MemoryStream(imagenBytes))
            {
                return System.Drawing.Image.FromStream(ms);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string filtro = txtBuscar.Text.ToLower();

            // Filtrar la lista de usuarios
            var usuariosFiltrados = usuarios.Where(u =>
                u.Documento.ToLower().Contains(filtro) ||
                u.Nombre.ToLower().Contains(filtro) ||
                u.Apellido.ToLower().Contains(filtro) ||
                u.Grado.ToLower().Contains(filtro) ||
                u.Unidad.ToLower().Contains(filtro)).ToList();

            // Asignar la lista filtrada al BindingSource
            bindingSource.DataSource = usuariosFiltrados;
        }
    }
}
