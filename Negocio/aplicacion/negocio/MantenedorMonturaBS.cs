using Modelo.aplicacion.modelo;
using Negocio.application.rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.aplicacion.negocio
{
    public class MantenedorMonturaBS
    {
        EmptyRule empR = new EmptyRule();

        public void Validacion(Montura montura)
        {
            //Construyendo objeto para validar regla de minimo y maximo
            MinMaxSizeRule min = new MinMaxSizeRule();
            //Construyendo objeto para validar regla de vacio


            empR.ValidarVacio(montura.CodMontura, "CODIGO");
            empR.ValidarVacio(montura.Color, "COLOR");
            empR.ValidarVacio(montura.TipoMontura,"TIPO MONTURA");
            empR.ValidarVacio(montura.Stock, "CANTIDAD");
            empR.ValidarVacio(montura.Proveedor, "PROVEEDOR");

        }
    }
}
