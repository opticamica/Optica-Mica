using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.aplicacion.modelo
{
    public class TipoUsuario
    {
        private int idTipo;
        private string nombre;

        public TipoUsuario()
        {

        }

        public TipoUsuario(int idTipo, string nombre)
        {
            this.IdTipo = idTipo;
            this.Nombre = nombre;
        }

        public int IdTipo { get => idTipo; set => idTipo = value; }
        public string Nombre { get => nombre; set => nombre = value; }

        public override string ToString()
        {
            return nombre;
        }

    }
}
