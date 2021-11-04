using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.aplicacion.modelo
{
    public class Montura
    {
        private int idMontura;
        private Sucursal sucursal;
        private string codMontura;
        private TipoMontura tipoMontura;
        private string color;
        private int stock;
        private Proveedor proveedor;
        private string imagen;

        public Montura()
        {

        }

        public Montura(int idMontura, Sucursal sucursal, string codMontura, TipoMontura tipoMontura, string color, int stock, Proveedor proveedor, string imagen)
        {
            this.IdMontura = idMontura;
            this.Sucursal = sucursal;
            this.CodMontura = codMontura;
            this.TipoMontura = tipoMontura;
            this.Color = color;
            this.Stock = stock;
            this.Proveedor = proveedor;
            this.Imagen = imagen;
        }

        public int IdMontura { get => idMontura; set => idMontura = value; }
        public Sucursal Sucursal { get => sucursal; set => sucursal = value; }
        public string CodMontura { get => codMontura; set => codMontura = value; }
        public TipoMontura TipoMontura { get => tipoMontura; set => tipoMontura = value; }
        public string Color { get => color; set => color = value; }
        public int Stock { get => stock; set => stock = value; }
        public Proveedor Proveedor { get => proveedor; set => proveedor = value; }
        public string Imagen { get => imagen; set => imagen = value; }

        public override string ToString()
        {
            return CodMontura;
        }
    }
}
