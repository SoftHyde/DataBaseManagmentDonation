using Npgsql;
using NpgsqlTypes;
using System;
using System.Data;
using System.Windows.Forms;

namespace Interfaz_Proyecto
{
    public partial class Form1 : Form
    {
        //Declaracion de variables necesarias
        NpgsqlConnection conexion = new NpgsqlConnection();
        string Comando_a_usar;
        elemento_usuario nuevo_user;
        bool user_admin, tabla_filtrada = false; //user_admin se usa para determinar si el programa esta funcionando en modo invitado o administrador
        DataTable tablita; //tabla que se muestra en la grilla

        //Declaracion de funciones necesarias
        //Una funcion que llama a otras que se encarga de realizar todas las actualizaciones de la pantalla
        void refrezcar_tabla(bool admin, bool filtro)
        {
            inicializar_tabla();
            Cargar_tabla(filtro);
            verificar_botones(admin);
        }
        //Se usa para bloquear ciertos botones segun el caso (EJ: bloquear botones para invitados o bloquear botones de modificar o eliminar cuando no hay datos en la tabla)
        void verificar_botones(bool admin) // false=invitado, true=admin
        {
            if (admin == false)
            {
                btngraficos.Enabled = false;
                btneliminar.Enabled = false;
                menuStrip1.Items[0].Enabled = false;
                menuStrip1.Items[2].Enabled = false;
                if (lista_donaciones.Rows.Count == 0)
                {
                    btnmoddon.Enabled = false;
                    menuStrip1.Items[1].Enabled = false;
                }
                else
                {
                    btnmoddon.Enabled = true;
                    menuStrip1.Items[1].Enabled = true;
                }
            }
            else
            {
                if (lista_donaciones.Rows.Count == 0)
                {
                    btneliminar.Enabled = false;
                    btnmoddon.Enabled = false;
                    btngraficos.Enabled = false;
                    menuStrip1.Items[1].Enabled = false;
                }
                else
                {
                    btneliminar.Enabled = true;
                    btnmoddon.Enabled = true;
                    btngraficos.Enabled = true;
                    menuStrip1.Items[1].Enabled = true;
                }
            }
        }
        //Crea el objeto tabla, que despues se usara para rellenar la planilla de la ventana principal
        void inicializar_tabla()
        {
            tablita = new DataTable();
            tablita.Columns.Add("Fecha", typeof(DateTime));
            tablita.Columns.Add("Voluntario 1", typeof(string));
            tablita.Columns.Add("Voluntario 2", typeof(string));
            tablita.Columns.Add("Voluntario 3", typeof(string));
            tablita.Columns.Add("Voluntario 4", typeof(string));
            tablita.Columns.Add("Negocio", typeof(string));
            tablita.Columns.Add("Número", typeof(int));
            tablita.Columns.Add("¿Puesto o Piso?", typeof(string));
            tablita.Columns.Add("Teléfono", typeof(string));
            tablita.Columns.Add("Mail", typeof(string));
            tablita.Columns.Add("Dueño", typeof(string));
            tablita.Columns.Add("Producto", typeof(string));
            tablita.Columns.Add("Peso", typeof(float));
            lista_donaciones.DataSource = tablita;
            lista_donaciones.Columns["Fecha"].SortMode = DataGridViewColumnSortMode.NotSortable;
            lista_donaciones.Columns["Voluntario 1"].SortMode = DataGridViewColumnSortMode.NotSortable;
            lista_donaciones.Columns["Voluntario 2"].SortMode = DataGridViewColumnSortMode.NotSortable;
            lista_donaciones.Columns["Voluntario 3"].SortMode = DataGridViewColumnSortMode.NotSortable;
            lista_donaciones.Columns["Voluntario 4"].SortMode = DataGridViewColumnSortMode.NotSortable;
            lista_donaciones.Columns["Negocio"].SortMode = DataGridViewColumnSortMode.NotSortable;
            lista_donaciones.Columns["Número"].SortMode = DataGridViewColumnSortMode.NotSortable;
            lista_donaciones.Columns["¿Puesto o Piso?"].SortMode = DataGridViewColumnSortMode.NotSortable;
            lista_donaciones.Columns["Teléfono"].SortMode = DataGridViewColumnSortMode.NotSortable;
            lista_donaciones.Columns["Mail"].SortMode = DataGridViewColumnSortMode.NotSortable;
            lista_donaciones.Columns["Dueño"].SortMode = DataGridViewColumnSortMode.NotSortable;
            lista_donaciones.Columns["Producto"].SortMode = DataGridViewColumnSortMode.NotSortable;
            lista_donaciones.Columns["Peso"].SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        //Aqui se carga en la tabla cada fila de datos de las donaciones (que llegan de la base de datos)
        void Cargar_tabla(bool filtro) //La uso para regenerar la tabla
        {
            if (filtro == false)
            {
                Comando_a_usar = ("SELECT obtener_tabla();");
                NpgsqlCommand command = new NpgsqlCommand(Comando_a_usar, conexion);
                command.ExecuteNonQuery();
            }
            else
            {
                Comando_a_usar = ("SELECT obtener_tabla_filtrada(@fecha);");
                using (NpgsqlCommand command = new NpgsqlCommand(Comando_a_usar, conexion))
                {
                    command.Parameters.Add(new NpgsqlParameter("@fecha", NpgsqlDbType.Date));
                    command.Prepare();
                    command.Parameters[0].Value = calendario.SelectionStart;
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
            }

            Comando_a_usar = ("SELECT * FROM auxiliar");
            using (NpgsqlCommand new_command = new NpgsqlCommand(Comando_a_usar, conexion))
            {
                NpgsqlDataReader reader = new_command.ExecuteReader();
                while (reader.Read())
                {
                    DataRow fila = tablita.NewRow();
                    fila[0] = reader[0].ToString();
                    fila[1] = reader[1].ToString();
                    fila[2] = reader[2].ToString();
                    fila[3] = reader[3].ToString();
                    fila[4] = reader[4].ToString();
                    fila[5] = reader[5].ToString();
                    fila[6] = reader[6].ToString();
                    fila[7] = reader[7].ToString();
                    fila[8] = reader[8].ToString();
                    fila[9] = reader[9].ToString();
                    fila[10] = reader[10].ToString();
                    fila[11] = reader[11].ToString();
                    fila[12] = reader[12].ToString();
                    tablita.Rows.Add(fila);
                }
                reader.Close();
                lista_donaciones.Refresh();
            }
        }

        public Form1()
        {
            InitializeComponent();
            //Inicializacion
            ventana_ingreso_db nueva_ventana_db = new ventana_ingreso_db();
            if (nueva_ventana_db.ShowDialog() == DialogResult.OK) conexion = nueva_ventana_db.retornar_conexion();
            else
            {
                Application.Exit();
                Environment.Exit(0);
            }
            /*CadenaConexion = "Server = 127.0.0.1; Port = 5432; Database = postgres;";
            CadenaConexion += "User ID = postgres;";
            CadenaConexion += "; Password = lukksdevilre5687";*/
            if (conexion.State.ToString() == "Open")
            {
                bool admins = false;
                using (NpgsqlCommand command = new NpgsqlCommand("Select verificar_users_admin()", conexion))
                {
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        admins = bool.Parse(reader[0].ToString());
                    }
                    reader.Close();
                }
                //Sigue este camino si ya existen usuarios administradores ingresados en la base de datos
                if (admins == true)
                {
                    Form13 nueva_ventana = new Form13(conexion);
                    if (nueva_ventana.ShowDialog() == DialogResult.OK)
                    {
                        nuevo_user = nueva_ventana.Ver_elemento();
                        user_admin = nuevo_user.es_Admin;
                        refrezcar_tabla(user_admin, tabla_filtrada);
                    }
                    else
                    {
                        Application.Exit();
                        Environment.Exit(0);
                    }
                }
                //Como no existen administradores ingresados en la base de datos te obliga a que ingreses al menos 1 y luego lo llevo a la ventana de iniciar sesion
                else
                {
                    Form11 nueva_ventana = new Form11(conexion, true);
                    if (nueva_ventana.ShowDialog() == DialogResult.OK)
                    {
                        Form13 nueva_ventana2 = new Form13(conexion);
                        if (nueva_ventana2.ShowDialog() == DialogResult.OK)
                        {
                            nuevo_user = nueva_ventana2.Ver_elemento();
                            user_admin = nuevo_user.es_Admin;
                            refrezcar_tabla(user_admin, tabla_filtrada);
                        }
                        else
                        {
                            Application.Exit();
                            Environment.Exit(0);
                        }
                    }
                    else
                    {
                        Application.Exit();
                        Environment.Exit(0);
                    }
                }
            }
        }

        //Se ejecuta al hacer click en agregar entrada. Si esta finaliza con un ok, llamo a la funcion que agrega una nueva donacion y actualizo la tabla
        private void agregar_entrada(object sender, EventArgs e)
        {
            Ventana_Agregar nueva_ventana = new Ventana_Agregar(calendario.SelectionStart, conexion);
            if (nueva_ventana.ShowDialog() == DialogResult.OK)
            {
                Elemento_Donacion nuevo_elemento = nueva_ventana.Ver_elemento();
                using (NpgsqlCommand command_insersion = new NpgsqlCommand("Select Insertar_donacion(@vol1, @nom_prod, @peso, @pop, @user, @numero, @negocio, @vol2, @vol3, @vol4, @date);", conexion))
                {
                    command_insersion.Parameters.Add(new NpgsqlParameter("@vol1", NpgsqlDbType.Varchar));
                    command_insersion.Parameters.Add(new NpgsqlParameter("@nom_prod", NpgsqlDbType.Varchar));
                    command_insersion.Parameters.Add(new NpgsqlParameter("@peso", NpgsqlDbType.Real));
                    command_insersion.Parameters.Add(new NpgsqlParameter("@pop", NpgsqlDbType.Varchar));
                    command_insersion.Parameters.Add(new NpgsqlParameter("@user", NpgsqlDbType.Varchar));
                    command_insersion.Parameters.Add(new NpgsqlParameter("@numero", NpgsqlDbType.Integer));
                    command_insersion.Parameters.Add(new NpgsqlParameter("@negocio", NpgsqlDbType.Varchar));
                    command_insersion.Parameters.Add(new NpgsqlParameter("@vol2", NpgsqlDbType.Varchar));
                    command_insersion.Parameters.Add(new NpgsqlParameter("@vol3", NpgsqlDbType.Varchar));
                    command_insersion.Parameters.Add(new NpgsqlParameter("@vol4", NpgsqlDbType.Varchar));
                    command_insersion.Parameters.Add(new NpgsqlParameter("@date", NpgsqlDbType.Date));
                    command_insersion.Prepare();
                    command_insersion.Parameters[0].Value = nuevo_elemento.vol1;
                    command_insersion.Parameters[1].Value = nuevo_elemento.nomprod;
                    command_insersion.Parameters[2].Value = nuevo_elemento.peso;
                    if (nuevo_elemento.puesto) command_insersion.Parameters[3].Value = "puesto";
                    else command_insersion.Parameters[3].Value = "piso";
                    command_insersion.Parameters[4].Value = nuevo_user.nombre;
                    command_insersion.Parameters[5].Value = nuevo_elemento.ubic;
                    command_insersion.Parameters[6].Value = nuevo_elemento.nomneg;
                    if (nuevo_elemento.vol2 == "Sin Completar") command_insersion.Parameters[7].Value = DBNull.Value;
                    else command_insersion.Parameters[7].Value = nuevo_elemento.vol2;
                    if (nuevo_elemento.vol3 == "Sin Completar") command_insersion.Parameters[8].Value = DBNull.Value;
                    else command_insersion.Parameters[8].Value = nuevo_elemento.vol3;
                    if (nuevo_elemento.vol4 == "Sin Completar") command_insersion.Parameters[9].Value = DBNull.Value;
                    else command_insersion.Parameters[9].Value = nuevo_elemento.vol4;
                    command_insersion.Parameters[10].Value = nuevo_elemento.fecha;
                    command_insersion.ExecuteNonQuery();
                    command_insersion.Parameters.Clear();
                    refrezcar_tabla(user_admin, tabla_filtrada);
                }
            }
        }

        //Se ejecuta al hacer click en eliminar. Llamo a la funcion que elimina una donacion y actualizo la tabla
        private void btneliminar_Click(object sender, EventArgs e)
        {
            DataGridViewCellCollection coleccion_celdas = lista_donaciones.CurrentRow.Cells;
            if (coleccion_celdas.Count == 0)
            {
                MessageBox.Show("No se ha seleccionado o no existe ninguna donación registrada.", "Error");
            }
            else
            {
                DialogResult resultado = MessageBox.Show("¿Esta seguro que desea eliminar la donación seleccionada?", "Advertencia", MessageBoxButtons.YesNo);
                if (resultado == DialogResult.Yes)
                {
                    DateTime aux_fecha = DateTime.Parse(coleccion_celdas[0].Value.ToString());
                    string aux_Negocio = coleccion_celdas[5].Value.ToString();
                    string aux_Producto = coleccion_celdas[11].Value.ToString();

                    using (NpgsqlCommand command_eliminacion = new NpgsqlCommand("select Eliminar_donacion(@auxfecha,@auxnegocio, @auxproducto);", conexion))
                    {
                        command_eliminacion.Parameters.Add(new NpgsqlParameter("@auxfecha", NpgsqlDbType.Date));
                        command_eliminacion.Parameters.Add(new NpgsqlParameter("@auxnegocio", NpgsqlDbType.Varchar));
                        command_eliminacion.Parameters.Add(new NpgsqlParameter("@auxproducto", NpgsqlDbType.Varchar));
                        command_eliminacion.Prepare();
                        command_eliminacion.Parameters[0].Value = aux_fecha;
                        command_eliminacion.Parameters[1].Value = aux_Negocio;
                        command_eliminacion.Parameters[2].Value = aux_Producto;
                        command_eliminacion.ExecuteNonQuery();
                        command_eliminacion.Parameters.Clear();
                        refrezcar_tabla(user_admin, tabla_filtrada);
                    }
                }
            }
        }

        //Se ejecuta al hacer click en modificar. Si esta finaliza con un ok, llamo a la funcion que modifica una donacion y actualizo la tabla
        private void button1_Click(object sender, EventArgs e)
        {
            if (lista_donaciones.SelectedRows.Count > 0)
            {
                DataGridViewCellCollection coleccion_celdas = lista_donaciones.CurrentRow.Cells;
                DateTime aux_fecha = DateTime.Parse(coleccion_celdas[0].Value.ToString());
                string aux_Negocio = coleccion_celdas[5].Value.ToString();
                string aux_Producto = coleccion_celdas[11].Value.ToString();
                Form4 nueva_ventana = new Form4(conexion, aux_fecha, aux_Negocio, aux_Producto);
                if (nueva_ventana.ShowDialog() == DialogResult.OK)
                {
                    refrezcar_tabla(user_admin, tabla_filtrada);
                }
            }
        }

        //Se ejecuta al hacer click en el boton resumenes, abriendo dicha ventana
        private void btngraficos_Click(object sender, EventArgs e)
        {
            Form5 nueva_ventana = new Form5(conexion);
            nueva_ventana.ShowDialog();
        }

        //Lleva a cabo la exportacion de la tabla, creando una instancia de la clase exortacion
        private void personalizarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new exportacion().exportar_tabla(lista_donaciones);
        }

        //Se ejecuta al hacer click en el boton menu-ayuda-acerca de, abriendo dicha ventana
        private void acercadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form7 nueva_ventana = new Form7();
            nueva_ventana.ShowDialog();
        }

        //Se ejecuta al hacer click en el boton menu-administrar-negocio-modificar, abriendo dicha ventana
        private void menu_mod_neg_Click(object sender, EventArgs e)
        {
            Form3 nueva_ventana = new Form3(conexion);
            if (nueva_ventana.ShowDialog() == DialogResult.OK)
            {
                refrezcar_tabla(user_admin, tabla_filtrada);
            }
        }

        //Se ejecuta al hacer click en el boton menu-administrar-negocio-ingresar, abriendo dicha ventana
        private void menu_ingres_neg_Click(object sender, EventArgs e)
        {
            Form6 nueva_ventana = new Form6(conexion);
            nueva_ventana.ShowDialog();
        }

        //Se ejecuta al hacer click en el boton menu-administrar-producto-ingresar, abriendo dicha ventana
        private void menu_ingres_prod_Click(object sender, EventArgs e)
        {
            Form8 nueva_ventana = new Form8(conexion, 0);
            nueva_ventana.ShowDialog();
        }

        //Se ejecuta al hacer click en el boton menu-administrar-duenio-ingresar, abriendo dicha ventana
        private void menu_ingres_duenio_Click(object sender, EventArgs e)
        {
            Form8 nueva_ventana = new Form8(conexion, 1);
            nueva_ventana.ShowDialog();
        }

        //Se ejecuta al hacer click en el boton menu-administrar-ubicacion-ingresar, abriendo dicha ventana
        private void ingresarToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form10 nueva_ventana = new Form10(conexion);
            nueva_ventana.ShowDialog();
        }

        //Se ejecuta al hacer click en el boton menu-usuarios-agregar, abriendo dicha ventana
        private void agregarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form11 nueva_ventana = new Form11(conexion, false);
            nueva_ventana.ShowDialog();
        }

        //Se ejecuta al hacer click en el boton menu-usuarios-modificar, abriendo dicha ventana
        private void modificarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form14 nueva_ventana = new Form14(conexion);
            nueva_ventana.ShowDialog();
        }

        //Se ejecuta al hacer click en el boton menu-administrar-producto-modificar, abriendo dicha ventana
        private void modificarToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form12 nueva_ventana = new Form12(conexion, 1);
            if (nueva_ventana.ShowDialog() == DialogResult.OK)
            {
                refrezcar_tabla(user_admin, tabla_filtrada);
            }
        }

        //Se ejecuta al hacer click en el boton menu-administrar-duenio-modificar, abriendo dicha ventana
        private void modificarToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Form12 nueva_ventana = new Form12(conexion, 2);
            if (nueva_ventana.ShowDialog() == DialogResult.OK)
            {
                refrezcar_tabla(user_admin, tabla_filtrada);
            }
        }

        //Se ejecuta al hacer click en el boton menu-administrar-ubicacion-modificar, abriendo dicha ventana
        private void modificarToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            Form15 nueva_ventana = new Form15(conexion);
            if (nueva_ventana.ShowDialog() == DialogResult.OK)
            {
                refrezcar_tabla(user_admin, tabla_filtrada);
            }
        }

        //Se ejecuta al hacer click en el boton cerrar sesion. Se abre la ventana de login para seleccionar un nuevo usuario. Si se cancela se cierra el programa
        private void cerrarSesiónToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DialogResult resultado = MessageBox.Show("Se cerrará la sesión activa y se abrira la ventana para loguearse con un nuevo usuario. ¿Esta seguro de cerrar su sesión actual?", "Advertencia", MessageBoxButtons.YesNo);
            if (resultado == DialogResult.Yes)
            {
                Form13 nueva_ventana = new Form13(conexion);
                if (nueva_ventana.ShowDialog() == DialogResult.OK)
                {
                    nuevo_user = nueva_ventana.Ver_elemento();
                    user_admin = nuevo_user.es_Admin;
                    refrezcar_tabla(user_admin, tabla_filtrada);
                }
                else
                {
                    Application.Exit();
                    Environment.Exit(0);
                }
            }
        }

        private void click_check(object sender, EventArgs e)
        {
            tabla_filtrada = !tabla_filtrada;
            calendario.SelectionStart = DateTime.Now;
            refrezcar_tabla(user_admin, tabla_filtrada);
        }

        private void seleccion_fecha(object sender, DateRangeEventArgs e)
        {
            refrezcar_tabla(user_admin, tabla_filtrada);
        }
    }
}