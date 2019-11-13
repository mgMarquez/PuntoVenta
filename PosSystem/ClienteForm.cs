﻿
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic;
using Entity;
using System;
using System.Windows.Forms;

namespace PosSystem
{
    public partial class ClienteForm : Form
    {

        DataSetCombox dtCombo = new DataSetCombox();
        static ClienteForm _instancia;
        DataSetComboxTableAdapters.CategoriaTableAdapter categoria = new DataSetComboxTableAdapters.CategoriaTableAdapter();
        DataSetComboxTableAdapters.PresentacionTableAdapter presentacion = new DataSetComboxTableAdapters.PresentacionTableAdapter();
        
        public ClienteForm()
        {
            InitializeComponent();
        }

        private int ID;

        public static ClienteForm GetInstancia()
        {

            if (_instancia == null)
            {
                _instancia = new ClienteForm();

            }
            return (_instancia);

        }


        public bool Validate()
        {
            bool valor = false;

            if (!string.IsNullOrWhiteSpace(txtNombre.Text) &&
                !string.IsNullOrWhiteSpace(txtApellido.Text)&&
                !string.IsNullOrWhiteSpace(txtDireccion.Text)&&
                !string.IsNullOrWhiteSpace(cbTipoDocumento.Text)//&&
                //!string.IsNullOrWhiteSpace(cbPresentacion.Text)
                )
            {
                valor = true;
            }
            return valor;
        }

        public void Limpiar(params TextBox[] text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                text[i].Clear();

            }
            cbTipoDocumento.Text = "";
            // cbPresentacion.Text = "";
            // pbImagen.Visible = false;

        }

        private void FocusTextBox()
        {
            if (tabControl1.SelectedIndex == 0)
            {
                txtApellido.Focus();

            }
            else if (tabControl1.SelectedIndex == 1)
            {
                txtBuscar.Focus();
            }
        }
         private void llenarGrid()
         {
            dataGridView1.DataSource = ArticuloBL.SelectAllArticulo();
         }
        private void ArticuloForm_Load(object sender, EventArgs e)
        {
            btnGuardar.Enabled = false;
            btnEliminar.Enabled = false;
            llenarGrid();
            try
            {
                categoria.Fill(dtCombo.Categoria);
                cbTipoDocumento.DataSource = dtCombo.Categoria;
                cbTipoDocumento.DisplayMember = "nombre";

                presentacion.Fill(dtCombo.Presentacion);
                // cbPresentacion.DataSource = dtCombo.Presentacion;
                // cbPresentacion.DisplayMember = "nombre";
            }
            catch (Exception)
            {
                MessageBox.Show("Error al llenar el ComboBox Categoria || Presentacion");
            }
        }

        private void PaginarTrabajador()
        {
            int fila = dataGridView1.CurrentRow.Index;
            try 
            { 
                //Extraer una Imagen al PictureBox desde un DataGridView
                byte[] datos = new byte[0];
                datos = (byte[])dataGridView1.Rows[fila].Cells["Imagen"].Value;
                System.IO.MemoryStream ms = new System.IO.MemoryStream(datos);
                // pbImagen.Image = System.Drawing.Bitmap.FromStream(ms);

                ID = Convert.ToInt32(dataGridView1.Rows[fila].Cells["IDArticulo"].Value);
                txtApellido.Text = dataGridView1.Rows[fila].Cells["Codigo_Articulo"].Value.ToString();
                txtNombre.Text = dataGridView1.Rows[fila].Cells["Nombre"].Value.ToString();
                cbTipoDocumento.Text = dataGridView1.Rows[fila].Cells["Categoria"].Value.ToString();
                // cbPresentacion.Text = dataGridView1.Rows[fila].Cells["Presentacion"].Value.ToString();
                txtDireccion.Text = dataGridView1.Rows[fila].Cells["Descripcion"].Value.ToString();

            }

            catch (Exception) { MessageBox.Show("Error al llenar el gridView"); }
        }

        private void btnImagen_Click(object sender, EventArgs e)
        {
            try
            {
                this.openFileDialog1.ShowDialog();
                if (this.openFileDialog1.FileName.Equals("") == false)
                {
                    // pbImagen.Visible = true;
                    // pbImagen.Load(this.openFileDialog1.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cargar la imagen: " + ex.ToString());
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                if (Validate())
                {
                    Articulo nuevo = new Articulo();
                    nuevo.Codido_Articulo = txtApellido.Text;
                    nuevo.IDCategoria = cbTipoDocumento.SelectedIndex+1;
                    // nuevo.IDPresentacion = cbPresentacion.SelectedIndex+1;
                    nuevo.Nombre = txtNombre.Text;
                    nuevo.Descripcion = txtDireccion.Text;
                    // nuevo.Imagen = pbImagen;
                    ArticuloBL.InsertArticulo(nuevo);
                    dataGridView1.Update();
                    llenarGrid();
                    MessageBox.Show("Articulo Agregado Exitosamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Limpiar(txtNombre, txtDireccion, txtApellido);
                }
                else
                {
                    MessageBox.Show("Debe llenar todos los Campos Requeridos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " Error Al Agregar el Articulo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Limpiar(txtNombre, txtDireccion, txtApellido);
            btnGuardar.Enabled = false;
            btnEliminar.Enabled = false;
            btnAgregar.Enabled = true;

        }

        private void txtBuscar_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == '\r')
                {
                    e.Handled = true;
                    btnBuscar.PerformClick();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "Error Al Consultar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.DataSource = ArticuloBL.SelectCodigoArticulo(txtBuscar.Text);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message + "Error Al Consultar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // pbImagen.Visible = true;
                PaginarTrabajador();
                tabControl1.SelectedIndex = 0;
                btnGuardar.Enabled = true;
                btnEliminar.Enabled = true;
                btnAgregar.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "Error del DataGrid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (Validate())
                {
                    Articulo modifica = new Articulo();
                    modifica.IDArticulo = ID;
                    modifica.Codido_Articulo = txtApellido.Text;
                    modifica.IDCategoria = cbTipoDocumento.SelectedIndex + 1;
                    //modifica.IDPresentacion = cbPresentacion.SelectedIndex + 1;
                    modifica.Nombre = txtNombre.Text;
                    modifica.Descripcion = txtDireccion.Text;
                    // modifica.Imagen = pbImagen;
                    ArticuloBL.UpdateArticulo(modifica);
                    dataGridView1.Update();
                    llenarGrid();
                    MessageBox.Show("Articulo Modificado Exitosamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Limpiar(txtNombre, txtDireccion, txtApellido);
                    btnGuardar.Enabled = false;
                    btnEliminar.Enabled = false;
                    btnAgregar.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Debe llenar todos los Campos Requeridos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " Error Al Modificar Articulo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult resultado = MessageBox.Show("Realmente desea eliminar el Articulo: " + txtNombre.Text + "?",
                                   "¿Desea eliminar?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (resultado == DialogResult.Yes)
                {
                    ArticuloBL.DeleteArticulo(ID);
                    MessageBox.Show("Articulo Eliminado.", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Limpiar(txtNombre, txtDireccion, txtApellido);
                    dataGridView1.Update();
                    llenarGrid();
                    btnGuardar.Enabled = false;
                    btnEliminar.Enabled = false;
                    btnAgregar.Enabled = true;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " Error Al Eliminar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtNombre_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
