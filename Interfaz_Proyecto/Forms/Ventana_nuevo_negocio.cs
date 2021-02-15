using Npgsql;
using NpgsqlTypes;
using System;
using System.Windows.Forms;

namespace Interfaz_Proyecto
{
    public partial class Form6 : Form
    {
        NpgsqlConnection conexion;
        utilidades nueva_utilidad = new utilidades();
        string errores;

        private void rellenar_lista()
        {
            lstnegocios.Items.Clear();
            using (NpgsqlCommand command = new NpgsqlCommand("SELECT Nombre FROM Negocio order by ID_Negocio", conexion))
            {
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    lstnegocios.Items.Add(reader[0].ToString());
                    lstnegocios.Update();
                }
                reader.Close();
                lstnegocios.Refresh();
            }
        }

        private void refrezcar_ventana()
        {
            txtnombre.Text = "";
            txtmail.Text = "";
            txttel.Text = "";
            rellenar_lista();
            txtnombre.Focus();
        }

        public Form6(NpgsqlConnection conexion_rec)
        {
            InitializeComponent();
            conexion = conexion_rec;
            rellenar_lista();
        }

        private bool listar_errores()
        {
            bool hay_errores = false;
            errores = "Lista de errores:" + Environment.NewLine;
            if (txtmail.Text.ToLower().Trim() != "" && nueva_utilidad.verificacion_mail(txtmail.Text)==false)
            {
                errores += "*El campo Mail debe tener una expresion valida (Ej: nombre@dominio.com)." + Environment.NewLine;
                hay_errores = true;
            }
            if (txtnombre.Text.ToLower().Trim() == "")
            {
                errores += "*Debe completar el campo 'Nombre' antes de agregar un nuevo negocio." + Environment.NewLine;
                hay_errores = true;
            }
            return hay_errores;
        }

        private void btnaceptar_Click(object sender, EventArgs e)
        {
            if (listar_errores() == true) MessageBox.Show(errores, "Error");
            else
            {
                bool ocupacion = false;
                using (NpgsqlCommand command = new NpgsqlCommand("Select Verificar_Ocupacion_Negocio(@nom);", conexion))
                {
                    command.Parameters.Add(new NpgsqlParameter("@nom", NpgsqlDbType.Varchar));
                    command.Prepare();
                    command.Parameters[0].Value = txtnombre.Text.ToLower().Trim();
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (bool.Parse(reader[0].ToString()) == true) ocupacion = true;
                    }
                    reader.Close();
                    command.Parameters.Clear();
                }
                if (ocupacion == true) MessageBox.Show("El nombre del negocio ya existe.", "Error");
                else
                {
                    using (NpgsqlCommand command_insersion = new NpgsqlCommand("Select Ingresar_negocio(@neg, @mail, @tel);", conexion))
                    {
                        command_insersion.Parameters.Add(new NpgsqlParameter("@neg", NpgsqlDbType.Varchar));
                        command_insersion.Parameters.Add(new NpgsqlParameter("@mail", NpgsqlDbType.Varchar));
                        command_insersion.Parameters.Add(new NpgsqlParameter("@tel", NpgsqlDbType.Varchar));
                        command_insersion.Prepare();
                        command_insersion.Parameters[0].Value = txtnombre.Text.ToLower().Trim();
                        if (txtmail.Text == "") command_insersion.Parameters[1].Value = DBNull.Value;
                        else command_insersion.Parameters[1].Value = txtmail.Text.ToLower().Trim();
                        if (txttel.Text == "") command_insersion.Parameters[2].Value = DBNull.Value;
                        else command_insersion.Parameters[2].Value = txttel.Text.ToLower().Trim();
                        command_insersion.ExecuteNonQuery();
                        command_insersion.Parameters.Clear();
                        refrezcar_ventana();
                    }
                }
            }
        }

        private void btncancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tecla_presionada(object sender, KeyPressEventArgs e)
        {
            nueva_utilidad.verificador_campo_nombre(sender, e);
        }

        private void tecla_presionada_tel(object sender, KeyPressEventArgs e)
        {
            nueva_utilidad.verificador_campo_numero_nodecimal(sender, e);
        }

        private void activacion_ventana(object sender, EventArgs e)
        {
            txtnombre.Focus();
        }
    }
}
