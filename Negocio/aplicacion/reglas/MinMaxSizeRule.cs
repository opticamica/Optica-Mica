using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.application.rule
{
    public class MinMaxSizeRule
    {

        public string MinMaxSizeMessage(string value, string name, int min, int max)
        {

            try
            {
                MinMaxSize(value, name, min, max);

                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public void MinMaxSize(string value, string name, int min, int max)
        {

            if (value == null || value.Count() < min || value.Count() > max)
            {

                throw new Exception(
                    "\nEL CAMPO " + name + " DEBE POSEER MÍN. " + min +
                    "\nY MÁX. " + max + " CARACTERES."
                    );
            }
        }
    }
}