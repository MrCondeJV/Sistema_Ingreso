using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;

namespace Sistema_Ingreso
{
    public partial class Usuarios : UserControl
    {
        public Usuarios()
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
    }
}
