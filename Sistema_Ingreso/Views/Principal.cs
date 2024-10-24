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
    public partial class Principal : Form
    {
        public Principal()
        {
            InitializeComponent();
        }

        private void pnlPrincipal_Paint(object sender, PaintEventArgs e)
        {

        }


        private void AbrirFormulario(UserControl userControl)
        {
            // Limpiar el panel antes de mostrar el nuevo User Control
            if (this.pnlPrincipal.Controls.Count > 0)
                this.pnlPrincipal.Controls.RemoveAt(0);

            // Configurar el User Control para que ocupe todo el panel
            userControl.Dock = DockStyle.Fill;
            this.pnlPrincipal.Controls.Add(userControl);
            this.pnlPrincipal.Tag = userControl;
            userControl.Show();
        }

        private void btnUsuarios_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new UsuariosView());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new Asistencia());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new Reportes());
        }
    }
}
