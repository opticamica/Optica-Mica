using Modelo.aplicacion.modelo;
using Negocio.application.rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.aplicacion.negocio
{
    public class MantenedorUsuarioBS
    {
        public void Validacion(Usuario usuario)
        {
            //Construyendo objeto para validar regla de rut
            RutRule rutR = new RutRule();
            //Validar rut
            rutR.ValidarRut(usuario.Rut);

            //Construyendo objeto para validar regla de minimo y maximo
            MinMaxSizeRule min = new MinMaxSizeRule();
            //min.MinMaxSize(usuario.Password, "CONTRASEÑA", 8, 25);


            //Construyendo objeto para validar regla de vacio
            EmptyRule empR = new EmptyRule();
            empR.ValidarVacio(usuario.Rut, "RUT");
            empR.ValidarVacio(usuario.Nombre, "NOMBRE");
            //VALIDANDO EL LARGO DE NOMBRE
            min.MinMaxSize(usuario.Nombre, "NOMBRE", 3, 20);
            empR.ValidarVacio(usuario.Apellidop, "APELLIDO PATERNO");

            min.MinMaxSize(usuario.Apellidop, "APELLIDO", 3, 25);
            empR.ValidarVacio(usuario.User, "NOMBRE USUARIO");
            empR.ValidarVacio(usuario.Password, "CONTRASEÑA");
            //VALIDANDO EL LARGO DE CONTRASEÑA
            min.MinMaxSize(usuario.Password, "CONTRASEÑA", 8, 25);
            empR.ValidarVacio(usuario.Correo, "CORREO");
            empR.ValidarVacio(usuario.Sucursal, "SUCURSAL");
            empR.ValidarVacio(usuario.TipoUsuario, "TIPO USUARIO");
            //contruyendo objeto para validar regla de mail
            MailRule MailR = new MailRule();
            MailR.VerificarEmail(usuario.Correo);
            
        }
    }
}
