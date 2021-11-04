using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Oracle.ManagedDataAccess.Client;
using Presentacion.vistas.ModuloCotizacion;
using Presentacion.vistas.ModuloInventario;
using Presentacion.vistas.ModuloPuntoVenta;
using Presentacion.vistas.utility;
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
using System.Windows.Threading;

namespace Presentacion.aplicacion
{
    /// <summary>
    /// Lógica de interacción para MenuPrincipal.xaml
    /// </summary>
    public partial class MenuPrincipal : MetroWindow
    {
        DispatcherTimer timer;
        double panelWidth;
        bool hidden;
        OracleConnection conn = null;
        string nombre;
        public MenuPrincipal(string nombre)
        {
            InitializeComponent();
            AbrirConexion();
            this.nombre = nombre;
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 0);
            timer.Tick += Timer_Tick;
            panelWidth = pnl_panelside.Width;
            //this.DataContext = new ModuloVenta(DialogCoordinator.Instance,nombre);
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (hidden)
            {
                pnl_panelside.Width += 1;
                if (pnl_panelside.Width >= panelWidth)
                {
                    timer.Stop();
                    hidden = false;
                }
            }
            else
            {
                pnl_panelside.Width -= 1;
                if (pnl_panelside.Width <= 40)
                {
                    timer.Stop();
                    hidden = true;
                }
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
                this.ShowMessageAsync("", "error de conexion");
                throw new Exception("error");
            }
        }
        private void IrPuntoVenta()
        {
            ModuloPuntoVenta moduloPunto = new ModuloPuntoVenta(nombre);
            moduloPunto.Owner = this;
            moduloPunto.ShowDialog();
            //moduloPunto.Show();
        }

        

        private void IrCotizacion()
        {
            Cotizacion cotizacion = new Cotizacion();
            FrameContent.Content = cotizacion;
        }

        private void btn_reservaHora_Click(object sender, RoutedEventArgs e)
        {
            FrameContent.Width = 1100;
            IrReservaHora();
        }

        private void IrReservaHora()
        {
            ModuloReservaHora moduloReservaHora = new ModuloReservaHora(nombre);
            moduloReservaHora.Owner = this;
            moduloReservaHora.ShowDialog();
        }

        private void btn_menu_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
            if (pnl_panelside.Width == 150)
            {

                FrameContent.Width = 1800;
            }
            else
            {

                FrameContent.Width = 1100;
            }
        }

        private void pnl_panelHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            FrameContent.Width = 1100;
            ModuloVenta puntoVenta = new ModuloVenta(nombre);
            FrameContent.Content =puntoVenta;

            //___Sin_nombre_.IsSelected = false;
            //IrPuntoVenta();

        }

        private void ListViewItem_Selected_1(object sender, RoutedEventArgs e)
        {
            FrameContent.Width = 1100;
            var userControl = new Inventario(nombre);
            var shellWindow = new MetroShellWindow();
            shellWindow.Content = userControl;
            FrameContent.Content = shellWindow.Content;
            //shellWindow.Show();
            //IrInvetario();
        }

        private void ListViewItem_Selected_2(object sender, RoutedEventArgs e)
        {
            FrameContent.Width = 1100;
            var userControl = new ModuloVenta(nombre);
            var shellWindow = new MetroShellWindow();
            shellWindow.Content = userControl;
            FrameContent.Content = userControl;
            //shellWindow.Show();
            /*
            //var userControl = new ModuloVenta(nombre);
            //var shellWindow = new MetroShellWindow();
            //FrameContent.Content = userControl;
            //shellWindow.Show();// *Diálogo * / ();
            FrameContent.Width = 1100;
            ModuloVenta venta = new ModuloVenta(DialogCoordinator.Instance, nombre);
            //grd_contenido.conte = venta;
            FrameContent.Content = venta;
            //IrReservaHora();*/
        }

        private void ListViewItem_Selected_3(object sender, RoutedEventArgs e)
        {
            FrameContent.Width = 1100;
            IrCotizacion();
            
        }

        private async void btn_cerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            MessageDialogResult m = await this.ShowMessageAsync("CERRAR SESION", "ESTA SEGURO QUE DESEA SALIR ? ", MessageDialogStyle.AffirmativeAndNegative);
            if (m == MessageDialogResult.Affirmative)
            {
                //ACA HAY QUE ACTUALIZAR LA TABLA SESION
                try
                {
                    OracleCommand cmd = new OracleCommand("SP_CERRAR_SESION", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("NOMBRE", OracleDbType.Varchar2).Value = nombre;
                    cmd.ExecuteNonQuery();
                    await this.ShowMessageAsync("", "SESION MODIFICADA");
                }
                catch (Exception ex)
                {
                    await this.ShowMessageAsync("", "ERROR AL AGREGAR DIRECCION: " + ex.Message);
                }
                this.Hide();
                MainWindow main = new MainWindow();
                main.Owner = this;
                main.ShowDialog();
            }
            else
            {

            }
            
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            lbl_nombreUsuario.Content ="Usuario: " + nombre;
        }
    }
}
