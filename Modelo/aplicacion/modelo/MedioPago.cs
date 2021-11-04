using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.aplicacion.modelo
{
    public class MedioPago
    {
        private int idMedioPago;
        private string nombre;

        public MedioPago()
        {

        }

        public MedioPago(int idMedioPago, string nombre)
        {
            IdMedioPago = idMedioPago;
            this.Nombre = nombre;
        }

        public int IdMedioPago { get => idMedioPago; set => idMedioPago = value; }
        public string Nombre { get => nombre; set => nombre = value; }

        public override string ToString()
        {
            return nombre;
        }
    }
}
