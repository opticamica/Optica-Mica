using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Negocio.application.rule
{
    class MailRule
    {
        public void VerificarEmail(string email)
        {
            //Se lanza excepción siempre que el EMAIL no sea válido.
            if (!ValidarEmail(email))
            {
                throw new Exception("EMAIL INVALIDO.\nFORMATO: XXXXXX@X.X");
            }

        }

        public Boolean ValidarEmail(String email)
        {
            String expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, String.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

    }
}