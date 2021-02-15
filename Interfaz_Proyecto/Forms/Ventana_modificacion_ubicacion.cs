using Npgsql;
using NpgsqlTypes;
using System;
using System.Windows.Forms;

namespace Interfaz_Proyecto
{
    public partial class Form15 : Form
    {
        NpgsqlConnection conexion;
        string errores;

        public void rellenar_duenios()
        {
            lstduenio.Items.Add("Sin Completar");
            using (NpgsqlCommand command = new NpgsqlCommand("SELECT Nombre FROM Duenio order by ID_Duenio", conexion))
            {
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    lstduenio.Items.Add(reader[0].ToString());
                    lstduenio.Update();
                }
                reader.Close();
                lstduenio.Refresh();
            }
        }
        public Form15(NpgsqlConnection conexion_recibida)
        {
            InitializeComponent();
            conexion = conexion_recibida;
            chkpuesto.Checked = true;
            rellenar_duenios();
        }

        private void btncancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void seleccion_puesto(object sender, EventArgs e)
        {
            lstnumeros.Items.Clear();
            using (NpgsqlCommand new_command = new NpgsqlCommand("select Numero from Ubicacion where ID_Tipo='2' order by Numero;", conexion))
            {
                NpgsqlDataReader reader = new_command.ExecuteReader();
                while (reader.Read())
                {
                    lstnumeros.Items.Add(reader[0].ToString());
                }
                reader.Close();
            }
        }

        private void seleccion_piso(object sender, EventArgs e)
        {
            lstnumeros.Items.Clear();
            using (NpgsqlCommand new_command = new NpgsqlCommand("select Numero from Ubicacion where ID_Tipo='1' order by Numero;", conexion))
            {
                NpgsqlDataReader reader = new_command.ExecuteReader();
                while (reader.Read())
                {
                    lstnumeros.Items.Add(reader[0].ToString());
                }
                reader.Close();
            }
        }

        private void seleccion_numero(object sender, EventArgs e)
        {
            using (NpgsqlCommand command = new NpgsqlCommand("select ID_Duenio from Ubicacion where Numero=@num and ID_Tipo=@tipo;", conexion))
            {
                command.Parameters.Add(new NpgsqlParameter("@num", NpgsqlDbType.Integer));
                command.Parameters.Add(new NpgsqlParameter("@tipo", NpgsqlDbType.Bigint));
                command.Prepare();
                command.Parameters[0].Value = int.Parse(lstnumeros.Text);
                if (chkpuesto.Checked == true) command.Parameters[1].Value = 2;
                else command.Parameters[1].Value = 1;
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader[0] == DBNull.Value) lstduenio.SelectedIndex = 0;
                    else lstduenio.SelectedIndex = int.Parse(reader[0].ToString()) + 1;
                }
                reader.Close();
                command.Parameters.Clear();
            }
        }

        private bool listar_errores()
        {
            bool hay_errores = false;
            errores = "Lista de errores:" + Environment.NewLine;
            if (lstnumeros.Text == "" || lstduenio.Text == "")
            {
                errores += "*Debe seleccionar un tipo de ubicacion, un numero y completar el campo dueño para poder continuar." + Environment.NewLine;
                hay_errores = true;
            }
            return hay_errores;
        }

        private void btnaceptar_Click(object sender, EventArgs e)
        {
            if (listar_errores() == true) MessageBox.Show(errores, "Error");
            else
            {
                using (NpgsqlCommand command = new NpgsqlCommand("select Modificar_ubicacion(@num, @pop, @duen);", conexion))
                {
                    command.Parameters.Add(new NpgsqlParameter("@num", NpgsqlDbType.Integer));
                    command.Parameters.Add(new NpgsqlParameter("@pop", NpgsqlDbType.Varchar));
                    command.Parameters.Add(new NpgsqlParameter("@duen", NpgsqlDbType.Bigint));
                    command.Prepare();
                    command.Parameters[0].Value = int.Parse(lstnumeros.Text);
                    if (chkpuesto.Checked == true) command.Parameters[1].Value = "puesto";
                    else command.Parameters[1].Value = "piso";
                    if (lstduenio.SelectedIndex == 0) command.Parameters[2].Value = DBNull.Value;
                    else command.Parameters[2].Value = lstduenio.SelectedIndex - 1;
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
