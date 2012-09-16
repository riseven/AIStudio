using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AIS
{
    public partial class AddItemToProjectForm : Form
    {
        public string ObjectName
        {
            get
            {
                return textBoxName.Text;
            }
            set
            {
                textBoxName.Text = value;
            }
        }

        public ComboBox.ObjectCollection BaseObjects
        {
            get
            {
                return comboBoxParent.Items;
            }
        }

        public Object SelectedObject
        {
            get
            {
                if (comboBoxParent.SelectedIndex == -1)
                {
                    throw new Exception("No se ha seleccionado un base object");
                }
                return comboBoxParent.Items[comboBoxParent.SelectedIndex];
            }
        }


        public AddItemToProjectForm()
        {
            InitializeComponent();
        }

        private void botonCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void botonAceptar_Click(object sender, EventArgs e)
        {
            if (comboBoxParent.SelectedIndex < 0)
            {
                MessageBox.Show("No se ha seleccionado ningún objeto base");
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}