using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.aplicacion.modelo
{
    public class Sucursal
    {
        private int idSucursal;
        private string nombre;
        private string direccion;
        private string descripcion;

        public Sucursal()
        {

        }

        public Sucursal(int idSucursal, string nombre, string direccion, string descripcion)
        {
            this.idSucursal = idSucursal;
            this.nombre = nombre;
            this.direccion = direccion;
            this.descripcion = descripcion;
        }

        public int IdSucursal { get => idSucursal; set => idSucursal = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string Direccion { get => direccion; set => direccion = value; }
        public string Descripcion { get => descripcion; set => descripcion = value; }

        public override string ToString()
        {
            return nombre;
        }
    }
}
