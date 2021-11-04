using MahApps.Metro.Controls;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using Modelo.aplicacion.modelo;
using Negocio.aplicacion.negocio;
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
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Presentacion.vistas.ModuloInventario
{
    /// <summary>
    /// Lógica de interacción para Inventario.xaml
    /// </summary>
    public partial class Inventario : UserControl
    {
        OracleConnection conn = null;
        MantenedorCristalBS mantenedorCristalBS;
        MantenedorMonturaBS mantenedorMonturaBS;
        string rutaImagen;
        public Inventario(string nombre)
        {
            InitializeComponent();
            AbrirConexion();
            this.nombre = nombre;
            //CRISTAL
            IniciarMaterialCristal();
            IniciarTipoLente();
            IniciarColor();
            IniciarProveedorCristal();
            //MONTURA
            IniciarProveedorMontura();
            IniciarTipoMontura();
            mantenedorCristalBS = new MantenedorCristalBS();
            mantenedorMonturaBS = new MantenedorMonturaBS();
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

        public void IniciarProveedorMontura()
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
            cmb_proveedorMontura.ItemsSource = proveedors;
            cmb_proveedorMontura.Items.Refresh();
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
                tipoMontura.IdTipoMontura = reader.GetInt32(0);
                tipoMontura.Nombre = reader.GetString(1);
                monturas.Add(tipoMontura);

            }
            cmb_tipoMontura.ItemsSource = monturas;
            cmb_tipoMontura.Items.Refresh();
        }
        public void IniciarColor()
        {
            OracleCommand cmd = new OracleCommand("FN_LISTAR_COLOR_FOTO", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            List<Colores> colors = new List<Colores>();

            OracleParameter output = cmd.Parameters.Add("L_CURSOR", OracleDbType.RefCursor);
            output.Direction = ParameterDirection.ReturnValue;
            cmd.ExecuteNonQuery();
            OracleDataReader reader = ((OracleRefCursor)output.Value).GetDataReader();

            while (reader.Read())
            {
                Colores color = new Colores();
                color.IdColor = reader.GetInt32(0);
                color.Nombre = reader.GetString(1);
                colors.Add(color);
            }
            cmb_colorFoto.ItemsSource = colors;
            cmb_colorFoto.Items.Refresh();
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
            if (file.ShowDialog() == true)
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
                    MessageBox.Show("ERROR AL CARGAR IMAGEN" + ex.Message, "ERROR");
                }
            }
        }
        /*
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
        */
        private void GuardarMontura()
        {

            Montura montura = CrearMontura();
            if (!ExisteMontura(montura.CodMontura))
            {
                try
                {
                    mantenedorMonturaBS.Validacion(montura);
                    OracleCommand cmd = new OracleCommand("SP_GUARDAR_MONTURA", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //mantenedorEmpleado.ValidarEmpleado(cmd);
                    cmd.Parameters.Add("idSucursal", OracleDbType.Int32).Value = ObtenerSucursal();

                    cmd.Parameters.Add("codMontura", OracleDbType.Varchar2).Value = txt_codMontura.Text;
                    cmd.Parameters.Add("tipoMontura", OracleDbType.Int32).Value = montura.TipoMontura.IdTipoMontura;
                    cmd.Parameters.Add("color", OracleDbType.Varchar2).Value = montura.Color;
                    cmd.Parameters.Add("stock", OracleDbType.Int32).Value = montura.Stock;

                    cmd.Parameters.Add("imagen", OracleDbType.Varchar2).Value = montura.Imagen;

                    cmd.Parameters.Add("proveedor", OracleDbType.Int32).Value = montura.Proveedor.IdProveedor;


                    cmd.ExecuteNonQuery();
                    MessageBox.Show("La montura: " + montura.CodMontura + " Fue Agregado al sistema");
                    CargarMonturas();
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
                montura.TipoMontura = (TipoMontura)cmb_tipoMontura.SelectedItem;
                montura.Color = txt_color.Text;
                montura.Stock = Convert.ToInt32(txt_cantidad.Text);
                montura.Proveedor = (Proveedor)cmb_proveedorMontura.SelectedItem;
                montura.Imagen = rutaImagen;
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


        private void dtg_montura_Loaded(object sender, RoutedEventArgs e)
        {
            CargarMonturas();
        }

        private void CargarMonturas()
        {
            try
            {
                ListarMonturas();
            }
            catch (Exception)
            {
                MessageBox.Show("Error al cargar datos");
            }
        }

        private void dtg_Cristal_Loaded(object sender, RoutedEventArgs e)
        {
            CargarCristales();
        }

        private void CargarCristales()
        {
            try
            {
                ListarCristales();
            }
            catch (Exception)
            {
                MessageBox.Show("Error al cargar datos");
            }
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
                montura.Imagen = reader.GetString(6);
                montura.Proveedor = new Proveedor();
                montura.Proveedor.Nombre = reader.GetString(7);
                lista.Add(montura);
            }
            dtg_montura.ItemsSource = lista;
            return lista;
        }

        private void dtg_montura_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object monturaSeleccionada = dtg_montura.SelectedItem;

            Montura montura = (Montura)monturaSeleccionada;
            if (montura != null)
            {
                if (montura.Imagen != "SIN IMAGEN" && montura.Imagen != null && montura.Imagen.Length != 0)
                {
                    Uri file = new Uri(montura.Imagen);
                    img_montura.Source = new BitmapImage(file);
                }
                else
                {
                    img_montura.Source = null;
                }
            }

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

        private void btn_cargarImagen_Click(object sender, RoutedEventArgs e)
        {
            object monturaSeleccionada = dtg_montura.SelectedItem;
            if (monturaSeleccionada != null)
            {
                string var;

                //reintentar:
                try
                {

                    var = Interaction.InputBox("INGRESE STOCK", "AGREGAR STOCK", "", -1, -1);
                    int aux = Convert.ToInt32(var);
                    //MessageBox.Show("" + aux.GetType().ToString());
                    if (!(aux.GetType().ToString() == "int32"))
                    {
                        Montura montura = (Montura)monturaSeleccionada;
                        OracleCommand cmd = new OracleCommand("SP_AGREGAR_STOCK_MONTURA", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("IDP", OracleDbType.Varchar2).Value = montura.CodMontura;
                        cmd.Parameters.Add("CANT_STOCK", OracleDbType.Int32).Value = Int32.Parse(var);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("STOCK AGREGADO");
                    }

                    CargarMonturas();
                }
                catch (Exception)
                {

                }
            }
            else
            {
                MessageBox.Show("DEBE SELECCIONAR UNA MONTURA EN LA TABLA MONTURA");
            }

        }

        private void btn_agregarCristal_Click(object sender, RoutedEventArgs e)
        {
            GuardarCristal();

        }
        public List<Cristal> ListarCristales()
        {
            OracleCommand cmd = new OracleCommand("FN_LISTAR_CRISTAL", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            List<Cristal> lista = new List<Cristal>();

            OracleParameter output = cmd.Parameters.Add("l_cursor", OracleDbType.RefCursor);
            output.Direction = ParameterDirection.ReturnValue;
            cmd.ExecuteNonQuery();
            OracleDataReader reader = ((OracleRefCursor)output.Value).GetDataReader();
            while (reader.Read())
            {
                Cristal cristal = new Cristal();
                cristal.IdCristal = reader.GetInt32(0);
                cristal.CodCristal = reader.GetString(1);
                cristal.Sucursal = new Sucursal();
                cristal.Sucursal.Nombre = reader.GetString(2);
                cristal.Material = new MaterialCristal();
                cristal.Material.Nombre = reader.GetString(3);
                cristal.FiltroAzul = reader.GetString(4);
                cristal.Fotocromatico = reader.GetString(5);
                cristal.Blanco = reader.GetString(6);
                cristal.Capa = reader.GetString(7);
                cristal.TipoLente = new TipoLente();
                cristal.TipoLente.Nombre = reader.GetString(8);
                cristal.Esfera = reader.GetString(9);
                cristal.Cilindro = reader.GetString(10);
                cristal.Stock = reader.GetInt32(11);
                cristal.Proveedor = new Proveedor();
                cristal.Proveedor.Nombre = reader.GetString(12);
                cristal.Precio = reader.GetString(13);
                lista.Add(cristal);

            }
            dtg_Cristal.ItemsSource = lista;
            return lista;
        }

        public void IniciarMaterialCristal()
        {
            OracleCommand cmd = new OracleCommand("FN_LISTAR_MATERIAL_INVENTARIO", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            List<MaterialCristal> materiales = new List<MaterialCristal>();

            OracleParameter output = cmd.Parameters.Add("L_CURSOR", OracleDbType.RefCursor);
            output.Direction = ParameterDirection.ReturnValue;
            cmd.ExecuteNonQuery();
            OracleDataReader reader = ((OracleRefCursor)output.Value).GetDataReader();

            while (reader.Read())
            {
                MaterialCristal material = new MaterialCristal();
                material.IdMaterialCristal = reader.GetInt32(0);
                material.Nombre = reader.GetString(1);
                materiales.Add(material);

            }
            cmb_material.ItemsSource = materiales;
            cmb_material.Items.Refresh();
        }

        public void IniciarTipoLente()
        {
            //OracleCommand cmd = new OracleCommand("SELECT * FROM TIPO_LENTE WHERE NOMBRE != 'Multifocal'", conn);
            OracleCommand cmd = new OracleCommand("SELECT * FROM TIPO_LENTE", conn);
            List<TipoLente> tipoLentes = new List<TipoLente>();
            OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                TipoLente tipo = new TipoLente();
                tipo.IdTipoLente = reader.GetInt32(0);
                tipo.Nombre = reader.GetString(1);
                tipoLentes.Add(tipo);
            }
            cmb_tipo_lente.ItemsSource = tipoLentes;
            cmb_tipo_lente.Items.Refresh();
        }

        public void IniciarProveedorCristal()
        {


            OracleCommand comando = new OracleCommand("SELECT * FROM PROVEEDOR  WHERE TIPO_PROVEEDOR = 'Cristal' ORDER BY ID_PROVEEDOR Asc", conn);
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


        private void btn_agregar_Click(object sender, RoutedEventArgs e)
        {
             GuardarCristal();
        }
        private void GuardarCristal()
        {
            Cristal cristal = CrearCristal();

            if (!ExisteCodCristal(cristal.CodCristal))
            {
                try
                {

                    mantenedorCristalBS.Validacion(cristal);
                    
                    txt_codCristal.Text = cristal.Proveedor.Nombre + cristal.Material.Nombre.Substring(0, 2) + cristal.TipoLente.Nombre.Substring(0, 2) + "E" + txt_esfera.Text + "C" + txt_cilindro.Text;
                    cristal.CodCristal = txt_codCristal.Text;
                    OracleCommand cmd = new OracleCommand("SP_GUARDAR_CRISTAL", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("codCristal", OracleDbType.Varchar2).Value = txt_codCristal.Text;
                    cmd.Parameters.Add("idSucursal", OracleDbType.Int32).Value = ObtenerSucursal();
                    cmd.Parameters.Add("idMaterial", OracleDbType.Int32).Value = cristal.Material.IdMaterialCristal;
                    cmd.Parameters.Add("filtroAzul", OracleDbType.Varchar2).Value = cristal.FiltroAzul;
                    cmd.Parameters.Add("fotocro", OracleDbType.Varchar2).Value = cristal.Fotocromatico;
                    cmd.Parameters.Add("blanco", OracleDbType.Varchar2).Value = cristal.Blanco;
                    cmd.Parameters.Add("capa", OracleDbType.Varchar2).Value = cristal.Capa;
                    cmd.Parameters.Add("idTipoLente", OracleDbType.Int32).Value = cristal.TipoLente.IdTipoLente;
                    cmd.Parameters.Add("esfera", OracleDbType.Varchar2).Value = cristal.Esfera;
                    cmd.Parameters.Add("cilindro", OracleDbType.Varchar2).Value = cristal.Cilindro;
                    cmd.Parameters.Add("stock", OracleDbType.Int32).Value = cristal.Stock;
                    cmd.Parameters.Add("proveedor", OracleDbType.Int32).Value = cristal.Proveedor.IdProveedor;
                    cmd.Parameters.Add("precio", OracleDbType.Varchar2).Value = cristal.Precio;

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("El Cristal: " + txt_codCristal.Text + " Fue Agregado al sistema");
                    /*
                    MessageBoxResult m = MessageBox.Show("Quiere agregar otro Cristal", "Guardar Cristal", MessageBoxButton.OKCancel);
                    if (m == MessageBoxResult.Cancel)
                    {

                        Inventario inventario = new Inventario(nombre);
                        //inventario.CargarCristales();
                        this.Close();
                    }
                    Limpiar();
                    */
                    CargarCristales();
                    limpiar();
                }
                catch (Exception ex)
                {

                    MessageBox.Show("ERROR AL CREAR CRISTAL: " + ex.Message);
                }


            }
            else
            {
                MessageBox.Show("El Codigo ya existe. ");
            }
        }



        private bool ExisteCodCristal(string codCristal)
        {

            bool existe = false;

            foreach (Cristal cri in ListarCristales2())
            {
                if (cri.CodCristal.Equals(codCristal))
                {
                    existe = true;
                }
            }

            return existe;
        }

        public List<Cristal> ListarCristales2()
        {
            OracleCommand cmd = new OracleCommand("FN_LISTAR_CRISTAL", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            List<Cristal> lista = new List<Cristal>();

            OracleParameter output = cmd.Parameters.Add("l_cursor", OracleDbType.RefCursor);
            output.Direction = ParameterDirection.ReturnValue;
            cmd.ExecuteNonQuery();
            OracleDataReader reader = ((OracleRefCursor)output.Value).GetDataReader();

            while (reader.Read())
            {
                Cristal cri = new Cristal();
                cri.CodCristal = reader.GetString(0);
                /*cri.Sucursal = reader.GetString(1);
                cri.ApellidoP = reader.GetString(2);
                cri.Telefono = reader.GetString(3);
                cri.Prevision = reader.GetString(4);
                cri.Direccion = reader.GetString(5);
                cri.Comuna = reader.GetString(6);
                cri.Correo = reader.GetString(7);
                */
                lista.Add(cri);
            }
            //dtg_clientes.ItemsSource = lista;
            return lista;
        }
        private Cristal CrearCristal()
        {
            Cristal cristal = new Cristal();

            try
            {
                cristal.Material = (MaterialCristal)cmb_material.SelectedItem;
                cristal.TipoLente = (TipoLente)cmb_tipo_lente.SelectedItem;
                cristal.FiltroAzul = ValidarChk(chk_azul.IsChecked.Value);
                Colores colores = (Colores)cmb_colorFoto.SelectedItem;
                if (chk_fotocromatico.IsChecked == true && colores == null)
                {
                    throw new Exception("DEBE SELECCIONAR EL COLOR");
                }
                else
                {
                    cristal.Fotocromatico = ValidarChkFoto(chk_fotocromatico.IsChecked.Value);
                }
                if (cristal.TipoLente.Nombre !="Monofocal")
                {
                    if (txt_add.Text.Length == 0)
                    {
                        throw new Exception("DEBE AGREGAR LA ADICION");
                    }
                    if (txt_add.Text.Substring(0, 1) != "+")
                        {
                            throw new Exception(
                                "\nDEBE AGREGAR EL SIMBOLO + EN ADICION");
                        }
                    
                }

                cristal.Blanco = ValidarChk(chk_blanco.IsChecked.Value);
                cristal.Capa = ValidarChk(chk_capa.IsChecked.Value);

                String add="";
                if (txt_add.Text.Length != 0)
                {
                    add = " add " + txt_add.Text;
                    cristal.Esfera = txt_esfera.Text + add;
                }
                else
                {
                    cristal.Esfera = txt_esfera.Text;
                }

                cristal.Cilindro = txt_cilindro.Text;
                cristal.Stock = Convert.ToInt32(txt_stock.Text);
                cristal.Proveedor = (Proveedor)cmb_proveedor.SelectedItem;

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CREAR CRISTAL DSDS: " + ex.Message);
            }
            return cristal;

        }
        private void BuscarMontura()
        {
            bool bandera = true;
            while (bandera == true)
            {
                string cili, esfe, stock;
                if (txt_cilindro.Text.Length == 0)
                {
                    cili = "";
                }
                else
                {
                    cili = " and c.cilindro = " + ValidarBusquedaTxt(txt_cilindro.Text) + "";
                }

                if (txt_esfera.Text.Length == 0)
                {
                    esfe = "";
                }
                else
                {
                    esfe = " and c.esfera = " + ValidarBusquedaTxt(txt_esfera.Text) + "";
                }

                if (txt_stock.Text.Length == 0)
                {
                    stock = "";
                }
                else
                {
                    stock = " and c.stock = " + ValidarBusquedaTxt(txt_stock.Text) + "";
                }


                string selectStr1 = "SELECT M.ID_MONTURA,S.NOMBRE,M.COD_MONTURA,T.NOMBRE,M.COLOR,M.STOCK,COALESCE(M.IMAGEN,'SIN IMAGEN'),P.NOMBRE" +
        " FROM MONTURA M JOIN SUCURSAL S ON" +
        " M.ID_SUCURSAL = S.ID_SUCURSAL JOIN TIPO_MONTURA T ON" +
        " T.ID_TIPO_MONTURA = M.TIPO_MONTURA JOIN PROVEEDOR P ON" +
        " P.ID_PROVEEDOR = M.PROVEEDOR  WHERE " +
        "m.cod_montura like '" + ValidarBusqueda(txt_codMontura.Text) + "%'" +
        " and t.nombre like '" + ValidarBusqueda(cmb_tipoMontura.SelectedItem) + "%'" +
        " and p.nombre like '" + ValidarBusqueda(cmb_proveedorMontura.SelectedItem) + "%'" +
        " and m.color like '" + ValidarBusqueda(txt_color.Text) + "%'" +
        " and m.stock like '" + ValidarBusqueda(txt_stock.Text) + "%'" +
        /*
        " and c.filtro_azul like '" + ValidarBusquedaChk(chk_azul.IsChecked.Value) + "%'" +
        " and c.fotocromatico like '" + ValidarBusquedaChk(chk_fotocromatico.IsChecked.Value) + "%'" +
        " and c.polarizado like '" + ValidarBusquedaChk(chk_polarizado.IsChecked.Value) + "%'" +
        " and c.espejado like '" + ValidarBusquedaChk(chk_espejado.IsChecked.Value) + "%'" +
        cili +
        esfe +
        stock +*/
        " ORDER BY M.ID_MONTURA DESC";
                OracleCommand comando = new OracleCommand(selectStr1, conn);
                OracleDataReader reader = comando.ExecuteReader();
                List<Montura> lista = new List<Montura>();
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
                    montura.Imagen = reader.GetString(6);
                    montura.Proveedor = new Proveedor();
                    montura.Proveedor.Nombre = reader.GetString(7);
                    lista.Add(montura);
                }
                dtg_montura.ItemsSource = lista;

                if (dtg_montura.Items.Count == 0)
                {
                    MessageBox.Show("No se encontraron valores");
                }
                bandera = false;
            }
        }

        private void BuscarCristal()
        {
            bool bandera = true;
            while (bandera == true)
            {
                string cili, esfe, stock, foto;

                if (txt_cilindro.Text.Length == 0)
                {
                    cili = "";
                }
                else
                {
                    cili = " and c.cilindro like " + ValidarBusquedaTxt(txt_cilindro.Text) + "";
                    MessageBox.Show(cili);
                }

                if (txt_esfera.Text.Length == 0)
                {
                    esfe = "";
                }
                else
                {
                    esfe = " and c.esfera like '" + ValidarBusquedaTxt(txt_esfera.Text) + "'";
                }

                if (txt_stock.Text.Length == 0)
                {
                    stock = "";
                }
                else
                {
                    stock = " and c.stock = " + ValidarBusquedaTxt(txt_stock.Text) + "";
                }

                if (chk_fotocromatico.IsChecked.Value == false)
                {
                    foto = "";
                }
                else
                {
                    if (cmb_colorFoto.SelectedItem == null)
                    {
                        foto = " and c.fotocromatico != 'No'";
                    }
                    else
                    {
                        foto = " and c.fotocromatico like '" + ValidarBusqueda(cmb_colorFoto.SelectedItem) + "%'";
                    }
                }
                string selectStr1 = "SELECT C.ID_CRISTAL,C.COD_CRISTAL,S.NOMBRE,MC.NOMBRE,C.FILTRO_AZUL,C.FOTOCROMATICO,C.BLANCO,C.CAPA,T.NOMBRE,C.ESFERA,C.CILINDRO,C.STOCK,P.NOMBRE,C.PRECIO" +
        " FROM CRISTAL C JOIN SUCURSAL S ON" +
        " C.ID_SUCURSAL = S.ID_SUCURSAL JOIN MATERIAL_CRISTAL MC ON" +
        " MC.ID_MATERIAL_CRISTAL = C.ID_MATERIAL JOIN TIPO_LENTE T ON" +
        " C.ID_TIPO_LENTE = T.ID_TIPO_LENTE JOIN PROVEEDOR P ON" +
        " P.ID_PROVEEDOR = C.PROVEEDOR  WHERE " +
        "mc.nombre like '" + ValidarBusqueda(cmb_material.SelectedItem) + "%'" +
        " and t.nombre like '" + ValidarBusqueda(cmb_tipo_lente.SelectedItem) + "%'" +
        " and p.nombre like '" + ValidarBusqueda(cmb_proveedor.SelectedItem) + "%'" +
        " and c.filtro_azul like '" + ValidarBusquedaChk(chk_azul.IsChecked.Value) + "%'" +
        foto +
        //" and c.fotocromatico like '" + ValidarBusqueda(cmb_colorFoto.SelectedItem) + "%'" +
        " and c.blanco like '" + ValidarBusquedaChk(chk_blanco.IsChecked.Value) + "%'" +
        " and c.capa like '" + ValidarBusquedaChk(chk_capa.IsChecked.Value) + "%'" +
        cili +
        esfe +
        stock +
        " ORDER BY C.ID_CRISTAL DESC";
                OracleCommand comando = new OracleCommand(selectStr1, conn);
                OracleDataReader reader = comando.ExecuteReader();
                List<Cristal> lista = new List<Cristal>();
                while (reader.Read())
                {
                    Cristal cristal = new Cristal();
                    cristal.IdCristal = reader.GetInt32(0);
                    cristal.CodCristal = reader.GetString(1);
                    cristal.Sucursal = new Sucursal();
                    cristal.Sucursal.Nombre = reader.GetString(2);
                    cristal.Material = new MaterialCristal();
                    cristal.Material.Nombre = reader.GetString(3);
                    cristal.FiltroAzul = reader.GetString(4);
                    cristal.Fotocromatico = reader.GetString(5);
                    cristal.Blanco = reader.GetString(6);
                    cristal.Capa = reader.GetString(7);
                    cristal.TipoLente = new TipoLente();
                    cristal.TipoLente.Nombre = reader.GetString(8);
                    cristal.Esfera = reader.GetString(9);
                    cristal.Cilindro = reader.GetString(10);
                    cristal.Stock = reader.GetInt32(11);
                    cristal.Proveedor = new Proveedor();
                    cristal.Proveedor.Nombre = reader.GetString(12);
                    cristal.Precio = reader.GetString(13);

                    lista.Add(cristal);
                }
                dtg_Cristal.ItemsSource = lista;
                if (dtg_Cristal.Items.Count == 0)
                {
                    MessageBox.Show("No se encontraron valores");
                }
                bandera = false;
            }
        }
        private string ValidarBusqueda(object value)
        {
            string cadena;
            if (value == null)
            {

                cadena = "";
            }
            else
            {
                cadena = value.ToString();
            }
            return cadena;
        }

        private string ValidarBusquedaChk(bool value)
        {
            string cadena;
            if (value == false)
            {
                cadena = "";
            }
            else
            {
                cadena = "Si";

            }
            return cadena;
        }

        private string ValidarChk(bool value)
        {
            string cadena;
            if (value == false)
            {
                cadena = "No";
            }
            else
            {

                cadena = "Si";
            }
            return cadena;
        }
        private string ValidarChkFoto(bool value)
        {
            string cadena;
            Colores colores = (Colores)cmb_colorFoto.SelectedItem;
            
            if (value == false)
            {
                cadena = "No";
            }
            else
            {

                cadena = colores.Nombre;

            }
            
            return cadena;
        }

        private string ValidarBusquedaTxt(string value)
        {
            string cadena;
            if (value.Length == 0)
            {
                cadena = "";
            }
            else
            {
                cadena = value;
            }
            return cadena;
        }

        private void btn_limpiar_Click(object sender, RoutedEventArgs e)
        {
            limpiar();
            CargarCristales();
        }

        private void limpiar()
        {
            /* cmb_material.SelectedItem = null;
             cmb_tipo_lente.SelectedItem = null;
             cmb_colorFoto.SelectedItem = null;
             cmb_proveedor.SelectedItem = null;
             txt_cilindro.Text = "";
             txt_codCristal.Text = "";
             txt_esfera.Text = "";
             txt_stock.Text = "";
             chk_azul.IsChecked = false;
             chk_blanco.IsChecked = false;
             chk_fotocromatico.IsChecked = false;
             chk_capa.IsChecked = false;
             cmb_colorFoto.Visibility = Visibility.Hidden;
             lbl_color.Visibility = Visibility.Hidden;
             lbl_add.Visibility = Visibility.Hidden;
             txt_add.Visibility = Visibility.Hidden;
             txt_add.Text = "";
            */
            txt_cilindro.Text = "";
            txt_codCristal.Text = "";
            txt_esfera.Text = "";
        }
        private void txt_esfera_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

            ValidarSoloDecimales(sender, e);
        }
        private void txt_cilindro_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidarSoloDecimales(sender, e);
        }
        private void ValidarSoloDecimales(object sender, TextCompositionEventArgs e)
        {
            int ascci = Convert.ToInt32(Convert.ToChar(e.Text));
            if ((ascci >= 48 && ascci <= 57) || (ascci >= 43 && ascci <= 45))
                e.Handled = false;
            else
                e.Handled = true;
        }
        private void txt_stock_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            int ascci = Convert.ToInt32(Convert.ToChar(e.Text));
            if (ascci >= 48 && ascci <= 57)
                e.Handled = false;
            else
                e.Handled = true;
        }
        private void txt_esfera_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            validarTextReceta(txt_esfera.Text, sender, e);
        }
        private void txt_cilindro_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            validarTextReceta(txt_cilindro.Text, sender, e);
        }
        public void validarTextReceta(string valor, object sender, KeyboardFocusChangedEventArgs e)
        {
            if (valor.Trim().Length == 0)
            {

                valor = null;
            }
            else if (!ValidDecimal(valor) || !CountIntsOrPoint(valor))
            {
                MessageBox.Show("Solo se permite el ingreso hasta 2 enteros, una coma y dos decimales.");
                e.Handled = true;
            }
        }
        private bool ValidDecimal(string valor)
        {
            int count = 0, len = valor.Length;

            //Si el punto está en el inicio o final del texto ...
            if (valor[0] == ',' || valor[len - 1] == ',')
                return false;
            for (int i = 0; i != len; ++i)
            {
                //Si es un punto, contamos ...
                if (valor[i] == ',')
                    ++count;
                if (valor[i] == '.')
                    return false;


                //Si no es un caracter númerico ...
                else if ((valor[i] >= '0' && valor[i] <= '9'))
                    return true;

                if (count > 1) //Si hay más de un punto decimal.
                    return false;
            }
            return true;
        }
        private bool CountIntsOrPoint(string valor)
        {
            int len = valor.Length;
            int count = 0;

            //int count2 = 0;
            bool point = false;
            //bool point2 = false;
            for (int i = 0; i != len; ++i)
            {
                ++count;
                if (valor[i] == ',')
                {
                    count = 0;
                    point = true;
                }


                //Si hay más de 2 enteros o más de dos decimales.
                else if ((point && count > 2) || (!point && count > 3))
                    return false;

            }
            return true;
        }

        private void btn_buscarMontura_Click(object sender, RoutedEventArgs e)
        {
            BuscarMontura();
        }

        private void btn_buscarCristal_Click(object sender, RoutedEventArgs e)
        {
            BuscarCristal();
        }

        private void cmb_material_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MaterialCristal cristal = (MaterialCristal)cmb_material.SelectedItem;
            if (cristal != null)
            {
                if (cristal.Nombre == "Orgánico 1.56" || cristal.Nombre == "Orgánico 1.61" || cristal.Nombre == "Mineral" || cristal.Nombre == "Orgánico 1.67" || cristal.Nombre == "Orgánico 1.49")
                {
                    chk_capa.IsChecked = true;
                    chk_capa.IsEnabled = false;
                }
                else
                {
                    chk_capa.IsChecked = false;
                    chk_capa.IsEnabled = true;
                }

                if (cristal.Nombre == "Mineral")
                {
                    chk_azul.IsEnabled = false;
                    chk_azul.IsChecked = false;
                }
                else
                {
                    chk_azul.IsEnabled = true;
                    chk_azul.IsChecked = false;
                }

                if (cristal.Nombre == "Mineral High Light")
                {
                    chk_azul.IsEnabled = false;
                    chk_azul.IsChecked = false;
                }
                else
                {
                    chk_azul.IsEnabled = true;
                    chk_azul.IsChecked = false;
                }

                if (cristal.Nombre == "Policarbonato")
                {
                    chk_blanco.IsEnabled = false;
                    chk_blanco.IsChecked = false;
                }
                else
                {
                    chk_blanco.IsEnabled = true;
                    chk_blanco.IsChecked = false;
                }
            }
            else
            {
                if (cristal == null)
                {
                    chk_capa.IsChecked = false;
                    chk_capa.IsEnabled = true;
                }

            }
            validarCombinacionCMB();
        }

        private void cmb_tipo_lente_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            validarCombinacionCMB();
        }

        private void validarCombinacionCMB()
        {
            TipoLente cristal = (TipoLente)cmb_tipo_lente.SelectedItem;
            MaterialCristal material = (MaterialCristal)cmb_material.SelectedItem;
            Colores colores = (Colores)cmb_colorFoto.SelectedItem;
            try
            {
                if (cristal != null && material != null)
                {
                    if (cristal.Nombre == "Bifocal" && material.Nombre == "Policarbonato")
                    {
                        throw new Exception("COMBINACION NO VALIDA: (BIFOCAL - POLICARBONATO)");
                    }
                    if (cristal.Nombre == "Bifocal Invisible" && material.Nombre == "Policarbonato")
                    {
                        throw new Exception("COMBINACION NO VALIDA: (BIFOCAL INVISIBLE - POLICARBONATO)");
                    }
                    if (cristal.Nombre == "Bifocal Invisible" && material.Nombre == "Mineral")
                    {
                        throw new Exception("COMBINACION NO VALIDA: (BIFOCAL INVISIBLE - MINERAL)");
                    }

                    if (cristal.Nombre == "Bifocal Invisible" && material.Nombre == "Mineral High Light")
                    {
                        throw new Exception("COMBINACION NO VALIDA: (BIFOCAL INVISIBLE - MINERAL)");
                    }
                }
                if (cristal != null)
                {
                    if (cristal.Nombre == "Monofocal")
                    {
                        lbl_add.Visibility = Visibility.Hidden;
                        txt_add.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        lbl_add.Visibility = Visibility.Visible;
                        txt_add.Visibility = Visibility.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR DE COMBINACION: \n" + ex.Message);
                cmb_tipo_lente.SelectedItem = null;
                cmb_material.SelectedItem = null;
                lbl_add.Visibility = Visibility.Hidden;
                txt_add.Visibility = Visibility.Hidden;
            }
        }
        private void chk_fotocromatico_Click(object sender, RoutedEventArgs e)
        {
            if (chk_fotocromatico.IsChecked == false)
            {
                cmb_colorFoto.Visibility = Visibility.Hidden;
                lbl_color.Visibility = Visibility.Hidden;
            }
            else
            {
                cmb_colorFoto.Visibility = Visibility.Visible;
                lbl_color.Visibility = Visibility.Visible;
            }
        }

        private void dtg_Cristal_LoadingRow(object sender, DataGridRowEventArgs e)
        {

        }

        private void chk_blanco_Click(object sender, RoutedEventArgs e)
        {
            if (chk_blanco.IsChecked == false)
            {
                chk_capa.IsChecked = true;
            }
            else
            {
                chk_capa.IsChecked = false;
            }
        }

        private void chk_capa_Click(object sender, RoutedEventArgs e)
        {
            if (chk_capa.IsChecked == false)
            {
                chk_blanco.IsChecked = true;
            }
            else
            {
                chk_blanco.IsChecked = false;
            }
        }
    }
}

