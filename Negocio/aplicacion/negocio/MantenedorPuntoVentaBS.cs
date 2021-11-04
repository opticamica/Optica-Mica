using Modelo.aplicacion.modelo;
using Negocio.application.rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.aplicacion.negocio
{
    public class MantenedorPuntoVentaBS
    {
        public void Validacion(Venta venta)
        {
            //Construyendo objeto para validar regla de minimo y maximo
            MinMaxSizeRule min = new MinMaxSizeRule();
            //Construyendo objeto para validar regla de vacio
            EmptyRule empR = new EmptyRule();
            empR.ValidarVacio(venta.MedioPago, "MEDIO PAGO");
            empR.ValidarVacio(venta.Despacho, "DESPACHO");
            empR.ValidarVacio(venta.Abono, "ABONO");
            if (venta.Despacho.Nombre == "SI")
            {
                empR.ValidarVacio(venta.Direccion, "DIRECCION");
                empR.ValidarVacio(venta.Comuna, "COMUNA");
            }
        }
    }
}
