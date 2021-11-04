using Modelo.aplicacion.modelo;
using Negocio.aplicacion.negocio;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
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
using System.Windows.Shapes;

namespace Presentacion.aplicacion
{
    /// <summary>
    /// Lógica de interacción para RegistroUsuario.xaml
    /// </summary>
    public partial class RegistroUsuario : Window
    {
        OracleConnection conn = null;
        private MantenedorUsuarioBS mantenedorUsuarioBS;
        public RegistroUsuario()
        {
            AbrirConexion();
            InitializeComponent();
            IniciarSucursal();
            IniciarTipoUsuario();
            mantenedorUsuarioBS = new MantenedorUsuarioBS();
        }

        private void AbrirConexion()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["oracleDB"].ConnectionString;
            conn = new OracleConnection(connectionString);
            try
            {
                conn.Open();
            }
            catch (Exception)
            {
                MessageBox.Show("error de conexion");
                throw new Exception("error");
            }
        }

        public void IniciarSucursal()
        {
            OracleCommand cmd = new OracleCommand("FN_LISTAR_SUCURSAL", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            List<Sucursal> sucursales = new List<Sucursal>();

            OracleParameter output = cmd.Parameters.Add("L_CURSOR", OracleDbType.RefCursor);
            output.Direction = ParameterDirection.ReturnValue;
            cmd.ExecuteNonQuery();
            OracleDataReader reader = ((OracleRefCursor)output.Value).GetDataReader();

            while (reader.Read())
            {
                Sucursal sucursal = new Sucursal();
                sucursal.IdSucursal = reader.GetInt32(0);
                sucursal.Nombre = reader.GetString(1);
                sucursales.Add(sucursal);

            }
            cmb_sucursal.ItemsSource = sucursales;
            cmb_sucursal.Items.Refresh();
        }

        public void IniciarTipoUsuario()
        {
            OracleCommand cmd = new OracleCommand("FN_LISTAR_TIPO_USUARIO", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            List<TipoUsuario> tipoUsuarios = new List<TipoUsuario>();

            OracleParameter output = cmd.Parameters.Add("L_CURSOR", OracleDbType.RefCursor);
            output.Direction = ParameterDirection.ReturnValue;
            cmd.ExecuteNonQuery();
            OracleDataReader reader = ((OracleRefCursor)output.Value).GetDataReader();

            while (reader.Read())
            {
                TipoUsuario tipo = new TipoUsuario();
                tipo.IdTipo = reader.GetInt32(0);
                tipo.Nombre = reader.GetString(1);
                tipoUsuarios.Add(tipo);

            }
            cmb_tipoUsuario.ItemsSource = tipoUsuarios;
            cmb_tipoUsuario.Items.Refresh();
        }

        private void btn_volver_Click(object sender, RoutedEventArgs e)
        {
            Volver();
            
        }

        private void Volver()
        {
            MainWindow main = new MainWindow();
            main.Owner = this;

            this.Close();
        }

        private void btn_guardar_Click(object sender, RoutedEventArgs e)
        {
            generarNombreUsuario();
            ValidarPassword();
            GuardarUsuario();
        }

         private Usuario CrearUsuario()
        {
            Usuario usuario= new Usuario();
            try
            {
                usuario.Rut = txt_rut.Text;
                usuario.Nombre = txt_nombre.Text;
                usuario.Apellidop = txt_aPaterno.Text;
                //usuario.Apellidom = txt_aMaterno.Text;
                usuario.User = txt_usuario.Text;
                usuario.Password = txt_password.Password;
                //usuario.Direccion = txt_direccion.Text;
                usuario.Correo = txt_correo.Text;
                //usuario.Telefono = txt_telefono.Text;
                usuario.Sucursal = (Sucursal)cmb_sucursal.SelectedItem;
                usuario.TipoUsuario = (TipoUsuario)cmb_tipoUsuario.SelectedItem; 
            }
            catch
            {
                //MessageBox.Show(""+ ex.Message);
            }
            return usuario;

        }
        
        private bool ExisteRut(string rut)
        {
            bool existe = false;

            foreach (Usuario usu in ListarUsuario())
            {
                if (usu.Rut.Equals(rut))
                {
                    existe = true;
                }
            }

            return existe;
        }

        private bool ExisteUsuario(string user)
        {
            bool existe = false;

            foreach (Usuario usu in ListarUsuario())
            {
                if (usu.User.Equals(user))
                {
                    existe = true;
                }
            }

            return existe;
        }


        public List<Usuario> ListarUsuario()
        {
            OracleCommand cmd = new OracleCommand("FN_LISTAR_USUARIO", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            List<Usuario> lista = new List<Usuario>();

            OracleParameter output = cmd.Parameters.Add("l_cursor", OracleDbType.RefCursor);
            output.Direction = ParameterDirection.ReturnValue;
            cmd.ExecuteNonQuery();
            OracleDataReader reader = ((OracleRefCursor)output.Value).GetDataReader();

            while (reader.Read())
            {
                Usuario emp = new Usuario();
                emp.Rut = reader.GetString(0);
                emp.Nombre = reader.GetString(1);
                emp.Apellidop = reader.GetString(2);
                //emp.Apellidom = reader.GetString(3);
                emp.User = reader.GetString(3);
                emp.Password = reader.GetString(4);
                //emp.Direccion = reader.GetString(6);
                emp.Correo = reader.GetString(5);
                //emp.Telefono = reader.GetString(8);
                emp.Sucursal = new Sucursal();
                emp.Sucursal.Nombre = reader.GetString(6);
                emp.TipoUsuario = new TipoUsuario();
                emp.TipoUsuario.Nombre = reader.GetString(7);


                lista.Add(emp);
                Debug.WriteLine(lista);
            }
            //dtg_usuarios.ItemsSource = lista;
            return lista;
        }

        private void GuardarUsuario()
        {
            if (txt_rut.Text.Length != 0)
            {
                Usuario usuario = CrearUsuario();
                //Empleado empleado = CrearEmpleado();
                if(!ExisteRut(usuario.Rut))
                {
                    try
                    {
                        //mantenedorEmpleado.ValidarEmpleado(empleado);
                        mantenedorUsuarioBS.Validacion(usuario);
                        OracleCommand cmd = new OracleCommand("SP_GUARDAR_USUARIO", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        //mantenedorEmpleado.ValidarEmpleado(cmd);
                        cmd.Parameters.Add("rut", OracleDbType.Varchar2).Value = txt_rut.Text;
                        cmd.Parameters.Add("nombre", OracleDbType.Varchar2).Value = txt_nombre.Text;
                        cmd.Parameters.Add("apellidop", OracleDbType.Varchar2).Value = txt_aPaterno.Text;
                        //cmd.Parameters.Add("apellidom", OracleDbType.Varchar2).Value = txt_aMaterno.Text;
                        cmd.Parameters.Add("usuario", OracleDbType.Varchar2).Value = txt_usuario.Text;
                        cmd.Parameters.Add("pass", OracleDbType.Varchar2).Value = txt_password.Password;
                        //cmd.Parameters.Add("direccion", OracleDbType.Varchar2).Value = txt_direccion.Text;
                        cmd.Parameters.Add("correo", OracleDbType.Varchar2).Value = txt_correo.Text;
                        //cmd.Parameters.Add("telefono", OracleDbType.Varchar2).Value = txt_telefono.Text;
                        cmd.Parameters.Add("sucursal", OracleDbType.Int32).Value = cmb_sucursal.SelectedIndex +1;
                        cmd.Parameters.Add("tipo", OracleDbType.Int32).Value = cmb_tipoUsuario.SelectedIndex +1;

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("El usuario: " + usuario.Nombre + " Fue Agregado al sistema\n"+"Su nombre de usuario es: "+usuario.User);
                        Limpiar();
                        Volver();
                    }
                    catch (Exception ex)
                    {

                        Debug.WriteLine(ex.Message);
                        MessageBox.Show("ERROR AL CREAR USUARIO: " + ex.Message);
                    }
                    //CargarDatos();

                    //GuardarDatos(empleado);
                }
                else
                {
                    MessageBox.Show("El Rut ya existe. ");

                }
                
            }
            else
            {
                MessageBox.Show("Debe ingresar Rut");
            }
        }


        public void ValidarPassword()
        {

            if (txt_password.Password != txt_password2.Password)
            {
                MessageBox.Show("Las contraseñas deben ser las mismas");
                txt_password.Password = "";
                txt_password2.Password = "";
            }
        }

        private void Limpiar()
        {
            txt_rut.Text = "";
            txt_nombre.Text = "";
            txt_aPaterno.Text = "";
            //txt_aMaterno.Text = "";
            txt_usuario.Text = "";
            txt_password.Password = "";
            txt_password2.Password = "";
            //txt_direccion.Text = "";
            txt_correo.Text = "";
            //txt_telefono.Text = "";
            cmb_sucursal.SelectedValue = null;
            cmb_tipoUsuario.SelectedValue = null;
        }


        public void generarNombreUsuario()
        {
            if (txt_nombre.Text.Length != 0 || txt_aPaterno.Text.Length != 0)
            {
                txt_usuario.Text = txt_nombre.Text.Substring(0, 1).ToLower() + txt_aPaterno.Text.ToLower();
                if (ExisteNombreUsuario(txt_usuario.Text))
                {
                    txt_usuario.Text = txt_nombre.Text.Substring(0, 2).ToLower() + txt_aPaterno.Text.ToLower();
                    if (ExisteNombreUsuario(txt_usuario.Text))
                    {
                        txt_usuario.Text = txt_nombre.Text.Substring(0, 3).ToLower() + txt_aPaterno.Text.ToLower();
                        if (ExisteNombreUsuario(txt_usuario.Text))
                        {
                            txt_usuario.Text = txt_nombre.Text.Substring(0, 4).ToLower() + txt_aPaterno.Text.ToLower();

                        }
                    }
                }
            }
        }


        private bool ExisteNombreUsuario(string nombre)
        {
            bool existe = false;

            foreach (Usuario usu in ListarUsuario())
            {
                if (usu.User.Equals(nombre))
                {
                    existe = true;
                }
            }

            return existe;
        }

        private void txt_nombre_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidarSoloTexto(sender,e);   
        }

        private void ValidarSoloTexto(object sender, TextCompositionEventArgs e)
        {
            int ascci = Convert.ToInt32(Convert.ToChar(e.Text));
          
            if (ascci >= 65 && ascci <= 90 || ascci >= 97 && ascci <= 122) 

                e.Handled = false;

            else e.Handled = true;
        }

        private void txt_aPaterno_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidarSoloTexto(sender, e);
        }
    }
}
