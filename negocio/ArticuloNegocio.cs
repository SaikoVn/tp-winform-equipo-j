using dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace negocio
{
    public class ArticuloNegocio
    {
        public List<Articulo> listar()
        {
            List<Articulo> listar = new List<Articulo>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("Select A.Id, A.Codigo, A.Nombre, A.Descripcion, A.Precio, A.IdMarca, M.Descripcion as Marca, A.IdCategoria, C.Descripcion as Categoria From ARTICULOS A Left Join MARCAS M on A.IdMarca = M.Id Left Join CATEGORIAS C on A.IdCategoria = C.Id");
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Codigo = (string)datos.Lector["Codigo"];

                    // Validación de nulo para la descripción del artículo
                    if (!(datos.Lector["Descripcion"] is DBNull))
                        aux.Descripcion = (string)datos.Lector["Descripcion"];

                    aux.Precio = (decimal)datos.Lector["Precio"];

                    // Validación para Marca
                    aux.Marca = new Marca();
                    if (!(datos.Lector["IdMarca"] is DBNull))
                        aux.Marca.Id = (int)datos.Lector["IdMarca"];

                    if (!(datos.Lector["Marca"] is DBNull))
                        aux.Marca.Descripcion = (string)datos.Lector["Marca"];
                    else
                        aux.Marca.Descripcion = "Sin marca";

                    // Validación para Categoría
                    aux.Categoria = new Categoria();
                    if (!(datos.Lector["IdCategoria"] is DBNull))
                        aux.Categoria.Id = (int)datos.Lector["IdCategoria"];

                    if (!(datos.Lector["Categoria"] is DBNull))
                        aux.Categoria.Descripcion = (string)datos.Lector["Categoria"];
                    else
                        aux.Categoria.Descripcion = "Sin categoría";

                    
                    listar.Add(aux);
                }

                return listar;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

                datos.cerrarConexion();

            }


        }
    }
}
