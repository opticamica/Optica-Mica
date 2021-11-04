using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.aplicacion.modelo
{
    public class Receta
    {
        private int idReceta;
        private TipoReceta tipoReceta;
        private Usuario usuario;
        private Cliente cliente;
        private int edad;
        private string observacion;
        private string esferaOD;
        private string cilindroOD;
        private string gradoOD;
        private string esferaOI;
        private string cilindroOI;
        private string gradoOI;
        private int? dpLejos;
        private int? dpCerca;
        private string adiccion;
        private DateTime fecha;

        public Receta()
        {

        }

        public Receta(int idReceta, TipoReceta tipoReceta, Usuario usuario, Cliente cliente, int edad, string observacion, string esferaOD, string cilindroOD, string gradoOD, string esferaOI, string cilindroOI, string gradoOI, int? dpLejos, int? dpCerca, string adiccion, DateTime fecha)
        {
            this.IdReceta = idReceta;
            this.TipoReceta = tipoReceta;
            this.Usuario = usuario;
            this.Cliente = cliente;
            this.Edad = edad;
            this.Observacion = observacion;
            this.EsferaOD = esferaOD;
            this.CilindroOD = cilindroOD;
            this.GradoOD = gradoOD;
            this.EsferaOI = esferaOI;
            this.CilindroOI = cilindroOI;
            this.GradoOI = gradoOI;
            this.DpLejos = dpLejos;
            this.DpCerca = dpCerca;
            this.Adiccion = adiccion;
            this.Fecha = fecha;
        }

        public int IdReceta { get => idReceta; set => idReceta = value; }
        public TipoReceta TipoReceta { get => tipoReceta; set => tipoReceta = value; }
        public Usuario Usuario { get => usuario; set => usuario = value; }
        public Cliente Cliente { get => cliente; set => cliente = value; }
        public int Edad { get => edad; set => edad = value; }
        public string Observacion { get => observacion; set => observacion = value; }
        public string EsferaOD { get => esferaOD; set => esferaOD = value; }
        public string CilindroOD { get => cilindroOD; set => cilindroOD = value; }
        public string GradoOD { get => gradoOD; set => gradoOD = value; }
        public string EsferaOI { get => esferaOI; set => esferaOI = value; }
        public string CilindroOI { get => cilindroOI; set => cilindroOI = value; }
        public string GradoOI { get => gradoOI; set => gradoOI = value; }
        public int? DpLejos { get => dpLejos; set => dpLejos = value; }
        public int? DpCerca { get => dpCerca; set => dpCerca = value; }
        public string Adiccion { get => adiccion; set => adiccion = value; }
        public DateTime Fecha { get => fecha; set => fecha = value; }

        public override string ToString()
        {
            return " " + IdReceta;
        }
    }
}
