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
using System.Globalization;
using Negocio.aplicacion.negocio;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Paragraph = iTextSharp.text.Paragraph;
using Microsoft.VisualBasic;
using Microsoft.Win32;

namespace Presentacion.aplicacion
{
    /// <summary>
    /// Lógica de interacción para AgregarReceta.xaml
    /// </summary>
    public partial class AgregarReceta : Window
    {
        OracleConnection conn = null;
        MantenedorRecetaBS mantenedorRecetaBS;
        string nombre;
        string formatoNombrePDF;
        bool pdf = false;
        public AgregarReceta(string nombre,Cliente cliente)
        {
            AbrirConexion();
            InitializeComponent();
            IniciarTipoReceta();
            this.nombre = nombre;
            dtp_fecha.SelectedDate = DateTime.Now;
            txt_rutCliente.Text = cliente.Rut;
            mantenedorRecetaBS = new MantenedorRecetaBS();
            
        }


        public void IniciarTipoReceta()
        {
            OracleCommand cmd = new OracleCommand("FN_LISTAR_TIPO_RECETA", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            List<TipoReceta> tipoRecetas = new List<TipoReceta>();
            OracleParameter output = cmd.Parameters.Add("L_CURSOR", OracleDbType.RefCursor);
            output.Direction = ParameterDirection.ReturnValue;
            cmd.ExecuteNonQuery();
            OracleDataReader reader = ((OracleRefCursor)output.Value).GetDataReader();

            while (reader.Read())
            {
                TipoReceta tipo = new TipoReceta();
                tipo.IdTipoReceta = reader.GetInt32(0);
                tipo.Nombre = reader.GetString(1);
                tipoRecetas.Add(tipo);

            }
            cmb_tipoReceta.ItemsSource = tipoRecetas;
            cmb_tipoReceta.Items.Refresh();
        }

        private void ObtenerDatos()
        {
            OracleCommand comando = new OracleCommand("SELECT u.rut,u.nombre,u.apellidop,t.nombre FROM USUARIO u join tipo_usuario t on u.id_tipo_usuario = t.id_tipo_usuario WHERE u.USUARIO = :usuario ", conn);
            comando.Parameters.Add(":usuario", lbl_nombreUsuario.Content);
            OracleDataReader reader = comando.ExecuteReader();

            while (reader.Read())
            {
                Usuario usuario = new Usuario();
                usuario.Rut = reader.GetString(0);
                usuario.Nombre = reader.GetString(1);
                usuario.Apellidop = reader.GetString(2);
                usuario.TipoUsuario = new TipoUsuario();
                usuario.TipoUsuario.Nombre = reader.GetString(3);
                lbl_tipo.Content = usuario.TipoUsuario.Nombre;

                txt_rutUsuario.Text = usuario.Rut;
                string nombreCompleto = usuario.Nombre + " " +usuario.Apellidop;
                lbl_nombreUsuario.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nombreCompleto);
            }
        }

        private void ObtenerNombrePaciente()
        {
            OracleCommand comando = new OracleCommand("SELECT nombre,apellidop FROM cliente WHERE rut= :cliente", conn);
            comando.Parameters.Add(":cliente", txt_rutCliente.Text);
            OracleDataReader reader = comando.ExecuteReader();

            while (reader.Read())
            {
                Cliente cliente = new Cliente();
                cliente.Nombre = reader.GetString(0);
                cliente.ApellidoP = reader.GetString(1);
                string nombreCompleto = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cliente.Nombre) + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cliente.ApellidoP);
                formatoNombrePDF = nombreCompleto;
            }
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

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            lbl_nombreUsuario.Content = nombre;
            ObtenerDatos();
        }

        private void btn_guardar_Click(object sender, RoutedEventArgs e)
        {
            GuardarReceta();
        
        }
        private void GuardarReceta()
        {
            object filaSeleccionada = cmb_tipoReceta.SelectedItem;
            if (filaSeleccionada != null)
            {
                ValidarRutCliente();
                Receta receta = CrearReceta();
                string var = txt_esf_od.Text + txt_cil_od.Text+txt_esf_oi.Text + txt_cil_oi.Text + txt_grado_od.Text + txt_grado_oi.Text;
                MessageBox.Show("ESFERA " + receta.EsferaOD + " cilindro " + receta.CilindroOD + " " + txt_grado_od.Text+ " " + txt_esf_oi.Text+ " " + txt_cil_oi.Text + " " + txt_grado_oi.Text);
                if (var.Length != 0)
                {
                    try
                    {
                        mantenedorRecetaBS.Validacion(receta);
                        TipoReceta tipo = (TipoReceta)filaSeleccionada;
                        OracleCommand cmd = new OracleCommand("SP_GUARDAR_RECETA", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("id_tipoReceta", OracleDbType.Int32).Value = tipo.IdTipoReceta;
                        cmd.Parameters.Add("rutUsuario", OracleDbType.Varchar2).Value = txt_rutUsuario.Text;
                        cmd.Parameters.Add("rutCliente", OracleDbType.Varchar2).Value = txt_rutCliente.Text;
                        cmd.Parameters.Add("edad1", OracleDbType.Int32).Value = receta.Edad;
                        cmd.Parameters.Add("obs", OracleDbType.Varchar2).Value = receta.Observacion;
                        cmd.Parameters.Add("esf_od", OracleDbType.Varchar2).Value = receta.EsferaOD;
                        cmd.Parameters.Add("cil_od", OracleDbType.Varchar2).Value = receta.CilindroOD;
                        cmd.Parameters.Add("gra_od", OracleDbType.Varchar2).Value = receta.GradoOD;
                        cmd.Parameters.Add("esf_oi", OracleDbType.Varchar2).Value = receta.EsferaOI;
                        cmd.Parameters.Add("cil_oi", OracleDbType.Varchar2).Value = receta.CilindroOI;
                        cmd.Parameters.Add("gra_oi", OracleDbType.Varchar2).Value = receta.GradoOI;
                        //cmd.Parameters.Add("gra_oi", OracleDbType.Varchar2).Value = txt_grado_oi.Text;
                        cmd.Parameters.Add("dp_lej", OracleDbType.Int32).Value = receta.DpLejos;
                        cmd.Parameters.Add("dp_cer", OracleDbType.Int32).Value = receta.DpCerca;
                        cmd.Parameters.Add("adicion1", OracleDbType.Varchar2).Value = receta.Adiccion;
                        cmd.Parameters.Add("fecha1", OracleDbType.Date).Value = receta.Fecha;
                        cmd.ExecuteNonQuery();
                        if (txt_grado_od.IsEnabled == true && txt_grado_od.Text.Length ==0 )
                        {
                            throw new Exception("\nDEBE AGREGAR EL GRADO OD");
                        }
                        if (txt_grado_oi.IsEnabled == true && txt_grado_oi.Text.Length == 0)
                        {
                            throw new Exception("\nDEBE AGREGAR EL GRADO OI");
                        }
                        MessageBox.Show("La receta Fue Agregado al sistema");
                        Limpiar();
                        MessageBoxResult m = MessageBox.Show("Quiere descargar la Receta en PDF", "Guardar Receta", MessageBoxButton.OKCancel);
                        if (m == MessageBoxResult.Cancel)
                        {
                            CerrarForm();
                        }
                        else
                        {
                            try
                            {
                                DescargarPdf();
                                CerrarForm();
                               
                            }
                            catch (Exception ex) 
                            {
                                MessageBox.Show("Error al descargar PDF: " + ex);
                            }
                            
                        }
                        pdf = true;
                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("ERROR AL CREAR RECETA: " + ex.Message + "  "+receta.EsferaOI );
                    }
                }
                else
                {
                    MessageBox.Show("Agrege un dato al menos en la receta ");      
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar un tipo de receta.");
            }
        }

        private void CerrarForm()
        {
            ModuloPuntoVenta moduloPuntoVenta = new ModuloPuntoVenta(nombre);
            moduloPuntoVenta.CargarClientes();
            moduloPuntoVenta.CargarRecetas();
            this.Close();
        }

        private void ValidarRutCliente()
        {
            OracleCommand comando = new OracleCommand("SELECT u.rut,u.nombre,u.apellidop,t.nombre FROM USUARIO u join tipo_usuario t on u.id_tipo_usuario = t.id_tipo_usuario WHERE u.USUARIO = :usuario ", conn);
            comando.Parameters.Add(":usuario", lbl_nombreUsuario.Content);
            OracleDataReader reader = comando.ExecuteReader();
            while (reader.Read())
            {
                Usuario usuario = new Usuario();
                usuario.Rut = reader.GetString(0);
                usuario.Nombre = reader.GetString(1);
                usuario.Apellidop = reader.GetString(2);
                txt_rutUsuario.Text= usuario.Rut;
                string nombreCompleto = usuario.Nombre + " " + usuario.Apellidop;
                lbl_nombreUsuario.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nombreCompleto);
            }
        }
        private void Limpiar()
        {
            txt_edad.Text = "";
            txt_observacion.Text = " ";
            txt_esf_od.Text = "";
            txt_cil_od.Text = "";
            txt_grado_od.Text = "";
            txt_esf_oi.Text = "";
            txt_cil_oi.Text = "";
            txt_grado_oi.Text = "";
            txt_dpLejos.Text = "";
            txt_dpCerca.Text = "";
            txt_add.Text = "";
           dtp_fecha.SelectedDate = DateTime.Now;
        }
        private Receta CrearReceta()
        {
            Receta receta = new Receta();
            try
            {
                receta.Usuario = new Usuario();
                receta.Usuario.Rut = txt_rutUsuario.Text;
                receta.Cliente = new Cliente();
                receta.Cliente.Rut = txt_rutCliente.Text;
                receta.Edad = Int32.Parse(txt_edad.Text);
                receta.Observacion = txt_observacion.Text;
                
                if (txt_esf_od.Text.Length == 0)
                {
                    receta.EsferaOD = null;

                }
                else
                {
                    receta.EsferaOD = txt_esf_od.Text;
                }
                if (txt_esf_oi.Text.Length == 0)
                {
                    receta.EsferaOI = null;

                }
                else
                {
                    receta.EsferaOI = txt_esf_oi.Text;
                }

                if (!(String.IsNullOrEmpty(txt_cil_od.Text)))
                {
                    receta.CilindroOD = txt_cil_od.Text;
                }
                else
                {
                    receta.CilindroOD = null;
                }

                if (!(String.IsNullOrEmpty(txt_cil_od.Text)))
                {
                    receta.CilindroOD = txt_cil_od.Text;
                }
                else
                {
                    receta.CilindroOD = null;
                }

                /*if (!(String.IsNullOrEmpty(txt_esf_oi.Text)))
                {
                    receta.EsferaOI = txt_esf_oi.Text;
                }
                else
                {
                    receta.EsferaOI = null;
                }*/
                if (!(String.IsNullOrEmpty(txt_cil_oi.Text)))
                {
                    receta.CilindroOI = txt_cil_oi.Text;
                }
                else
                {
                    receta.CilindroOI = null;
                }
                if (!(String.IsNullOrEmpty(txt_add.Text)))
                {
                    receta.Adiccion = txt_add.Text;
                }
                else
                {
                    receta.Adiccion = null;
                }
                int valorInt;
                if (Int32.TryParse(txt_dpLejos.Text, out valorInt))
                {
                    receta.DpLejos = valorInt;
                }
                else
                {
                    receta.DpLejos = 0;
                }
                if (Int32.TryParse(txt_dpCerca.Text, out valorInt))
                {
                    receta.DpCerca = valorInt;
                }
                else
                {
                    receta.DpCerca = 0;
                }
                if (!(String.IsNullOrEmpty(txt_grado_od.Text)))
                {
                     receta.GradoOD=txt_grado_od.Text ;
                }
                else
                {
                    receta.GradoOD = null;
                }
                if (!(String.IsNullOrEmpty(txt_grado_oi.Text)))
                {
                    receta.GradoOI = txt_grado_oi.Text;
                }
                else
                {
                    receta.GradoOI = null;
                }
                //receta.EsferaOD = decimal.Parse(txt_esf_od.Text);
                //receta.CilindroOD = decimal.Parse(txt_cil_od.Text);

                //receta.DpLejos = Convert.ToInt32(txt_dpLejos.Text);
                //receta.DpCerca = Convert.ToInt32(txt_dpCerca.Text);
                //receta.Adiccion = txt_add.Text;
                receta.Fecha = (DateTime)dtp_fecha.SelectedDate;

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR AL CREAR RECETA: bb" + ex.Message);
            }
            return receta;
        }
        private void btn_guardar_Copy_Click(object sender, RoutedEventArgs e)
        {
            Limpiar();
        }
        private void ValidarSoloNumeros(object sender, TextCompositionEventArgs e)
        {
            int ascci = Convert.ToInt32(Convert.ToChar(e.Text));
            if (ascci >= 48 && ascci <= 57) 
                e.Handled = false;
            else 
                e.Handled = true;
        }
        private void txt_edad_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {   
            ValidarSoloNumeros(sender, e);
        }
        private void txt_dpLejos_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidarSoloNumeros(sender, e);
        }
        private void txt_dpCerca_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidarSoloNumeros(sender, e);
        }
       
        private void txt_add_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            validarTextReceta(txt_add.Text,sender, e);
        }

        private void txt_esf_od_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            validarTextReceta(txt_esf_od.Text, sender, e);
        }
        private void txt_cil_od_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            validarTextReceta(txt_cil_od.Text, sender, e);   
        }
        private void txt_esf_oi_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            validarTextReceta(txt_esf_oi.Text, sender, e);
        }
        private void txt_cil_oi_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            validarTextReceta(txt_cil_oi.Text, sender, e);
        }
        public void validarTextReceta(string valor, object sender, KeyboardFocusChangedEventArgs e)
        {
            if (valor.Trim().Length == 0)
            {
                valor = null;
            }
            else if ((!ValidDecimal(valor) || !CountIntsOrPoint(valor)))
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

        private void txt_grado_od_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
             ValidarSoloNumeros(sender, e);
        }
        private void txt_grado_oi_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidarSoloNumeros(sender, e);
        }

        private void ValidarSoloDecimales(object sender, TextCompositionEventArgs e)
        {
            int ascci = Convert.ToInt32(Convert.ToChar(e.Text));
            if ((ascci >= 48 && ascci <= 57) || (ascci >= 43  && ascci <= 45))
                 e.Handled = false;
            else
                e.Handled = true;
        }



        private void txt_esf_od_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidarSoloDecimales(sender, e);
        }

        private void txt_cil_od_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
             ValidarSoloDecimales(sender, e);
        }

        private void txt_esf_oi_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
             ValidarSoloDecimales(sender, e);
        }

        private void txt_cil_oi_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
             ValidarSoloDecimales(sender, e);
        }

        private void txt_cil_od_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txt_cil_od.Text.Length == 0)
            {

                txt_grado_od.IsEnabled = false;
            }
            else
            {

                txt_grado_od.IsEnabled = true;

            }
        }

        private void txt_cil_oi_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txt_cil_oi.Text.Length == 0)
            {

                txt_grado_oi.IsEnabled = false;
            }
            else
            {

                txt_grado_oi.IsEnabled = true;

            }
        }

        
        public List<Receta> ListarRecetas()

        {
            OracleCommand comando = new OracleCommand("SELECT r.id_receta,t.nombre,r.rut_usuario,r.rut_cliente,r.edad,r.observaciones,COALESCE(r.esfera_od,'0'),COALESCE(r.cilindro_od,'0'),COALESCE(r.grados_od,'N/A'),COALESCE(r.esfera_oi,'0'),COALESCE(r.cilindro_oi,'0'),COALESCE(r.grados_oi,'N/A'),COALESCE(r.dp_lejos,0),COALESCE(r.dp_cerca,0),COALESCE(r.adicion,'0'),r.fecha,c.nombre,c.apellidop,ti.nombre  " +
                "FROM receta r join tipo_receta t on r.id_tipo_receta = t.id_tipo_receta" +
                " join cliente c on c.rut = r.rut_cliente join usuario u on u.rut = r.rut_usuario join tipo_usuario ti on ti.id_tipo_usuario = u.id_tipo_usuario where r.id_receta = (select max(id_receta) from receta where rut_usuario = :rut)", conn);
            comando.Parameters.Add(":rut", txt_rutUsuario.Text);
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
                receta.Cliente.Nombre = reader.GetString(16);
                receta.Cliente.ApellidoP = reader.GetString(17);
                receta.Usuario.TipoUsuario = new TipoUsuario();
                receta.Usuario.TipoUsuario.Nombre = reader.GetString(18);
                lista.Add(receta);
            }

            return lista;
        }

        private void btn_pdf_Click(object sender, RoutedEventArgs e)
        {
            if (pdf==true)
            {
                DescargarPdf();
            }
            else
            {
                MessageBox.Show("DEBE GUARDAR LA RECETA PRIMERO");
            }
            
        }

        private void DescargarPdf()
        {

            ObtenerNombrePaciente();
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Archivos de Pdf|*.pdf";
            saveFileDialog.Title = "Guardar archivo";
            saveFileDialog.FileName = formatoNombrePDF + DateTime.Now.ToString("dd-MM-yyyy");
            saveFileDialog.ShowDialog();

            Document doc = new Document(iTextSharp.text.PageSize.LETTER, 30f, 30f, 20f, 20f);
            var path = string.Empty;
            path = saveFileDialog.FileName;
            PdfWriter wri = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
            //doc1.Open();

            //FileStream fs = new FileStream(@"C:\Users\Analista\Desktop\Receta.pdf", FileMode.Create);
            //MemoryStream ms = new MemoryStream();
            //Document doc = new Document(iTextSharp.text.PageSize.LETTER, 30f, 30f, 20f, 20f);
            //PdfWriter pw = PdfWriter.GetInstance(doc, fs);
            doc.Open();
            doc.AddAuthor("Optica Mica");
            doc.AddTitle("Receta");
            //fuente
            iTextSharp.text.Font standartfont = new iTextSharp.text.Font(iTextSharp.text.Font.HELVETICA, 14, iTextSharp.text.Font.NORMAL, iTextSharp.text.Color.BLACK);
            iTextSharp.text.Font negritafont = new iTextSharp.text.Font(iTextSharp.text.Font.HELVETICA, 16, iTextSharp.text.Font.BOLD, iTextSharp.text.Color.BLACK);

            //BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA,BaseFont.CP1252,BaseFont.EMBEDDED);

            //encabezadi
            //imagen logo
            iTextSharp.text.Image imagen = iTextSharp.text.Image.GetInstance(@"C:\Users\Analista\Desktop\logoAzul.png");
            imagen.BorderWidth = 20;
            imagen.Alignment = Element.ALIGN_LEFT;
            float percentage = 0.0f;
            percentage = 150 / imagen.Width;
            imagen.ScalePercent(percentage * 150);

            //doc.Add(imagen);
            //imagen logo
            //doc.Add(new Paragraph("Optica Mica"));
            //doc.Add(Chunk.NEWLINE);

            //byte[] bytesStream = ms.ToArray();
            //ms = new MemoryStream();
            //ms.Write(bytesStream, 0, bytesStream.Length);
            //ms.Position = 0;
            //encabezado de columnas


            PdfPTable tblReceta = new PdfPTable(4);
            tblReceta.WidthPercentage = 60;
            tblReceta.HorizontalAlignment = Element.ALIGN_LEFT;
            //tblReceta.HorizontalAlignment = HorizontalAlignment.Left;

            PdfPCell clEspacio = new PdfPCell(new Phrase("", standartfont));
            clEspacio.BorderWidth = 1;


            PdfPCell clEsferaOD = new PdfPCell(new Phrase("ESFERA", standartfont));
            clEsferaOD.BorderWidth = 1;
            clEsferaOD.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell clCilindroOD = new PdfPCell(new Phrase("CILINDRO", standartfont));
            clCilindroOD.BorderWidth = 1;
            clCilindroOD.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell clGradoOD = new PdfPCell(new Phrase("GRADO", standartfont));
            clGradoOD.BorderWidth = 1;
            clGradoOD.HorizontalAlignment = Element.ALIGN_CENTER;

            tblReceta.AddCell(clEspacio);
            tblReceta.AddCell(clEsferaOD);
            tblReceta.AddCell(clCilindroOD);
            tblReceta.AddCell(clGradoOD);




            PdfPCell clEsferaOI = new PdfPCell(new Phrase("", standartfont));
            clEsferaOI.BorderWidth = 1;
            clEsferaOI.BorderWidthBottom = 0.5f;

            PdfPCell clCilindroOI = new PdfPCell(new Phrase("", standartfont));
            clCilindroOI.BorderWidth = 1;
            clCilindroOI.BorderWidthBottom = 0.5f;

            PdfPCell clGradoOI = new PdfPCell(new Phrase("GRADO OI", standartfont));
            clGradoOI.BorderWidth = 1;
            clGradoOI.BorderWidthBottom = 0.5f;

            //
            PdfPTable tblAdd = new PdfPTable(1);
            tblAdd.WidthPercentage = 15;
            tblAdd.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell clAdd = new PdfPCell(new Phrase("Adicion", standartfont));
            clAdd.BorderWidth = 1;
            clAdd.HorizontalAlignment = Element.ALIGN_CENTER;


            tblAdd.AddCell(clAdd);

            PdfPTable tblDp = new PdfPTable(2);
            tblDp.WidthPercentage = 30;
            tblDp.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell clDpLejos = new PdfPCell(new Phrase("Dp Lejos", standartfont));
            clDpLejos.BorderWidth = 1;
            clDpLejos.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell clDpCerca = new PdfPCell(new Phrase("Dp Cerca", standartfont));
            clDpCerca.BorderWidth = 1;
            clDpCerca.HorizontalAlignment = Element.ALIGN_CENTER;

            tblDp.AddCell(clDpLejos);
            tblDp.AddCell(clDpCerca);


            doc.Add(Chunk.NEWLINE);

            //agregando datos
            foreach (Receta receta in ListarRecetas())
            {

                //receta.Usuario = new Usuario();
                //PdfCell _cell = new PdfCell(new Paragraph("\tNumero de Receta: " + receta.IdReceta));
                Paragraph paragraph = new Paragraph();
                paragraph.Clear();
                paragraph.Alignment = Element.ALIGN_CENTER;
                paragraph.Font = standartfont;
                //paragraph.Add("Numero de Receta: \t" + receta.IdReceta+"\n");
                //paragraph.Add(lbl_nombreUsuario.Content + "\n");
                paragraph.Add(receta.Usuario.TipoUsuario.Nombre + "\n");
                paragraph.Add(receta.Usuario.Rut + "\n");
                Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator());

                Paragraph paragraph2 = new Paragraph();
                paragraph2.Clear();
                paragraph2.Alignment = Element.ALIGN_LEFT;
                paragraph2.Font = standartfont;
                paragraph2.Add("\nNombre Paciente: " + receta.Cliente.Nombre + " " + receta.Cliente.ApellidoP + "\n");
                paragraph2.Add("Rut Paciente: " + receta.Cliente.Rut + "\n");
                paragraph2.Add("Edad Paciente: " + receta.Edad + "\n");
                paragraph2.Add("Observaciones: \n\n" + receta.Observacion + "\n");

                Paragraph numeroReceta = new Paragraph();
                numeroReceta.Alignment = Element.ALIGN_RIGHT;
                numeroReceta.Font = standartfont;
                numeroReceta.Add("Receta N°: \t" + receta.IdReceta + "      \n ");

                Paragraph nombreNegrita = new Paragraph();
                nombreNegrita.Alignment = Element.ALIGN_CENTER;
                nombreNegrita.Font = negritafont;
                nombreNegrita.Add(lbl_nombreUsuario.Content + "\n");


                doc.Add(numeroReceta);

                doc.Add(imagen);
                doc.Add(nombreNegrita);

                doc.Add(paragraph);
                doc.Add(linea);
                doc.Add(paragraph2);

                clEspacio = new PdfPCell(new Phrase("OD", standartfont));
                clEspacio.BorderWidth = 1;
                clEspacio.HorizontalAlignment = Element.ALIGN_CENTER;

                clEsferaOD = new PdfPCell(new Phrase(receta.EsferaOD, standartfont));
                clEsferaOD.BorderWidth = 1;
                clEsferaOD.HorizontalAlignment = Element.ALIGN_CENTER;

                clCilindroOD = new PdfPCell(new Phrase(receta.CilindroOD, standartfont));
                clCilindroOD.BorderWidth = 1;
                clCilindroOD.HorizontalAlignment = Element.ALIGN_CENTER;

                clGradoOD = new PdfPCell(new Phrase(receta.GradoOD, standartfont));
                clGradoOD.BorderWidth = 1;
                clGradoOD.HorizontalAlignment = Element.ALIGN_CENTER;


                tblReceta.AddCell(clEspacio);
                tblReceta.AddCell(clEsferaOD);
                tblReceta.AddCell(clCilindroOD);
                tblReceta.AddCell(clGradoOD);

                clEspacio = new PdfPCell(new Phrase("OI", standartfont));
                clEspacio.BorderWidth = 1;
                clEspacio.HorizontalAlignment = Element.ALIGN_CENTER;

                clEsferaOI = new PdfPCell(new Phrase(receta.EsferaOI, standartfont));
                clEsferaOI.BorderWidth = 1;
                clEsferaOI.HorizontalAlignment = Element.ALIGN_CENTER;

                clCilindroOI = new PdfPCell(new Phrase(receta.CilindroOI, standartfont));
                clCilindroOI.BorderWidth = 1;
                clCilindroOI.HorizontalAlignment = Element.ALIGN_CENTER;

                clGradoOI = new PdfPCell(new Phrase(receta.GradoOI, standartfont));
                clGradoOI.BorderWidth = 1;
                clGradoOI.HorizontalAlignment = Element.ALIGN_CENTER;

                tblReceta.AddCell(clEspacio);
                tblReceta.AddCell(clEsferaOI);
                tblReceta.AddCell(clCilindroOI);
                tblReceta.AddCell(clGradoOI);



                //

                clAdd = new PdfPCell(new Phrase(receta.Adiccion, standartfont));
                clAdd.BorderWidth = 1;
                clAdd.HorizontalAlignment = Element.ALIGN_CENTER;

                tblAdd.AddCell(clAdd);
                //doc.Add(linea);

                //

                clDpLejos = new PdfPCell(new Phrase(receta.DpLejos.ToString(), standartfont));
                clDpLejos.BorderWidth = 1;
                clDpLejos.HorizontalAlignment = Element.ALIGN_CENTER;

                clDpCerca = new PdfPCell(new Phrase(receta.DpCerca.ToString(), standartfont));
                clDpCerca.BorderWidth = 1;
                clDpCerca.HorizontalAlignment = Element.ALIGN_CENTER;



                tblDp.AddCell(clDpLejos);
                tblDp.AddCell(clDpCerca);
                doc.Add(linea);
                /*
                Paragraph paragraph4 = new Paragraph();
                paragraph4.Clear();
                paragraph4.Alignment = Element.ALIGN_LEFT;
                paragraph4.Font.Size = 12;
                paragraph4.Add("\n\tFecha: " + receta.Fecha.ToString()+ "\t\t\t\t\t\t"+ "Firma: ");

                doc.Add(paragraph4);
                */
            }

            Paragraph paragraph3 = new Paragraph();
            //paragraph3.Clear();
            paragraph3.Add("\n");

            doc.Add(tblAdd);
            doc.Add(paragraph3);
            doc.Add(tblReceta);
            doc.Add(paragraph3);
            doc.Add(tblDp);

            //doc.Add(paragraph3);
            Chunk linea2 = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator());
            doc.Add(linea2);
            doc.Add(new Phrase("\n\n\n\n\n\n\n\n\n\n\n\t\t\t\t\t\t\t\t\t\t                 Fecha: " + DateTime.Now.ToString("dd-MM-yyyy") + "\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t                       Firma: _________________"));
            doc.Close();
            //pw.Close();
            MessageBox.Show("documento exportado");
        }
    }
}
