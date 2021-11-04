using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.aplicacion.modelo
{
    public class TipoMontura
    {
        private int idTipoMontura;
        private string nombre;
        private int precio;

        public TipoMontura()
        {

        }

        public TipoMontura(int idTipoMontura, string nombre, int precio)
        {
            this.IdTipoMontura = idTipoMontura;
            this.Nombre = nombre;
            this.Precio = precio;
        }

        public int IdTipoMontura { get => idTipoMontura; set => idTipoMontura = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public int Precio { get => precio; set => precio = value; }

        public override string ToString()
        {
            return Nombre;
        }
    }
}
