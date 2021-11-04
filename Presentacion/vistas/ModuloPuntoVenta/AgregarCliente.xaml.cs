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

namespace Presentacion.vistas.ModuloPuntoVenta
{
    /// <summary>
    /// Lógica de interacción para AgregarCliente.xaml
    /// </summary>
    public partial class AgregarCliente : UserControl
    {
    OracleConnection conn = null;
    MantenedorClienteBS mantenedorClienteBS;
        public AgregarCliente(string nombre)
        {
            AbrirConexion();
            InitializeComponent();
            mantenedorClienteBS = new MantenedorClienteBS();
            this.nombre = nombre;
        }
        string nombre;
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

        private void btn_guardar_Click(object sender, RoutedEventArgs e)
        {
            GuardarCliente();
        }

        private void btn_volver_Click(object sender, RoutedEventArgs e)
        {
            Volver();

        }

        private void Volver()
        {
            ModuloVenta main = new ModuloVenta(nombre);
            Content = main;

        }

        private Cliente CrearCliente()
        {
            Cliente cliente = new Cliente();
            try
            {
                cliente.Rut = txt_rut.Text;
                cliente.Nombre = txt_nombre.Text;
                cliente.ApellidoP = txt_aPaterno.Text;
                cliente.Telefono = txt_telefono.Text;
                cliente.Prevision = txt_prevision.Text;
                cliente.Direccion = txt_direccion.Text;
                cliente.Comuna = txt_comuna.Text;
                cliente.Correo = txt_correo.Text;
            }
            catch
            {
                //MessageBox.Show(""+ ex.Message);
            }
            return cliente;

        }
        private bool ExisteRut(string rut)
        {
            bool existe = false;

            foreach (Cliente cli in ListarClientes())
            {
                if (cli.Rut.Equals(rut))
                {
                    existe = true;
                }
            }

            return existe;
        }

        public List<Cliente> ListarClientes()
        {
            OracleCommand cmd = new OracleCommand("FN_LISTAR_CLIENTE", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            List<Cliente> lista = new List<Cliente>();

            OracleParameter output = cmd.Parameters.Add("l_cursor", OracleDbType.RefCursor);
            output.Direction = ParameterDirection.ReturnValue;
            cmd.ExecuteNonQuery();
            OracleDataReader reader = ((OracleRefCursor)output.Value).GetDataReader();

            while (reader.Read())
            {
                Cliente cli = new Cliente();
                cli.Rut = reader.GetString(0);
                cli.Nombre = reader.GetString(1);
                cli.ApellidoP = reader.GetString(2);
                cli.Telefono = reader.GetString(3);
                cli.Prevision = reader.GetString(4);
                cli.Direccion = reader.GetString(5);
                cli.Comuna = reader.GetString(6);
                cli.Correo = reader.GetString(7);


                lista.Add(cli);
                Debug.WriteLine(lista);
            }
            dtg_clientes.ItemsSource = lista;
            return lista;
        }
        private void GuardarCliente()
        {
            if (txt_rut.Text.Length != 0)
            {
                Cliente cliente = CrearCliente();
                if (!ExisteRut(cliente.Rut))
                {
                    try
                    {
                        //mantenedorEmpleado.ValidarEmpleado(empleado);
                        mantenedorClienteBS.Validacion(cliente);
                        OracleCommand cmd = new OracleCommand("SP_GUARDAR_CLIENTE", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        //mantenedorEmpleado.ValidarEmpleado(cmd);
                        cmd.Parameters.Add("rut", OracleDbType.Varchar2).Value = txt_rut.Text;
                        cmd.Parameters.Add("nombre", OracleDbType.Varchar2).Value = txt_nombre.Text;
                        cmd.Parameters.Add("apellidop", OracleDbType.Varchar2).Value = txt_aPaterno.Text;
                        cmd.Parameters.Add("telefono", OracleDbType.Varchar2).Value = txt_telefono.Text;
                        cmd.Parameters.Add("prevision", OracleDbType.Varchar2).Value = txt_prevision.Text;
                        cmd.Parameters.Add("direccion", OracleDbType.Varchar2).Value = txt_direccion.Text;
                        cmd.Parameters.Add("comuna", OracleDbType.Varchar2).Value = txt_comuna.Text;
                        cmd.Parameters.Add("correo", OracleDbType.Varchar2).Value = txt_correo.Text;
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("El cliente: " + cliente.Nombre + " Fue Agregado al sistema");
                        MessageBoxResult m = MessageBox.Show("Quiere agregar otro cliente", "Guardar Cliente", MessageBoxButton.OKCancel);
                        if (m == MessageBoxResult.Cancel)
                        {
                            ModuloVenta main = new ModuloVenta(nombre);
                            Content = main;
                        }
                        Limpiar();
                    }
                    catch (Exception ex)
                    {

                        Debug.WriteLine(ex.Message);
                        MessageBox.Show("ERROR AL CREAR CLIENTE: " + ex.Message);
                    }

                    CargarClientes();
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

        private void Limpiar()
        {
            txt_rut.Text = "";
            txt_nombre.Text = "";
            txt_aPaterno.Text = "";
            txt_telefono.Text = "";
            txt_prevision.Text = "";
            txt_direccion.Text = "";
            txt_correo.Text = "";
            txt_comuna.Text = "";
        }

        private void dtg_clientes_Loaded(object sender, RoutedEventArgs e)
        {
            CargarClientes();
        }

        private void CargarClientes()
        {
            try
            {

                ListarClientes();

            }
            catch (Exception)
            {

                MessageBox.Show("Error al cargar datos");
            }
        }

        private void txt_nombre_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidarSoloTexto(sender, e);
        }
        private void txt_aPaterno_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidarSoloTexto(sender, e);
        }
        private void txt_comuna_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidarSoloTexto(sender, e);
        }
        private void ValidarSoloTexto(object sender, TextCompositionEventArgs e)
        {
            int ascci = Convert.ToInt32(Convert.ToChar(e.Text));

            if (ascci >= 65 && ascci <= 90 || ascci >= 97 && ascci <= 122)

                e.Handled = false;

            else e.Handled = true;
        }

        private void dtg_clientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}

