using Modelo.aplicacion.modelo;
using Negocio.application.rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.aplicacion.negocio
{
    public class MantenedorCristalBS
    {
        EmptyRule empR = new EmptyRule();

        public void Validacion(Cristal cristal)
        {
            empR.ValidarVacio(cristal.Material, "MATERIAL");
            empR.ValidarVacio(cristal.TipoLente, "TIPO CRISTAL");
            empR.ValidarVacio(cristal.Esfera, "ESFERA");
            empR.ValidarVacio(cristal.Cilindro, "CILINDRO");
            empR.ValidarVacio(cristal.Stock, "CANTIDAD");
            empR.ValidarVacio(cristal.Proveedor, "PROVEEDOR");

            if (cristal.Esfera == "+" || cristal.Esfera == "-")
            {
                throw new Exception(
                    "\nDATO NO VALIDO EN ESFERA");
            }

            if (cristal.Cilindro == "-")
            {
                throw new Exception(
                    "\nDATO NO VALIDO EN CILINDRO");
            }
            string adicion = "";
            if (cristal.Esfera.Contains(' '))
            {
                string esfera;
                string c = cristal.Esfera.Trim();
                string[] valor = cristal.Esfera.Split(' ');
                esfera = valor[0];
                adicion = valor[2];
                ValidacionPrecioNoMonofocal(cristal);
                cristal.Esfera = esfera;
                
            }
            
            if (cristal.TipoLente.Nombre == "Monofocal")
            {
                //throw new Exception("cristal " + cristal.Esfera);
                if (Convert.ToDecimal(cristal.Esfera) > 0 && cristal.Esfera.Substring(0, 1) != "+")
                {
                    throw new Exception(
                        "\nDEBE AGREGAR EL SIMBOLO + EN ESFERA");
                }
                decimal b = Convert.ToDecimal(cristal.Esfera);
                if (!(b <= 30 && b >= -30))
                {
                    throw new Exception(
                         "\nLA ESFERA DEBE TENER UN VALOR ENTRE -30 \nY 30 ");
                }
                decimal a = Convert.ToDecimal(cristal.Cilindro);
                if (!(a <= 0 && a >= -10))
                {
                    throw new Exception(
                       "\nEL CILINDRO DEBE TENER UN VALOR ENTRE -10 \nY 0 ");
                }
                ValidacionPrecio(cristal);
                //cristal.Esfera = cristal.Esfera + " add " + adicion;
                //throw new Exception("cristal " + cristal.Esfera);
            }
            else
            {
                cristal.Esfera = cristal.Esfera + " add " + adicion;
                //throw new Exception("cristal "+cristal.Esfera);
            }
        }

        private void ValidacionPrecioNoMonofocal(Cristal cristal)
        {
            // VALIDAR PRECIO BIFOCALES
            
            decimal add = 0, esfera = 0;
            string c = cristal.Esfera.Trim();
            // throw new Exception(
            //"\nPARA DIOPTRIAS BAJO ESF +/-6 CON CIL -4 \nDEBE USAR MINERAL");

            if (c.Contains("add"))
            {
                string[] valor = cristal.Esfera.Split(' ');
                int precioBase = 8900;
                esfera = Convert.ToDecimal(valor[0]);
                add = Convert.ToDecimal(valor[2]);
                if ((Convert.ToDouble(esfera) >= 0 && Convert.ToDecimal(esfera) <= 3) || (Convert.ToDouble(esfera) < 0 && Convert.ToDecimal(esfera) >= -3) && Convert.ToDouble(add) >= 0.75 && Convert.ToDecimal(add) <= 3)
                {
                    if (cristal.Cilindro == "0")
                    {
                        if (cristal.TipoLente.Nombre == "Bifocal")
                        {
                            cristal.Precio = 22500 + precioBase + "";
                            //throw new Exception("" + cristal.Precio);
                        }
                        if (cristal.TipoLente.Nombre == "Bifocal Invisible")
                        {
                            cristal.Precio = 30000 + precioBase + "";
                            //throw new Exception("" + cristal.Precio);
                        }
                        if (cristal.TipoLente.Nombre == "Multifocal")
                        {
                            cristal.Precio = 49500 + precioBase + "";
                            //throw new Exception("" + cristal.Precio);
                        }
                    }
                    if (Convert.ToDecimal(cristal.Cilindro) < 0 && Convert.ToDecimal(cristal.Cilindro) >= -4)
                    {
                        if (cristal.TipoLente.Nombre == "Bifocal")
                        {
                            cristal.Precio = 37500 + precioBase + "";
                            //throw new Exception("" + cristal.Precio);
                        }
                        if (cristal.TipoLente.Nombre == "Bifocal Invisible")
                        {
                            cristal.Precio = 47500 + precioBase + "";
                            //throw new Exception("" + cristal.Precio);
                        }
                        if (cristal.TipoLente.Nombre == "Multifocal")
                        {
                            cristal.Precio = 57500 + precioBase + "";
                            //throw new Exception("" + cristal.Precio);
                        }
                    }
                    if (Convert.ToDecimal(cristal.Cilindro) < -4 && Convert.ToDecimal(cristal.Cilindro) >= -6)
                    {
                        if (cristal.TipoLente.Nombre == "Bifocal")
                        {
                            cristal.Precio = 50000 + precioBase + "";
                            //throw new Exception("" + cristal.Precio);
                        }
                        if (cristal.TipoLente.Nombre == "Bifocal Invisible")
                        {
                            cristal.Precio = 60000 + precioBase + "";
                            //throw new Exception("" + cristal.Precio);
                        }
                        if (cristal.TipoLente.Nombre == "Multifocal")
                        {
                            cristal.Precio = 72500 + precioBase + "";
                            //throw new Exception("" + cristal.Precio);
                        }
                    }

                    if (cristal.Material.Nombre == "Orgánico 1.56" || cristal.Material.Nombre == "Mineral")
                    {
                        cristal.Precio = cristal.Precio;
                    }
                    if (cristal.Material.Nombre == "Orgánico 1.61")
                    {
                        cristal.Precio = (Convert.ToInt32(cristal.Precio) + 25000)+"";
                    }
                    if (cristal.Material.Nombre == "Orgánico 1.67")
                    {
                        cristal.Precio = (Convert.ToInt32(cristal.Precio) + 55000) + "";
                    }
                    if (cristal.FiltroAzul == "Si")
                    {
                        cristal.Precio = (Convert.ToInt32(cristal.Precio) + 25000) + "";
                    }
                    if (cristal.Fotocromatico != "No")
                    {
                        cristal.Precio = (Convert.ToInt32(cristal.Precio) + 30000) + "";
                    }
                }
            }
  
        }

        public void ValidacionPrecio(Cristal cristal)
        {
        //aca cambie el precio base segun se quiera
        int precioBase = 8900, precioEsfera = 0, precioCilindro = 0,precioFoto=0,precioBlue=0, precioSinFiltro=0;
        // VALIDAR PRECIO ESFERAS MONOFOCALES
        if (cristal.TipoLente.Nombre == "Monofocal" && (cristal.Material.Nombre == "Orgánico 1.56" || cristal.Material.Nombre == "Orgánico 1.61" || cristal.Material.Nombre == "Mineral" || cristal.Material.Nombre == "Policarbonato"))
        {
            if ((Convert.ToDecimal(cristal.Esfera) >= 0 && Convert.ToDecimal(cristal.Esfera) <= 2) || (Convert.ToDecimal(cristal.Esfera) < 0 && Convert.ToDecimal(cristal.Esfera) >= -2))
            {
                if (cristal.Fotocromatico == "No" && cristal.FiltroAzul =="No")
                {
                    precioSinFiltro = 0;
                }
                if (cristal.Fotocromatico != "No")
                {
                    precioFoto = 7500;
                }
                if (cristal.FiltroAzul == "Si")
                {
                    precioBlue = 7500;
                }

                if (cristal.Material.Nombre == "Policarbonato")
                    {
                        precioEsfera = precioFoto + precioBlue + precioBase + 7500;
                        
                }
                    else
                    {
                        precioEsfera = precioFoto + precioBlue + precioSinFiltro + precioBase;
                        //throw new Exception("hola" + cristal.Precio);
                    }
                    
                //MessageBox.Show("precio" + precioEsfera);
            }
            if ((Convert.ToDecimal(cristal.Esfera) > 2 && Convert.ToDecimal(cristal.Esfera) <= 4) || (Convert.ToDecimal(cristal.Esfera) < -2 && Convert.ToDecimal(cristal.Esfera) >= -4))
            {
                    if (cristal.Fotocromatico == "No" && cristal.FiltroAzul == "No")
                    {
                        precioSinFiltro = 2500;
                    }
                    if (cristal.Fotocromatico != "No")
                    {
                        precioFoto = 7500;
                    }
                    if (cristal.FiltroAzul == "Si")
                    {
                        precioBlue = 7500;
                    }
                    
                    if (cristal.Material.Nombre == "Policarbonato")
                    {
                        precioEsfera = precioFoto + precioBlue + precioBase + 7500;
                        
                    }
                    else
                    {
                        precioEsfera = precioFoto + precioBlue + precioSinFiltro + precioBase;
                    }
                    //MessageBox.Show("precio" + precioEsfera);
                }
            if ((Convert.ToDecimal(cristal.Esfera) > 4 && Convert.ToDecimal(cristal.Esfera) <= 6) || (Convert.ToDecimal(cristal.Esfera) < -4 && Convert.ToDecimal(cristal.Esfera) >= -6))
            {
                    if (cristal.Fotocromatico == "No" && cristal.FiltroAzul == "No")
                    {
                        precioSinFiltro = 5000;
                    }
                    if (cristal.Fotocromatico != "No")
                    {
                        precioFoto = 10000;
                    }
                    if (cristal.FiltroAzul == "Si")
                    {
                        precioBlue = 10000;
                    }
                    precioEsfera = precioFoto + precioBlue + precioSinFiltro + precioBase;
                    if (cristal.Material.Nombre == "Policarbonato")
                    {
                        precioEsfera = precioFoto + precioBlue + precioBase + 10000;
                    }
                }
            if ((Convert.ToDecimal(cristal.Esfera) > 6 && Convert.ToDecimal(cristal.Esfera) <= 8) || (Convert.ToDecimal(cristal.Esfera) < -6 && Convert.ToDecimal(cristal.Esfera) >= -8))
            {
                    if (cristal.Fotocromatico == "No" && cristal.FiltroAzul == "No")
                    {
                        precioSinFiltro = 10000;
                    }
                    if (cristal.Fotocromatico != "No")
                    {
                        cristal.Precio = "CONSULTAR MIGUEL";
                        //throw new Exception("CONSULTAR");
                    }
                    if (cristal.FiltroAzul == "Si")
                    {
                        cristal.Precio = "CONSULTAR MIGUEL";
                        //throw new Exception("CONSULTAR");
                    }
                    
                        precioEsfera = precioFoto + precioBlue + precioSinFiltro + precioBase;
                    
                    
                }
            if ((Convert.ToDecimal(cristal.Esfera) > 8 && Convert.ToDecimal(cristal.Esfera) <= 30) || (Convert.ToDecimal(cristal.Esfera) < -8 && Convert.ToDecimal(cristal.Esfera) >= -30))
            {    
                throw new Exception("SELECCIONAR ORGANICO 1.67");
            }

 // PRECIO CILINDRO
            if (Convert.ToDecimal(cristal.Cilindro) <= 0 && Convert.ToDecimal(cristal.Cilindro) >= -2)
            {
                    if (cristal.Material.Nombre != "Policarbonato")
                    {
                        if (cristal.Fotocromatico == "No" && cristal.FiltroAzul == "No")
                        {
                            precioCilindro = precioEsfera;
                            cristal.Precio = precioCilindro.ToString();
                            //throw new Exception("hola si" + cristal.Precio);
                        }

                        else
                        {
                            precioCilindro = precioEsfera + validarPrecioFiltro(cristal, precioCilindro, 7500, 7500, 7500);
                            cristal.Precio = precioCilindro.ToString();
                            //throw new Exception("hola sino" + precioEsfera);
                        }
                    }
                    else
                    {
                        precioCilindro = precioEsfera + validarPrecioFiltro(cristal, precioCilindro, 7500, 7500, 7500);
                        cristal.Precio = precioCilindro.ToString();
                        //throw new Exception("hola hola" + cristal.Precio);
                    }


                /*if (cristal.Material.Nombre == "Policarbonato")
                {
                        if (cristal.Fotocromatico == "No" && cristal.FiltroAzul == "No")
                        {
                            precioCilindro = precioEsfera + 7500;
                            cristal.Precio = precioCilindro.ToString();
                            //throw new Exception("hola" + cristal.Precio);
                        }
                        else
                        {
                            precioCilindro = 7500 + precioEsfera + validarPrecioFiltro(cristal, precioCilindro, 7500, 7500);
                            cristal.Precio = precioCilindro.ToString();
                        }
                    }*/


                }
            if (Convert.ToDecimal(cristal.Cilindro) < -2 && Convert.ToDecimal(cristal.Cilindro) >= -4)
            {

                    if (cristal.Material.Nombre != "Policarbonato")
                    {
                        if (cristal.Fotocromatico == "No" && cristal.Blanco == "No" && cristal.FiltroAzul == "No")
                        {

                            precioCilindro = precioEsfera + 5000;
                            cristal.Precio = precioCilindro.ToString();
                        }
                        else
                        {
                            precioCilindro = precioEsfera + validarPrecioFiltro(cristal, precioCilindro, 12500, 12500, 12500);
                            cristal.Precio = precioCilindro.ToString();
                        }
                    }
                    else
                    {
                        precioCilindro = precioEsfera + validarPrecioFiltro(cristal, precioCilindro, 12500, 12500, 12500);
                        cristal.Precio = precioCilindro.ToString();   
                    }   
                }
            if (Convert.ToDecimal(cristal.Cilindro) < -4 && Convert.ToDecimal(cristal.Cilindro) >= -6)
            {
                    if (cristal.Material.Nombre != "Policarbonato")
                    {
                        if (cristal.Fotocromatico == "No" && cristal.FiltroAzul == "No")
                        {
                            precioCilindro = precioEsfera + 10000;
                            cristal.Precio = precioCilindro.ToString();
                        }
                        else
                        {
                            precioCilindro = precioEsfera + validarPrecioFiltro(cristal, precioCilindro, 30000, 30000, 30000);
                            cristal.Precio = precioCilindro.ToString();
                        }
                    }
                    else
                    {
                        precioCilindro = precioEsfera + validarPrecioFiltro(cristal, precioCilindro, 30000, 30000, 30000);
                        cristal.Precio = precioCilindro.ToString();
                    }
                        
            }
            if (Convert.ToDecimal(cristal.Cilindro) < -6)
            {

                if (cristal.Fotocromatico == "No" && cristal.Material.Nombre != "Policarbonato" && cristal.FiltroAzul == "No")
                {
                    precioCilindro = precioEsfera + 65000;
                    cristal.Precio = precioCilindro.ToString();
                }
                else
                {
                    throw new Exception("SELECCIONAR ORGANICO 1.67");
                }
            }
                
                // excepcion cuando es fotocromatico o blue de 0 a 2 esf y 0 a 2 cil
                if (((Convert.ToDecimal(cristal.Esfera) >= 0 && Convert.ToDecimal(cristal.Esfera) <= 2) || (Convert.ToDecimal(cristal.Esfera) < 0 && Convert.ToDecimal(cristal.Esfera) >= -2)) && Convert.ToDecimal(cristal.Cilindro) <= 0 && Convert.ToDecimal(cristal.Cilindro) >= -2)
                {
                    if (cristal.Fotocromatico != "No")
                    {
                        precioFoto = 7500;
                    }
                    else
                    {
                        precioFoto = 0;
                    }
                    if (cristal.FiltroAzul == "Si")
                    {
                        precioBlue = 7500;
                    }
                    else
                    {
                        precioBlue = 0;
                    }
                    if (cristal.Material.Nombre == "Policarbonato")
                    {
                        precioEsfera = precioFoto + precioBlue + precioBase + 7500;

                    }
                    else
                    {
                        precioEsfera = precioFoto + precioBlue + precioSinFiltro + precioBase;
                        //throw new Exception("hola" + cristal.Precio);
                    }
                    //precioEsfera = precioFoto + precioBlue + precioSinFiltro + precioBase;

                    cristal.Precio = precioEsfera.ToString();
                }
                
                //VALIDAR CONSULTAR MIGUEL (VAN AL ULTIMO PORQUE SINO NO TOMA EL VALOR "CONSULTAR MIGUEL")
                if ((Convert.ToDecimal(cristal.Esfera) > 6 && Convert.ToDecimal(cristal.Esfera) <= 8) || (Convert.ToDecimal(cristal.Esfera) < -6 && Convert.ToDecimal(cristal.Esfera) >= -8))
                {
                    if (cristal.Fotocromatico == "No" && cristal.FiltroAzul == "No")
                    {
                        precioSinFiltro = 10000;
                    }
                    if (cristal.Fotocromatico != "No")
                    {
                        cristal.Precio = "CONSULTAR MIGUEL";
                        //throw new Exception("CONSULTAR");
                    }
                    if (cristal.FiltroAzul == "Si")
                    {
                        cristal.Precio = "CONSULTAR MIGUEL";
                        //throw new Exception("CONSULTAR");
                    }
                    if (cristal.Material.Nombre == "Policarbonato")
                    {
                        cristal.Precio = "CONSULTAR MIGUEL";

                    }
                    precioEsfera = precioFoto + precioBlue + precioSinFiltro + precioBase;


                }

            }
            

//VALIDAR ORGANICO 167
            if (cristal.TipoLente.Nombre == "Monofocal" && (cristal.Material.Nombre == "Orgánico 1.67" || cristal.Material.Nombre == "Mineral High Light"))
            {
                if (Convert.ToDecimal(cristal.Esfera) > -4 && Convert.ToDecimal(cristal.Esfera) <= 4 && cristal.Material.Nombre != "Mineral High Light")
                {
                    throw new Exception("PARA DIOPTRIAS ENTRE -4 Y 4 DEBE SELECCIONAR UN MATERIAL MAS ECONOMICO");
                }
                if ((Convert.ToDecimal(cristal.Esfera) > 4 && Convert.ToDecimal(cristal.Esfera) <= 6) || (Convert.ToDecimal(cristal.Esfera) < -4 && Convert.ToDecimal(cristal.Esfera) >= -6))
                {
                    if (Convert.ToDecimal(cristal.Cilindro) < 0 && Convert.ToDecimal(cristal.Cilindro) >= -2)
                    {
                        precioEsfera = 12500 + precioBase;
                        cristal.Precio = "" + precioEsfera;
                    }
                    else
                    {
                        if ((Convert.ToDecimal(cristal.Cilindro) < -2 )&&( Convert.ToDecimal(cristal.Cilindro) >= -4))
                        {
                            precioEsfera = 22500 + precioBase;
                            cristal.Precio = "" + precioEsfera;
                        }
                        else
                        {
                            if ((Convert.ToDecimal(cristal.Cilindro) < -4 && Convert.ToDecimal(cristal.Cilindro) >= -10))
                            {
                                precioEsfera = 80000 + precioBase;
                                cristal.Precio = "" + precioEsfera;
                            }
                        }


                    }
                }
                if ((Convert.ToDecimal(cristal.Esfera) > 6 && Convert.ToDecimal(cristal.Esfera) <= 8) || (Convert.ToDecimal(cristal.Esfera) < -6 && Convert.ToDecimal(cristal.Esfera) >= -8))
                {
                    if ((Convert.ToDecimal(cristal.Cilindro) < 0 && Convert.ToDecimal(cristal.Cilindro) >= -2))
                    {
                        precioEsfera = 16000 + precioBase;
                        cristal.Precio = precioEsfera.ToString();
                        //MessageBox.Show("precio" + precioEsfera);
                    }
                    else
                    {
                        if ((Convert.ToDecimal(cristal.Cilindro) < -2 && Convert.ToDecimal(cristal.Cilindro) >= -4))
                        {
                            precioEsfera = 26000 + precioBase;
                            cristal.Precio = precioEsfera.ToString();
                            //MessageBox.Show("precio" + precioEsfera);
                        }
                        else
                        {
                            if ((Convert.ToDecimal(cristal.Cilindro) < -4 && Convert.ToDecimal(cristal.Cilindro) >= -10))
                            {
                                precioEsfera = 80000 + precioBase;
                                cristal.Precio = precioEsfera.ToString();
                                //MessageBox.Show("precio" + precioEsfera);
                            }
                        }
                    }

                }
                if ((Convert.ToDecimal(cristal.Esfera) > 8 && Convert.ToDecimal(cristal.Esfera) <= 10) || (Convert.ToDecimal(cristal.Esfera) < -8 && Convert.ToDecimal(cristal.Esfera) >= -10))
                {
                    if ((Convert.ToDecimal(cristal.Cilindro) < 0 && Convert.ToDecimal(cristal.Cilindro) >= -2))
                    {
                        precioEsfera = 20000 + precioBase;
                        cristal.Precio = precioEsfera.ToString();
                        //MessageBox.Show("precio" + precioEsfera);
                    }
                    else
                    {
                        if ((Convert.ToDecimal(cristal.Cilindro) < -2 && Convert.ToDecimal(cristal.Cilindro) >= -4))
                        {
                            precioEsfera = 32500 + precioBase;
                            cristal.Precio = precioEsfera.ToString();
                            //MessageBox.Show("precio" + precioEsfera);
                        }
                        else
                        {
                            if ((Convert.ToDecimal(cristal.Cilindro) < -4 && Convert.ToDecimal(cristal.Cilindro) >= -10))
                            {
                                precioEsfera = 80000 + precioBase;
                                cristal.Precio = precioEsfera.ToString();
                                //MessageBox.Show("precio" + precioEsfera);
                            }
                        }
                    }

                }
                if ((Convert.ToDecimal(cristal.Esfera) > 10 && Convert.ToDecimal(cristal.Esfera) <= 12) || (Convert.ToDecimal(cristal.Esfera) < -10 && Convert.ToDecimal(cristal.Esfera) >= -12))
                {
                    if (Convert.ToDecimal(cristal.Cilindro) < 0 && Convert.ToDecimal(cristal.Cilindro) >= -2)
                    {
                        precioEsfera = 25000 + precioBase;
                        cristal.Precio = precioEsfera.ToString();
                        //MessageBox.Show("precio" + precioEsfera);
                    }
                    else
                    {
                        if (Convert.ToDecimal(cristal.Cilindro) < -2 && Convert.ToDecimal(cristal.Cilindro) >= -4)
                        {
                            precioEsfera = 37000 + precioBase;
                            cristal.Precio = precioEsfera.ToString();
                            //MessageBox.Show("precio" + precioEsfera);
                        }
                        else
                        {
                            if (Convert.ToDecimal(cristal.Cilindro) < -4 && Convert.ToDecimal(cristal.Cilindro) >= -10)
                            {
                                precioEsfera = 80000 + precioBase;
                                cristal.Precio = precioEsfera.ToString();
                                //MessageBox.Show("precio" + precioEsfera);
                            }
                        }
                    }
                }
//VALIDAR CONSULTAR MIGUEL ORG 1.67
                if ((Convert.ToDecimal(cristal.Esfera) > 12 && Convert.ToDecimal(cristal.Esfera) <= 30) || (Convert.ToDecimal(cristal.Esfera) < -12 && Convert.ToDecimal(cristal.Esfera) >= -30))
                {
                    if (Convert.ToDecimal(cristal.Cilindro) < 0 && Convert.ToDecimal(cristal.Cilindro) >= -2)
                    {
                        precioEsfera = 80000 + precioBase;
                        cristal.Precio = precioEsfera.ToString();
                        //MessageBox.Show("precio" + precioEsfera);
                    }
                    else
                    {
                        cristal.Precio = "CONSULTAR MIGUEL";
                    }
                }

                //VALIDAR MINERAL HIGH LIGHT -10%
                if (cristal.TipoLente.Nombre == "Monofocal" && cristal.Material.Nombre == "Mineral High Light")
                {
                    cristal.Precio = "" + (Convert.ToInt32(cristal.Precio) * 0.90);
                }
            }
//VALIDAR ADVERTENCIA MINERALES ADELGAZADOS
            decimal b = Convert.ToDecimal(cristal.Esfera);
            decimal a = Convert.ToDecimal(cristal.Cilindro);

            if (cristal.TipoLente.Nombre == "Monofocal" && cristal.Material.Nombre == "Mineral")
            {

                if (Convert.ToDecimal(cristal.Esfera) >= 6 || Convert.ToDecimal(cristal.Esfera) <= -6 && Convert.ToDecimal(cristal.Cilindro) < -4)
                {
                    throw new Exception(
                    "\nPARA DIOPTRIAS SOBRE ESF +/-6 CON CIL -4 \nDEBE USAR MINERAL HIGH LIGHT");
                }

            }
            if (cristal.Fotocromatico != "No")
            {
                if (cristal.Fotocromatico == "Cafe" || cristal.Fotocromatico == "Gris")
                {
                    if (b > 6 || b < -6 && a < -6)
                    {
                        cristal.Precio = "CONSULTAR MIGUEL";
                    }

                    
                }
                if (cristal.Fotocromatico == "Azul" || cristal.Fotocromatico == "Purpura" || cristal.Fotocromatico == "Rosado")
                {
                    if (b > 4 || b < -4 && a < -4)
                    {
                        cristal.Precio = "CONSULTAR MIGUEL";
                    }
                }
            }
            if (cristal.TipoLente.Nombre == "Monofocal" && cristal.Material.Nombre == "Mineral High Light" && b <= 6 && b >= -6 && a >= -4)
            {
                throw new Exception(
               "\nPARA DIOPTRIAS BAJO ESF +/-6 CON CIL -4 \nDEBE USAR MINERAL");
            }
        }

        private int validarPrecioFiltro(Cristal cristal, int precioEsfera,int valorFoto,int valorAzul,int valorPoli)
        {
            int precioFoto=0,precioAzul=0 , precioPoli =0;
            if (cristal.Fotocromatico != "No")
            {
                precioFoto = valorFoto;
            }
            if (cristal.FiltroAzul == "Si")
            {
                precioAzul = valorAzul;
            }
            if (cristal.Material.Nombre == "Policarbonato")
            {
                precioPoli = valorPoli;
            }
            return precioFoto + precioAzul + precioPoli;
        }
        public string ValidarPlazo(Cristal cristal)
        {
            string plazo=" ";
            try
            {
                decimal a = Convert.ToDecimal(cristal.Esfera);
                decimal b = Convert.ToDecimal(cristal.Cilindro);
                decimal add = 0;
                if (cristal.Esfera.Contains(" "))
                {
                    string[] valor = cristal.Esfera.Split(' ');
                    add = Convert.ToDecimal(valor[2]);
                    throw new Exception("add: "+add);
                }
                //decimal add = 0;
                if (cristal.Material.Nombre == "Orgánico 1.61")
                {
                    plazo = "7 A 10 DIAS (ORG 1.61)";
                }
                if (cristal.Material.Nombre == "Orgánico 1.67" && cristal.Fotocromatico == "Si")
                {
                    plazo = "7 a 10 DIAS(ORG 1.67 + FOTO)";
                }
                if (cristal.TipoLente.Nombre == "Bifocal" || cristal.TipoLente.Nombre == "Bifocal Invisible" || cristal.TipoLente.Nombre == "Multifocal")
                {
                    if ((a >= -3 && a <= 3) && (add >= 0 && add <= 3))
                    {
                        if (b < 0)
                        {
                            plazo = "7 a 10 DIAS(" + cristal.TipoLente.Nombre.Substring(0, 4) + " CON CILINDRO)";
                        }
                        else
                        {
                            plazo = "3 DIAS (STOCK RAPIDO 0+/-3 + AD 0 A 3)";
                        }

                    }
                    else
                    {
                        plazo = "7 a 10 DIAS (SUPERIORI A 0 +- 3 Y ADD 0-3)";
                    }

                }
                if (cristal.TipoLente.Nombre == "Monofocal" && cristal.Material.Nombre == "Orgánico 1.67" && ((a >= -4 && a <= 4) && b < -2))
                {
                    plazo = "7 a 10 DIAS (MONO + ORG 1.67 A 0 +- 3 Y ADD 0-3)";
                }
                if (cristal.TipoLente.Nombre == "Monofocal" && cristal.Material.Nombre == "Orgánico 1.56")
                {
                    plazo = "3 DIAS (MONO + ORG 1.56)";
                }
                if (cristal.TipoLente.Nombre == "Monofocal" && cristal.Fotocromatico == "Si")
                {
                    if (a <= -4 && a >= 4)
                    {
                        plazo = "7 A 10 DIAS (MONO+FOTO > +-4 ESF )";
                    }
                    else
                    {
                        plazo = "3 DIAS (MONO+FOTO < +-4 ESF )";
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception("erro plazo :"+ex);
            }


            return plazo;
        }

        
    }

}
