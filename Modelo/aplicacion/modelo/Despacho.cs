using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.aplicacion.modelo
{
    public class Despacho
    {
        private int idDespacho;
        private string nombre;

        public Despacho()
        {

        }

        public Despacho(int idDespacho, string nombre)
        {
            this.IdDespacho = idDespacho;
            this.Nombre = nombre;
        }

        public int IdDespacho { get => idDespacho; set => idDespacho = value; }
        public string Nombre { get => nombre; set => nombre = value; }

        public override string ToString()
        {
            return nombre;
        }
    }
}
