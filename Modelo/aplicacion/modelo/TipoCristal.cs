using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.aplicacion.modelo
{
    public class TipoCristal
    {
        private int idTipoCristal;
        private string nombre;

        public TipoCristal()
        {

        }

        public TipoCristal(int idTipoCristal, string nombre)
        {
            this.IdTipoCristal = idTipoCristal;
            this.Nombre = nombre;
        }

        public int IdTipoCristal { get => idTipoCristal; set => idTipoCristal = value; }
        public string Nombre { get => nombre; set => nombre = value; }

        public override string ToString()
        {
            return nombre;
        }
    }
}
