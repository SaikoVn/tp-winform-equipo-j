using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dominio
{
    public class Articulo
    {
        public int Id { get; set; }

        // [DisplayName("...")] define el nombre visible que tendra una propiedad cuando se muestra en un control de la interfaz como el DataGridView
        // Agarraría solamente el nombre exacto de la propiedad (Codigo, sin tilde, ej)
        [DisplayName("Código")]
        public string Codigo { get; set; }

        [DisplayName("Nombre")]
        public string Nombre { get; set; }

        [DisplayName("Descripción")]
        public string Descripcion { get; set; }

        [DisplayName("Marca")]
        public Marca Marca { get; set; }

        [DisplayName("Categoría")]
        public Categoria Categoria { get; set; }

        [DisplayName("Precio")]
        public decimal Precio { get; set; }

        public List<Imagen> Imagenes { get; set; } = new List<Imagen>();

        public string ImagenUrl { get; set; }
    }
}

