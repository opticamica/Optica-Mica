using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.aplicacion.modelo
{
    public class Promocion
    {
        private string nombre;
        private int precio;

        public Promocion()
        {

        }

        public Promocion(string nombre, int precio)
        {
            this.Nombre = nombre;
            this.Precio = precio;
        }

        public string Nombre { get => nombre; set => nombre = value; }
        public int Precio { get => precio; set => precio = value; }
    }
}
