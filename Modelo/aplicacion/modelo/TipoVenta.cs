using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.aplicacion.modelo
{
    public class TipoVenta
    {
        private int idTipoVenta;
        private string nombre;

        public TipoVenta()
        {

        }

        public TipoVenta(int idTipoVenta, string nombre)
        {
            this.IdTipoVenta = idTipoVenta;
            this.Nombre = nombre;
        }

        public int IdTipoVenta { get => idTipoVenta; set => idTipoVenta = value; }
        public string Nombre { get => nombre; set => nombre = value; }

        public override string ToString()
        {
            return nombre;
        }
    }
}
