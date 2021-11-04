using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.aplicacion.modelo
{
    public class Reparacion
    {
        private int idReparacion;
        private string nombreUsuario;
        private Sucursal sucursal;
        private string descripcion;
        private MedioPago medioPago;
        private int montoPagado;

        public Reparacion()
        {

        }

        public Reparacion(int idReparacion, string nombreUsuario, Sucursal sucursal, string descripcion, MedioPago medioPago, int montoPagado)
        {
            this.IdReparacion = idReparacion;
            this.NombreUsuario = nombreUsuario;
            this.Sucursal = sucursal;
            this.Descripcion = descripcion;
            this.MedioPago = medioPago;
            this.MontoPagado = montoPagado;
        }

        public int IdReparacion { get => idReparacion; set => idReparacion = value; }
        public string NombreUsuario { get => nombreUsuario; set => nombreUsuario = value; }
        public Sucursal Sucursal { get => sucursal; set => sucursal = value; }
        public string Descripcion { get => descripcion; set => descripcion = value; }
        public MedioPago MedioPago { get => medioPago; set => medioPago = value; }
        public int MontoPagado { get => montoPagado; set => montoPagado = value; }

        public override string ToString()
        {
            return "" + IdReparacion;
        }
    }
}
