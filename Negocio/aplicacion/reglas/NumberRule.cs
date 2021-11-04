using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.application.rule
{
    class NumberRule
    {
        public string ValidarNumeroMsg(object value, string name)
        {
            string errorMsg = "";

            try
            {
                ValidarNumero(value, name);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message + "\n";
            }

            return errorMsg;
        }
        public string ValidarNumeroMsg(string value, string name)
        {
            string errorMsg = "";

            try
            {
                ValidarNumero(value, name);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message + "\n";
            }

            return errorMsg;
        }
        public void ValidarNumero(object value, string name)
        {
            try
            {
                int aux = Convert.ToInt32(value);
            }
            catch (Exception)
            {

                throw new Exception("DEBE INGRESAR VALORES NUMÉRICOS: " +
                     name);
            }

            if (value == null)
            {
                throw new Exception("ESTE(OS) CAMPO(S) SÓLO ACEPTA(N) VALORES NUMÉRICOS: " +
                     name);
            }
        }





    }
}
