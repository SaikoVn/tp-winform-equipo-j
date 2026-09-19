using dominio;
using negocio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace winform_app
{
    public partial class Form1 : Form
    {
        private List<Articulo> listaArticulo;
        private List<Imagen> imagenesArticuloActual;
        private int indiceImagen = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void cargar()
        {
            cboCampo.Items.Clear();
            cboCampo.Items.Add("Código");
            cboCampo.Items.Add("Nombre");
            cboCampo.Items.Add("Marca");
            cboCampo.Items.Add("Categoría");
            cboCampo.Items.Add("Precio");

            ArticuloNegocio negocio = new ArticuloNegocio();
            try
            {
                listaArticulo = negocio.listar();
                dgvArticulos.DataSource = null;
                dgvArticulos.DataSource = listaArticulo;
                ocultarColumnas();

                if (listaArticulo != null && listaArticulo.Count > 0)
                {
                    cargarImagen(listaArticulo[0].ImagenUrl);
                }
                else
                {
                    cargarImagen("");
                    lblPaginacion.Text = "0 / 0";
                    btnAnterior.Enabled = false;
                    btnSiguiente.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ocultarColumnas()
        {
            if (dgvArticulos.Columns["Id"] != null)
                dgvArticulos.Columns["Id"].Visible = false;
            if (dgvArticulos.Columns["ImagenUrl"] != null)
                dgvArticulos.Columns["ImagenUrl"].Visible = false;
            if (dgvArticulos.Columns["Imagenes"] != null)
                dgvArticulos.Columns["Imagenes"].Visible = false;
            if (dgvArticulos.Columns["Precio"] != null)
                dgvArticulos.Columns["Precio"].DefaultCellStyle.Format = "C2";
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            cargar();
            
        }

        private void pbxArticulo_Click(object sender, EventArgs e)
        {

        }

        private void cargarImagen(string imagen)
        {
            try
            {
                pbxArticulo.Load(imagen);
            }
            catch (Exception)
            { 
                try
                {
                    // Intenta un enlace de una imagen vacia por defecto
                    pbxArticulo.Load("https://efectocolibri.com/wp-content/uploads/2021/01/placeholder.png");
                }
                catch (Exception)
                {
                    // Si tampoco hay internet, deja el recuadro limpio sin romper la app
                    pbxArticulo.Image = null;
                }
            }
        }


        private void pbxArticulo_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {

        }

        private void dgvArticulos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvArticulos.CurrentRow != null)
            {
                Articulo seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
                ImagenNegocio imgNegocio = new ImagenNegocio();
                imagenesArticuloActual = imgNegocio.listar(seleccionado.Id);
                indiceImagen = 0;
                mostrarImagenActual();
            }
            else
            {
                imagenesArticuloActual = null;
                cargarImagen("");
                lblPaginacion.Text = "0 / 0";
                btnAnterior.Enabled = false;
                btnSiguiente.Enabled = false;
            }
        }

        private void mostrarImagenActual()
        {
            if (imagenesArticuloActual != null && imagenesArticuloActual.Count > 0)
            {
                cargarImagen(imagenesArticuloActual[indiceImagen].ImagenUrl);
                lblPaginacion.Text = $"{indiceImagen + 1} / {imagenesArticuloActual.Count}";
                btnAnterior.Enabled = indiceImagen > 0;
                btnSiguiente.Enabled = indiceImagen < imagenesArticuloActual.Count - 1;
            }
            else
            {
                cargarImagen(""); // Carga el placeholder
                lblPaginacion.Text = "0 / 0";
                btnAnterior.Enabled = false;
                btnSiguiente.Enabled = false;
            }
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            if (indiceImagen > 0)
            {
                indiceImagen--;
                mostrarImagenActual();
            }
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            if (indiceImagen < imagenesArticuloActual.Count - 1)
            {
                indiceImagen++;
                mostrarImagenActual();
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmAltaArticulo alta = new frmAltaArticulo();
            alta.ShowDialog();
            cargar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvArticulos.CurrentRow != null)
            {
                Articulo seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
                frmAltaArticulo modificar = new frmAltaArticulo(seleccionado);
                modificar.ShowDialog();
                cargar();
            }
            else
            {
                MessageBox.Show("Seleccione un artículo para modificar.");
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvArticulos.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un artículo para eliminar.");
                return;
            }

            Articulo seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
            ArticuloNegocio negocio = new ArticuloNegocio();

            DialogResult respuesta = MessageBox.Show(
                $"¿Seguro que desea eliminar '{seleccionado.Nombre}'?",
                "Eliminar Artículo",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (respuesta == DialogResult.Yes)
            {
                try
                {
                    negocio.eliminar(seleccionado.Id);
                    cargar(); // Refresca la grilla tras la baja
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void txtFiltro_TextChanged(object sender, EventArgs e)
        {
            List<Articulo> listaFiltrada;
            string filtro = txtFiltro.Text;

            if (filtro.Length >= 2)
            {
                listaFiltrada = listaArticulo.FindAll(x =>
                    x.Nombre.ToUpper().Contains(filtro.ToUpper()) ||
                    x.Codigo.ToUpper().Contains(filtro.ToUpper()) ||
                    (x.Descripcion != null && x.Descripcion.ToUpper().Contains(filtro.ToUpper())) ||
                    (x.Marca != null && x.Marca.Descripcion.ToUpper().Contains(filtro.ToUpper())) ||
                    (x.Categoria != null && x.Categoria.Descripcion.ToUpper().Contains(filtro.ToUpper())) ||
                    x.Precio.ToString().Contains(filtro) // Convierte el número a texto para coincidir
                );
            }
            else
            {
                listaFiltrada = listaArticulo;
            }

            dgvArticulos.DataSource = null;
            dgvArticulos.DataSource = listaFiltrada;
            ocultarColumnas();
        }

        private void cboCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboCampo.SelectedItem == null)
                return;
            string opcion = cboCampo.SelectedItem.ToString();
            cboCriterio.Items.Clear();

            if (opcion == "Precio")
            {
                cboCriterio.Items.Add("Mayor a");
                cboCriterio.Items.Add("Menor a");
                cboCriterio.Items.Add("Igual a");
            }
            else
            {
                cboCriterio.Items.Add("Comienza con");
                cboCriterio.Items.Add("Termina con");
                cboCriterio.Items.Add("Contiene");
            }
        }

        private void btnBuscarFiltro_Click(object sender, EventArgs e)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            try
            {
                if (validarFiltro())
                    return;

                string campo = cboCampo.SelectedItem.ToString();
                string criterio = cboCriterio.SelectedItem.ToString();
                string filtro = txtFiltroAvanzado.Text;

                dgvArticulos.DataSource = negocio.filtrar(campo, criterio, filtro);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private bool validarFiltro()
        {
            if (cboCampo.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccione el campo para filtrar.");
                return true;
            }
            if (cboCriterio.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccione el criterio para filtrar.");
                return true;
            }
            if (cboCampo.SelectedItem.ToString() == "Precio")
            {
                if (string.IsNullOrEmpty(txtFiltroAvanzado.Text))
                {
                    MessageBox.Show("Debe ingresar un valor numérico en el filtro.");
                    return true;
                }
                string filtroLimpio = txtFiltroAvanzado.Text.Trim().Replace(".", ",");
                if (!decimal.TryParse(filtroLimpio, out _))
                {
                    MessageBox.Show("Solo ingrese números para filtrar por precio.");
                    return true;
                }
            }

            return false;
        }

        private void btnVerDetalle_Click(object sender, EventArgs e)
        {
            if (dgvArticulos.CurrentRow != null)
            {
                Articulo seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
                frmAltaArticulo detalle = new frmAltaArticulo(seleccionado, true);
                detalle.ShowDialog();
            }
            else
            {
                MessageBox.Show("Seleccione un artículo para ver su detalle.");
            }
        }

        private void btnLimpiarFiltro_Click(object sender, EventArgs e)
        {
            // Limpia el filtro rápido en memoria
            txtFiltro.Text = string.Empty;

            // Limpia y resetea los campos del filtro avanzado
            cboCampo.SelectedIndex = -1;
            cboCriterio.Items.Clear();
            cboCriterio.SelectedIndex = -1;
            txtFiltroAvanzado.Text = string.Empty;

            // Recarga la lista original completa desde la base de datos
            cargar();
        }

        private void menuMarcas_Click(object sender, EventArgs e)
        {
            frmMarcasCategorias marcas = new frmMarcasCategorias(0);
            marcas.ShowDialog();
            cargar();
        }

        private void menuCategorias_Click(object sender, EventArgs e)
        {
            frmMarcasCategorias categorias = new frmMarcasCategorias(1);
            categorias.ShowDialog();
            cargar();
        }

        private void menuMarcasCategorias_Click(object sender, EventArgs e)
        {
            frmMarcasCategorias marcasCategorias = new frmMarcasCategorias(0);
            marcasCategorias.ShowDialog();
            cargar();
        }
    }
}
