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
            // Crear una instancia de BarcodeWriter
            BarcodeWriter escritorCodigoBarras = new BarcodeWriter
            {
                Format = BarcodeFormat.CODE_128,  // Puedes cambiar a otros formatos si lo prefieres
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 300,  // Ancho del código de barras
                    Height = 100,  // Altura del código de barras
                    Margin = 10    // Margen
                }
            };

            // Generar el código de barras en formato Bitmap
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
                Imagen = ConvertirImagenABlobs(imagenUsuario), // Usando el método renombrado
                CarnetCodigoBarras = GenerarCodigoDeBarrasComoBlobs(txtDocumento.Text), // Usando el método renombrado
                Vehiculo = new Vehiculo
                {
                    Marca = txtMarca.Text,
                    Modelo = txtModelo.Text,
                    Placa = txtPlaca.Text,
                    Imagen_vehiculo = ConvertirImagenABlobs(imagenVehiculo) // Usando el método renombrado
                }
            };

            UsuarioController usuarioController = new UsuarioController();
            bool exito = usuarioController.GuardarUsuario(nuevoUsuario);

            if (exito)
            {
                MessageBox.Show("Usuario y vehículo guardados exitosamente.");

                // Generar el PDF del carnet
                GenerarCarnetPDF(nuevoUsuario);

                LimpiarCampos();
            }
        }

        public byte[] ConvertirImagenABlobs(System.Drawing.Image imagen)
        {
            if (imagen != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    imagen.Save(ms, System.Drawing.Imaging.ImageFormat.Png); // Guardar como PNG
                    return ms.ToArray(); // Retornar el byte array
                }
            }
            return null; // Retornar null si la imagen es nula
        }

        // Método para generar un código de barras como un BLOB (byte array)
        public byte[] GenerarCodigoDeBarrasComoBlobs(string codigo)
        {
            if (!string.IsNullOrEmpty(codigo))
            {
                // Crear el generador de código de barras
                BarcodeWriter barcodeWriter = new BarcodeWriter
                {
                    Format = BarcodeFormat.CODE_128, // Puedes cambiar el formato según tus necesidades
                    Options = new ZXing.Common.EncodingOptions
                    {
                        Width = 300, // Ancho del código de barras
                        Height = 100, // Altura del código de barras
                        Margin = 10 // Margen
                    }
                };

                // Generar la imagen del código de barras
                using (Bitmap bitmap = barcodeWriter.Write(codigo))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png); // Guardar como PNG
                        return ms.ToArray(); // Retornar el byte array
                    }
                }
            }
            return null; // Retornar null si el código es nulo o vacío
        }


        private void LimpiarCampos()
        {
            // Limpiar los campos de texto
            txtDocumento.Text = string.Empty;
            txtNombre.Text = string.Empty;
            txtApellido.Text = string.Empty;
            txtGrado.Text = string.Empty;
            txtUnidad.Text = string.Empty;
            txtMarca.Text = string.Empty;
            txtModelo.Text = string.Empty;
            txtPlaca.Text = string.Empty;

            // Restablecer las imágenes a null o una imagen predeterminada
            imagenUsuario = null; // Asigna null o la imagen predeterminada
            imagenVehiculo = null; // Asigna null o la imagen predeterminada

            // Si tienes PictureBox para mostrar imágenes, también debes limpiar su contenido
            pctbFotoUsuario.Image = null; // PictureBox para la imagen del usuario
            pctbVehiculo.Image = null; // PictureBox para la imagen del vehículo


        }



        // Método modificado para convertir Bitmap a byte[]
        

        private void btnCargarFoto_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    imagenUsuario = new Bitmap(openFileDialog.FileName); // Guardar la imagen en la variable
                    pctbFotoUsuario.Image = imagenUsuario; // Mostrar la imagen en el PictureBox
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
                    imagenVehiculo = new Bitmap(openFileDialog.FileName); // Guardar la imagen en la variable
                    pctbVehiculo.Image = imagenVehiculo; // Mostrar la imagen en el PictureBox
                }
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

                // Añadir la imagen del usuario
                if (usuario.Imagen != null)
                {
                    iTextSharp.text.Image fotoUsuario = iTextSharp.text.Image.GetInstance(usuario.Imagen);
                    fotoUsuario.ScaleToFit(50f, 70f); // Ajustar el tamaño de la imagen
                    fotoUsuario.Alignment = Element.ALIGN_CENTER;
                    documento.Add(fotoUsuario);
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

                // Generar código de barras
                if (usuario.CarnetCodigoBarras != null)
                {
                    iTextSharp.text.Image codigoBarras = iTextSharp.text.Image.GetInstance(usuario.CarnetCodigoBarras);
                    codigoBarras.ScaleToFit(150f, 30f); // Ajustar el tamaño del código de barras
                    codigoBarras.Alignment = Element.ALIGN_CENTER;
                    documento.Add(codigoBarras);
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





        // Método para generar un código de barras como imagen en bytes
        public byte[] ObtenerCodigoDeBarrasComoBytes(string codigo)
        {
            if (!string.IsNullOrEmpty(codigo))
            {
                Barcode128 barcode = new Barcode128
                {
                    Code = codigo
                };
                using (MemoryStream ms = new MemoryStream())
                {
                    // Especificar el espacio de nombres para evitar la ambigüedad
                    System.Drawing.Image imagenCodigo = barcode.CreateDrawingImage(Color.Black, Color.White);
                    imagenCodigo.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return ms.ToArray();
                }
            }
            return null;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Verifica si la ventana ListaUsuarios ya está abierta
            foreach (Form form in Application.OpenForms)
            {
                if (form is ListaUsuarios)
                {
                    form.BringToFront(); // Si ya está abierta, la trae al frente
                    return;
                }
            }

            // Si no está abierta, crea una nueva instancia
            ListaUsuarios listaUsuarios = new ListaUsuarios();
            listaUsuarios.Show(); // o listaUsuarios.ShowDialog() para un diálogo modal
        }

    }
}

