using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace Sistema_Ingreso.Models
{
    internal class Conexion
    {
        private MySqlConnection myCon;
        private string server;
        private string database;
        private string username;
        private string password;
        private string cadenaConexion;

        public Conexion()
        {
            server = "localhost";
            database = "db_sistema_ingreso"; // Corrige el nombre de la base de datos
            username = "root";
            password = "";
            cadenaConexion = "server=" + server + ";database=" + database + ";uid=" + username + ";pwd=" + password;
            myCon = new MySqlConnection(cadenaConexion);
        }

        public MySqlConnection ObtenerConexion()
        {
            return myCon;
        }

        public void Conectar()
        {
            try
            {
                myCon.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: " + ex.Message);
            }
        }

        public void Cerrar()
        {
            if (myCon != null && myCon.State == System.Data.ConnectionState.Open)
            {
                myCon.Close();
            }
        }
    }
}
