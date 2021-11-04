using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.aplicacion.modelo
{
    public class FormaMontura
    {
        private int idFormaMontura;
        private string nombre;
        public FormaMontura()
        {

        }

        public FormaMontura(int idFormaMontura, string nombre)
        {
            this.IdFormaMontura = idFormaMontura;
            this.Nombre = nombre;
        }

        public int IdFormaMontura { get => idFormaMontura; set => idFormaMontura = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public override string ToString()
        {
            return nombre;
        }
    }
}
