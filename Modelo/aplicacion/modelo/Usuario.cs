using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.aplicacion.modelo
{
    public class Usuario
    {
        private string rut;
        private string nombre;
        private string apellidop;
        private string apellidom;
        private string user;
        private string password;
        private string direccion;
        private string correo;
        private string telefono;
        private Sucursal sucursal;
        private TipoUsuario tipoUsuario;

        public Usuario()
        {

        }

        public Usuario(string rut, string nombre, string apellidop, string apellidom, string user, string password, string direccion, string correo, string telefono, Sucursal sucursal, TipoUsuario tipoUsuario)
        {
            this.Rut = rut;
            this.Nombre = nombre;
            this.Apellidop = apellidop;
            this.Apellidom = apellidom;
            this.User = user;
            this.Password = password;
            this.Direccion = direccion;
            this.Correo = correo;
            this.Telefono = telefono;
            this.Sucursal = sucursal;
            this.TipoUsuario = tipoUsuario;
        }

        public string Rut { get => rut; set => rut = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string Apellidop { get => apellidop; set => apellidop = value; }
        public string Apellidom { get => apellidom; set => apellidom = value; }
        public string User { get => user; set => user = value; }
        public string Password { get => password; set => password = value; }
        public string Direccion { get => direccion; set => direccion = value; }
        public string Correo { get => correo; set => correo = value; }
        public string Telefono { get => telefono; set => telefono = value; }
        public Sucursal Sucursal { get => sucursal; set => sucursal = value; }
        public TipoUsuario TipoUsuario { get => tipoUsuario; set => tipoUsuario = value; }

        public override string ToString()
        {
            return Rut;
        }
    }
}
