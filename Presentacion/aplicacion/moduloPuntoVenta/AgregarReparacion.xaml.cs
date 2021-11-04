using Modelo.aplicacion.modelo;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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

namespace Presentacion.aplicacion.moduloPuntoVenta
{
    /// <summary>
    /// Lógica de interacción para AgregarReparacion.xaml
    /// </summary>
    public partial class AgregarReparacion : Window
    {

        OracleConnection conn = null;
        public AgregarReparacion(string nombre)
        {
            this.nombre = nombre;
            InitializeComponent();
            AbrirConexion();
            IniciarMedioPago();
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

        public void IniciarMedioPago()
        {
            OracleCommand cmd = new OracleCommand("FN_LISTAR_MEDIO_PAGO", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            List<MedioPago> medioPagos = new List<MedioPago>();

            OracleParameter output = cmd.Parameters.Add("L_CURSOR", OracleDbType.RefCursor);
            output.Direction = ParameterDirection.ReturnValue;
            cmd.ExecuteNonQuery();
            OracleDataReader reader = ((OracleRefCursor)output.Value).GetDataReader();

            while (reader.Read())
            {
                MedioPago medio = new MedioPago();
                medio.IdMedioPago = reader.GetInt32(0);
                medio.Nombre = reader.GetString(1);
                medioPagos.Add(medio);
            }
            cmb_medioPago.ItemsSource = medioPagos;
            cmb_medioPago.Items.Refresh();
        }

        public int ObtenerSucursal()
        {


            OracleCommand comando = new OracleCommand("SELECT ID_SUCURSAL FROM USUARIO  WHERE USUARIO = :nombre", conn);
            comando.Parameters.Add(":nombre", nombre);
            OracleDataReader reader = comando.ExecuteReader();
            while (reader.Read())
            {
                Usuario usuario = new Usuario();
                usuario.Sucursal = new Sucursal();
                usuario.Sucursal.IdSucursal = reader.GetInt32(0);
                int sucu = usuario.Sucursal.IdSucursal;
                return sucu;
            }

            return 0;
        }

        private void btn_cancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btn_confirmar_Click(object sender, RoutedEventArgs e)
        {
            GuardarReparacion();
        }

        private void GuardarReparacion()
        {
            Reparacion reparacion = CrearReparacion();
            try
            {
                object medioSeleccionado = cmb_medioPago.SelectedItem;
                MedioPago medio = (MedioPago)medioSeleccionado;
                //mantenedorMonturaBS.Validacion(montura);
                OracleCommand cmd = new OracleCommand("SP_GUARDAR_REPARACION", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("nombreUsuario", OracleDbType.Varchar2).Value = reparacion.NombreUsuario;
                cmd.Parameters.Add("idSucursal", OracleDbType.Int32).Value = ObtenerSucursal();
                cmd.Parameters.Add("descripcion", OracleDbType.Varchar2).Value = reparacion.Descripcion;
                if (medio == null)
                {
                    throw new Exception("DEBE AGREGAR EL MEDIO DE PAGO");
                }
                else
                {

                    cmd.Parameters.Add("idMedioPago", OracleDbType.Int32).Value = medio.IdMedioPago;
                }
                cmd.Parameters.Add("montoPagado", OracleDbType.Int32).Value = reparacion.MontoPagado;
                cmd.ExecuteNonQuery();
                MessageBox.Show("La reparacion Fue Agregado al sistema");
                Close();
                //Limpiar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CREAR CLIENTE: " + ex.Message);
            }

            //CargarClientes();
            //GuardarDatos(empleado);
        }

        private Reparacion CrearReparacion()
        {
            Reparacion reparacion = new Reparacion();
            try
            {
                reparacion.Sucursal = new Sucursal();
                reparacion.Sucursal.IdSucursal = ObtenerSucursal();
                reparacion.NombreUsuario = nombre;
                reparacion.Descripcion = txt_observacion.Text;
                //reparacion.MedioPago = (MedioPago)cmb_medioPago.SelectedItem;
                
                //reparacion.MedioPago.IdMedioPago = (MedioPago)cmb_medioPago.SelectedItem;
                reparacion.MontoPagado = Convert.ToInt32(txt_monto.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CREAR REPARACION: " + ex.Message);
            }
            return reparacion;
        }
    }
}