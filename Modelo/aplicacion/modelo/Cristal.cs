using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.aplicacion.modelo
{
    public class Cristal
    {
        private int idCristal;
        private string codCristal;
        private Sucursal sucursal;
        private MaterialCristal material;
        private string filtroAzul;
        private string fotocromatico;
        private string blanco;
        private string capa;
        private TipoLente tipoLente;
        private string esfera;
        private string cilindro;
        private int stock;
        private Proveedor proveedor;
        private string precio;

        public Cristal()
        {

        }

        public Cristal(int idCristal, string codCristal, Sucursal sucursal, MaterialCristal material, string filtroAzul, string fotocromatico, string blanco, string capa, TipoLente tipoLente, string esfera, string cilindro, int stock, Proveedor proveedor, string precio)
        {
            this.IdCristal = idCristal;
            this.CodCristal = codCristal;
            this.Sucursal = sucursal;
            this.Material = material;
            this.FiltroAzul = filtroAzul;
            this.Fotocromatico = fotocromatico;
            this.Blanco = blanco;
            this.Capa = capa;
            this.TipoLente = tipoLente;
            this.Esfera = esfera;
            this.Cilindro = cilindro;
            this.Stock = stock;
            this.Proveedor = proveedor;
            this.Precio = precio;
        }

        public int IdCristal { get => idCristal; set => idCristal = value; }
        public string CodCristal { get => codCristal; set => codCristal = value; }
        public Sucursal Sucursal { get => sucursal; set => sucursal = value; }
        public MaterialCristal Material { get => material; set => material = value; }
        public string FiltroAzul { get => filtroAzul; set => filtroAzul = value; }
        public string Fotocromatico { get => fotocromatico; set => fotocromatico = value; }
        public string Blanco { get => blanco; set => blanco = value; }
        public string Capa { get => capa; set => capa = value; }
        public TipoLente TipoLente { get => tipoLente; set => tipoLente = value; }
        public string Esfera { get => esfera; set => esfera = value; }
        public string Cilindro { get => cilindro; set => cilindro = value; }
        public int Stock { get => stock; set => stock = value; }
        public Proveedor Proveedor { get => proveedor; set => proveedor = value; }
        public string Precio { get => precio; set => precio = value; }

        public override string ToString()
        {
            return CodCristal;
        }
    }
}
