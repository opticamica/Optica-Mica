using Modelo.aplicacion.modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.application.rule
{
    public class EmptyRule
    {
        public void ValidarVacio(string value, string name)
        {

            if (value == null || value.Count() == 0)
            {
                throw new Exception("Debe ingresar información en " +
                    "el campo " + name);
            }
        }

        public void ValidarVacio(decimal value, string name)
        {

            if (value == 0)
            {
                throw new Exception("Debe ingresar información en " +
                    "el campo " + name);
            }
        }

        public void ValidarVacio(DateTime value, string name)
        {

            if (value == null)
            {
                throw new Exception("Debe ingresar información en " +
                    "el campo " + name);
            }
        }

        public void ValidarVacio(object value, string name)
        {

            if (value == null)
            {
                throw new Exception("DEBE SELECCIONAR UN ELEMENTO " +
                    "para el campo " + name);
            }
        }

        public void ValidarVacio(TipoReceta value, string name)
        {

            if (value == null)
            {
                throw new Exception("DEBE SELECCIONAR UN ELEMENTO " +
                    "para el campo " + name);
            }
        }

        public void ValidarVacio(int value, string name)
        {

            if (value == 0)
            {
                throw new Exception("INGRESE LA CANTIDAD DE " + name);
            }
        }

        public void ValidarVacio(double value, string name)
        {

            if (value == 0)
            {
                throw new Exception("PRESIONE EL BOTON: " + name);
            }
        }

        public string EmptyMessage(string value, string name)
        {

            try
            {
                IsEmpty(value, name);

                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public void IsEmpty(string value, string name)
        {

            if (value == null || value.Count() == 0 || value.Trim().Count() == 0)
            {

                throw new Exception(
                    "\nEL CAMPO " + name + " NO DEBE ESTAR VACÍO."
                    );
            }
        }

    }
}

