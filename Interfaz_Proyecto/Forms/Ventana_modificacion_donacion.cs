using Npgsql;
using NpgsqlTypes;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Interfaz_Proyecto
{
    public partial class Form4 : Form
    {
        string Comando_a_usar, negocio, produc, errores;
        String[] nombres = new[] { "1", "2", "3", "4" };
        NpgsqlConnection conexion;
        DateTime fecha_var;
        utilidades nueva_utilidad = new utilidades();

        private void rellenar_listas_vol()
        {
            txtvol1.Items.Add("Sin Completar");
            txtvol2.Items.Add("Sin Completar");
            txtvol3.Items.Add("Sin Completar");
            txtvol4.Items.Add("Sin Completar");
            using (NpgsqlCommand command = new NpgsqlCommand("SELECT Nombre From Voluntario order by ID_Voluntario;", conexion))
            {
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    txtvol1.Items.Add(reader[0].ToString());
                    txtvol2.Items.Add(reader[0].ToString());
                    txtvol3.Items.Add(reader[0].ToString());
                    txtvol4.Items.Add(reader[0].ToString());
                }
                reader.Close();
            }
        }

        private void tecla_precionada(object sender, KeyPressEventArgs e)
        {
            nueva_utilidad.verificador_campo_numero(e, txtpeso.Text);
        }

        private bool verificar_voluntarios_repetidos(String[] valores)
        {
            bool resultado = valores.Distinct().Count() == valores.Length;
            nombres[0] = "1";
            nombres[1] = "2";
            nombres[2] = "3";
            nombres[3] = "4";
            return resultado;
        }

        private void rellenar_string()
        {
            nombres[0] = txtvol1.Text;
            if (txtvol2.Text != "Sin Completar") nombres[1] = txtvol2.Text;
            if (txtvol3.Text != "Sin Completar") nombres[2] = txtvol2.Text;
            if (txtvol3.Text != "Sin Completar") nombres[3] = txtvol2.Text;
        }

        public Form4(NpgsqlConnection conexion_recibida, DateTime fecha, string neg, string prod)
        {
            InitializeComponent();
            conexion = conexion_recibida;
            rellenar_listas_vol();

            fecha_var = fecha;
            negocio = neg;
            produc = prod;
            Comando_a_usar = ("select D.Peso, V1.ID_Voluntario, V2.ID_Voluntario, V3.ID_Voluntario, V4.ID_Voluntario from Donacion D left join Voluntario V1 on D.ID_Voluntario1=V1.ID_Voluntario left join Voluntario V2 on D.ID_Voluntario2 = V2.ID_Voluntario left join Voluntario V3 on D.ID_Voluntario3 = V3.ID_Voluntario left join Voluntario V4 on D.ID_Voluntario4 = V4.ID_Voluntario WHERE D.Fecha=@fecha and D.ID_Producto=(select ID_Producto from Producto where Nombre=@nom) and D.ID_Negocio=(select ID_Negocio from negocio where Nombre=@neg)");
            using (NpgsqlCommand new_command = new NpgsqlCommand(Comando_a_usar, conexion_recibida))
            {
                new_command.Parameters.Add(new NpgsqlParameter("@fecha", NpgsqlDbType.Date));
                new_command.Parameters.Add(new NpgsqlParameter("@nom", NpgsqlDbType.Varchar));
                new_command.Parameters.Add(new NpgsqlParameter("@neg", NpgsqlDbType.Varchar));
                new_command.Prepare();
                new_command.Parameters[0].Value = fecha;
                new_command.Parameters[1].Value = prod;
                new_command.Parameters[2].Value = neg;
                NpgsqlDataReader reader = new_command.ExecuteReader();
                while (reader.Read())
                {
                    txtpeso.Text = reader[0].ToString();
                    txtvol1.SelectedIndex = (reader[1].ToString() == "") ? 0 : int.Parse(reader[1].ToString()) + 1;
                    txtvol2.SelectedIndex = (reader[2].ToString() == "") ? 0 : int.Parse(reader[2].ToString()) + 1;
                    txtvol3.SelectedIndex = (reader[3].ToString() == "") ? 0 : int.Parse(reader[3].ToString()) + 1;
                    txtvol4.SelectedIndex = (reader[4].ToString() == "") ? 0 : int.Parse(reader[4].ToString()) + 1;
                }
                reader.Close();
            }
        }

        private bool listar_errores()
        {
            bool hay_errores = false;
            errores = "Lista de errores:" + Environment.NewLine;
            if (txtpeso.Text == "" || txtvol1.Text == "Sin Completar")
            {
                errores += "*Debe completar todos los campos obligatorios (*)." + Environment.NewLine;
                hay_errores = true;
            }
            if (txtpeso.Text == ".")
            {
                errores += "*Debe ingresar un numero valido en el campo 'Peso'." + Environment.NewLine;
                hay_errores = true;
            }
            if (float.Parse(nueva_utilidad.remp_coma_punto(txtpeso.Text)) == 0)
            {
                errores += "*Debe ingresar un valor mayor a 0 en el campo 'Peso'." + Environment.NewLine;
                hay_errores = true;
            }
            return hay_errores;
        }

        private void btnaceptar_Click(object sender, EventArgs e)
        {
            if (listar_errores() == true) MessageBox.Show(errores, "Error");
            else
            {
                rellenar_string();
                if (verificar_voluntarios_repetidos(nombres) == true)
                {
                    using (NpgsqlCommand command = new NpgsqlCommand("SELECT Modificar_donacion (@prod, @neg, @fecha, @vol1, @peso, @vol2, @vol3, @vol4);", conexion))
                    {
                        command.Parameters.Add(new NpgsqlParameter("@prod", NpgsqlDbType.Varchar));
                        command.Parameters.Add(new NpgsqlParameter("@neg", NpgsqlDbType.Varchar));
                        command.Parameters.Add(new NpgsqlParameter("@fecha", NpgsqlDbType.Date));
                        command.Parameters.Add(new NpgsqlParameter("@vol1", NpgsqlDbType.Varchar));
                        command.Parameters.Add(new NpgsqlParameter("@peso", NpgsqlDbType.Real));
                        command.Parameters.Add(new NpgsqlParameter("@vol2", NpgsqlDbType.Varchar));
                        command.Parameters.Add(new NpgsqlParameter("@vol3", NpgsqlDbType.Varchar));
                        command.Parameters.Add(new NpgsqlParameter("@vol4", NpgsqlDbType.Varchar));
                        command.Prepare();
                        command.Parameters[0].Value = produc;
                        command.Parameters[1].Value = negocio;
                        command.Parameters[2].Value = fecha_var;
                        if (txtvol1.Text == "Sin Completar") command.Parameters[3].Value = DBNull.Value;
                        else command.Parameters[3].Value = txtvol1.Text;
                        command.Parameters[4].Value = float.Parse(nueva_utilidad.remp_coma_punto(txtpeso.Text));
                        if (txtvol2.Text == "Sin Completar") command.Parameters[5].Value = DBNull.Value;
                        else command.Parameters[5].Value = txtvol2.Text;
                        if (txtvol3.Text == "Sin Completar") command.Parameters[6].Value = DBNull.Value;
                        else command.Parameters[6].Value = txtvol3.Text;
                        if (txtvol4.Text == "Sin Completar") command.Parameters[7].Value = DBNull.Value;
                        else command.Parameters[7].Value = txtvol4.Text;
                        command.ExecuteNonQuery();
                        command.Parameters.Clear();
                    }
                    this.DialogResult = DialogResult.OK;
                }
                else MessageBox.Show("Ha seleccionado voluntarios repetidos.", "Error");
            }
        }

        private void btncancear_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
