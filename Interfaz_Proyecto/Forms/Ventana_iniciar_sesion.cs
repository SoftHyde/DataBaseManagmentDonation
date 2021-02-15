using Npgsql;
using NpgsqlTypes;
using System;
using System.Windows.Forms;

namespace Interfaz_Proyecto
{
    public partial class Form13 : Form
    {
        NpgsqlConnection conexion = new NpgsqlConnection();
        elemento_usuario nuevo_user = new elemento_usuario();
        string errores;

        public void rellenar_lista()
        {
            lstusers.Items.Clear();
            if (checkadmin.Checked == true)
            {
                using (NpgsqlCommand command_insersion = new NpgsqlCommand("Select DNI from Voluntario where Es_Admin=true order by ID_Voluntario;", conexion))
                {
                    NpgsqlDataReader reader = command_insersion.ExecuteReader();
                    while (reader.Read())
                    {
                        lstusers.Items.Add(reader[0].ToString());
                    }
                    reader.Close();
                }
            }
            else
            {
                using (NpgsqlCommand command_insersion = new NpgsqlCommand("Select DNI from Voluntario where Es_Admin=false order by ID_Voluntario;", conexion))
                {
                    NpgsqlDataReader reader = command_insersion.ExecuteReader();
                    while (reader.Read())
                    {
                        lstusers.Items.Add(reader[0].ToString());
                    }
                    reader.Close();
                }
            }
        }

        public Form13(NpgsqlConnection conexion_recibida)
        {
            InitializeComponent();
            conexion = conexion_recibida;
            rellenar_lista();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        public elemento_usuario Ver_elemento()
        {
            return nuevo_user;
        }

        private void cambio_check(object sender, EventArgs e)
        {
            rellenar_lista();
        }

        private bool listar_errores()
        {
            bool hay_errores = false;
            errores = "Lista de errores:" + Environment.NewLine;
            if (lstusers.Text == "")
            {
                errores += "*Debe seleccionar un DNI de la lista." + Environment.NewLine;
                hay_errores = true;
            }
            if (txtpass.Text == "")
            {
                errores += "*Debe completar el campo 'Contraseña'." + Environment.NewLine;
                hay_errores = true;
            }
            return hay_errores;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listar_errores() == true) MessageBox.Show(errores, "Error");
            else
            {
                using (NpgsqlCommand command_insersion = new NpgsqlCommand("Select Verificar_Usuario(@user, @pass);", conexion))
                {
                    command_insersion.Parameters.Add(new NpgsqlParameter("@user", NpgsqlDbType.Varchar));
                    command_insersion.Parameters.Add(new NpgsqlParameter("@pass", NpgsqlDbType.Varchar));
                    command_insersion.Prepare();
                    command_insersion.Parameters[0].Value = lstusers.Text.ToLower();
                    command_insersion.Parameters[1].Value = txtpass.Text;
                    NpgsqlDataReader reader = command_insersion.ExecuteReader();
                    while (reader.Read())
                    {
                        if (bool.Parse(reader[0].ToString()) == true)
                        {
                            nuevo_user.nombre = lstusers.Text.ToLower();
                            nuevo_user.es_Admin = checkadmin.Checked;
                            this.DialogResult = DialogResult.OK;
                        }
                        else
                        {
                            MessageBox.Show("DNI o contraseña incorrecto. Intente nuevamente.", "Error");
                        }
                    }
                    reader.Close();
                    command_insersion.Parameters.Clear();
                }
            }
        }

        private void seleccion_usuario(object sender, EventArgs e)
        {
            txtpass.Focus();
        }
    }
}
