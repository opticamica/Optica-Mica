using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.aplicacion.modelo
{
    public class Cliente
    {
        private string rut;
        private string nombre;
        private string apellidoP;
        private string telefono;
        private string prevision;
        private string direccion;
        private string comuna;
        private string correo;

        public Cliente()
        {
                
        }

        public Cliente(string rut, string nombre, string apellidoP, string telefono, string prevision, string direccion, string comuna, string correo)
        {
            this.Rut = rut;
            this.Nombre = nombre;
            this.ApellidoP = apellidoP;
            this.Telefono = telefono;
            this.Prevision = prevision;
            this.Direccion = direccion;
            this.Comuna = comuna;
            this.Correo = correo;
        }

        public string Rut { get => rut; set => rut = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string ApellidoP { get => apellidoP; set => apellidoP = value; }
        public string Telefono { get => telefono; set => telefono = value; }
        public string Prevision { get => prevision; set => prevision = value; }
        public string Direccion { get => direccion; set => direccion = value; }
        public string Comuna { get => comuna; set => comuna = value; }
        public string Correo { get => correo; set => correo = value; }

        public override string ToString()
        {
            return Rut;
        }
    }
}
