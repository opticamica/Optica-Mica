using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.aplicacion.modelo
{
    public class TipoReceta
    {
        private int idTipoReceta;
        private string nombre;

        public TipoReceta()
        {

        }

        public TipoReceta(int idTipoReceta, string nombre)
        {
            this.IdTipoReceta = idTipoReceta;
            this.Nombre = nombre;
        }

        public int IdTipoReceta { get => idTipoReceta; set => idTipoReceta = value; }
        public string Nombre { get => nombre; set => nombre = value; }

        public override string ToString()
        {
            return nombre;
        }
    }
}
