using System;
using System.Collections.Generic;
using System.Windows.Forms;
using dominio;
using negocio;

namespace winform_app
{
    public partial class frmMarcasCategorias : Form
    {
        private List<Marca> listaMarcas;
        private List<Categoria> listaCategorias;

        public frmMarcasCategorias()
        {
            InitializeComponent();
        }

        public frmMarcasCategorias(int pestañaInicial) : this()
        {
            if (pestañaInicial >= 0 && pestañaInicial < tabCtrl.TabCount)
                tabCtrl.SelectedIndex = pestañaInicial;
        }

        private void frmMarcasCategorias_Load(object sender, EventArgs e)
        {
            cargarMarcas();
            cargarCategorias();
        }

        #region Marcas

        private void cargarMarcas()
        {
            MarcaNegocio negocio = new MarcaNegocio();
            try
            {
                listaMarcas = negocio.listar();
                dgvMarcas.DataSource = null;
                dgvMarcas.DataSource = listaMarcas;
                if (dgvMarcas.Columns["Id"] != null)
                    dgvMarcas.Columns["Id"].Visible = false;

                txtDescripcionMarca.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void dgvMarcas_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMarcas.CurrentRow != null)
            {
                Marca seleccionada = (Marca)dgvMarcas.CurrentRow.DataBoundItem;
                txtDescripcionMarca.Text = seleccionada.Descripcion;
            }
        }

        private void btnAgregarMarca_Click(object sender, EventArgs e)
        {
            string desc = txtDescripcionMarca.Text.Trim();
            if (string.IsNullOrWhiteSpace(desc))
            {
                MessageBox.Show("Ingrese una descripción para la marca.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDescripcionMarca.Focus();
                return;
            }

            MarcaNegocio negocio = new MarcaNegocio();
            try
            {
                Marca nueva = new Marca { Descripcion = desc };
                negocio.agregar(nueva);
                MessageBox.Show("Marca agregada con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cargarMarcas();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnModificarMarca_Click(object sender, EventArgs e)
        {
            if (dgvMarcas.CurrentRow == null)
            {
                MessageBox.Show("Seleccione una marca de la lista para modificar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string desc = txtDescripcionMarca.Text.Trim();
            if (string.IsNullOrWhiteSpace(desc))
            {
                MessageBox.Show("La descripción no puede estar vacía.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDescripcionMarca.Focus();
                return;
            }

            Marca seleccionada = (Marca)dgvMarcas.CurrentRow.DataBoundItem;
            seleccionada.Descripcion = desc;

            MarcaNegocio negocio = new MarcaNegocio();
            try
            {
                negocio.modificar(seleccionada);
                MessageBox.Show("Marca modificada con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cargarMarcas();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnEliminarMarca_Click(object sender, EventArgs e)
        {
            if (dgvMarcas.CurrentRow == null)
            {
                MessageBox.Show("Seleccione una marca de la lista para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Marca seleccionada = (Marca)dgvMarcas.CurrentRow.DataBoundItem;
            MarcaNegocio negocio = new MarcaNegocio();

            try
            {
                if (negocio.tieneArticulosAsociados(seleccionada.Id))
                {
                    MessageBox.Show($"No se puede eliminar la marca '{seleccionada.Descripcion}' porque está asociada a uno o más artículos en el catálogo.", "Operación no permitida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult respuesta = MessageBox.Show(
                    $"¿Está seguro de que desea eliminar la marca '{seleccionada.Descripcion}'?",
                    "Confirmar Eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (respuesta == DialogResult.Yes)
                {
                    negocio.eliminar(seleccionada.Id);
                    MessageBox.Show("Marca eliminada con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    cargarMarcas();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #endregion

        #region Categorias

        private void cargarCategorias()
        {
            CategoriaNegocio negocio = new CategoriaNegocio();
            try
            {
                listaCategorias = negocio.listar();
                dgvCategorias.DataSource = null;
                dgvCategorias.DataSource = listaCategorias;
                if (dgvCategorias.Columns["Id"] != null)
                    dgvCategorias.Columns["Id"].Visible = false;

                txtDescripcionCategoria.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void dgvCategorias_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCategorias.CurrentRow != null)
            {
                Categoria seleccionada = (Categoria)dgvCategorias.CurrentRow.DataBoundItem;
                txtDescripcionCategoria.Text = seleccionada.Descripcion;
            }
        }

        private void btnAgregarCategoria_Click(object sender, EventArgs e)
        {
            string desc = txtDescripcionCategoria.Text.Trim();
            if (string.IsNullOrWhiteSpace(desc))
            {
                MessageBox.Show("Ingrese una descripción para la categoría.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDescripcionCategoria.Focus();
                return;
            }

            CategoriaNegocio negocio = new CategoriaNegocio();
            try
            {
                Categoria nueva = new Categoria { Descripcion = desc };
                negocio.agregar(nueva);
                MessageBox.Show("Categoría agregada con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cargarCategorias();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnModificarCategoria_Click(object sender, EventArgs e)
        {
            if (dgvCategorias.CurrentRow == null)
            {
                MessageBox.Show("Seleccione una categoría de la lista para modificar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string desc = txtDescripcionCategoria.Text.Trim();
            if (string.IsNullOrWhiteSpace(desc))
            {
                MessageBox.Show("La descripción no puede estar vacía.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDescripcionCategoria.Focus();
                return;
            }

            Categoria seleccionada = (Categoria)dgvCategorias.CurrentRow.DataBoundItem;
            seleccionada.Descripcion = desc;

            CategoriaNegocio negocio = new CategoriaNegocio();
            try
            {
                negocio.modificar(seleccionada);
                MessageBox.Show("Categoría modificada con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cargarCategorias();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnEliminarCategoria_Click(object sender, EventArgs e)
        {
            if (dgvCategorias.CurrentRow == null)
            {
                MessageBox.Show("Seleccione una categoría de la lista para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Categoria seleccionada = (Categoria)dgvCategorias.CurrentRow.DataBoundItem;
            CategoriaNegocio negocio = new CategoriaNegocio();

            try
            {
                if (negocio.tieneArticulosAsociados(seleccionada.Id))
                {
                    MessageBox.Show($"No se puede eliminar la categoría '{seleccionada.Descripcion}' porque está asociada a uno o más artículos en el catálogo.", "Operación no permitida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult respuesta = MessageBox.Show(
                    $"¿Está seguro de que desea eliminar la categoría '{seleccionada.Descripcion}'?",
                    "Confirmar Eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (respuesta == DialogResult.Yes)
                {
                    negocio.eliminar(seleccionada.Id);
                    MessageBox.Show("Categoría eliminada con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    cargarCategorias();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #endregion

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
