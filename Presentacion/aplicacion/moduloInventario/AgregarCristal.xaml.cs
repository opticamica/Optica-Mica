using MahApps.Metro.Controls;
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

namespace Presentacion.aplicacion.moduloInventario
{
    /// <summary>
    /// Lógica de interacción para AgregarCristal.xaml
    /// </summary>
    public partial class AgregarCristal : MetroWindow
    {
        OracleConnection conn = null;
        public AgregarCristal(string nombre)
        {
            InitializeComponent();
            AbrirConexion();
            IniciarMaterialCristal();
            IniciarTipoLente();
            IniciarProveedor();

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

        public void IniciarTipoLente()
        {
            OracleCommand cmd = new OracleCommand("FN_LISTAR_TIPO_LENTE", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            List<TipoLente> tipoLentes = new List<TipoLente>();

            OracleParameter output = cmd.Parameters.Add("L_CURSOR", OracleDbType.RefCursor);
            output.Direction = ParameterDirection.ReturnValue;
            cmd.ExecuteNonQuery();
            OracleDataReader reader = ((OracleRefCursor)output.Value).GetDataReader();

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

        public void IniciarProveedor()
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
            comando.Parameters.Add(":nombre",nombre );
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

                MessageBox.Show("" + ObtenerSucursal());
                try
                {
                    //mantenedorEmpleado.ValidarEmpleado(empleado);
                    //mantenedorClienteBS.Validacion(cliente);
                    OracleCommand cmd = new OracleCommand("SP_GUARDAR_CRISTAL", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("codCristal", OracleDbType.Varchar2).Value = txt_codCristal.Text;
                    cmd.Parameters.Add("idSucursal", OracleDbType.Int32).Value = ObtenerSucursal();
                    
                    cmd.Parameters.Add("idMaterial", OracleDbType.Int32).Value = cristal.Material.IdMaterialCristal;
                    cmd.Parameters.Add("filtroAzul", OracleDbType.Char).Value = cristal.FiltroAzul;
                    cmd.Parameters.Add("fotocro", OracleDbType.Char).Value = cristal.Fotocromatico;
                    cmd.Parameters.Add("polari", OracleDbType.Char).Value = cristal.Blanco;
                    cmd.Parameters.Add("espejado", OracleDbType.Char).Value = cristal.Capa;

                    cmd.Parameters.Add("idTipoLente", OracleDbType.Decimal).Value = cristal.TipoLente.IdTipoLente;
                    cmd.Parameters.Add("esfera", OracleDbType.Decimal).Value = cristal.Esfera;
                    cmd.Parameters.Add("cilindro", OracleDbType.Decimal).Value = cristal.Cilindro;
                    cmd.Parameters.Add("stock", OracleDbType.Int32).Value = cristal.Stock;
                    cmd.Parameters.Add("proveedor", OracleDbType.Int32).Value = cristal.Proveedor.IdProveedor;
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("El Cristal: " + txt_codCristal.Text + " Fue Agregado al sistema");
                    MessageBoxResult m = MessageBox.Show("Quiere agregar otro Cristal", "Guardar Cristal", MessageBoxButton.OKCancel);
                    if (m == MessageBoxResult.Cancel)
                    {
                         
                        Inventario inventario = new Inventario(nombre);
                        //inventario.CargarCristales();
                        this.Close();
                    }
                    //Limpiar();
                }
                catch (Exception ex)
                {

                    MessageBox.Show("ERROR AL CREAR CLIENTE: " + ex.Message);
                }

                //CargarCristales();
            }
            else
            {
                MessageBox.Show("El Codigo ya existe. ");
            }
        }

        private bool ExisteCodCristal(string codCristal)
        {
            
            bool existe = false;

            foreach (Cristal cri in ListarCristales())
            {
                if (cri.CodCristal.Equals(codCristal))
                {
                    existe = true;
                }
            }

            return existe;
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
                object materialselect = cmb_material.SelectedItem, tipoLenteselect = cmb_tipo_lente.SelectedItem, proveedorselect = cmb_proveedor.SelectedItem;
                MaterialCristal materialCristal = (MaterialCristal)materialselect;
                TipoLente tipoLente = (TipoLente)tipoLenteselect;
                Proveedor proveedor = (Proveedor)proveedorselect;

                //cristal.CodCristal = txt_codCristal.Text;
                cristal.Material = new MaterialCristal();
                cristal.Material.IdMaterialCristal = materialCristal.IdMaterialCristal;
                
                if (chk_azul.IsChecked == true)
                {
                    cristal.FiltroAzul = "Si";
                }
                else
                {
                    cristal.FiltroAzul = "No";
                }

                if (chk_fotocromatico.IsChecked == true)
                {
                    cristal.Fotocromatico = "Si";
                }
                else
                {
                    cristal.Fotocromatico = "No";
                }
                if (chk_polarizado.IsChecked == true)
                {
                    cristal.Blanco = "Si";
                }
                else
                {
                    cristal.Blanco= "No";
                }
                if (chk_espejado.IsChecked == true)
                {
                    cristal.Capa= "Si";
                }
                else
                {
                    cristal.Capa= "No";
                }
                cristal.TipoLente = new TipoLente();
                cristal.TipoLente.IdTipoLente = tipoLente.IdTipoLente;
                cristal.Esfera =txt_esfera.Text;
                cristal.Cilindro = txt_cilindro.Text;
                cristal.Stock = Convert.ToInt32(txt_stock.Text);
                cristal.Proveedor = new Proveedor();
                cristal.Proveedor.IdProveedor =proveedor.IdProveedor;
                txt_codCristal.Text = proveedor.Nombre+materialCristal.Nombre.Substring(0, 2) + tipoLente.Nombre.Substring(0,2)+"E"+txt_esfera.Text+"C"+txt_cilindro.Text;

                cristal.CodCristal = txt_codCristal.Text;

            }
            catch(Exception ex)
            {
                MessageBox.Show("hola"+ ex.Message);
            }
            return cristal;

        }
    }
}

