using Npgsql;
using NpgsqlTypes;
using System;
using System.Windows.Forms;

namespace Interfaz_Proyecto
{
    public partial class Form3 : Form
    {
        string Comando_a_usar, seleccion, errores, nombre_orig = "";
        bool ocupado = false;
        NpgsqlConnection conexion;
        utilidades nueva_utilidad = new utilidades();

        public void rellenar_campos(string nombre)
        {
            Comando_a_usar = ("SELECT Nombre, Telefono, Mail FROM Negocio WHERE Nombre=@nom");
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
                    txttel.Text = reader[1].ToString();
                    txtmail.Text = reader[2].ToString();
                }
                reader.Close();
            }
        }

        public Form3(NpgsqlConnection conexion_recibida)
        {
            InitializeComponent();
            conexion = conexion_recibida;
            Comando_a_usar = ("SELECT Nombre FROM Negocio order by ID_Negocio");
            using (NpgsqlCommand new_command = new NpgsqlCommand(Comando_a_usar, conexion_recibida))
            {
                NpgsqlDataReader reader = new_command.ExecuteReader();
                while (reader.Read())
                {
                    lstnegocios.Items.Add(reader[0].ToString());
                    lstnegocios.Update();
                }
                reader.Close();
                lstnegocios.Refresh();
            }
        }

        private bool listar_errores()
        {
            bool hay_errores = false;
            errores = "Lista de errores:" + Environment.NewLine;
            if (lstnegocios.Text == "")
            {
                errores += "*Debe haber seleccionado un negocio de la lista." + Environment.NewLine;
                hay_errores = true;
            }
            if (txtnombre.Text.Trim() == "")
            {
                errores += "*El nombre del negocio no puede quedar vacio." + Environment.NewLine;
                hay_errores = true;
            }
            if (txtmail.Text.ToLower().Trim() != "" && nueva_utilidad.verificacion_mail(txtmail.Text) == false)
            {
                errores += "*El campo Mail debe tener una expresion valida (Ej: nombre@dominio.com)." + Environment.NewLine;
                hay_errores = true;
            }
            return hay_errores;
        }

        private void aceptar_modif(object sender, EventArgs e)
        {
            if (listar_errores() == true) MessageBox.Show(errores, "Error");
            else
            {
                if (nombre_orig != txtnombre.Text)
                {
                    Comando_a_usar = ("SELECT Verificar_Ocupacion_Negocio(@nom)");
                    using (NpgsqlCommand new_command = new NpgsqlCommand(Comando_a_usar, conexion))
                    {
                        new_command.Parameters.Add(new NpgsqlParameter("@nom", NpgsqlDbType.Varchar));
                        new_command.Prepare();
                        new_command.Parameters[0].Value = txtnombre.Text.ToLower().Trim();
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
                    using (NpgsqlCommand command = new NpgsqlCommand("SELECT Modificar_negocio (@neg, @nuevo, @mai, @tel);", conexion))
                    {
                        command.Parameters.Add(new NpgsqlParameter("@neg", NpgsqlDbType.Varchar));
                        command.Parameters.Add(new NpgsqlParameter("@nuevo", NpgsqlDbType.Varchar));
                        command.Parameters.Add(new NpgsqlParameter("@mai", NpgsqlDbType.Varchar));
                        command.Parameters.Add(new NpgsqlParameter("@tel", NpgsqlDbType.Varchar));
                        command.Prepare();
                        command.Parameters[0].Value = seleccion;
                        command.Parameters[1].Value = txtnombre.Text.ToLower().Trim(); ;
                        if (txtmail.Text == "") command.Parameters[2].Value = DBNull.Value;
                        else command.Parameters[2].Value = txtmail.Text.ToLower().Trim();
                        if (txttel.Text == "") command.Parameters[3].Value = DBNull.Value;
                        else command.Parameters[3].Value = txttel.Text.ToLower().Trim();
                        command.ExecuteNonQuery();
                        command.Parameters.Clear();
                    }
                    this.DialogResult = DialogResult.OK;
                }
                else MessageBox.Show("El nombre de negocio que intenta ingresar esta ocupado.", "Error");
            }
        }

        private void cancelar_modif(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ventana_activada(object sender, EventArgs e)
        {
            txtnombre.Focus();
        }

        private void elemento_seleccionado(object sender, EventArgs e)
        {
            seleccion = lstnegocios.Text;
            rellenar_campos(seleccion);
            txtnombre.Focus();
        }

        private void tecla_presionada_tel(object sender, KeyPressEventArgs e)
        {
            nueva_utilidad.verificador_campo_numero_nodecimal(sender, e);
        }

        private void tecla_presionada(object sender, KeyPressEventArgs e)
        {
            nueva_utilidad.verificador_campo_nombre(sender, e);
        }
    }
}
