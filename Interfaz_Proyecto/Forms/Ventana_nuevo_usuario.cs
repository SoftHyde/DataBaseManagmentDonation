using Npgsql;
using NpgsqlTypes;
using System;
using System.Windows.Forms;

namespace Interfaz_Proyecto
{
    public partial class Form11 : Form
    {
        NpgsqlConnection conexion = new NpgsqlConnection();
        utilidades nueva_utilidad = new utilidades();
        bool ingresar_admin_oblig = new bool();
        string errores;

        public Form11(NpgsqlConnection conexion_recibida, bool obligatorio)
        {
            InitializeComponent();
            conexion = conexion_recibida;
            ingresar_admin_oblig = obligatorio;
            if (obligatorio == true)
            {
                checkadmin.Checked = true;
                checkadmin.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //se ejecuta cuando se agrega el usuario, borrando los campos y dejando la ventana limpia para ingresar uno nuevo
        private void refrezcar_ventana()
        {
            txtnombre.Text = "";
            txtpass.Text = "";
            txtuser.Text = "";
            checkadmin.Checked = false;
            txtnombre.Focus();
        }

        private bool listar_errores()
        {
            bool hay_errores = false;
            errores = "Lista de errores:" + Environment.NewLine;
            if (txtuser.Text.Trim() == "" || txtnombre.Text.Trim() == "" || txtpass.Text == "")
            {
                errores += "*Debe completar todos los campos obligatorios (*)." + Environment.NewLine;
                hay_errores = true;
            }
            if (txtuser.Text.Length != 8)
            {
                errores += "*El campo 'DNI' Solo puede tener 8 caracteres." + Environment.NewLine;
                hay_errores = true;
            }
            if (txtpass.Text.Length < 4)
            {
                errores += "*El campo 'Contraseña' debe tener al menos 4 caracteres." + Environment.NewLine;
                hay_errores = true;
            }
            return hay_errores;
        }

        //se ejecuta al hacer click en agregar
        private void button1_Click(object sender, EventArgs e)
        {
            if (listar_errores() == true) MessageBox.Show(errores, "Error");
            else
            {
                bool user_libre = true;
                //verifica que el nombre no este ocupado
                using (NpgsqlCommand command = new NpgsqlCommand("Select Verificar_Ocupacion_Voluntario_nombre(@nom);", conexion))
                {
                    command.Parameters.Add(new NpgsqlParameter("@nom", NpgsqlDbType.Varchar));
                    command.Prepare();
                    command.Parameters[0].Value = txtnombre.Text.ToLower().Trim();
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (bool.Parse(reader[0].ToString()) == true) user_libre = false;
                        else user_libre = true;
                    }
                    reader.Close();
                    command.Parameters.Clear();

                    if (user_libre == true)
                    {
                        //verifica que el dni no este ocupado
                        using (NpgsqlCommand new_command = new NpgsqlCommand("Select Verificar_Ocupacion_Voluntario_dni(@dni);", conexion))
                        {
                            new_command.Parameters.Add(new NpgsqlParameter("@dni", NpgsqlDbType.Varchar));
                            new_command.Prepare();
                            new_command.Parameters[0].Value = txtuser.Text.ToLower().Trim();
                            NpgsqlDataReader new_reader = new_command.ExecuteReader();
                            while (reader.Read())
                            {
                                if (bool.Parse(reader[0].ToString()) == true) user_libre = false;
                                else user_libre = true;
                            }
                            reader.Close();
                            command.Parameters.Clear();
                        }
                        if (user_libre == true)
                        {
                            //si todo es correcto, ingresa el usuario
                            using (NpgsqlCommand command_insersion = new NpgsqlCommand("Select Ingresar_voluntario(@nombre, @DNI, @admin, @pass);", conexion))
                            {
                                command_insersion.Parameters.Add(new NpgsqlParameter("@nombre", NpgsqlDbType.Varchar));
                                command_insersion.Parameters.Add(new NpgsqlParameter("@DNI", NpgsqlDbType.Varchar));
                                command_insersion.Parameters.Add(new NpgsqlParameter("@admin", NpgsqlDbType.Boolean));
                                command_insersion.Parameters.Add(new NpgsqlParameter("@pass", NpgsqlDbType.Varchar));
                                command_insersion.Prepare();
                                command_insersion.Parameters[0].Value = txtnombre.Text.ToLower().Trim();
                                command_insersion.Parameters[1].Value = txtuser.Text.ToLower().Trim();
                                command_insersion.Parameters[2].Value = checkadmin.Checked;
                                command_insersion.Parameters[3].Value = txtpass.Text;
                                command_insersion.ExecuteNonQuery();
                                command_insersion.Parameters.Clear();
                                MessageBox.Show("Usuario agregado satisfactoriamente.", "Agregación exitosa");
                                if (ingresar_admin_oblig == true) this.DialogResult = DialogResult.OK;
                                refrezcar_ventana();
                            }
                        }
                        else MessageBox.Show("El DNI ingresado ya esta dentro de la base de datos.", "Error");
                    }
                    else MessageBox.Show("El Nombre ingresado ya esta dentro de la base de datos.", "Error");
                }
            }
        }

        private void tecla_precionada(object sender, KeyPressEventArgs e)
        {
            nueva_utilidad.verificador_campo_numero_nodecimal(sender, e);
        }

        private void enfocar(object sender, EventArgs e)
        {
            txtnombre.Focus();
        }

        private void tecla_presionada(object sender, KeyPressEventArgs e)
        {
            nueva_utilidad.verificador_campo_nombre(sender, e);
        }

        private void checked_admin(object sender, EventArgs e)
        {
            txtuser.Focus();
        }
    }
}
