using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
namespace negocio
{
    public class AccesoDatos
    {
        private SqlConnection conexion;
        private SqlCommand comando;
        private SqlDataReader lector
         {
            get { return lector; }
        }

        public AccesoDatos() { 
        // INICIALIZAR LA CONEXION Y EL COMANDO
        // ---
        // ---
        } 
    }
}
