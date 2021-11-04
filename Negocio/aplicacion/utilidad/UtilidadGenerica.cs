using Modelo.aplicacion.modelo;
using Negocio.application.rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.aplicacion.utilidad
{
    public class UtilidadGenerica
    {
    /// <summary>
    /// Método encargado de validar textbox
    /// </summary>
    /// <param name="value"></param> valor del textbox, propiedad text
    /// <param name="name"></param> nombre del textbox
    /// <param name="min"></param> valor mínimo que acepta el textbox
    /// <param name="max"></param> valor máximo que acepta el textbox
    /// <returns></returns>
    public string ValidarTextBox(string value, string name, int min, int max)
    {

        EmptyRule emptyRule = new EmptyRule();
        string errorMessage = "";
        errorMessage = errorMessage + emptyRule.EmptyMessage(value, name);

        if (errorMessage == null || errorMessage.Count() == 0)
        {

            MinMaxSizeRule minMaxSizeRule = new MinMaxSizeRule();
            errorMessage = minMaxSizeRule.MinMaxSizeMessage(value, name, min, max);
        }

        return errorMessage;
    }

    public string ValidarCombobox(object value, string name)
    {
        string errorMessage = "";
        if (value == null)
        {

            errorMessage = "\nDEBE SELECCIONAR " + name;
        }

        return errorMessage;
    }
    public string ValidarCombobox(TipoReceta value, string name)
    {
        string errorMessage = "";
        if (value == null)
        {

            errorMessage = "\nDEBE SELECCIONAR " + name;
        }

        return errorMessage;
    }




        }

    }


