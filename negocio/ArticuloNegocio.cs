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
                datos.setearConsulta("Select A.Id, A.Codigo, A.Nombre, A.Descripcion, A.Precio, A.IdMarca, M.Descripcion as Marca, A.IdCategoria, C.Descripcion as Categoria, (Select Top 1 ImagenUrl From IMAGENES Where IdArticulo = A.Id) as ImagenUrl From ARTICULOS A Left Join MARCAS M on A.IdMarca = M.Id Left Join CATEGORIAS C on A.IdCategoria = C.Id");
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

                    if (!(datos.Lector["Precio"] is DBNull))
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

                    if (!(datos.Lector["ImagenUrl"] is DBNull))
                        aux.ImagenUrl = (string)datos.Lector["ImagenUrl"];

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
        public void agregar(Articulo nuevo)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("Insert into ARTICULOS (Codigo, Nombre, Descripcion, IdMarca, IdCategoria, Precio) values (@codigo, @nombre, @descripcion, @idMarca, @idCategoria, @precio); Insert into IMAGENES (IdArticulo, ImagenUrl) values (SCOPE_IDENTITY(), @imagenUrl);");

                datos.setearParametro("@codigo", nuevo.Codigo);
                datos.setearParametro("@nombre", nuevo.Nombre);
                datos.setearParametro("@descripcion", nuevo.Descripcion);
                datos.setearParametro("@idMarca", nuevo.Marca.Id);
                datos.setearParametro("@idCategoria", nuevo.Categoria.Id);
                datos.setearParametro("@precio", nuevo.Precio);
                datos.setearParametro("@imagenUrl", nuevo.ImagenUrl ?? (object)DBNull.Value);

                datos.ejecutarAccion();
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

        public void modificar(Articulo art)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("Update ARTICULOS set Codigo = @codigo, Nombre = @nombre, Descripcion = @descripcion, IdMarca = @idMarca, IdCategoria = @idCategoria, Precio = @precio Where Id = @id;");

                datos.setearParametro("@codigo", art.Codigo);
                datos.setearParametro("@nombre", art.Nombre);
                datos.setearParametro("@descripcion", art.Descripcion);
                
                datos.setearParametro("@idMarca", (art.Marca != null && art.Marca.Id != 0) ? (object)art.Marca.Id : DBNull.Value);
                datos.setearParametro("@idCategoria", (art.Categoria != null && art.Categoria.Id != 0) ? (object)art.Categoria.Id : DBNull.Value);
                datos.setearParametro("@precio", art.Precio);
                datos.setearParametro("@id", art.Id);

                datos.ejecutarAccion();
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

        public void eliminar(int id)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                // Borra primero las imágenes asociadas y luego el artículo
                datos.setearConsulta("Delete From IMAGENES Where IdArticulo = @id; Delete From ARTICULOS Where Id = @id;");
                datos.setearParametro("@id", id);
                datos.ejecutarAccion();
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

        public List<Articulo> filtrar(string campo, string criterio, string filtro)
        {
            List<Articulo> lista = new List<Articulo>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                string consulta = "Select A.Id, A.Codigo, A.Nombre, A.Descripcion, A.Precio, A.IdMarca, M.Descripcion as Marca, A.IdCategoria, C.Descripcion as Categoria, (Select Top 1 ImagenUrl From IMAGENES Where IdArticulo = A.Id) as ImagenUrl From ARTICULOS A Left Join MARCAS M on A.IdMarca = M.Id Left Join CATEGORIAS C on A.IdCategoria = C.Id Where ";

                if (campo == "Precio")
                {
                    switch (criterio)
                    {
                        case "Mayor a":
                            consulta += "A.Precio > @filtro";
                            break;
                        case "Menor a":
                            consulta += "A.Precio < @filtro";
                            break;
                        default:
                            consulta += "A.Precio = @filtro";
                            break;
                    }
                    decimal valorFiltro = 0;
                    decimal.TryParse(filtro.Trim().Replace(".", ","), out valorFiltro);
                    datos.setearParametro("@filtro", valorFiltro);
                }
                else
                {
                    string columna = "A.Nombre";
                    if (campo == "Código")
                        columna = "A.Codigo";
                    else if (campo == "Nombre")
                        columna = "A.Nombre";
                    else if (campo == "Marca")
                        columna = "M.Descripcion";
                    else if (campo == "Categoría")
                        columna = "C.Descripcion";

                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += columna + " like @filtro";
                            datos.setearParametro("@filtro", filtro + "%");
                            break;
                        case "Termina con":
                            consulta += columna + " like @filtro";
                            datos.setearParametro("@filtro", "%" + filtro);
                            break;
                        default:
                            consulta += columna + " like @filtro";
                            datos.setearParametro("@filtro", "%" + filtro + "%");
                            break;
                    }
                }

                datos.setearConsulta(consulta);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Codigo = (string)datos.Lector["Codigo"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];
                    if (!(datos.Lector["Precio"] is DBNull))
                        aux.Precio = (decimal)datos.Lector["Precio"];

                    if (!(datos.Lector["ImagenUrl"] is DBNull))
                        aux.ImagenUrl = (string)datos.Lector["ImagenUrl"];

                    aux.Marca = new Marca();
                    if (!(datos.Lector["IdMarca"] is DBNull))
                    {
                        aux.Marca.Id = (int)datos.Lector["IdMarca"];
                        aux.Marca.Descripcion = (string)datos.Lector["Marca"];
                    }

                    aux.Categoria = new Categoria();
                    if (!(datos.Lector["IdCategoria"] is DBNull))
                    {
                        aux.Categoria.Id = (int)datos.Lector["IdCategoria"];
                        aux.Categoria.Descripcion = (string)datos.Lector["Categoria"];
                    }

                    lista.Add(aux);
                }

                return lista;
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

        public int agregarConId(Articulo nuevo)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("Insert into ARTICULOS (Codigo, Nombre, Descripcion, IdMarca, IdCategoria, Precio) output inserted.Id values (@codigo, @nombre, @descripcion, @idMarca, @idCategoria, @precio)");

                datos.setearParametro("@codigo", nuevo.Codigo);
                datos.setearParametro("@nombre", nuevo.Nombre);
                datos.setearParametro("@descripcion", nuevo.Descripcion);
                datos.setearParametro("@idMarca", (nuevo.Marca != null && nuevo.Marca.Id != 0) ? (object)nuevo.Marca.Id : DBNull.Value);
                datos.setearParametro("@idCategoria", (nuevo.Categoria != null && nuevo.Categoria.Id != 0) ? (object)nuevo.Categoria.Id : DBNull.Value);
                datos.setearParametro("@precio", nuevo.Precio);

                datos.ejecutarLectura();

                if (datos.Lector.Read())
                    return (int)datos.Lector[0];

                return 0;
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
