using Modelo.aplicacion.modelo;
using Negocio.application.rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.aplicacion.negocio
{
    public class MantenedorClienteBS
    {
        public void Validacion(Cliente cliente)
        {
            //Construyendo objeto para validar regla de rut
            RutRule rutR = new RutRule();
            //Construyendo objeto para validar regla de minimo y maximo
            MinMaxSizeRule min = new MinMaxSizeRule();
            //contruyendo objeto para validar regla de mail
            MailRule MailR = new MailRule();
            //Construyendo objeto para validar regla de vacio
            EmptyRule empR = new EmptyRule();
            empR.ValidarVacio(cliente.Rut, "RUT");
            //Validar rut
            rutR.ValidarRut(cliente.Rut);
            empR.ValidarVacio(cliente.Nombre, "NOMBRE");
            //VALIDANDO LARGO DE NOMBRE
            min.MinMaxSize(cliente.Nombre, "NOMBRE", 3, 25);
            empR.ValidarVacio(cliente.ApellidoP, "APELLIDO PATERNO");
            //VALIDANDO LARGO DE APELLIDO
            min.MinMaxSize(cliente.ApellidoP, "APELLIDO", 3, 25);
            empR.ValidarVacio(cliente.Telefono, "TELEFONO");
            //VALIDANDO LARGO DE TELEFONO
            min.MinMaxSize(cliente.Telefono, "TELEFONO", 8, 12);
            empR.ValidarVacio(cliente.Prevision, "PREVISION");
            /*
            empR.ValidarVacio(cliente.Correo, "CORREO");
            MailR.VerificarEmail(cliente.Correo);
            empR.ValidarVacio(cliente.Comuna, "COMUNA");
            min.MinMaxSize(cliente.Comuna, "COMUNA", 3, 25);
            empR.ValidarVacio(cliente.Direccion, "DIRECCION");
            min.MinMaxSize(cliente.Direccion, "DIRECCION", 5, 25);
            */

        }
    }
}
