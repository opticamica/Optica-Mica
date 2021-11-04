using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.aplicacion.modelo
{
    public class Venta
    {
        private int idVenta;
        private Usuario usuario;
        private string nombreUsuario;
        private Cliente cliente;
        private string nombreCliente;
        private Receta receta;
        private MedioPago medioPago;
        private DateTime fechaPago;
        private Montura montura;
        private Despacho despacho;
        private Cristal cristal;
        private Cristal cristal2;
        private int monto;
        private int abono;
        private int saldo;
        private int cant;     
        private string observacion;
        private string direccion;
        private string comuna;
        private string estadoVenta;
        private string entregado;

        public Venta()
        {

        }

        public Venta(int idVenta, Usuario usuario, string nombreUsuario, Cliente cliente, string nombreCliente, Receta receta, MedioPago medioPago, DateTime fechaPago, Montura montura, Despacho despacho, Cristal cristal, Cristal cristal2, int monto, int abono, int saldo, int cant, string observacion, string direccion, string comuna, string estadoVenta, string entregado)
        {
            this.IdVenta = idVenta;
            this.Usuario = usuario;
            this.NombreUsuario = nombreUsuario;
            this.Cliente = cliente;
            this.NombreCliente = nombreCliente;
            this.Receta = receta;
            this.MedioPago = medioPago;
            this.FechaPago = fechaPago;
            this.Montura = montura;
            this.Despacho = despacho;
            this.Cristal = cristal;
            this.Cristal2 = cristal2;
            this.Monto = monto;
            this.Abono = abono;
            this.Saldo = saldo;
            this.Cant = cant;
            this.Observacion = observacion;
            this.Direccion = direccion;
            this.Comuna = comuna;
            this.EstadoVenta = estadoVenta;
            this.Entregado = entregado;
        }

        public int IdVenta { get => idVenta; set => idVenta = value; }
        public Usuario Usuario { get => usuario; set => usuario = value; }
        public string NombreUsuario { get => nombreUsuario; set => nombreUsuario = value; }
        public Cliente Cliente { get => cliente; set => cliente = value; }
        public string NombreCliente { get => nombreCliente; set => nombreCliente = value; }
        public Receta Receta { get => receta; set => receta = value; }
        public MedioPago MedioPago { get => medioPago; set => medioPago = value; }
        public DateTime FechaPago { get => fechaPago; set => fechaPago = value; }
        public Montura Montura { get => montura; set => montura = value; }
        public Despacho Despacho { get => despacho; set => despacho = value; }
        public Cristal Cristal { get => cristal; set => cristal = value; }
        public Cristal Cristal2 { get => cristal2; set => cristal2 = value; }
        public int Monto { get => monto; set => monto = value; }
        public int Abono { get => abono; set => abono = value; }
        public int Saldo { get => saldo; set => saldo = value; }
        public int Cant { get => cant; set => cant = value; }
        public string Observacion { get => observacion; set => observacion = value; }
        public string Direccion { get => direccion; set => direccion = value; }
        public string Comuna { get => comuna; set => comuna = value; }
        public string EstadoVenta { get => estadoVenta; set => estadoVenta = value; }
        public string Entregado { get => entregado; set => entregado = value; }

        public override string ToString()
        {
            return " " + IdVenta; 
        }
    }
}
