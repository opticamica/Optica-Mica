using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.aplicacion.modelo
{
    public class Proveedor
    {
        private int idProveedor;
        private string nombre;
        private string tipoProveedor;

        public Proveedor()
        {

        }

        public Proveedor(int idProveedor, string nombre, string tipoProveedor)
        {
            this.IdProveedor = idProveedor;
            this.Nombre = nombre;
            this.TipoProveedor = tipoProveedor;
        }

        public int IdProveedor { get => idProveedor; set => idProveedor = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string TipoProveedor { get => tipoProveedor; set => tipoProveedor = value; }

        public override string ToString()
        {
            return nombre;
        }


    }
}
