using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.aplicacion.modelo
{
    public class Colores
    {
        private int idColor;
        private string nombre;

        public Colores()
        {

        }

        public Colores(int idColor, string nombre)
        {
            this.IdColor = idColor;
            this.Nombre = nombre;
        }

        public int IdColor { get => idColor; set => idColor = value; }
        public string Nombre { get => nombre; set => nombre = value; }

        public override string ToString()
        {
            return nombre;
        }
    }
}
