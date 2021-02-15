using Npgsql;
using NpgsqlTypes;
using System;
using System.Windows.Forms;

namespace Interfaz_Proyecto
{
    public partial class Form14 : Form
    {
        NpgsqlConnection conexion;
        bool ocupacion = false;
        string errores, nomb_user_inicial = "";
        utilidades nueva_utilidad = new utilidades();

        private void rellenar_lista()
        {
            using (NpgsqlCommand command = new NpgsqlCommand("SELECT DNI FROM Voluntario order by ID_Voluntario;", conexion))
            {
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    lstusers.Items.Add(reader[0].ToString());
                    lstusers.Update();
                }
                reader.Close();
                lstusers.Refresh();
            }
        }

        private void completar_campos(string seleccion)
        {
            using (NpgsqlCommand command = new NpgsqlCommand("SELECT Nombre FROM Voluntario where ID_Voluntario=(SELECT ID_Voluntario FROM Voluntario where DNI=@dni);", conexion))
            {
                command.Parameters.Add(new NpgsqlParameter("@dni", NpgsqlDbType.Varchar));
                command.Prepare();
                command.Parameters[0].Value = seleccion;
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    txtnombre_a_mod.Text = reader[0].ToString();
                    nomb_user_inicial = txtnombre_a_mod.Text;
                }
                reader.Close();
            }
        }

        public Form14(NpgsqlConnection conexion_recibida)
        {
            InitializeComponent();
            conexion = conexion_recibida;
            rellenar_lista();
            chkpass.Checked = false;
            label6.Enabled = false;
            txtpassnueva.Enabled = false;
        }

        private void btncancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool listar_errores()
        {
            bool hay_errores = false;
            errores = "Lista de errores:" + Environment.NewLine;
            if (txtnombre_a_mod.Text.Trim() == "")
            {
                errores += "*El campo 'Nombre' no puede estar vacio." + Environment.NewLine;
                hay_errores = true;
            }
            return hay_errores;
        }

        private void btnaceptar_Click(object sender, EventArgs e)
        {
            if (listar_errores() == true) MessageBox.Show(errores, "Error");
            else
            {
                if (nomb_user_inicial != txtnombre_a_mod.Text)
                {
                    using (NpgsqlCommand command = new NpgsqlCommand("Select Verificar_Ocupacion_Voluntario_nombre(@nom);", conexion))
                    {
                        command.Parameters.Add(new NpgsqlParameter("@nom", NpgsqlDbType.Varchar));
                        command.Prepare();
                        command.Parameters[0].Value = txtnombre_a_mod.Text.ToLower().Trim();
                        NpgsqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            if (bool.Parse(reader[0].ToString()) == true) ocupacion = true;
                            else ocupacion = false;
                        }
                        reader.Close();
                        command.Parameters.Clear();
                    }
                }
                else ocupacion = false;
                if (ocupacion == false)
                {
                    if (txtpassvieja.Text == "") MessageBox.Show("Para modificar un usuario debe ingresar su contraseña actual.", "Error");
                    else
                    {
                        if (chkpass.Checked == true && txtpassnueva.Text == "" || chkpass.Checked == true && txtpassnueva.Text.Length < 4) MessageBox.Show("Si modificara la contraseña, debe completar el campo de nueva contraseña, la cual debe tener al menos 4 digitos.", "Error");
                        else
                        {
                            using (NpgsqlCommand command = new NpgsqlCommand("SELECT Modificar_voluntario(@nuevo_nom, @dni_user, @pass, @nuevo_pass);", conexion))
                            {
                                command.Parameters.Add(new NpgsqlParameter("@nuevo_nom", NpgsqlDbType.Varchar));
                                command.Parameters.Add(new NpgsqlParameter("@dni_user", NpgsqlDbType.Varchar));
                                command.Parameters.Add(new NpgsqlParameter("@pass", NpgsqlDbType.Varchar));
                                command.Parameters.Add(new NpgsqlParameter("@nuevo_pass", NpgsqlDbType.Varchar));
                                command.Prepare();
                                command.Parameters[0].Value = txtnombre_a_mod.Text.Trim();
                                command.Parameters[1].Value = lstusers.Text.Trim();
                                command.Parameters[2].Value = txtpassvieja.Text;
                                if (txtpassnueva.Text == "") command.Parameters[3].Value = DBNull.Value;
                                else command.Parameters[3].Value = txtpassnueva.Text;
                                command.ExecuteNonQuery();
                                command.Parameters.Clear();
                            }
                            this.DialogResult = DialogResult.OK;
                        }
                    }
                }
                else MessageBox.Show("El usuario elegido ya existe.", "Error");
            }
        }

        private void seleccion_user(object sender, EventArgs e)
        {
            completar_campos(lstusers.Text);
            txtnombre_a_mod.Focus();
        }

        private void click_modif_pass(object sender, EventArgs e)
        {
            if (chkpass.Checked == true)
            {
                label6.Enabled = true;
                txtpassnueva.Enabled = true;
                txtpassnueva.Text = "";
                txtpassnueva.Focus();
            }
            else
            {
                label6.Enabled = false;
                txtpassnueva.Enabled = false;
                txtpassnueva.Text = "";
            }
        }

        private void tecla_precionada(object sender, KeyPressEventArgs e)
        {
            nueva_utilidad.verificador_campo_numero_nodecimal(sender, e);
        }

        private void tecla_presionada(object sender, KeyPressEventArgs e)
        {
            nueva_utilidad.verificador_campo_nombre(sender, e);
        }
    }
}
