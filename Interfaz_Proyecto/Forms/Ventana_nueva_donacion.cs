using Npgsql;
using NpgsqlTypes;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Interfaz_Proyecto
{
    public partial class Ventana_Agregar : Form
    {
        Elemento_Donacion nueva_fila;
        DateTime fecha_rec;
        string Comando_a_usar, errores;
        String[] nombres = new[] { "1", "2", "3", "4" };
        NpgsqlConnection conexion;
        utilidades nueva_utilidad = new utilidades();

        private void rellenar_listas_voluntarios()
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

        private bool verificar_voluntarios_repetidos(String[] valores)
        {
            bool resultado = valores.Distinct().Count() == valores.Count();
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
            if (txtvol3.Text != "Sin Completar") nombres[2] = txtvol3.Text;
            if (txtvol3.Text != "Sin Completar") nombres[3] = txtvol4.Text;
        }

        private void rellenar_listas_productos()
        {
            using (NpgsqlCommand command = new NpgsqlCommand("SELECT Nombre From Producto order by ID_Producto;", conexion))
            {
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    txtproducto.Items.Add(reader[0].ToString());
                }
                reader.Close();
            }
        }

        private void rellenar_listas_negocios()
        {
            using (NpgsqlCommand command = new NpgsqlCommand("SELECT Nombre From Negocio order by ID_Negocio;", conexion))
            {
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    txtnegocio.Items.Add(reader[0].ToString());
                }
                reader.Close();
            }
        }

        private void inicializar_listas()
        {
            rellenar_listas_voluntarios();
            rellenar_listas_productos();
            rellenar_listas_negocios();
        }

        public Ventana_Agregar(DateTime fecha, NpgsqlConnection conexion_recibida)
        {
            InitializeComponent();
            fecha_rec = fecha;
            conexion = conexion_recibida;
            inicializar_listas();
            voluntarios_iniciales();
        }

        public Elemento_Donacion Ver_elemento()
        {
            return nueva_fila;
        }

        private void btncancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool listar_errores()
        {
            bool hay_errores = false;
            errores = "Lista de errores:" + Environment.NewLine;
            if (txtvol1.Text == "Sin Completar" || txtpeso.Text == "" || txtnumubic.Text == "" || txtnegocio.Text == "" || txtproducto.Text == "")
            {
                errores += "*Debe completar todos los campos obligatorios (*) antes de agregar una nueva donación." + Environment.NewLine;
                hay_errores = true;
            }
            if (radbtnpiso.Checked == false && radbtnpuesto.Checked == false)
            {
                errores += "*Debe seleccionar si la ubicación es un Puesto o un Piso." + Environment.NewLine;
                hay_errores = true;
            }
            if (txtpeso.Text == ".")
            {
                errores += "*Debe ingresar un numero valido en el campo 'Peso'." + Environment.NewLine;
                hay_errores = true;
            }
            if (txtpeso.Text != "" && txtpeso.Text != "." && float.Parse(nueva_utilidad.remp_coma_punto(txtpeso.Text)) == 0)
            {
                errores += "*Debe ingresar un peso mayor a 0." + Environment.NewLine;
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
                if (radbtnpuesto.Checked == true)
                {
                    using (NpgsqlCommand new_command = new NpgsqlCommand("SELECT verificar_puesto(@num, @neg)", conexion))
                    {
                        new_command.Parameters.Add(new NpgsqlParameter("@num", NpgsqlDbType.Integer));
                        new_command.Parameters.Add(new NpgsqlParameter("@neg", NpgsqlDbType.Varchar));
                        new_command.Prepare();
                        new_command.Parameters[0].Value = int.Parse(txtnumubic.Text);
                        new_command.Parameters[1].Value = txtnegocio.Text;
                        NpgsqlDataReader reader = new_command.ExecuteReader();
                        while (reader.Read())
                        {
                            ocupacion = Boolean.Parse(reader[0].ToString());
                        }
                        reader.Close();
                        new_command.Parameters.Clear();
                    }
                }
                if (ocupacion == true)
                {
                    MessageBox.Show("La ubicación " + txtnumubic.Text + " seleccionada de tipo puesto ya tiene un negocio asignado, seleccione otra ubicación.", "Error");
                }
                else
                {
                    bool nueva_ocupacion = false;
                    using (NpgsqlCommand new_command = new NpgsqlCommand("SELECT verificar_unicidad(@fecha, @neg, @prod)", conexion))
                    {
                        new_command.Parameters.Add(new NpgsqlParameter("@fecha", NpgsqlDbType.Date));
                        new_command.Parameters.Add(new NpgsqlParameter("@neg", NpgsqlDbType.Varchar));
                        new_command.Parameters.Add(new NpgsqlParameter("@prod", NpgsqlDbType.Varchar));
                        new_command.Prepare();
                        new_command.Parameters[0].Value = fecha_rec;
                        new_command.Parameters[1].Value = txtnegocio.Text;
                        new_command.Parameters[2].Value = txtproducto.Text;
                        NpgsqlDataReader reader = new_command.ExecuteReader();
                        while (reader.Read())
                        {
                            nueva_ocupacion = Boolean.Parse(reader[0].ToString());
                        }
                        reader.Close();
                        new_command.Parameters.Clear();
                    }
                    if (nueva_ocupacion == false) MessageBox.Show("Ya se registro una donacion el dia de hoy para el negocio " + txtnegocio.Text + " y el producto " + txtproducto.Text + ". Puede editar dicha donacion modificando el peso total en la ventana principal.", "Error");
                    else
                    {
                        rellenar_string();
                        if (verificar_voluntarios_repetidos(nombres) == true)
                        {
                            nueva_fila = new Elemento_Donacion
                            {
                                vol1 = txtvol1.Text,
                                vol2 = txtvol2.Text,
                                vol3 = txtvol3.Text,
                                vol4 = txtvol4.Text,
                                nomprod = txtproducto.Text,
                                nomneg = txtnegocio.Text,
                                ubic = int.Parse(txtnumubic.Text),
                                peso = float.Parse(nueva_utilidad.remp_coma_punto(txtpeso.Text)),
                                puesto = radbtnpuesto.Checked,
                                piso = radbtnpiso.Checked,
                                fecha = fecha_rec
                            };
                            this.DialogResult = DialogResult.OK;
                        }
                        else MessageBox.Show("Ha seleccionado voluntarios repetidos.", "Error");
                    }
                }
            }
        }

        //Se encarga de filtrar la lista de numeros de ubicacion unicamente con las de tipo puesto
        private void seleccion_puesto(object sender, EventArgs e)
        {
            txtnumubic.Items.Clear();
            using (NpgsqlCommand new_command = new NpgsqlCommand("select Numero from Ubicacion where ID_Tipo='2' order by Numero;", conexion))
            {
                NpgsqlDataReader reader = new_command.ExecuteReader();
                while (reader.Read())
                {
                    txtnumubic.Items.Add(reader[0].ToString());
                }
                reader.Close();
            }
        }

        private void tecla_precionada(object sender, KeyPressEventArgs e)
        {
            nueva_utilidad.verificador_campo_numero(e, txtpeso.Text);
        }

        private void voluntarios_iniciales() //se encarga de seleccionar los voluntarios, el negocio, el tipo y numero de ubicacion de la ultima donacion al abrir la ventana
        {
            NpgsqlCommand command = new NpgsqlCommand("SELECT recom_ult_don();", conexion);
            command.ExecuteNonQuery();
            Comando_a_usar = ("SELECT * FROM Auxiliar");
            string tipo = "puesto", num = "vacio";
            using (NpgsqlCommand new_command = new NpgsqlCommand(Comando_a_usar, conexion))
            {
                NpgsqlDataReader reader = new_command.ExecuteReader();
                while (reader.Read())
                {
                    txtvol1.SelectedIndex = (reader[1].ToString() == "") ? 0 : int.Parse(reader[1].ToString()) + 1;
                    txtvol2.SelectedIndex = (reader[2].ToString() == "") ? 0 : int.Parse(reader[2].ToString()) + 1;
                    txtvol3.SelectedIndex = (reader[3].ToString() == "") ? 0 : int.Parse(reader[3].ToString()) + 1;
                    txtvol4.SelectedIndex = (reader[4].ToString() == "") ? 0 : int.Parse(reader[4].ToString()) + 1;
                    if (reader[5].ToString() != "") txtnegocio.SelectedIndex = int.Parse(reader[5].ToString());
                    if (reader[7].ToString() == "piso") tipo = "piso";
                    if (reader[6].ToString() != "") num = reader[6].ToString();
                }
                reader.Close();
            }
            if (tipo == "piso") radbtnpiso.Checked = true;
            else radbtnpuesto.Checked = true;
            if (num != "vacio") txtnumubic.SelectedIndex = txtnumubic.FindString(num);
        }

        //Se encarga de filtrar la lista de numeros de ubicacion unicamente con las de tipo piso
        private void seleccion_piso(object sender, EventArgs e)
        {
            txtnumubic.Items.Clear();
            using (NpgsqlCommand new_command = new NpgsqlCommand("select Numero from Ubicacion where ID_Tipo='1' order by Numero;", conexion))
            {
                NpgsqlDataReader reader = new_command.ExecuteReader();
                while (reader.Read())
                {
                    txtnumubic.Items.Add(reader[0].ToString());
                }
                reader.Close();
            }
        }

        private void seleccion_producto(object sender, EventArgs e)
        {
            txtpeso.Focus();
        }

        //Se encarga de autocompletar el negocio si se selecciona un puesto que ya registra una donacion
        private void seleccion_numero(object sender, EventArgs e)
        {
            if (radbtnpuesto.Checked && txtnumubic.Text != "")
            {
                using (NpgsqlCommand command = new NpgsqlCommand("SELECT obtener_info_puesto (@ubic);", conexion))
                {
                    command.Parameters.Add(new NpgsqlParameter("@ubic", NpgsqlDbType.Integer));
                    command.Prepare();
                    command.Parameters[0].Value = int.Parse(txtnumubic.Text);
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
                Comando_a_usar = ("SELECT * FROM Auxiliar_ID");
                using (NpgsqlCommand new_command = new NpgsqlCommand(Comando_a_usar, conexion))
                {
                    NpgsqlDataReader reader = new_command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader[0].ToString() != "") txtnegocio.SelectedIndex = int.Parse(reader[0].ToString());
                    }
                    reader.Close();
                }
            }
        }
    }
}
