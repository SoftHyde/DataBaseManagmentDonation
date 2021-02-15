using Npgsql;
using NpgsqlTypes;
using System;
using System.Windows.Forms;

namespace Interfaz_Proyecto
{
    public partial class Form10 : Form
    {
        NpgsqlConnection conexion;
        utilidades nueva_utilidad = new utilidades();
        string errores;

        private void rellenar_lista()
        {
            txtnumero.Items.Clear();
            lstubicaciones.Items.Clear();
            txtduenio.Items.Clear();
            for (int i = 0; i < 140; i++)
            {
                txtnumero.Items.Add((i + 1).ToString());
            }
            using (NpgsqlCommand command = new NpgsqlCommand("select (U.Numero ||' - '|| T.Nombre) from ubicacion U inner join Tipo T on U.ID_Tipo=T.ID_Tipo order by U.ID_Ubicacion", conexion))
            {
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    lstubicaciones.Items.Add(reader[0].ToString());
                    lstubicaciones.Update();
                }
                reader.Close();
                lstubicaciones.Refresh();
            }

            txtduenio.Items.Add("Sin Completar");
            using (NpgsqlCommand command = new NpgsqlCommand("select Nombre from Duenio order by ID_Duenio", conexion))
            {
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    txtduenio.Items.Add(reader[0].ToString());
                }
                reader.Close();
            }
            txtduenio.SelectedIndex = 0;
        }

        public Form10(NpgsqlConnection conexion_recibida)
        {
            InitializeComponent();
            conexion = conexion_recibida;
            rellenar_lista();
            radbtnpuesto.Checked = true;
            txtduenio.SelectedIndex = 0;
        }

        private void btncancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tecla_precionada(object sender, KeyPressEventArgs e)
        {
            nueva_utilidad.verificador_campo_numero_nodecimal(sender, e);
        }

        private bool listar_errores()
        {
            bool hay_errores = false;
            errores = "Lista de errores:" + Environment.NewLine;
            if (txtnumero.Text == "")
            {
                errores += "*Debe completar el campo 'Numero' antes de agregar una nueva ubicacion." + Environment.NewLine;
                hay_errores = true;
            }
            if (radbtnpiso.Checked == false && radbtnpuesto.Checked == false)
            {
                errores += "*Debe seleccionar si la ubicación es un Puesto o un Piso." + Environment.NewLine;
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
                using (NpgsqlCommand command = new NpgsqlCommand("Select Verificar_ubicacion(@num,@pop);", conexion))
                {
                    command.Parameters.Add(new NpgsqlParameter("@num", NpgsqlDbType.Integer));
                    command.Parameters.Add(new NpgsqlParameter("@pop", NpgsqlDbType.Varchar));
                    command.Prepare();
                    command.Parameters[0].Value = int.Parse(txtnumero.Text);
                    if (radbtnpiso.Checked) command.Parameters[1].Value = "piso";
                    else command.Parameters[1].Value = "puesto";
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (bool.Parse(reader[0].ToString()) == true) ocupacion = true;
                    }
                    reader.Close();
                    command.Parameters.Clear();
                }
                if (ocupacion == true) MessageBox.Show("La ubicación ingresada del tipo seleccionado ya existe.", "Error");
                else
                {
                    using (NpgsqlCommand command_insersion = new NpgsqlCommand("Select Ingresar_ubicacion(@num, @pop, @due);", conexion))
                    {
                        command_insersion.Parameters.Add(new NpgsqlParameter("@num", NpgsqlDbType.Integer));
                        command_insersion.Parameters.Add(new NpgsqlParameter("@pop", NpgsqlDbType.Varchar));
                        command_insersion.Parameters.Add(new NpgsqlParameter("@due", NpgsqlDbType.Varchar));
                        command_insersion.Prepare();
                        command_insersion.Parameters[0].Value = int.Parse(txtnumero.Text);
                        if (radbtnpiso.Checked) command_insersion.Parameters[1].Value = "piso";
                        else command_insersion.Parameters[1].Value = "puesto";
                        if (txtduenio.SelectedIndex == 0) command_insersion.Parameters[2].Value = DBNull.Value;
                        else command_insersion.Parameters[2].Value = txtduenio.Text;
                        command_insersion.ExecuteNonQuery();
                        command_insersion.Parameters.Clear();
                        rellenar_lista();
                    }
                }
            }
        }
    }
}