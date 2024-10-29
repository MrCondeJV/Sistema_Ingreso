using iTextSharp.text.pdf;
using iTextSharp.text;
using Sistema_Ingreso.Controller;
using Sistema_Ingreso.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using Sistema_Ingreso.Views;

namespace Sistema_Ingreso
{
    public partial class UsuariosView : UserControl
    {
        Bitmap imagenVehiculo;
        Bitmap imagenUsuario;

        public UsuariosView()
        {
            InitializeComponent();
        }

        private void btnGenerrarBarra_Click(object sender, EventArgs e)
        {
            // Obtener el texto del TextBox que será convertido en código de barras
            string data = txtDocumento.Text;

            // Verificar si el texto no está vacío
            if (!string.IsNullOrEmpty(data))
            {
                // Llamar al método que genera el código de barras
                pictureBoxBarcode.Image = GenerarCodigoBarras(data);
            }
            else
            {
                MessageBox.Show("Por favor, ingresa un texto o número para generar el código de barras.");
            }
        }

        // Método para generar el código de barras
        private Bitmap GenerarCodigoBarras(string data)
        {
            BarcodeWriter escritorCodigoBarras = new BarcodeWriter
            {
                Format = BarcodeFormat.CODE_128,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 300,
                    Height = 100,
                    Margin = 10
                }
            };

            Bitmap bitmap = escritorCodigoBarras.Write(data);
            return bitmap;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void txtGrado_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtApellido_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtNombre_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtDocumento_TextChanged(object sender, EventArgs e)
        {

        }
        private void button2_Click(object sender, EventArgs e)
        {
            Models.Usuario nuevoUsuario = new Models.Usuario
            {
                Documento = txtDocumento.Text,
                Nombre = txtNombre.Text,
                Apellido = txtApellido.Text,
                Grado = txtGrado.Text,
                Unidad = txtUnidad.Text,
                Imagen = ConvertirImagenABlobs(imagenUsuario),
                CarnetCodigoBarras = GenerarCodigoDeBarrasComoBlobs(txtDocumento.Text),
                Vehiculo = new Vehiculo
                {
                    Marca = txtMarca.Text,
                    Modelo = txtModelo.Text,
                    Placa = txtPlaca.Text,
                    Imagen_vehiculo = ConvertirImagenABlobs(imagenVehiculo)
                }
            };

            UsuarioController usuarioController = new UsuarioController();
            bool exito = usuarioController.GuardarUsuario(nuevoUsuario);

            if (exito)
            {
                MessageBox.Show("Usuario y vehículo guardados exitosamente.");

                // Generar la imagen del carnet en lugar de PDF
                GenerarCarnetPNG(nuevoUsuario);

                LimpiarCampos();
            }
        }

        public byte[] ConvertirImagenABlobs(System.Drawing.Image imagen)
        {
            if (imagen != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    imagen.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return ms.ToArray();
                }
            }
            return null;
        }

        public byte[] GenerarCodigoDeBarrasComoBlobs(string codigo)
        {
            if (!string.IsNullOrEmpty(codigo))
            {
                BarcodeWriter barcodeWriter = new BarcodeWriter
                {
                    Format = BarcodeFormat.CODE_128,
                    Options = new ZXing.Common.EncodingOptions
                    {
                        Width = 300,
                        Height = 100,
                        Margin = 10
                    }
                };

                using (Bitmap bitmap = barcodeWriter.Write(codigo))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        return ms.ToArray();
                    }
                }
            }
            return null;
        }

        private void LimpiarCampos()
        {
            txtDocumento.Text = string.Empty;
            txtNombre.Text = string.Empty;
            txtApellido.Text = string.Empty;
            txtGrado.Text = string.Empty;
            txtUnidad.Text = string.Empty;
            txtMarca.Text = string.Empty;
            txtModelo.Text = string.Empty;
            txtPlaca.Text = string.Empty;

            imagenUsuario = null;
            imagenVehiculo = null;

            pctbFotoUsuario.Image = null;
            pctbVehiculo.Image = null;
        }

        private void btnCargarFoto_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    imagenUsuario = new Bitmap(openFileDialog.FileName);
                    pctbFotoUsuario.Image = imagenUsuario;
                }
            }
        }

        private void btnCargarFotoVehiculo_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    imagenVehiculo = new Bitmap(openFileDialog.FileName);
                    pctbVehiculo.Image = imagenVehiculo;
                }
            }
        }

        public void GenerarCarnetPNG(Models.Usuario usuario)
        {
            // Ruta donde se guardará el PNG
            string rutaPNG = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{usuario.Documento}_Carnet.png");

            using (Bitmap bitmap = new Bitmap(400, 600)) // Dimensiones del carnet
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.White);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    // Añadir la imagen del usuario
                    if (usuario.Imagen != null)
                    {
                        using (System.Drawing.Image fotoUsuario = System.Drawing.Image.FromStream(new MemoryStream(usuario.Imagen)))
                        {
                            // Ajusta la posición y tamaño de la imagen del usuario
                            int fotoAncho = 100;
                            int fotoAlto = 150;
                            int fotoX = (bitmap.Width - fotoAncho) / 2; // Centrado horizontal
                            int fotoY = 10; // Posición vertical
                            g.DrawImage(fotoUsuario, new System.Drawing.Rectangle(fotoX, fotoY, fotoAncho, fotoAlto));
                        }
                    }

                    // Añadir la información del usuario
                    g.DrawString($"Documento: {usuario.Documento}", new System.Drawing.Font("Arial", 12), Brushes.Black, new PointF(10, 170));
                    g.DrawString($"Nombre: {usuario.Nombre} {usuario.Apellido}", new System.Drawing.Font("Arial", 12), Brushes.Black, new PointF(10, 210));
                    g.DrawString($"Grado: {usuario.Grado}", new System.Drawing.Font("Arial", 12), Brushes.Black, new PointF(10, 250));
                    g.DrawString($"Unidad: {usuario.Unidad}", new System.Drawing.Font("Arial", 12), Brushes.Black, new PointF(10, 290));

                    // Añadir código de barras
                    if (usuario.CarnetCodigoBarras != null)
                    {
                        using (System.Drawing.Image codigoBarras = System.Drawing.Image.FromStream(new MemoryStream(usuario.CarnetCodigoBarras)))
                        {
                            // Ajusta la posición y tamaño del código de barras
                            int codigoAncho = 280;
                            int codigoAlto = 100;
                            int codigoX = (bitmap.Width - codigoAncho) / 2; // Centrado horizontal
                            int codigoY = 320; // Posición vertical
                            g.DrawImage(codigoBarras, new System.Drawing.Rectangle(codigoX, codigoY, codigoAncho, codigoAlto));
                        }
                    }
                }

                // Guardar el bitmap como PNG
                bitmap.Save(rutaPNG, System.Drawing.Imaging.ImageFormat.Png);
                MessageBox.Show("PNG del carnet generado exitosamente.");
            }
        }



        private void button3_Click(object sender, EventArgs e)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form is ListaUsuarios)
                {
                    form.BringToFront();
                    return;
                }
            }

            ListaUsuarios listaUsuarios = new ListaUsuarios();
            listaUsuarios.Show();
        }
    }
}
