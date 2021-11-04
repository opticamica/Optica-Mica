using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.aplicacion.modelo
{
    public class MaterialCristal
    {
        private int idMaterialCristal;
        private string nombre;

        public MaterialCristal()
        {

        }

        public MaterialCristal(int idMaterialCristal, string nombre)
        {
            this.IdMaterialCristal = idMaterialCristal;
            this.Nombre = nombre;
        }

        public int IdMaterialCristal { get => idMaterialCristal; set => idMaterialCristal = value; }
        public string Nombre { get => nombre; set => nombre = value; }

        public override string ToString()
        {
            return nombre;
        }
    }
}
