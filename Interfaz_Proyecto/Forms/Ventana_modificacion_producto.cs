using Npgsql;
using NpgsqlTypes;
using System;
using System.Windows.Forms;

namespace Interfaz_Proyecto
{
    public partial class Form12 : Form
    {
        string seleccion, errores, Comando_a_usar, nombre_orig = "";
        bool ocupado = false;
        NpgsqlConnection conexion;
        utilidades nueva_utilidad = new utilidades();
        int tipo; //1 - Producto, 2 - Dueño

        public void rellenar_campos(string nombre)
        {
            if (tipo == 1) Comando_a_usar = ("SELECT Nombre FROM Producto WHERE Nombre=@nom");
            else Comando_a_usar = ("SELECT Nombre FROM Duenio WHERE Nombre=@nom");

            using (NpgsqlCommand new_command = new NpgsqlCommand(Comando_a_usar, conexion))
            {
                new_command.Parameters.Add(new NpgsqlParameter("@nom", NpgsqlDbType.Varchar));
                new_command.Prepare();
                new_command.Parameters[0].Value = nombre;
                NpgsqlDataReader reader = new_command.ExecuteReader();
                while (reader.Read())
                {
                    txtnombre.Text = reader[0].ToString();
                    nombre_orig = txtnombre.Text;
                }
                reader.Close();
            }
        }

        public Form12(NpgsqlConnection conexion_recibida, int tipo_ven)
        {
            InitializeComponent();
            conexion = conexion_recibida;
            tipo = tipo_ven;
            if (tipo == 1)
            {
                this.Text = "Modificación de Datos de Producto";
                Comando_a_usar = ("SELECT Nombre FROM Producto order by ID_Producto");
            }
            else
            {
                this.Text = "Modificación de Datos de Dueño";
                Comando_a_usar = ("SELECT Nombre FROM Duenio order by ID_Duenio");
            }
            using (NpgsqlCommand new_command = new NpgsqlCommand(Comando_a_usar, conexion_recibida))
            {
                NpgsqlDataReader reader = new_command.ExecuteReader();
                while (reader.Read())
                {
                    lstproductos.Items.Add(reader[0].ToString());
                    lstproductos.Update();
                }
                reader.Close();
                lstproductos.Refresh();
            }
        }

        private void btncancear_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool listar_errores()
        {
            bool hay_errores = false;
            errores = "Lista de errores:" + Environment.NewLine;
            if (lstproductos.Text == "")
            {
                if (tipo == 0) errores += "*Debe seleccionar un producto de la lista para continuar." + Environment.NewLine;
                else errores += "*Debe seleccionar un dueño de la lista para continuar." + Environment.NewLine;
                hay_errores = true;
            }
            if (txtnombre.Text.Trim() == "")
            {
                if (tipo == 0) errores += "*El nombre del producto no puede quedar vacio." + Environment.NewLine;
                else errores += "*El nombre del producto no puede quedar vacio." + Environment.NewLine;
                hay_errores = true;
            }
            return hay_errores;
        }

        private void btnaceptar_Click(object sender, EventArgs e)
        {
            if (listar_errores() == true) MessageBox.Show(errores, "Error");
            else
            {
                if (nombre_orig != txtnombre.Text)
                {
                    if (tipo == 1) Comando_a_usar = ("SELECT Verificar_Ocupacion_Producto(@nom)");
                    else Comando_a_usar = ("SELECT Verificar_Ocupacion_Duenio(@nom)");
                    using (NpgsqlCommand new_command = new NpgsqlCommand(Comando_a_usar, conexion))
                    {
                        new_command.Parameters.Add(new NpgsqlParameter("@nom", NpgsqlDbType.Varchar));
                        new_command.Prepare();
                        new_command.Parameters[0].Value = nueva_utilidad.verificador_palabras_plurales(txtnombre.Text.ToLower().Trim());
                        NpgsqlDataReader reader = new_command.ExecuteReader();
                        while (reader.Read())
                        {
                            ocupado = bool.Parse(reader[0].ToString());
                        }
                        reader.Close();
                    }
                }
                if (ocupado == false)
                {
                    if (tipo == 1) Comando_a_usar = ("SELECT Modificar_producto (@nom, @nuevo);");
                    else Comando_a_usar = ("SELECT Modificar_duenio (@nom, @nuevo);");
                    using (NpgsqlCommand command = new NpgsqlCommand(Comando_a_usar, conexion))
                    {
                        command.Parameters.Add(new NpgsqlParameter("@nom", NpgsqlDbType.Varchar));
                        command.Parameters.Add(new NpgsqlParameter("@nuevo", NpgsqlDbType.Varchar));
                        command.Prepare();
                        command.Parameters[0].Value = seleccion;
                        command.Parameters[1].Value = nueva_utilidad.verificador_palabras_plurales(txtnombre.Text.ToLower().Trim());
                        command.ExecuteNonQuery();
                        command.Parameters.Clear();
                    }
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    if (tipo == 1) MessageBox.Show("El nombre del producto que intenta ingresar esta ocupado.", "Error");
                    else MessageBox.Show("El nombre del dueño que intenta ingresar esta ocupado.", "Error");
                }
            }
        }

        private void elemento_seleccionado(object sender, EventArgs e)
        {
            seleccion = lstproductos.Text;
            rellenar_campos(seleccion);
            txtnombre.Focus();
        }

        private void tecla_presionada(object sender, KeyPressEventArgs e)
        {
            nueva_utilidad.verificador_campo_nombre(sender, e);
        }
    }
}
