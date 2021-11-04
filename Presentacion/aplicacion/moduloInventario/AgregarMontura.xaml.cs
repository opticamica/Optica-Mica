using Microsoft.Win32;
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
using System.Drawing;
using System.IO;
using Negocio.aplicacion.negocio;
using MahApps.Metro.Controls;

namespace Presentacion.aplicacion.moduloInventario
{
    /// <summary>
    /// Lógica de interacción para AgregarMontura.xaml
    /// </summary>
    public partial class AgregarMontura : MetroWindow
    {
        OracleConnection conn = null;
        MantenedorMonturaBS mantenedorMonturaBS;
        string nombre,codMontura;
        string rutaImagen;
        public AgregarMontura(string nombre,string codMontura)
        {
            AbrirConexion();
            InitializeComponent();
            this.nombre = nombre;
            this.codMontura = codMontura;
            ObtenerSucursal();
            IniciarTipoMontura();
            IniciarProveedor();
            mantenedorMonturaBS = new MantenedorMonturaBS();
            txt_codMontura.Text = codMontura;
            txt_codMontura.IsEnabled = false;
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

        public void IniciarProveedor()
        {
            OracleCommand comando = new OracleCommand("SELECT * FROM PROVEEDOR  WHERE TIPO_PROVEEDOR = 'Montura' ORDER BY ID_PROVEEDOR Asc", conn);
            //comando.Parameters.Add(":nombre", txt_buscarNombrePaciente.Text);
            OracleDataReader reader = comando.ExecuteReader();
            List<Proveedor> proveedors = new List<Proveedor>();
            while (reader.Read())
            {
                Proveedor proveedor = new Proveedor();
                proveedor.IdProveedor = reader.GetInt32(0);
                proveedor.Nombre = reader.GetString(1);
                proveedors.Add(proveedor);
            }
            cmb_proveedor.ItemsSource = proveedors;
            cmb_proveedor.Items.Refresh();
        }
        public void IniciarTipoMontura()
        {
            OracleCommand cmd = new OracleCommand("FN_LISTAR_TIPO_MONTURA", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            List<TipoMontura> monturas = new List<TipoMontura>();

            OracleParameter output = cmd.Parameters.Add("L_CURSOR", OracleDbType.RefCursor);
            output.Direction = ParameterDirection.ReturnValue;
            cmd.ExecuteNonQuery();
            OracleDataReader reader = ((OracleRefCursor)output.Value).GetDataReader();

            while (reader.Read())
            {
                TipoMontura tipoMontura = new TipoMontura();
                tipoMontura.IdTipoMontura= reader.GetInt32(0);
                tipoMontura.Nombre = reader.GetString(1);
                monturas.Add(tipoMontura);

            }
            cmb_tipoMontura.ItemsSource = monturas;
            cmb_tipoMontura.Items.Refresh();
        }

        
        public byte getJPG(BitmapImage imageC)
        {
            MemoryStream memory = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageC));
            encoder.Save(memory);
            return Convert.ToByte(memory);
        }

        private void btn_guardar_Copy_Click(object sender, RoutedEventArgs e)
        {
            GuardarImagen();
        }

        private void GuardarImagen()
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "Archivos de imagen (.jpg)|*.jpg|All files (*.*)|*.*";
            file.FilterIndex = 1;
            file.Multiselect = false;
            if (file.ShowDialog()==true)
            {
                try
                {
                    BitmapImage foto = new BitmapImage();
                    foto.BeginInit();
                    foto.UriSource = new Uri(file.FileName);
                    foto.EndInit();
                    foto.Freeze();
                    
                    img_montura.Source = foto;
                    rutaImagen = foto.ToString();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR AL CARGAR IMAGEN"+ex.Message, "ERROR");
                }
            }
        }

        private int ObtenerSucursal()
        {

            OracleCommand comando = new OracleCommand("SELECT id_sucursal FROM USUARIO  WHERE USUARIO = :usuario ", conn);
            comando.Parameters.Add(":usuario", nombre);
            OracleDataReader reader = comando.ExecuteReader();
            Usuario usuario = new Usuario();
            usuario.Sucursal = new Sucursal();
            while (reader.Read())
            {
                
                usuario.Sucursal.IdSucursal = reader.GetInt32(0);
 
            }
            return usuario.Sucursal.IdSucursal;
        }
        
        private void GuardarMontura()
        {

            Montura montura = CrearMontura();
            if (!ExisteMontura(montura.CodMontura))
            {
                try
                {
                    mantenedorMonturaBS.Validacion(montura);
                    object tipoSeleccionado = cmb_tipoMontura.SelectedItem;
                    object ProveedorSeleccionado = cmb_proveedor.SelectedItem;
                    TipoMontura tipoMontura = (TipoMontura)tipoSeleccionado;
                    Proveedor proveedor = (Proveedor)ProveedorSeleccionado;
                    //mantenedorClienteBS.Validacion(cliente);
                    OracleCommand cmd = new OracleCommand("SP_GUARDAR_MONTURA", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //mantenedorEmpleado.ValidarEmpleado(cmd);
                    cmd.Parameters.Add("idSucursal", OracleDbType.Int32).Value = ObtenerSucursal();
                    
                    cmd.Parameters.Add("codMontura", OracleDbType.Varchar2).Value = txt_codMontura.Text;
                    cmd.Parameters.Add("tipoMontura", OracleDbType.Int32).Value =tipoMontura.IdTipoMontura;
                    cmd.Parameters.Add("color", OracleDbType.Varchar2).Value = txt_color.Text;
                    cmd.Parameters.Add("stock", OracleDbType.Int32).Value = txt_stock.Text;

                    cmd.Parameters.Add("imagen", OracleDbType.Varchar2).Value = montura.Imagen;

                    cmd.Parameters.Add("proveedor", OracleDbType.Int32).Value = proveedor.IdProveedor;


                    cmd.ExecuteNonQuery();
                    MessageBox.Show("La montura: " + montura.CodMontura + " Fue Agregado al sistema");
                    MessageBoxResult m = MessageBox.Show("Quiere agregar otra montura", "Guardar Montura", MessageBoxButton.OKCancel);
                    if (m == MessageBoxResult.Cancel)
                    {
                        Inventario inventario = new Inventario(nombre);
                        //inventario.CargarMonturas();
                        this.Close();
                    }
                    //Limpiar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR AL CREAR CLIENTE: " + ex.Message);
                }

                //CargarClientes();
                //GuardarDatos(empleado);
            }
            else
            {
                MessageBox.Show("El codigo de montura ya existe. ");
            }
            
        }

        private Montura CrearMontura()
        {
            Montura montura = new Montura();
            try
            {
                montura.Sucursal = new Sucursal();
                montura.Sucursal.IdSucursal = ObtenerSucursal();
                montura.CodMontura = txt_codMontura.Text;
                montura.Color = txt_color.Text;
                montura.TipoMontura = (TipoMontura)cmb_tipoMontura.SelectedItem;
                montura.Proveedor = (Proveedor)cmb_proveedor.SelectedItem;
                montura.Stock = Convert.ToInt32(txt_stock.Text);
                montura.Imagen = rutaImagen;
                    
                /*
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    // Se guarda la imagen en el buffer
                    img_montura(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    // Se extraen los bytes del buffer para asignarlos como valor para el 
                    // parámetro.
                    cmd.Parameters["@image"].Value = ms.GetBuffer();
                
                    // crear un bitmap para la imagen
                    // 
                    BitmapImage bitImg = new BitmapImage();
                    // asignar los bytes obtenidos de la BBDD al bitmap                
                    // se cogera el primer registro el campo 2 que contiene los bytes de la imagen
                    bitImg.BeginInit();
                    //System.IO.Stream ms = new System.IO.MemoryStream((byte[])table.Rows[0][1]);

                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    bitImg.StreamSource = ms;
                    bitImg.EndInit();

                    // asociar el bitmap a la imagen
                    img_montura.Source = bitImg;
                montura.Imagen = Convert.ToByte(img_montura.Source);
                */

                }
            catch
            {
                //MessageBox.Show("ERROR AL CREAR RECETA: " + ex.Message);
            }
            return montura;
            
        }

        private bool ExisteMontura(string codMontura)
        {
            bool existe = false;

            foreach (Montura mon in ListarMonturas())
            {
                if (mon.CodMontura.Equals(codMontura))
                {
                    existe = true;
                }
            }

            return existe;
        }
        

        public List<Montura> ListarMonturas()
        {
            OracleCommand cmd = new OracleCommand("FN_LISTAR_MONTURA", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            List<Montura> lista = new List<Montura>();

            OracleParameter output = cmd.Parameters.Add("l_cursor", OracleDbType.RefCursor);
            output.Direction = ParameterDirection.ReturnValue;
            cmd.ExecuteNonQuery();
            OracleDataReader reader = ((OracleRefCursor)output.Value).GetDataReader();

            while (reader.Read())
            {
                Montura montura = new Montura();
                montura.IdMontura = reader.GetInt32(0);
                montura.Sucursal = new Sucursal();
                montura.Sucursal.Nombre = reader.GetString(1);
                montura.CodMontura = reader.GetString(2);
                montura.TipoMontura = new TipoMontura();
                montura.TipoMontura.Nombre = reader.GetString(3);
                montura.Color = reader.GetString(4);
                montura.Stock = reader.GetInt32(5);
                //montura.Imagen = reader.GetByte(7);


                lista.Add(montura);
            }
            //dtg_clientes.ItemsSource = lista;
            return lista;
        }

        private void btn_guardar_Click(object sender, RoutedEventArgs e)
        {
            GuardarMontura();
        }

        private void txt_stock_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidarSoloNumeros(sender, e);
        }

        private void ValidarSoloNumeros(object sender, TextCompositionEventArgs e)
        {
            int ascci = Convert.ToInt32(Convert.ToChar(e.Text));
            if (ascci >= 48 && ascci <= 57)
                e.Handled = false;
            else
                e.Handled = true;
        }

        
    }
}
