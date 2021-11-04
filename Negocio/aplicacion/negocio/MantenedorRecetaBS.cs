using Modelo.aplicacion.modelo;
using Negocio.application.rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.aplicacion.negocio
{
    public class MantenedorRecetaBS
    {
        EmptyRule empR = new EmptyRule();
        public void Validacion(Receta receta)
        {
            //Construyendo objeto para validar regla de minimo y maximo
            MinMaxSizeRule min = new MinMaxSizeRule();
            //Construyendo objeto para validar regla de vacio


            empR.ValidarVacio(receta.Edad, "EDAD"); 
            empR.ValidarVacio(receta.DpCerca, "Dp Cerca"); 
            empR.ValidarVacio(receta.DpLejos, "Dp Lejos");
            min.MinMaxSize(receta.Edad.ToString(), "EDAD", 0, 2);
            //empR.ValidarVacio(receta.GradoOD, "GRADO OD");
            //min.MinMaxSize(receta.GradoOD.ToString(), "GRADO OD", 0, 3);
            //min.MinMaxSize(receta.GradoOI.ToString(), "GRADO OI", 0, 3);

            //VALIDANDO LARGO DE NOMBRE

            /*
            min.MinMaxSize(receta.EsferaOD.ToString(), "ESFERA OD", 0, 3);
            empR.ValidarVacio(receta.EsferaOD, "ESFERA OD"); 

            min.MinMaxSize(receta.CilindroOD.ToString(), "CILINDRO OD", 0, 3);
            empR.ValidarVacio(receta.CilindroOD, "CILINDRO OD");

            min.MinMaxSize(receta.EsferaOI.ToString(), "ESFERA OI", 0, 3);
            empR.ValidarVacio(receta.EsferaOI, "ESFERA OI");

            min.MinMaxSize(receta.CilindroOI.ToString(), "CILINDRO OI", 0, 3);
            empR.ValidarVacio(receta.CilindroOI, "CILINDRO OI");

            
            min.MinMaxSize(receta.DpLejos.ToString(), "DP LEJOS", 0, 2);
            empR.ValidarVacio(receta.DpLejos, "DP LEJOS");
            min.MinMaxSize(receta.DpCerca.ToString(), "DP CERCA", 0, 3);
            empR.ValidarVacio(receta.DpCerca, "DP CERCA");
            min.MinMaxSize(receta.Adiccion.ToString(), "ADICION", 0, 3);
            empR.ValidarVacio(receta.Adiccion, "ADICION");
            */
            if (receta.EsferaOD == "+" || receta.EsferaOD == "-")
            {
                throw new Exception(
                    "\nDATO NO VALIDO EN ESFERA OD");
            }
            
            if (receta.CilindroOD == "+" || receta.CilindroOD == "-")
            {
                throw new Exception(
                    "\nDATO NO VALIDO EN CILINDRO OD");
            }
            if (receta.EsferaOI == "+" || receta.EsferaOI == "-")
            {
                throw new Exception(
                    "\nDATO NO VALIDO EN ESFERA OI");
            }
            if (receta.CilindroOI == "+" || receta.CilindroOI == "-")
            {
                throw new Exception(
                    "\nDATO NO VALIDO EN CILINDRO OI");
            }
            if (receta.Adiccion == "+" || receta.Adiccion == "-")
            {
                throw new Exception(
                    "\nDATO NO VALIDO EN ADICION");
            }

            if (Convert.ToDecimal(receta.EsferaOD) > 0 && receta.EsferaOD.Substring(0, 1) != "+")
            {
                throw new Exception(
                    "\nDEBE AGREGAR EL SIMBOLO + EN ESFERA OD");
            }
            
            if (Convert.ToDecimal(receta.EsferaOI) > 0 && receta.EsferaOI.Substring(0, 1) != "+")
            {
                throw new Exception(
                    "\nDEBE AGREGAR EL SIMBOLO + EN ESFERA OI");
            }
            if (Convert.ToDecimal(receta.Adiccion) > 0 && receta.Adiccion.Substring(0, 1) != "+")
            {
                throw new Exception(
                    "\nDEBE AGREGAR EL SIMBOLO + EN ADICION");
            }
            ValidacionEsfera(Convert.ToDecimal(receta.EsferaOD), "ESFERA OD");
            //ValidacionCilindro(Convert.ToInt32(receta.CilindroOD), "CILINDRO OD");
            ValidacionEsfera(Convert.ToDecimal(receta.EsferaOI), "ESFERA OI");
            
            ValidacionCilindroOD(Convert.ToDecimal(receta.CilindroOD), "CILINDRO OD");
            
            ValidacionCilindroOI(Convert.ToDecimal(receta.CilindroOI), "CILINDRO OI");

            ValidacionGrados(Convert.ToInt32(receta.GradoOD), "GRADO OD");
            ValidacionGrados(Convert.ToInt32(receta.GradoOI), "GRADO OI");
            if (receta.DpLejos.Value != 0)
            {
                ValidacionDp(Convert.ToInt32(receta.DpLejos), "DP LEJOS");
            }
            ValidacionAdicion(Convert.ToDecimal(receta.Adiccion), "ADICION");
            if (receta.DpCerca.Value != 0)
            {

                ValidacionDp(Convert.ToInt32(receta.DpCerca), "DP CERCA");
            }

        }

        


        private void ValidacionGrados(int value, string name)
        {
            if (!(value >= 0 && value <= 180))
            {

                throw new Exception(
                    "\nEL " + name + " DEBE TENER UN VALOR ENTRE 0 \nY 180 ");
            }
        }

        private void ValidacionDp(int value, string name)
        {
            if (!(value >= 40 && value <= 80))
            {

                throw new Exception(
                    "\nLA " + name + " DEBE TENER UN VALOR ENTRE 40 \nY 80 ");
            }
        }
        private void ValidacionAdicion(decimal value, string name)
        {
            if (!(value >= 0 && value <= 4))
            {

                throw new Exception(
                    "\nLA " + name + " DEBE TENER UN VALOR ENTRE 0 \nY 4 ");
            }
        }
        private void ValidacionEsfera(decimal value, string name)
        {
            if (!(value <= 30 && value >= -30))
            {
                throw new Exception(
                    "\nLA " + name + " DEBE TENER UN VALOR ENTRE -30 \nY 30 ");
            }


            /*
            if (value > 0)
            { 
                throw new Exception(
                   "\nDEBE AÑADIR EL SIGNO (+/-)");
                value = '+' + value;
            }
            */
        }
        /*
        private void ValidacionCilindro(int value, string name)
        {
            if (!(value <= 0 && value >= -10))
            {

                throw new Exception(
                    "\nLA " + name + " DEBE TENER UN VALOR ENTRE -10 \nY 0 ");
            }
        }
            */
        private void ValidacionCilindroOD(decimal valueOD, string nameOD)
        {
            if (!(valueOD <= 0 && valueOD >= -10))
            {
                throw new Exception(
                   "\nLA " + nameOD + " DEBE TENER UN VALOR ENTRE -10 \nY 0 ");
            }
        }
        private void ValidacionCilindroOI(decimal valueOI, string nameOI)
        {
            if (!(valueOI <= 0 && valueOI >= -10))
            {
             
                throw new Exception(
                    "\nLA " + nameOI + " DEBE TENER UN VALOR ENTRE -10 \nY 0 ");
            }
        }
    }
}
