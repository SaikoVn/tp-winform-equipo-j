using dominio;
using negocio;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace winform_app
{
    public partial class frmAltaArticulo : Form
    {

        private Articulo articulo = null;
        private List<string> listaUrls = new List<string>();
        public frmAltaArticulo()
        {
            InitializeComponent();
        }

        // Constructor sobrecargado para modificaciones
        public frmAltaArticulo(Articulo articulo)
        {
            InitializeComponent();
            this.articulo = articulo;
            Text = "Modificar Artículo";
        }

        // Constructor para modo solo lectura
        public frmAltaArticulo(Articulo articulo, bool soloLectura)
        {
            InitializeComponent();
            this.articulo = articulo;
            Text = "Detalle del Artículo";

            if (soloLectura)
            {
                txtCodigo.ReadOnly = true;
                txtNombre.ReadOnly = true;
                txtDescripcion.ReadOnly = true;
                txtPrecio.ReadOnly = true;
                txtUrlImagen.ReadOnly = true;
                btnAgregarImagen.Enabled = false;
                btnEliminarImagen.Enabled = false;
                cboMarca.Enabled = false;
                cboCategoria.Enabled = false;
                btnAceptar.Visible = false;
                btnCancelar.Text = "Cerrar";
            }
        }

        private void frmAltaArticulo_Load(object sender, EventArgs e)
        {
            MarcaNegocio marcaNegocio = new MarcaNegocio();
            CategoriaNegocio categoriaNegocio = new CategoriaNegocio();

            try
            {
                // 1. Cargar desplegables
                cboMarca.ValueMember = "Id";
                cboMarca.DisplayMember = "Descripcion";
                cboMarca.DataSource = marcaNegocio.listar();

                cboCategoria.ValueMember = "Id";
                cboCategoria.DisplayMember = "Descripcion";
                cboCategoria.DataSource = categoriaNegocio.listar();

                // 2. Precarga de datos al modificar o ver detalle
                if (articulo != null)
                {
                    txtCodigo.Text = articulo.Codigo;
                    txtNombre.Text = articulo.Nombre;
                    txtDescripcion.Text = articulo.Descripcion;
                    txtPrecio.Text = articulo.Precio.ToString("0.00");

                    if (articulo.Marca != null && articulo.Marca.Id > 0)
                        cboMarca.SelectedValue = articulo.Marca.Id;

                    if (articulo.Categoria != null && articulo.Categoria.Id > 0)
                        cboCategoria.SelectedValue = articulo.Categoria.Id;

                    ImagenNegocio imgNegocio = new ImagenNegocio();
                    List<Imagen> imagenesExistentes = imgNegocio.listar(articulo.Id);
                    listaUrls = imagenesExistentes.ConvertAll(img => img.ImagenUrl);
                    lbxImagenes.DataSource = listaUrls;

                    if (listaUrls.Count > 0)
                        cargarImagen(listaUrls[0]);
                    else
                        cargarImagen("");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // Carga segura del visor
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
                    pbxArticulo.Load("https://efectocolibri.com/wp-content/uploads/2021/01/placeholder.png");
                }
                catch (Exception)
                {
                    pbxArticulo.Image = null;
                }
            }
        }

        // Cierra la ventana sin guardar
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txtUrlImagen_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtUrlImagen_Leave(object sender, EventArgs e)
        {
            cargarImagen(txtUrlImagen.Text);
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (!validarCampos())
                return;

            ArticuloNegocio negocio = new ArticuloNegocio();

            try
            {
                if (articulo == null)
                    articulo = new Articulo();

                articulo.Codigo = txtCodigo.Text.Trim();
                articulo.Nombre = txtNombre.Text.Trim();
                articulo.Descripcion = txtDescripcion.Text.Trim();
                articulo.Precio = decimal.Parse(txtPrecio.Text.Trim().Replace(".", ","));

                articulo.Marca = (Marca)cboMarca.SelectedItem;
                articulo.Categoria = (Categoria)cboCategoria.SelectedItem;

                // Si escribió una URL en el TextBox pero olvidó tocar el botón '+', la incluimos
                if (!string.IsNullOrWhiteSpace(txtUrlImagen.Text))
                {
                    string urlPendiente = txtUrlImagen.Text.Trim();
                    if (!listaUrls.Contains(urlPendiente))
                        listaUrls.Add(urlPendiente);
                }

                ImagenNegocio imgNegocio = new ImagenNegocio();

                if (articulo.Id != 0) // Modificación
                {
                    negocio.modificar(articulo);
                    imgNegocio.eliminarPorArticulo(articulo.Id);

                    foreach (string url in listaUrls)
                    {
                        imgNegocio.agregar(articulo.Id, url);
                    }

                    MessageBox.Show("Modificado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else // Alta nueva
                {
                    int idNuevo = negocio.agregarConId(articulo);

                    foreach (string url in listaUrls)
                    {
                        imgNegocio.agregar(idNuevo, url);
                    }

                    MessageBox.Show("Agregado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private bool validarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                MessageBox.Show("El campo 'Código' es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCodigo.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El campo 'Nombre' es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            if (cboMarca.SelectedIndex < 0)
            {
                MessageBox.Show("Debe seleccionar una Marca.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboMarca.Focus();
                return false;
            }

            if (cboCategoria.SelectedIndex < 0)
            {
                MessageBox.Show("Debe seleccionar una Categoría.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboCategoria.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPrecio.Text))
            {
                MessageBox.Show("El campo 'Precio' es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecio.Focus();
                return false;
            }

            string precioTexto = txtPrecio.Text.Trim().Replace(".", ",");
            if (!decimal.TryParse(precioTexto, out decimal precio) || precio < 0)
            {
                MessageBox.Show("Ingrese un precio numérico válido (mayor o igual a 0).", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecio.Focus();
                return false;
            }

            return true;
        }

        private void btnAgregarImagen_Click(object sender, EventArgs e)
        {
            string url = txtUrlImagen.Text.Trim();
            if (!string.IsNullOrEmpty(url))
            {
                listaUrls.Add(url);
                lbxImagenes.DataSource = null;
                lbxImagenes.DataSource = listaUrls;
                txtUrlImagen.Clear();
                cargarImagen(url);
            }
        }

        private void btnEliminarImagen_Click(object sender, EventArgs e)
        {
            if (lbxImagenes.SelectedItem != null)
            {
                string seleccionada = (string)lbxImagenes.SelectedItem;
                listaUrls.Remove(seleccionada);
                lbxImagenes.DataSource = null;
                lbxImagenes.DataSource = listaUrls;

                if (listaUrls.Count > 0)
                    cargarImagen(listaUrls[0]);
                else
                    cargarImagen("");
            }
        }

        private void lbxImagenes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxImagenes.SelectedItem != null)
            {
                string url = lbxImagenes.SelectedItem.ToString();
                cargarImagen(url);
            }
        }
    }
}