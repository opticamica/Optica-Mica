using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.VisualBasic;
using Modelo.aplicacion.modelo;
using Negocio.aplicacion.negocio;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using Presentacion.aplicacion.moduloInventario;
using Presentacion.aplicacion.moduloPuntoVenta;
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

namespace Presentacion.aplicacion
{
    /// <summary>
    /// Lógica de interacción para ModuloPuntoVenta.xaml
    /// </summary>
    public partial class ModuloPuntoVenta : MetroWindow
    {
        OracleConnection conn = null;
        MantenedorPuntoVentaBS puntoBS;
        MantenedorCristalBS mantenedorCristalBS;
        public ModuloPuntoVenta(string nombre)
        {
            InitializeComponent();
            AbrirConexion();
            this.nombre = nombre;
            txt_nombreUsuario.Text = nombre;
            IniciarMedioPago();
            IniciarTipoLente();
            IniciarDespacho();

            dtp_fechaPago.SelectedDate = DateTime.Now;
            puntoBS = new MantenedorPuntoVentaBS();
            mantenedorCristalBS = new MantenedorCristalBS();
            rdb_rut.IsChecked = true;
            ObtenerPrecioMontura();
            IniciarMaterialCristal();
            IniciarColor();
            


        }
        string nombre,rut,clienteRut,totalOd, totalOi;
        int idCristalOD, idCristalOI, idCristal2OD, idCristal2OI, idMontura, stockMontura, stockCristalOD, stockCristalOI, stockCristal2OD, stockCristal2OI;

        public void ActivarRadioButtonRut()
        {
            rdb_rut.IsChecked = true;
            txt_buscarRutPaciente.IsEnabled = true;
            rdb_nombrePaciente.IsChecked = false;
            txt_buscarNombrePaciente.IsEnabled = false;
        }

        public void ActivarRadioButtonNombre()
        {
            rdb_rut.IsChecked = false;
            txt_buscarRutPaciente.IsEnabled = false;
            rdb_nombrePaciente.IsChecked = true;
            txt_buscarNombrePaciente.IsEnabled = true;
        }


        private void CargarvaloresVenta()
        {
            if (txt_abono.Text == "")
            {
                txt_abono.Text = "0";
            }
            txt_saldo.Text = "" + (Convert.ToInt32(txt_monto.Text) - Convert.ToInt32(txt_abono.Text));
            if (txt_adicion.Text != "0")
            {
                CalcularTotal2doLente();
            }
            
        }

        private void CalcularTotal2doLente()
        {
            /*if (txt_precioConsulta.Text.Length > 0 && txt_precioCristalOD.Text.Length > 0 && txt_precioCristalOI.Text.Length > 0)
            {
                txt_monto2.Text = "" + (Convert.ToInt32(txt_precioMontura.Text) + Convert.ToInt32(totalOd) + Convert.ToInt32(totalOi));
            }*/
            TipoLente tipo = (TipoLente)cmb_tipoCristal.SelectedItem;
            if (tipo != null && txt_adicion.Text !="0" && tipo.Nombre =="Monofocal")
            {
                txt_monto2.Text = "" + (Convert.ToInt32(txt_precioMontura.Text) + Convert.ToInt32(totalOd) + Convert.ToInt32(totalOi));
                lbl_monto2.Visibility = Visibility.Visible;
                txt_total.Visibility = Visibility.Visible;
                lbl_total2.Visibility = Visibility.Visible;
                txt_total.Text = "" + (Convert.ToInt32(txt_monto.Text) + Convert.ToInt32(txt_monto2.Text));
                if (txt_abono.Text =="")
                {
                    txt_abono.Text = "0";   
                }
                txt_saldo.Text = "" + (Convert.ToInt32(txt_total.Text) - Convert.ToInt32(txt_abono.Text));
            }
            else
            {
                txt_monto2.Text = "";
                txt_monto2.Visibility = Visibility.Hidden;
                txt_total.Visibility = Visibility.Hidden;
                lbl_total2.Visibility = Visibility.Hidden;
                lbl_monto2.Visibility = Visibility.Hidden;
                txt_total.Text = "";
                txt_saldo.Text = "";

            }
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

        public void IniciarDespacho()
        {
            OracleCommand cmd = new OracleCommand("FN_LISTAR_DESPACHO", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            List<Despacho> despachos = new List<Despacho>();

            OracleParameter output = cmd.Parameters.Add("L_CURSOR", OracleDbType.RefCursor);
            output.Direction = ParameterDirection.ReturnValue;
            cmd.ExecuteNonQuery();
            OracleDataReader reader = ((OracleRefCursor)output.Value).GetDataReader();

            while (reader.Read())
            {
                Despacho despacho = new Despacho();
                despacho.IdDespacho = reader.GetInt32(0);
                despacho.Nombre = reader.GetString(1);
                despachos.Add(despacho);

            }
            cmb_despacho.ItemsSource = despachos;
            cmb_despacho.Items.Refresh();
        }


        public void IniciarMaterialCristal()
        {
            OracleCommand cmd = new OracleCommand("FN_LISTAR_MATERIAL_CRISTAL", conn);
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

        public void IniciarTipoLente()
        {
            OracleCommand cmd = new OracleCommand("FN_LISTAR_TIPO_LENTE", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            List<TipoLente> tipos = new List<TipoLente>();
            OracleParameter output = cmd.Parameters.Add("L_CURSOR", OracleDbType.RefCursor);
            output.Direction = ParameterDirection.ReturnValue;
            cmd.ExecuteNonQuery();
            OracleDataReader reader = ((OracleRefCursor)output.Value).GetDataReader();
            while (reader.Read())
            {
                TipoLente tipo = new TipoLente();
                tipo.IdTipoLente = reader.GetInt32(0);
                tipo.Nombre = reader.GetString(1);
                tipos.Add(tipo);
            }
            cmb_tipoCristal.ItemsSource = tipos;
            cmb_tipoCristal.Items.Refresh();
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
                this.ShowMessageAsync("", "error de conexion");
                throw new Exception("error");
            }
        }

        private void btn_agregar_cliente_Click(object sender, RoutedEventArgs e)
        {
            AgregarCliente agregar = new AgregarCliente(nombre);
            agregar.Owner = this;
            agregar.ShowDialog();
        }

        private void btn_agregar_receta_Click(object sender, RoutedEventArgs e)
        {
            object clienteSeleccionada = dtg_clientes.SelectedItem;
            if (clienteSeleccionada != null)
            {
                Cliente cliente = (Cliente)clienteSeleccionada;

                AgregarReceta agregar = new AgregarReceta(nombre, cliente);
                agregar.Owner = this;
                agregar.ShowDialog();
            }
            else
            {
                this.ShowMessageAsync("", "Para poder realizar la receta debe Seleccionar un cliente en la tabla");
            }
        }
        private void dtg_clientes_Loaded(object sender, RoutedEventArgs e)
        {
            CargarClientes();
        }
        public void CargarClientes()
        {
            try
            {
                ListarClientes();
            }
            catch (Exception)
            {
                this.ShowMessageAsync("", "Error al cargar datos");
            }
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
            }
            dtg_clientes.ItemsSource = lista;
            return lista;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool rutActivo = (rdb_rut.IsChecked != null) ? (bool)rdb_rut.IsChecked : false;
            bool nombreActivo = (rdb_nombrePaciente.IsChecked != null) ? (bool)rdb_nombrePaciente.IsChecked : false;

            if (rutActivo)
            {
                BuscarPorRut();
            }
            if (nombreActivo)
            {
                BuscarPorNombre();
            }
        }
        
        private void BuscarPorNombre()
        {
            object clienteSeleccionada = dtg_clientes.SelectedItem;
            if (clienteSeleccionada != null)
            {
                Cliente cliente = (Cliente)clienteSeleccionada;
                txt_buscarNombrePaciente.Text = cliente.Nombre;
                txt_nombreCliente.Text = cliente.Nombre + " " + cliente.ApellidoP;
                txt_direccion.Text = cliente.Direccion;
                txt_comuna.Text = cliente.Comuna;
            }
            string consulta = "SELECT rut,nombre,apellidop,telefono,prevision,COALESCE(direccion, 'no tiene'),COALESCE(comuna, 'no tiene'),COALESCE(correo, 'no tiene') FROM CLIENTE  WHERE nombre like '%" + txt_buscarNombrePaciente.Text + "%' ORDER BY RUT Asc";
            OracleCommand comando = new OracleCommand(consulta, conn);
            //comando.Parameters.Add(":nombre", txt_buscarNombrePaciente.Text);
            OracleDataReader reader = comando.ExecuteReader();
            List<Cliente> lista = new List<Cliente>();


            if (txt_buscarNombrePaciente.Text.Length == 0)
            {
                ListarClientes();
                CargarRecetas();
            }
            else
            {
                if (reader.Read())
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
                    dtg_clientes.ItemsSource = lista;
                    ListarRecetas();
                }
                else
                {
                    this.ShowMessageAsync("", "No se encontraron datos");
                }
                txt_buscarRutPaciente.Text = "";
                txt_buscarNombrePaciente.Text = "";
            }
        }

        
        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            CargarRecetas();
        }
        public void CargarRecetas()
        {
            try
            {
                ListarRecetas();

            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("", "Error al cargar datos" + ex.Message);
            }
        }
        public List<Receta> ListarRecetas()
        {
            string consulta;
            if (rdb_rut.IsChecked == true)
            {
                if (txt_buscarRutPaciente.Text.Length == 0)
                {
                    txt_buscarRutPaciente.Text = " ";
                }
                consulta = "SELECT r.id_receta,t.nombre,r.rut_usuario,r.rut_cliente,r.edad,r.observaciones,COALESCE(r.esfera_od,'0'),COALESCE(r.cilindro_od,'0'),COALESCE(r.grados_od,'N/A'),COALESCE(r.esfera_oi,'0'),COALESCE(r.cilindro_oi,'0'),COALESCE(r.grados_oi,'N/A'),COALESCE(r.dp_lejos,0),COALESCE(r.dp_cerca,0),COALESCE(r.adicion,'0'),r.fecha FROM receta r join tipo_receta t on r.id_tipo_receta = t.id_tipo_receta WHERE r.RUT_cliente like '%" + txt_buscarRutPaciente.Text + "%' order by id_receta desc";
                dtg_recetas.SelectedIndex = 0;
            }
            else
            {
                //consulta = "SELECT r.id_receta,t.nombre,r.rut_usuario,r.rut_cliente,r.edad,r.observaciones,COALESCE(r.esfera_od,'0'),COALESCE(r.cilindro_od,'0'),COALESCE(r.grados_od,'N/A'),COALESCE(r.esfera_oi,'0'),COALESCE(r.cilindro_oi,'0'),COALESCE(r.grados_oi,'N/A'),COALESCE(r.dp_lejos,0),COALESCE(r.dp_cerca,0),COALESCE(r.adicion,'0'),r.fecha FROM receta r join tipo_receta t on r.id_tipo_receta = t.id_tipo_receta WHERE c.nombre like '%" + txt_buscarNombrePaciente.Text + "%'order by id_receta desc";
                //CargarClientes();
                txt_buscarNombrePaciente.Text = " ";
                consulta = "SELECT r.id_receta,t.nombre,r.rut_usuario,r.rut_cliente,r.edad,r.observaciones,COALESCE(r.esfera_od,'0'),COALESCE(r.cilindro_od,'0'),COALESCE(r.grados_od,'N/A'),COALESCE(r.esfera_oi,'0'),COALESCE(r.cilindro_oi,'0'),COALESCE(r.grados_oi,'N/A'),COALESCE(r.dp_lejos,0),COALESCE(r.dp_cerca,0),COALESCE(r.adicion,'0'),r.fecha FROM receta r join tipo_receta t on r.id_tipo_receta = t.id_tipo_receta WHERE r.RUT_cliente =(SELECT rut FROM CLIENTE WHERE NOMBRE like '"+txt_buscarNombrePaciente.Text+"%') order by id_receta desc";
                dtg_recetas.SelectedIndex = 0;
            }

            OracleCommand comando = new OracleCommand(consulta, conn);
            //comando.Parameters.Add(":rut", txt_buscarRutPaciente.Text);
            OracleDataReader reader = comando.ExecuteReader();
            List<Receta> lista = new List<Receta>();

            while (reader.Read())
            {
                Receta receta = new Receta();
                receta.IdReceta = reader.GetInt32(0);
                receta.TipoReceta = new TipoReceta();
                receta.TipoReceta.Nombre = reader.GetString(1);
                receta.Usuario = new Usuario();
                receta.Usuario.Rut = reader.GetString(2);
                receta.Cliente = new Cliente();
                receta.Cliente.Rut = reader.GetString(3);
                receta.Edad = reader.GetInt32(4);
                receta.Observacion = reader.GetString(5);
                receta.EsferaOD = reader.GetString(6);
                receta.CilindroOD = reader.GetString(7);
                receta.GradoOD = reader.GetString(8);
                receta.EsferaOI = reader.GetString(9);
                receta.CilindroOI = reader.GetString(10);
                receta.GradoOI = reader.GetString(11);
                receta.DpLejos = reader.GetInt32(12);
                receta.DpCerca = reader.GetInt32(13);
                receta.Adiccion = reader.GetString(14);
                receta.Fecha = reader.GetDateTime(15);
                lista.Add(receta);
            }

            dtg_recetas.ItemsSource = lista;
            return lista;
        }

        private void txt_buscarRutPaciente_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BuscarPorRut();
            }
        }

        private void BuscarPorRut()
        {
            object clienteSeleccionada = dtg_clientes.SelectedItem;
            if (clienteSeleccionada != null)
            {
                Cliente cliente = (Cliente)clienteSeleccionada;
                txt_buscarRutPaciente.Text = cliente.Rut;
                txt_nombreCliente.Text = cliente.Nombre + " " + cliente.ApellidoP;
                txt_direccion.Text = cliente.Direccion;
                txt_comuna.Text = cliente.Comuna;
            }
            OracleCommand comando = new OracleCommand("SELECT rut,nombre,apellidop,telefono,prevision,COALESCE(direccion, 'no tiene'),COALESCE(comuna, 'no tiene'),COALESCE(correo, 'no tiene') FROM CLIENTE  WHERE RUT like :rut  ORDER BY RUT Asc", conn);
            comando.Parameters.Add(":rut", txt_buscarRutPaciente.Text);
            OracleDataReader reader = comando.ExecuteReader();
            List<Cliente> lista = new List<Cliente>();
            if (txt_buscarRutPaciente.Text.Length == 0)
            {
                ListarClientes();
                CargarRecetas();
            }
            else
            {
                if (reader.Read())
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
                    dtg_clientes.ItemsSource = lista;
                    ListarRecetas();
                }
                else
                {
                    this.ShowMessageAsync("", "No se encontraron datos");
                }
                txt_buscarRutPaciente.Text = "";
            }
            //dtg_clientes.SelectedIndex = 0;
        }

        private void buscarRutUsuario()
        {
            OracleCommand comando = new OracleCommand("SELECT rut FROM USUARIO WHERE USUARIO = :usuario ", conn);
            comando.Parameters.Add(":usuario", nombre);
            OracleDataReader reader = comando.ExecuteReader();

            while (reader.Read())
            {
                Usuario usuario = new Usuario();
                rut = reader.GetString(0);
            }
        }

        private void btn_vender_Click(object sender, RoutedEventArgs e)
        {
            AgregarVenta();
        }

        private void Limpiar()
        {
            txt_nombreCliente.Text = "";
            txt_monto.Text = "";
            txt_abono.Text = "";
            txt_saldo.Text = "";
            cmb_medioPago.SelectedItem = null;
            cmb_tipoCristal.SelectedItem = null;
            cmb_despacho.SelectedItem = null;
            dtp_fechaPago.SelectedDate = DateTime.Now;
            cmb_material.SelectedItem = null;
            txt_cantidad.Text = "1";
            txt_montura.Text = "";
            txt_observacion.Text = "";
            txt_abonoCliente.Text = "";
            cmb_despacho.SelectedItem = false;
            txt_comuna.Text = "";
            txt_direccion.Text = "";
            chk_entregado.IsChecked = false;
            //dtg_clientes.SelectedItem = null;
            dtg_recetas.SelectedItem = null;
            //CargarRecetas();
            txt_cilindroOD.Text = "";
            txt_cilindroOI.Text = "";
            txt_esferaOD.Text = "";
            txt_esferaOI.Text = "";
            txt_adicion.Text = "";
            //
            txt_precioConsulta.Text = "0";
            txt_precioCristalOD.Text = "0";
            txt_precioCristalOI.Text = "0";
            txt_precioMontura.Text = "0";
        }

        private void AgregarVenta()
        {
            object ClienteSeleccionado = dtg_clientes.SelectedItem;
            object RecetaSeleccionada = dtg_recetas.SelectedItem;
            MaterialCristal materialCristal = (MaterialCristal)cmb_material.SelectedItem;
            TipoLente tipoLente = (TipoLente)cmb_tipoCristal.SelectedItem;


            if (txt_nombreCliente.Text.Length != 0)
            {
                if (RecetaSeleccionada != null)
                {
                    if (materialCristal != null)
                    {
                        if (tipoLente != null)
                        {
                            
                        Venta venta = CrearVenta();
                        buscarRutUsuario();
                        try
                        {
                                puntoBS.Validacion(venta);
                            Cliente cliente = (Cliente)ClienteSeleccionado;
                            Receta receta = (Receta)RecetaSeleccionada;
                            object medioSeleccionado = cmb_medioPago.SelectedItem;
                            MedioPago medio = (MedioPago)medioSeleccionado;
                            object cristalSeleccionado = cmb_tipoCristal.SelectedItem;
                            TipoLente tipoCristal = (TipoLente)cristalSeleccionado;
                            object despachoselect = cmb_despacho.SelectedItem;
                            Despacho despacho = (Despacho)despachoselect;
                            OracleCommand cmd = new OracleCommand("SP_GUARDAR_VENTA", conn);
                            //this.ShowMessageAsync("", "cadena "+"N RECETA"+ receta.IdReceta+" MONTO  "+ venta.Monto + " ABONO "+ venta.Abono + " SALDO " + venta.Saldo + "\nmedio.IdMedioPago"+ venta.MedioPago.IdMedioPago + " fecha " + dtp_fechaPago.SelectedDate + "Id tipo CristalOD " + idCristalOD + "Id tipo CristalOI " + idCristalOI + "Id tipo CristalOD2 " + idCristal2OD + "Id tipo Cristal2OI " + idCristal2OI + " cantidad " + venta.Cant + " montura " + venta.Montura+" observacion "+ venta.Observacion+" estado "+ venta.EstadoVenta+" entregado "+ venta.Entregado);
                            clienteRut = receta.Usuario.Rut;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("rutUsuario", OracleDbType.Varchar2).Value = rut;
                            cmd.Parameters.Add("nombreUsuario", OracleDbType.Varchar2).Value = nombre;
                            cmd.Parameters.Add("rutCliente", OracleDbType.Varchar2).Value = receta.Usuario.Rut;
                            cmd.Parameters.Add("nombreCliente", OracleDbType.Varchar2).Value = txt_nombreCliente.Text;
                            cmd.Parameters.Add("idReceta", OracleDbType.Int32).Value = receta.IdReceta;
                            cmd.Parameters.Add("medioPago", OracleDbType.Int32).Value = medio.IdMedioPago;
                            cmd.Parameters.Add("fechaPago", OracleDbType.Date).Value = dtp_fechaPago.SelectedDate;
                            cmd.Parameters.Add("idMontura", OracleDbType.Int32).Value = venta.Montura.IdMontura;
                            cmd.Parameters.Add("codMontura", OracleDbType.Varchar2).Value = venta.Montura.CodMontura;
                            cmd.Parameters.Add("idDespacho", OracleDbType.Int32).Value = despacho.IdDespacho;
                            if (idCristalOD == 0)
                            {
                                GuardarCristalPorPedir(txt_esferaOD.Text, txt_cilindroOD.Text);
                               cmd.Parameters.Add("idCristalOD", OracleDbType.Int32).Value = ObtenerIdCristal(txt_esferaOD.Text,txt_cilindroOD.Text);
                            }
                            else
                            {
                                cmd.Parameters.Add("idCristalOD", OracleDbType.Int32).Value = idCristalOD;
                            }

                            if (idCristalOI == 0)
                            {
                                GuardarCristalPorPedir(txt_esferaOI.Text, txt_cilindroOI.Text);
                                cmd.Parameters.Add("idCristalOI", OracleDbType.Int32).Value = ObtenerIdCristal(txt_esferaOI.Text, txt_cilindroOI.Text);
                            }
                            else
                            {
                                cmd.Parameters.Add("idCristalOI", OracleDbType.Int32).Value = idCristalOI;
                            }
                                
                            if (txt_adicion.Text != "0" && tipoCristal.Nombre=="Monofocal")
                            {
                                    decimal esfOd, esfOi;
                                    string esfOD, esfOI;
                                    string add = txt_adicion.Text.Substring(1, txt_adicion.Text.Length - 1);
                                    string esfo = txt_esferaOD.Text.Substring(1, txt_esferaOD.Text.Length - 1);
                                    string esfi = txt_esferaOI.Text.Substring(1, txt_esferaOI.Text.Length - 1);
                                    if (txt_adicion.Text.Substring(0, 1) == "+")
                                    {
                                        esfOd = Convert.ToDecimal(add) + Convert.ToDecimal(esfo);
                                        esfOi = Convert.ToDecimal(add) + Convert.ToDecimal(esfi);
                                    }
                                    else
                                    {
                                        esfOd = Convert.ToDecimal(txt_adicion.Text) + Convert.ToDecimal(txt_esferaOD.Text);
                                        esfOi = Convert.ToDecimal(txt_adicion.Text) + Convert.ToDecimal(txt_esferaOI.Text);
                                    }
                                    if (esfOd > 0)
                                    {
                                        esfOD = "+" + esfOd;
                                    }
                                    else
                                    {
                                        esfOD = "" + esfOd;
                                    }

                                    if (esfOi > 0)
                                    {
                                        esfOI = "+" + esfOi;
                                    }
                                    else
                                    {
                                        esfOI = "" + esfOi;
                                    }
                                    //cmd.Parameters.Add("idCristal2OD", OracleDbType.Int32).Value = idCristal2OD;
                                    //cmd.Parameters.Add("idCristal2OI", OracleDbType.Int32).Value = idCristal2OI;
                                    if (idCristal2OD == 0)
                                    {
                                        GuardarCristalPorPedir(esfOD, txt_cilindroOD.Text);
                                        cmd.Parameters.Add("idCristal2OD", OracleDbType.Int32).Value = ObtenerIdCristal(esfOD, txt_cilindroOD.Text);

                                    }
                                    else
                                    {
                                        cmd.Parameters.Add("idCristal2OD", OracleDbType.Int32).Value = idCristal2OD;
                                    }
                                    if (idCristal2OI == 0)
                                    {
                                        GuardarCristalPorPedir(esfOI, txt_cilindroOI.Text);
                                        cmd.Parameters.Add("idCristal2OI", OracleDbType.Int32).Value = ObtenerIdCristal(esfOI, txt_cilindroOI.Text);
                                    }
                                    else
                                    {
                                        cmd.Parameters.Add("idCristal2OI", OracleDbType.Int32).Value = idCristal2OI;
                                    }
                                }
                            else
                            {
                                cmd.Parameters.Add("idCristal2OD", OracleDbType.Int32).Value = 0;
                                cmd.Parameters.Add("idCristal2OI", OracleDbType.Int32).Value = 0;
                            }
                            cmd.Parameters.Add("monto", OracleDbType.Int32).Value = venta.Monto;
                            cmd.Parameters.Add("abono", OracleDbType.Int32).Value = venta.Abono;
                            cmd.Parameters.Add("saldo", OracleDbType.Int32).Value = venta.Saldo;
                            cmd.Parameters.Add("cantidad", OracleDbType.Int32).Value = venta.Cant;
                            //cmd.Parameters.Add("idTipoVenta", OracleDbType.Int32).Value = venta.TipoVenta.IdTipoVenta;
                            cmd.Parameters.Add("observacion", OracleDbType.Varchar2).Value = txt_observacion.Text;
                            cmd.Parameters.Add("estado", OracleDbType.Varchar2).Value = venta.EstadoVenta;
                            cmd.Parameters.Add("entregado", OracleDbType.Varchar2).Value = venta.Entregado;
                            this.ShowMessageAsync("", "idCristalOD: " + idCristalOD + " idCristalOI " + idCristalOI + " idCristal2OD " + idCristal2OD + " idCristal2OI " + idCristal2OI);
                            cmd.ExecuteNonQuery();

                            if (despacho.Nombre == "SI")
                            {
                                ActualizarDireccion();
                            }
                            if (!(stockMontura <= 0))
                            {
                                ActualizarStockMontura();
                            }
                            else
                            {
                                this.ShowMessageAsync("", "NO QUEDA STOCK DE LA MONTURA");
                            }


                            if (txt_precioCristalOD.Text !="0" && idCristalOD !=0)
                            {
                                if (!(stockCristalOD <= 0))
                                {

                                    ActualizarStockCristal(idCristalOD);
                                }
                                else
                                {
                                    this.ShowMessageAsync("", "NO QUEDA STOCK DEl CRISTAL: "+idCristalOD);
                                }
                            }
                            /*else
                            {
                                if (idCristalOD == 0)
                                {
                                    GuardarCristalPorPedir(txt_esferaOD.Text,txt_cilindroOD.Text);
                                }
                            }*/
                            if (txt_precioCristalOI.Text != "0" && idCristalOI != 0)
                            {
                                if (!(stockCristalOI <= 0))
                                {
                                    ActualizarStockCristal(idCristalOI);
                                }
                                else
                                {
                                    this.ShowMessageAsync("", "NO QUEDA STOCK DEl CRISTAL: " + idCristalOI);
                                }
                            }
                            /*else
                            {
                                if (idCristalOI == 0)
                                {
                                    GuardarCristalPorPedir(txt_esferaOI.Text, txt_cilindroOI.Text);
                                }
                            }*/
                            if (txt_monto2.Text.Length > 1)
                            {
                                if (!(stockCristal2OD <= 0) && idCristal2OD != 0)
                                {
                                    ActualizarStockCristal(idCristal2OD);
                                }
                                else
                                {
                                    this.ShowMessageAsync("", "NO QUEDA STOCK DEl CRISTAL: " + idCristal2OD +" st ");
                                }
                                /*if (idCristal2OD == 0 && txt_adicion.Text != "0")
                                {
                                    string odAdd = "" + (Convert.ToDecimal(txt_adicion.Text) + Convert.ToDecimal(txt_esferaOD.Text));
                                    GuardarCristalPorPedir(odAdd, txt_cilindroOD.Text);
                                }*/
                                if (!(stockCristal2OI <= 0) && idCristal2OI != 0)
                                {
                                    ActualizarStockCristal(idCristal2OI);
                                }
                                else
                                {
                                    this.ShowMessageAsync("", "NO QUEDA STOCK DEl CRISTAL: " + idCristal2OI + " st " + stockCristal2OI);
                                }
                                /*if (idCristal2OI == 0 && txt_adicion.Text != "0")
                                {
                                    string oiAdd = "" + (Convert.ToDecimal(txt_adicion.Text) + Convert.ToDecimal(txt_esferaOI.Text));
                                    GuardarCristalPorPedir(oiAdd,txt_cilindroOD.Text);
                                }*/
                            }

                            this.ShowMessageAsync("", "La venta Fue realizada exitosamente");
                            //Limpiar();
                        }
                        catch (Exception ex)
                        {
                            this.ShowMessageAsync("", "ERROR AL REALIZAR VENTA: " + ex.Message);
                        }
                    }
                    else
                    {
                        this.ShowMessageAsync("", "DEBE SELECCIONAR UNA RECETA DE LA TABLA");

                    }
                }
                else
                {
                    this.ShowMessageAsync("", "DEBE SELECCIONAR UN CLIENTE DE LA TABLA");

                }
                    
                }
                else
                {
                    this.ShowMessageAsync("", "DEBE SELECCIONAR UN TIPO DE CRISTAL");
                }
            }
            else
            {
                this.ShowMessageAsync("", "DEBE SELECCIONAR UN MATERIAL DE CRISTAL");
            }

        }

        private int ObtenerIdCristal(string esfera,string cilindro)
        {
            string consulta = "select max(id_cristal) from cristal where esfera = '"+esfera+"' and cilindro = '"+cilindro+"'";
            OracleCommand cmd = new OracleCommand(consulta, conn);
            //cmd.Parameters.Add("idsucursal", OracleDbType.Int32).Value = ObtenerIdSucursal();
            OracleDataReader reader = cmd.ExecuteReader();
            int idVenta = 0;
            while (reader.Read())
            {
                idVenta = reader.GetInt32(0);
                this.ShowMessageAsync("", ""+idVenta);
            }
             return idVenta;
        }
        


        private void ActualizarDireccion()
        {
            try
            {
                OracleCommand cmd = new OracleCommand("SP_ACTUALIZAR_DIRECCION", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("RUTCLIENTE", OracleDbType.Varchar2).Value = clienteRut;
                cmd.Parameters.Add("DIRECCION", OracleDbType.Varchar2).Value = txt_direccion.Text;
                cmd.Parameters.Add("COMUNA", OracleDbType.Varchar2).Value = txt_comuna.Text;
                cmd.ExecuteNonQuery();
                this.ShowMessageAsync("", "DIRECCION AGREGADA AL CLIENTE");
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("", "ERROR AL AGREGAR DIRECCION: " + ex.Message);
            }
            CargarClientes();
        }

        private void ActualizarStockMontura()
        {
            try
            {
                OracleCommand cmd = new OracleCommand("SP_ACTUALIZAR_STOCK_MONTURA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("IDP", OracleDbType.Varchar2).Value = txt_montura.Text;
                cmd.Parameters.Add("CANT_STOCK", OracleDbType.Int32).Value =Convert.ToInt32(txt_cantidad.Text);
                cmd.ExecuteNonQuery();
                this.ShowMessageAsync("", "MONTURA "+ txt_montura.Text+" ACTUALIZADA");
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("", "ERROR AL ACTUALIZAR MONTURA: " + ex.Message);
            }
        }

        private void ActualizarStockCristal(int idCristal)
        {
            try
            {
                OracleCommand cmd = new OracleCommand("SP_ACTUALIZAR_STOCK_CRISTAL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("IDP", OracleDbType.Int32).Value = idCristal;
                //cmd.Parameters.Add("CANT_STOCK", OracleDbType.Int32).Value = Convert.ToInt32(txt_cantidad.Text);
                cmd.ExecuteNonQuery();
                this.ShowMessageAsync("", "CRISTAL " + idCristal + " ACTUALIZADA");
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("", "ERROR AL ACTUALIZAR MONTURA: " + ex.Message);
            }
        }

        private Venta CrearVenta()
        {
            Venta venta = new Venta();
            try
            {
                venta.NombreUsuario = txt_nombreUsuario.Text;
                venta.NombreCliente = txt_nombreCliente.Text;
                venta.MedioPago = (MedioPago)cmb_medioPago.SelectedItem;
                venta.FechaPago = (DateTime)dtp_fechaPago.SelectedDate;
                venta.Montura = new Montura();
                venta.Montura.IdMontura = idMontura;
                venta.Montura.CodMontura = txt_montura.Text;
                venta.Despacho = (Despacho)cmb_despacho.SelectedItem;
                venta.Direccion = txt_direccion.Text;
                venta.Comuna = txt_comuna.Text;
                venta.Observacion = txt_observacion.Text;
                venta.Abono = Int32.Parse(txt_abono.Text);
                if (txt_adicion.Text != "0")
                {
                    venta.Monto = Int32.Parse(txt_total.Text);
                }
                else
                {
                    venta.Monto = Int32.Parse(txt_monto.Text);
                }
                venta.Saldo = Int32.Parse(txt_saldo.Text);
                /*if (txt_cantidad.Text.Length ==0)
                {
                    venta.Cant = 1;
                }
                else
                {
                    venta.Cant = Int32.Parse(txt_cantidad.Text);
                }*/
                venta.Cant = Int32.Parse(txt_cantidad.Text);
                venta.Cristal = new Cristal();
                //venta.Cristal.IdCristal = idCristal;
                //venta.Cristal2.IdCristal = idCristal2;

                if (venta.Saldo > 0)
                {
                    venta.EstadoVenta = "En Curso";
                }
                else
                {
                    venta.EstadoVenta = "finalizada";
                }
                if (chk_entregado.IsChecked == true)
                {
                    venta.Entregado = "Si";
                }
                else
                {
                    venta.Entregado = "No";
                }
               

            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("", "ERROR AL CREAR VENTA" + ex.Message);
            }
            return venta;
        }

       

        /*private void txt_abono_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            CargarvaloresVenta();
        }*/

        private void ValidarSoloNumeros(object sender, TextCompositionEventArgs e)
        {
            int ascci = Convert.ToInt32(Convert.ToChar(e.Text));
            if (ascci >= 48 && ascci <= 57)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void cmb_despacho_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmb_despacho.SelectedItem !=null)
            {
                try
                {
                    object clienteSeleccionado = dtg_clientes.SelectedItem;
                    Cliente cliente = (Cliente)clienteSeleccionado;
                    object seleccion = cmb_despacho.SelectedItem;
                    Despacho despacho = (Despacho)seleccion;
                    if (despacho.Nombre == "SI")
                    {
                        txt_direccion.IsEnabled = true;
                        txt_comuna.IsEnabled = true;
                        //txt_direccion.Text = cliente.Direccion;
                        //txt_comuna.Text = cliente.Comuna;
                    }
                    if (despacho.Nombre == "NO")
                    {
                        txt_direccion.IsEnabled = false;
                        txt_comuna.IsEnabled = false;
                    }
   
                }
                catch (Exception ex)
                {
                    this.ShowMessageAsync("", "" + ex.Message);
                }
            }
        }

        /*private void cmb_tipoCristal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmb_tipoCristal.SelectedItem !=null)
            {

            object seleccion = cmb_tipoCristal.SelectedItem;
                TipoLente tipo = (TipoLente)seleccion;
            
            if (tipo.IdTipoLente == 4)
            {
                txt_cantidad.IsEnabled = true;
            }
            else
            {
                txt_cantidad.IsEnabled = false;
                txt_cantidad.Text = "1";
            }

            }
        }
        */

        private void txt_abono_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidarSoloNumeros(sender, e);
        }

        private void txt_cantidad_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidarSoloNumeros(sender, e);
        }

        private void txt_saldo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txt_saldo.Text == "0")
            {
                chk_entregado.IsEnabled = true;
            }
            else
            {
                chk_entregado.IsEnabled = false;
            }
        }

        private void txt_monto_TextChanged(object sender, TextChangedEventArgs e)
        {
            CargarvaloresVenta();
        }

        private void txt_abono_TextChanged(object sender, TextChangedEventArgs e)
        {
            CargarvaloresVenta();
        }

        private async void dtg_clientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object clienteSeleccionada = dtg_clientes.SelectedItem;
            
            if (clienteSeleccionada != null || dtg_clientes.SelectedIndex ==0)
            {
                Cliente cliente = (Cliente)clienteSeleccionada;
                txt_buscarRutPaciente.Text = cliente.Rut;
                txt_buscarNombrePaciente.Text=cliente.Nombre;
                txt_nombreCliente.Text = cliente.Nombre + " " + cliente.ApellidoP;
                txt_direccion.Text = cliente.Direccion;
                txt_comuna.Text = cliente.Comuna;
                Venta venta = ObtenerDatosVenta(txt_nombreCliente.Text);
                if (venta.Saldo > 0)
                {
                    MessageDialogResult m =await this.ShowMessageAsync("SEGUNDO ABONO", "DESEA PAGAR LA DEUDA PENDIENTE DE LA VENTA N° " + venta.IdVenta + "?", MessageDialogStyle.AffirmativeAndNegative);
                    //MessageBoxResult m = MessageBox.Show("DESEA PAGAR LA DEUDA PENDIENTE DE LA VENTA N° "+venta.IdVenta + "?", "SEGUNDO ABONO", MessageBoxButton.OKCancel);
                    if (m == MessageDialogResult.Affirmative )
                    {
                        /*cmb_medioPago.SelectedIndex = venta.MedioPago.IdMedioPago;
                        dtp_fechaPago.SelectedDate = venta.FechaPago;
                        txt_montura.Text = venta.Montura.CodMontura;
                        cmb_despacho.SelectedIndex = venta.Despacho.IdDespacho;
                        txt_observacion.Text = venta.Observacion;
                        txt_abono.Text = venta.Abono.ToString();
                        txt_buscarRutPaciente.Text = venta.Cliente.Rut;
                        BuscarPorRut();
                        dtg_recetas.SelectedIndex = 0;
                        txt_saldo.Text = venta.Saldo.ToString();*/
                        
                        reintentar:
                        try
                        {
                            var result  =await this.ShowInputAsync("FINALIZAR ENTREGA", "INGRESE MONTO A PAGAR \nSALDO RESTANTE A PAGAR: " + venta.Saldo);

                            //await this.ShowMessageAsync("", "PAGO COMPLETADO"+result.GetType().ToString());
                            //var = Interaction.InputBox("INGRESE MONTO A PAGAR \nSALDO RESTANTE A PAGAR: "+venta.Saldo, "FINALIZAR ENTREGA", "", -1, -1);
                            //MessageBox.Show("RESULT" +result.Result);
                            int aux = Convert.ToInt32(result);
                            //MessageBox.Show("AUX " + aux + "TIPO AUX: "+ aux.GetType().ToString());
                            //this.ShowMessageAsync("", "" + aux.GetType().ToString());
                            if (result == null)
                            {
                                return;
                            }
                            if (!(aux.GetType().ToString() == "int32"))
                            {
                                if (venta.Saldo != aux)
                                {
                                    await this.ShowMessageAsync("", "INGRESE MONTO COMPLETO");
                                    goto reintentar;
                                }
                                OracleCommand cmd = new OracleCommand("SP_CONFIRMAR_VENTA", conn);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("IDP", OracleDbType.Varchar2).Value = venta.IdVenta;
                                cmd.Parameters.Add("SALDO_RESTANTE", OracleDbType.Int32).Value = Convert.ToInt32(result);
                                cmd.ExecuteNonQuery();
                                await this.ShowMessageAsync("", "PAGO COMPLETADO");
                            }
                        }
                        catch 
                        {
                            goto reintentar;
                            //MessageBox.Show(""+ex.Message);

                        }
                    }
                    if (m == MessageDialogResult.Negative)
                    {
                        
                    }
                    
                }
                else
                {
                    txt_buscarRutPaciente.Text = cliente.Rut;
                    ListarRecetas();

                    dtg_recetas.SelectedIndex = 0;
                    //dtg_clientes.SelectedIndex = 0;
                }
                txt_buscarNombrePaciente.Text = "";
                txt_buscarRutPaciente.Text = "";
            }
        }

        private Venta ObtenerDatosVenta(string nombreCliente)
        {
            OracleCommand comando = new OracleCommand("SELECT medio_pago,fecha_pago,coalesce(cod_montura,' '),id_despacho,coalesce(observacion,' '),abono,id_receta,saldo,rut_cliente,ID_VENTA from venta where id_venta = (select max(id_venta) from venta)and nombre_cliente = :nombre", conn);
            comando.Parameters.Add(":nombre", nombreCliente);
            OracleDataReader reader = comando.ExecuteReader();
            Venta venta= new Venta();

            while (reader.Read())
            {
                venta.MedioPago = new MedioPago();
                venta.MedioPago.IdMedioPago = reader.GetInt32(0);
                //venta.FechaPago = new TipoReceta();
                venta.FechaPago = reader.GetDateTime(1);
                //venta.Usuario = new Usuario();
                venta.Montura = new Montura();
                venta.Montura.CodMontura = reader.GetString(2);
                //venta.Despacho.IdDespacho = new Cliente();
                venta.Despacho = new Despacho();
                venta.Despacho.IdDespacho = reader.GetInt32(3);
                venta.Observacion = reader.GetString(4);
                venta.Abono = reader.GetInt32(5);
                venta.Receta = new Receta();
                venta.Receta.IdReceta = reader.GetInt32(6);
                venta.Saldo = reader.GetInt32(7);
                venta.Cliente = new Cliente();
                venta.Cliente.Rut = reader.GetString(8);
                venta.IdVenta = reader.GetInt32(9);
                /*venta.CilindroOD = reader.GetString(7);
                venta.GradoOD = reader.GetString(8);
                venta.EsferaOI = reader.GetString(9);
                venta.CilindroOI = reader.GetString(10);
                venta.GradoOI = reader.GetString(11);
                venta.DpLejos = reader.GetInt32(12);
                venta.DpCerca = reader.GetInt32(13);
                venta.Adiccion = reader.GetString(14);
                venta.Fecha = reader.GetDateTime(15);
                venta.Cliente.Nombre = reader.GetString(16);
                venta.Cliente.ApellidoP = reader.GetString(17);
                venta.Usuario.TipoUsuario = new TipoUsuario();
                venta.Usuario.TipoUsuario.Nombre = reader.GetString(18);
                */
            }

            return venta;
        }

        private void rdb_nombrePaciente_Checked(object sender, RoutedEventArgs e)
        {
            ActivarRadioButtonNombre();
            txt_buscarNombrePaciente.Text = "";
            txt_buscarRutPaciente.Text = "";
        }

        private void rdb_rut_Checked(object sender, RoutedEventArgs e)
        {
            ActivarRadioButtonRut();
            txt_buscarNombrePaciente.Text = "";
            txt_buscarRutPaciente.Text = "";
        }

        private void txt_montura_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ObtenerPrecioMontura();
            CargarvaloresVenta();

        }

        private void ObtenerPrecioMontura()
        {
            if (txt_montura.Text.Length != 0)
            {

                OracleCommand comando = new OracleCommand("SELECT m.id_montura,t.precio,T.NOMBRE,m.stock FROM MONTURA M JOIN TIPO_MONTURA T ON M.TIPO_MONTURA = T.ID_TIPO_MONTURA WHERE M.COD_MONTURA = :CODIGO ", conn);
                comando.Parameters.Add(":CODIGO", txt_montura.Text);
                OracleDataReader reader = comando.ExecuteReader();
                List<Montura> lista = new List<Montura>();

                if (reader.Read())
                {
                    Montura mon = new Montura();
                    mon.TipoMontura = new TipoMontura();
                    idMontura = reader.GetInt32(0);
                    mon.TipoMontura.Precio = reader.GetInt32(1);
                    mon.TipoMontura.Nombre = reader.GetString(2);
                    stockMontura = reader.GetInt32(3);
                    lista.Add(mon);
                    txt_precioMontura.Text = mon.TipoMontura.Precio.ToString();
                }
                else
                {
                    //var m = this.ShowMessageAsync("ERROR EN CODIGO MONTURA", "MONTURA NO ENCONTRADA DESEA AGREGARLA?", MessageDialogStyle.AffirmativeAndNegative);
                    MessageBoxResult m = MessageBox.Show("MONTURA NO ENCONTRADA DESEA AGREGARLA?", "ERROR EN CODIGO MONTURA", MessageBoxButton.OKCancel);
                    if (m == MessageBoxResult.OK)
                    {
                        AgregarMontura inventario = new AgregarMontura(nombre, txt_montura.Text);
                        //inventario.CargarCristales();
                        inventario.Owner = this;
                        inventario.ShowDialog();
                    }
                    if (m == MessageBoxResult.Cancel)
                    {
                        txt_montura.Text = "";
                    }
                }
                CalcularMonto();
            }
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

        private Cristal BuscarCristal(string esfera, string cilindro)
        {

            bool bandera = true;
            while (bandera == true)
            {
                if (cmb_material.SelectedItem != null && cmb_tipoCristal.SelectedItem != null)
                {

                Receta receta = (Receta)dtg_recetas.SelectedItem;
                string cili, esfe,foto;
                if (cilindro.Length == 0)
                {
                    cili = "";
                }
                else
                {
                    cili = " and c.cilindro like '" + ValidarBusquedaTxt(cilindro) + "'";
                }

                if (esfera.Length == 0)
                {
                    esfe = "";
                }
                else
                {
                    esfe = " and c.esfera like '" + ValidarBusquedaTxt(esfera) + "'";
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
        " and t.nombre like '" + ValidarBusqueda(cmb_tipoCristal.SelectedItem) + "%'" +
        //" and p.nombre like '" + ValidarBusqueda(cmb_proveedor.SelectedItem) + "%'" +
        " and c.filtro_azul like '" + ValidarBusquedaChk(chk_azul.IsChecked.Value) + "%'" +
        foto +
        //" and c.fotocromatico like '" + ValidarBusqueda(cmb_colorFoto.SelectedItem) + "%'" +
        " and c.blanco like '" + ValidarBusquedaChk(chk_blanco.IsChecked.Value) + "%'" +
        " and c.capa like '" + ValidarBusquedaChk(chk_capa.IsChecked.Value) + "%'" +
        cili +
        esfe +
        //stock +
        " and c.stock = (select max(c.stock) from cristal c JOIN MATERIAL_CRISTAL MC ON MC.ID_MATERIAL_CRISTAL = C.ID_MATERIAL JOIN TIPO_LENTE T ON C.ID_TIPO_LENTE = T.ID_TIPO_LENTE where mc.nombre ='" + cmb_material.SelectedItem.ToString() + "' and t.nombre ='"+ cmb_tipoCristal.SelectedItem.ToString() + "' and esfera like '" + ValidarBusquedaTxt(esfera) + "' and cilindro like '" + ValidarBusquedaTxt (cilindro)+"'"+foto+
         " and c.filtro_azul like '" + ValidarBusquedaChk(chk_azul.IsChecked.Value) + "%'" +
         " and c.blanco like '" + ValidarBusquedaChk(chk_blanco.IsChecked.Value) + "%'" +
        " and c.capa like '" + ValidarBusquedaChk(chk_capa.IsChecked.Value) + "%')" +
        "AND c.tipo_venta like 'Stock'"+
                " ORDER BY C.ID_CRISTAL DESC";
                OracleCommand comando = new OracleCommand(selectStr1, conn);
                OracleDataReader reader = comando.ExecuteReader();
                List<Cristal> lista = new List<Cristal>();

                Cristal cristal = new Cristal();
                while (reader.Read())
                {
                    cristal.IdCristal = reader.GetInt32(0);
                    //idCristal = cristal.IdCristal;
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
                    //this.ShowMessageAsync("", "id cristal: " + cristal.IdCristal + " cod cristal: " + cristal.CodCristal + "precio: " + cristal.Precio+" proveedor: "+cristal.Proveedor);
                }
                return cristal;
                    /*
                    if (lista.Count == 0)
                    {
                        //this.ShowMessageAsync("", "No hay");
                        //bandera = false;
                    }
                    else
                    {

                    }*/
                }
            }
            return null;
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
        private void dtg_recetas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object recetaSelect = dtg_recetas.SelectedItem;
            if (recetaSelect != null)
            {
                Receta receta= (Receta)recetaSelect;
                //txt_buscarRutPaciente.Text = cliente.Rut;
                txt_cilindroOD.Text = receta.CilindroOD ;
                txt_esferaOD.Text = receta.EsferaOD;
                txt_cilindroOI.Text = receta.CilindroOI;
                txt_esferaOI.Text = receta.EsferaOI;
                txt_adicion.Text = receta.Adiccion;
                if (receta.TipoReceta.Nombre == "Receta Optica Mica")
                {
                    txt_precioConsulta.Text = "6900"; 
                }
                else
                {
                    txt_precioConsulta.Text = "0";

                }
                if (cmb_material.SelectedItem != null && (cmb_tipoCristal.SelectedItem != null || cmb_colorFoto.SelectedItem != null))
                {
                    CalcularTotales();
                }
            }
        }

        private void cmb_tipoCristal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmb_material.SelectedItem != null)
            {
                CalcularTotales();
            }
               
        }
        private void CalcularTotales()
        {
            TipoLente tipoLente = (TipoLente)cmb_tipoCristal.SelectedItem;
            if (dtg_recetas.SelectedItem == null)
            {
                this.ShowMessageAsync("", "DEBE SELEECIONAR RECETA");
            }
            else
            {
                if (cmb_tipoCristal.SelectedItem != null)
                {
                    Cristal cristalOD = BuscarCristal(txt_esferaOD.Text, txt_cilindroOD.Text);
                    if (cristalOD.IdCristal == 0)
                    {
                        Cristal cristal = CrearCristal(txt_esferaOD.Text, txt_cilindroOD.Text);
                        try
                        {
                            mantenedorCristalBS.ValidacionPrecio(cristal);
                            txt_precioCristalOD.Text = cristal.Precio;
                            idCristalOD = cristal.IdCristal;
                            //txt plazo debe validar los dias de plazo segun su categoria: stock bodega, stock rapido de comprar y laboratorio
                            txt_plazo.Text = "7 a 10 dias";
                            //stockCristalOD = cristal.Stock;
                            this.ShowMessageAsync("", "precio cristal od " + cristal.Precio + " id " + cristal.IdCristal);
                        }
                        catch (Exception ex)
                        {

                            this.ShowMessageAsync("", " " + ex.Message);
                        }
                    }
                    else
                    {
                        txt_precioCristalOD.Text = cristalOD.Precio;
                        idCristalOD = cristalOD.IdCristal;
                        stockCristalOD = cristalOD.Stock;
                    }

                    Cristal cristalOI = BuscarCristal(txt_esferaOI.Text, txt_cilindroOI.Text);
                    if (cristalOI.IdCristal == 0)
                    {
                        Cristal cristal = CrearCristal(txt_esferaOI.Text, txt_cilindroOI.Text);
                        try
                        {
                            mantenedorCristalBS.ValidacionPrecio(cristal);
                            txt_precioCristalOI.Text = cristal.Precio;
                            idCristalOI = cristal.IdCristal;

                            txt_plazo.Text = "7 a 10 dias";

                            this.ShowMessageAsync("", "precio cristal oi " + cristal.Precio + " id " + cristal.IdCristal);
                        }
                        catch (Exception ex)
                        {

                            this.ShowMessageAsync("", " " + ex.Message);
                        }
                    }
                    else
                    {
                        txt_precioCristalOI.Text = cristalOI.Precio;
                        idCristalOI = cristalOI.IdCristal;
                        stockCristalOI = cristalOI.Stock;
                    }

                    if (txt_precioCristalOD.Text != "Consultar Miguel" && txt_precioCristalOI.Text != "Consultar Miguel")
                    {
                        if (txt_precioCristalOD.Text == "")
                        {
                            txt_precioCristalOD.Text = "0";
                        }
                        if (txt_precioCristalOI.Text == "")
                        {
                            txt_precioCristalOI.Text = "0";
                        }

                        txt_monto.Text = "" + (Convert.ToInt32(txt_precioConsulta.Text) + Convert.ToInt32(txt_precioMontura.Text) + Convert.ToInt32(txt_precioCristalOD.Text) + Convert.ToInt32(txt_precioCristalOI.Text));
                    }
                    if (stockCristalOD == 0 || stockCristalOI == 0)
                    {
                        txt_plazo.Text = "7 a 10 dias";
                    }
                    else
                    {
                        txt_plazo.Text = "3 dias";
                    }
                    if (tipoLente.Nombre == "Monofocal" && txt_adicion.Text != "0")
                    {
                        txt_monto2.Visibility = Visibility.Visible;
                        lbl_monto2.Visibility = Visibility.Visible;
                        CalcularTotalMonofocalConAdd();
                    }
                }
            }
            
        }

        private void GuardarCristalPorPedir(string esfera,string cilindro)
        {
            Cristal cristal = CrearCristal(esfera, cilindro);
            try
            {
                mantenedorCristalBS.ValidacionPrecio(cristal);
                cristal.CodCristal = cristal.Material.Nombre.Substring(0, 2) + cristal.TipoLente.Nombre.Substring(0, 2) + "E" + esfera + "C" + cilindro;
                OracleCommand cmd = new OracleCommand("SP_GUARDAR_CRISTAL_PEDIDO", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("codCristal", OracleDbType.Varchar2).Value = cristal.CodCristal;
                cmd.Parameters.Add("idSucursal", OracleDbType.Int32).Value = ObtenerSucursal();
                //cmd.Parameters.Add("idVenta", OracleDbType.Int32).Value = idVenta;
                cmd.Parameters.Add("idMaterial", OracleDbType.Int32).Value = cristal.Material.IdMaterialCristal;
                cmd.Parameters.Add("filtroAzul", OracleDbType.Varchar2).Value = cristal.FiltroAzul;
                cmd.Parameters.Add("fotocro", OracleDbType.Varchar2).Value = cristal.Fotocromatico;
                cmd.Parameters.Add("blanco", OracleDbType.Varchar2).Value = cristal.Blanco;
                cmd.Parameters.Add("capa", OracleDbType.Varchar2).Value = cristal.Capa;
                cmd.Parameters.Add("idTipoLente", OracleDbType.Int32).Value = cristal.TipoLente.IdTipoLente;
                String add;
                TipoLente tipoLente = (TipoLente)cmb_tipoCristal.SelectedItem;
                if (txt_adicion.Text != "0" && tipoLente.Nombre != "Monofocal")
                {
                    add = " ADD " + txt_adicion.Text;
                }
                else
                {
                    add = "";
                }
                cmd.Parameters.Add("esfera", OracleDbType.Varchar2).Value = cristal.Esfera + add;
                cmd.Parameters.Add("cilindro", OracleDbType.Varchar2).Value = cristal.Cilindro;
                cmd.Parameters.Add("stock", OracleDbType.Int32).Value = cristal.Stock;
                cmd.Parameters.Add("proveedor", OracleDbType.Int32).Value = 10;
                cmd.Parameters.Add("precio", OracleDbType.Varchar2).Value = cristal.Precio;
                cmd.Parameters.Add("TIPO_VENTA", OracleDbType.Varchar2).Value = txt_plazo.Text;
                cmd.ExecuteNonQuery();
                this.ShowMessageAsync("", "El Cristal: " + cristal.CodCristal + " No esta en stock , se ha agregado a pendientes");
                
                   
            }
            catch (Exception ex)
            {

                this.ShowMessageAsync("", "ERROR AL CREAR CRISTAL: " + ex.Message);
            }

        }

        private Cristal CrearCristal(string esf,string cil)
        {
            Cristal cristal = new Cristal();

            try
            {
                cristal.Material = (MaterialCristal)cmb_material.SelectedItem;
                cristal.TipoLente = (TipoLente)cmb_tipoCristal.SelectedItem;
                cristal.FiltroAzul = ValidarChk(chk_azul.IsChecked.Value);
                cristal.Fotocromatico = ValidarChkFoto(chk_fotocromatico.IsChecked.Value);
                cristal.Blanco = ValidarChk(chk_blanco.IsChecked.Value);
                cristal.Capa = ValidarChk(chk_capa.IsChecked.Value);

                cristal.Esfera = esf;
                cristal.Cilindro = cil;
                cristal.Stock = 0;
                cristal.Proveedor = null;
                /*this.ShowMessageAsync("", "material " + cristal.Material + " tipolente " + cristal.TipoLente +" azul " +cristal.FiltroAzul + " foto " + cristal.Fotocromatico + " blanco " + cristal.Blanco +" esfera "+ cristal.Esfera +
                "ciliundro " +cristal.Cilindro 
                + " stock "+cristal.Stock 
                + " proveedor "+cristal.Proveedor);*/
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("", "ERROR AL CREAR CRISTAL: " + ex.Message);
            }
            return cristal;

        }

        private void txt_abonoCliente_SelectionChanged(object sender, RoutedEventArgs e)
        {
            CalcularMonto();
        }

        private void dtp_fechaPago_Drop(object sender, DragEventArgs e)
        {
            
        }

        private void dtp_fechaPago_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dtp_fechaPago.SelectedDate < DateTime.Today)
            {
                this.ShowMessageAsync("", "FECHA NO VALIDA");
                dtp_fechaPago.SelectedDate = DateTime.Now;
            }
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
                if (colores != null)
                {
                    cadena = colores.Nombre;

                }
                else
                {
                    throw new Exception("DEBE SELECCIONAR EL COLOR");
                }

            }
            return cadena;
        }

        

        

        private void txt_buscarNombrePaciente_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BuscarPorNombre();
            }
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

        private void CalcularTotalMonofocalConAdd()
        {
            decimal esfOd, esfOi;
            string add = txt_adicion.Text.Substring(1, txt_adicion.Text.Length - 1);
            string esfo = txt_esferaOD.Text.Substring(1, txt_esferaOD.Text.Length - 1);
            string esfi = txt_esferaOI.Text.Substring(1, txt_esferaOI.Text.Length - 1);
            if (txt_adicion.Text.Substring(0, 1) == "+")
            {
                esfOd = Convert.ToDecimal(add) + Convert.ToDecimal(esfo);
                esfOi = Convert.ToDecimal(add) + Convert.ToDecimal(esfi);
            }
            else
            {
                esfOd = Convert.ToInt32(txt_adicion.Text) + Convert.ToInt32(txt_esferaOD.Text);
                esfOi = Convert.ToInt32(txt_adicion.Text) + Convert.ToInt32(txt_esferaOI.Text);
            }
            
            if (Convert.ToDecimal(txt_esferaOD.Text) > 0)
            {
                Cristal cristalOD = BuscarCristal("+" + esfOd, txt_cilindroOD.Text);

                if (cristalOD.IdCristal == 0)
                {
                    Cristal cristal = CrearCristal("+" + esfOd, txt_cilindroOD.Text);
                    try
                    {
                        mantenedorCristalBS.ValidacionPrecio(cristal);
                        totalOd = cristal.Precio;
                        idCristal2OD = cristal.IdCristal;
                        txt_plazo.Text = "7 a 10 dias";
                        //stockCristalOD = cristal.Stock;
                        this.ShowMessageAsync("", "precio cristal od " + cristal.Precio + " id " + cristal.IdCristal);
                    }
                    catch (Exception ex)
                    {

                        this.ShowMessageAsync("", " " + ex.Message);
                    }
                }
                else
                {
                    totalOd = cristalOD.Precio;
                    idCristal2OD = cristalOD.IdCristal;
                    stockCristal2OD = cristalOD.Stock;
                    this.ShowMessageAsync("", "id cristal: " + cristalOD.IdCristal);
                }
                
            }
            else
            {
                Cristal cristalOD = BuscarCristal("" + esfOd, txt_cilindroOD.Text);
                if (cristalOD.IdCristal == 0)
                {
                    Cristal cristal = CrearCristal("" + esfOd, txt_cilindroOD.Text);
                    try
                    {
                        mantenedorCristalBS.ValidacionPrecio(cristal);
                        totalOd = cristal.Precio;
                        idCristal2OD = cristal.IdCristal;
                        txt_plazo.Text = "7 a 10 dias";
                        //stockCristalOD = cristal.Stock;
                        this.ShowMessageAsync("", "precio cristal od " + cristal.Precio + " id " + cristal.IdCristal);
                    }
                    catch (Exception ex)
                    {

                        this.ShowMessageAsync("", " " + ex.Message);
                    }
                }
                else
                {
                    totalOd = cristalOD.Precio;
                    idCristal2OD = cristalOD.IdCristal;
                    stockCristal2OD = cristalOD.Stock;
                    this.ShowMessageAsync("", "id cristal: " + cristalOD.IdCristal);
                }
            }

            if (totalOd == "0")
            {
                txt_precioCristalOD.Text = "0";
            }

            if (Convert.ToDecimal(txt_esferaOI.Text) > 0)
            {
                Cristal cristalOI = BuscarCristal("+" + esfOi, txt_cilindroOI.Text);

                if (cristalOI.IdCristal == 0)
                {
                    Cristal cristal = CrearCristal("+" + esfOi, txt_cilindroOI.Text);
                    try
                    {
                        mantenedorCristalBS.ValidacionPrecio(cristal);
                        totalOi = cristal.Precio;
                        idCristal2OI = cristal.IdCristal;
                        txt_plazo.Text = "7 a 10 dias";
                        //stockCristalOD = cristal.Stock;
                        this.ShowMessageAsync("", "precio cristal oI " + cristal.Precio + " id " + cristal.IdCristal);
                    }
                    catch (Exception ex)
                    {

                        this.ShowMessageAsync("", " " + ex.Message);
                    }
                }
                else
                {
                    totalOi = cristalOI.Precio;
                    idCristal2OI = cristalOI.IdCristal;
                    stockCristal2OI = cristalOI.Stock;
                    this.ShowMessageAsync("", "id cristal: " + cristalOI.IdCristal);
                }

            }
            else
            {
                Cristal cristalOI = BuscarCristal("" + esfOi, txt_cilindroOI.Text);
                if (cristalOI.IdCristal == 0)
                {
                    Cristal cristal = CrearCristal("" + esfOi, txt_cilindroOI.Text);
                    try
                    {
                        mantenedorCristalBS.ValidacionPrecio(cristal);
                        totalOi = cristal.Precio;
                        idCristal2OI = cristal.IdCristal;
                        txt_plazo.Text = "7 a 10 dias";
                        //stockCristalOD = cristal.Stock;
                        this.ShowMessageAsync("", "precio cristal oI " + cristal.Precio + " id " + cristal.IdCristal);
                    }
                    catch (Exception ex)
                    {

                        this.ShowMessageAsync("", " " + ex.Message);
                    }
                }
                else
                {
                    totalOi = cristalOI.Precio;
                    idCristal2OI = cristalOI.IdCristal;
                    stockCristal2OI = cristalOI.Stock;
                    this.ShowMessageAsync("", "id cristal: " + cristalOI.IdCristal);
                }
            }

            if (totalOd == "0")
            {
                txt_precioCristalOD.Text = "0";
            }
            
            if (stockCristal2OD == 0 || stockCristal2OI == 0)
            {
                txt_plazo.Text = "7 a 10 dias";
            }
            else
            {
                txt_plazo.Text = "3 dias";
            }
            CalcularTotal2doLente();
            
        }

        private void txt_montura_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (txt_montura.Text.Length == 0)
            {
                txt_precioMontura.Text = "0";
                
            }
                CalcularMonto();
         
        }

        private void chk_fotocromatico_Click(object sender, RoutedEventArgs e)
        {
            if (chk_fotocromatico.IsChecked == false)
            {
                cmb_colorFoto.Visibility = Visibility.Hidden;
                lbl_colorFoto.Visibility = Visibility.Hidden;
            }
            else
            {
                cmb_colorFoto.Visibility = Visibility.Visible;
                lbl_colorFoto.Visibility = Visibility.Visible;
                cmb_colorFoto.SelectedItem = null;
            }
        }


        private void CalcularMonto()
        {
            txt_monto.Text = "" + (Convert.ToInt32(txt_precioConsulta.Text) + Convert.ToInt32(txt_precioMontura.Text) + Convert.ToInt32(txt_precioCristalOD.Text) + Convert.ToInt32(txt_precioCristalOI.Text));
            
            txt_saldo.Text = "" + (Convert.ToInt32(txt_monto.Text) - Convert.ToInt32(txt_abono.Text));
        }


        private void txt_monto2_TextChanged(object sender, TextChangedEventArgs e)
        {
            CargarvaloresVenta();
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

            if (cmb_tipoCristal.SelectedItem != null)
            {
                CalcularTotales();
            }   
        }

        private void validarCombinacionCMB()
        {
            TipoLente cristal = (TipoLente)cmb_tipoCristal.SelectedItem;
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
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("", "ERROR DE COMBINACION: \n" + ex.Message);
                cmb_tipoCristal.SelectedItem = null;
                cmb_material.SelectedItem = null;
            }
        }

        private void cmb_colorFoto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmb_colorFoto.SelectedItem != null)
            {
                CalcularTotales();
            }
        }

        private void chk_sol_Click(object sender, RoutedEventArgs e)
        {
            if (chk_sol.IsChecked==true)
            {
                lbl_cantidad.Visibility = Visibility.Visible;
                txt_cantidad.Visibility = Visibility.Visible;
            }
            else
            {
                lbl_cantidad.Visibility = Visibility.Hidden;
                txt_cantidad.Visibility = Visibility.Hidden;
                txt_cantidad.Text = "1";
            }
        }

        private void cmb_despacho_DropDownOpened(object sender, EventArgs e)
        {
            try
            {
                object clienteSeleccionado = dtg_clientes.SelectedItem;
                Cliente cliente = (Cliente)clienteSeleccionado;
                if (txt_nombreCliente.Text == "")
                {
                    throw new Exception("DEBE SELECCIONAR UN CLIENTE DE LA TABLA");
                }
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("", "" + ex.Message);
            }
        }

        private void btn_reparacion_Click(object sender, RoutedEventArgs e)
        {
            AgregarReparacion reparacion = new AgregarReparacion(nombre);
            reparacion.Owner = this;
            reparacion.ShowDialog();
        }

        private void txt_montoCliente_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            txt_abono.Text = txt_abonoCliente.Text;
        }
    }
}
