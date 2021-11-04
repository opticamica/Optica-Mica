using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.aplicacion.modelo
{
    public class TipoLente
    {
        private int idTipoLente;
        private string nombre;

        public TipoLente()
        {

        }

        public TipoLente(int idTipoLente, string nombre)
        {
            this.IdTipoLente = idTipoLente;
            this.Nombre = nombre;
        }

        public int IdTipoLente { get => idTipoLente; set => idTipoLente = value; }
        public string Nombre { get => nombre; set => nombre = value; }

        public override string ToString()
        {
            return nombre;
        }
    }
}
