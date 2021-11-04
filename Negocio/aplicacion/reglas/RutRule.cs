using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.application.rule
{
    public class RutRule
    {
        public void ValidarRut(string rut)
        {
            //Se lanza excepción siempre que el rut no sea válido.
            if (!VerificarRut(rut))
            {
                throw new Exception("RUT INVALIDO.\nFORMATO: XXXXXXXX-Y");
            }

        }

        public bool VerificarRut(string rut)
        {

            bool validacion = false;
            try
            {
                //Transformamos texto en mayúscula
                rut = rut.ToUpper();
                //Quitamos los puntos del texto
                rut = rut.Replace(".", "");
                //Quitamos los guiones del rut
                rut = rut.Replace("-", "");
                //Dividmos el texto y transformamos la parte númerica 
                //en una variable númerica
                int rutAux = int.Parse(rut.Substring(0, rut.Length - 1));
                //Dividimos el texto y obtenemos el digito verificador
                char dv = char.Parse(rut.Substring(rut.Length - 1, 1));
                //Calculo del digito verificador a partir de la 
                //parte númerica del rut
                int m = 0, s = 1;
                for (; rutAux != 0; rutAux /= 10)
                {
                    s = (s + rutAux % 10 * (9 - m++ % 6)) % 11;
                }
                //Verificar digito calculado debe ser igual a 
                //digito ingresado en el texto de ser así
                //Se devuelve verdadero
                if (dv == (char)(s != 0 ? s + 47 : 75))
                {
                    validacion = true;
                }
            }
            catch (Exception)
            {
            }
            return validacion;
        }
    }
}