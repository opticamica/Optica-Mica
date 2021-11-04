using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Build.Utilities;
using Microsoft.Extensions.Logging;
using Modelo.aplicacion.modelo;
using Oracle.ManagedDataAccess.Client;
using Presentacion.aplicacion;
using Presentacion.vistas.ModuloPuntoVenta;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Presentacion
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        OracleConnection conn = null;
        string nombre;
        public MainWindow()
        {
            InitializeComponent();

            AbrirConexion();

        }
        

        private void AbrirConexion()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["oracleDB"].ConnectionString;
            conn = new OracleConnection(connectionString);
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("", "error de conexion");
                throw new Exception("error"+ex);
            }
        }

        private void btn_aceptar_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void Login()
        {
            if (txt_usuario.Text.Length != 0)
            {
                if (txt_password.Password.Length != 0)
            {
                OracleCommand comando = new OracleCommand("SELECT * FROM USUARIO WHERE USUARIO = :usuario AND CONTRASEÑA = :contra", conn);
                comando.Parameters.Add(":usuario", txt_usuario.Text.ToLower());
                comando.Parameters.Add(":contra", txt_password.Password);
                OracleDataReader reader = comando.ExecuteReader();
                    
                if (reader.Read())
                {
                    IniciarSesion();
                    MenuPrincipal menuPrincipal = new MenuPrincipal(nombre.ToLower());
                        //menuPrincipal.Owner = this;
                        //menuPrincipal.ShowDialog();
                        menuPrincipal.Show();
                    Close();
                }

                else
                {
                        this.ShowMessageAsync("", "Usuario o contraseña invalida");
                }


            }
            else
            {
                    this.ShowMessageAsync("", "DEBE INGRESAR CONTRASEÑA");
                }
            }
            else
            {
                this.ShowMessageAsync("", "DEBE INGRESAR USUARIO");
            }
        }

        public void IniciarSesion()
        {
            try
            {

            Usuario usuario = new Usuario();
            OracleCommand cmd = new OracleCommand("SP_INICIO_SESION", conn);
            cmd.CommandType = CommandType.StoredProcedure;
                //mantenedorEmpleado.ValidarEmpleado(cmd);
                cmd.Parameters.Add("usuario1", OracleDbType.Varchar2).Value = txt_usuario.Text.ToLower();
                nombre = txt_usuario.Text;
            //MessageBox.Show("inicio sesion");
            cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message);
                this.ShowMessageAsync("", "ERROR AL INICIAR SESION: " + ex.Message);
            }
            
        }

        public Usuario CrearUsuario()
        {
            Usuario usuario = new Usuario();
            try
            {
                usuario.User = txt_usuario.Text;
            }
            catch
            {
                //MessageBox.Show(""+ ex.Message);
            }
            return usuario;

        }

        private void txt_password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login();
            }
        }

        private void btn_registrar_Click(object sender, RoutedEventArgs e)
        {
            RegistroUsuario registro= new RegistroUsuario();
            registro.Owner = this;
            registro.ShowDialog();
        }
        
    }
}
