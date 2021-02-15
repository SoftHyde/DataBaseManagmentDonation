using Npgsql;
using NpgsqlTypes;
using System;
using System.Windows.Forms;

namespace Interfaz_Proyecto
{
    public partial class Form8 : Form
    {
        NpgsqlConnection conexion;
        int tipo_ventana;
        utilidades nueva_utilidad = new utilidades();
        string errores;

        //Segun sea dueño o producto, relleno las listas de los que ya estan ingresados en la base de datos
        private void rellenar_lista(int tipo)
        {
            lstproductos.Items.Clear();
            if (tipo == 0) //es un producto
            {
                using (NpgsqlCommand command = new NpgsqlCommand("SELECT Nombre FROM Producto order by ID_Producto", conexion))
                {
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        lstproductos.Items.Add(reader[0].ToString());
                        lstproductos.Update();
                    }
                    reader.Close();
                    lstproductos.Refresh();
                }
            }
            else //es un dueño
            {
                using (NpgsqlCommand command = new NpgsqlCommand("SELECT Nombre FROM Duenio order by ID_Duenio", conexion))
                {
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        lstproductos.Items.Add(reader[0].ToString());
                        lstproductos.Update();
                    }
                    reader.Close();
                    lstproductos.Refresh();
                }
            }
        }

        //una vez ingresado (duenio o producto), limpio los campos para dejar la ventana lista para ingresar otro
        private void refrezcar_ventana(int tipo)
        {
            txtnombre.Text = "";
            rellenar_lista(tipo);
            txtnombre.Focus();
        }

        public Form8(NpgsqlConnection conexion_rec, int tipo)
        {
            InitializeComponent();
            tipo_ventana = tipo;
            conexion = conexion_rec;
            rellenar_lista(tipo);
            if (tipo_ventana == 0)
            {
                this.Text = "Agregar Nuevo Producto";
                label1.Text = "Ingrese los datos para un nuevo Producto";
                label5.Text = "Productos actualmente ingresados:";
            }
            else
            {
                this.Text = "Agregar Nuevo Dueño";
                label1.Text = "Ingrese los datos para un nuevo Dueño";
                label5.Text = "Dueños actualmente ingresados:";
            }
        }

        private void btncancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool listar_errores()
        {
            bool hay_errores = false;
            errores = "Lista de errores:" + Environment.NewLine;
            if (txtnombre.Text.Trim() == "")
            {
                if (tipo_ventana == 0) errores += "*Debe completar el campo 'Nombre' antes de agregar un nuevo producto." + Environment.NewLine;
                else errores += "*Debe completar el campo 'Nombre' antes de agregar un nuevo dueño." + Environment.NewLine;
                hay_errores = true;
            }
            return hay_errores;
        }

        //Tanto para duenio como producto, primero veo si el nombre no esta ocupado ya. En caso que no lo este, si todo esta correcto, lo ingreso a la bd
        private void btnaceptar_Click(object sender, EventArgs e)
        {
            if (listar_errores() == true) MessageBox.Show(errores, "Error");
            else
            {
                if (tipo_ventana == 0) //producto
                {
                    bool ocupacion = false;
                    using (NpgsqlCommand command = new NpgsqlCommand("Select Verificar_Ocupacion_Producto(@nom);", conexion))
                    {
                        command.Parameters.Add(new NpgsqlParameter("@nom", NpgsqlDbType.Varchar));
                        command.Prepare();
                        command.Parameters[0].Value = nueva_utilidad.verificador_palabras_plurales(txtnombre.Text.ToLower().Trim());
                        NpgsqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            if (bool.Parse(reader[0].ToString()) == true) ocupacion = true;
                        }
                        reader.Close();
                        command.Parameters.Clear();
                    }
                    if (ocupacion == true) MessageBox.Show("El nombre del producto ya existe.", "Error");
                    else
                    {
                        using (NpgsqlCommand command_insersion = new NpgsqlCommand("Select Ingresar_producto(@nom);", conexion))
                        {
                            command_insersion.Parameters.Add(new NpgsqlParameter("@nom", NpgsqlDbType.Varchar));
                            command_insersion.Prepare();
                            command_insersion.Parameters[0].Value = nueva_utilidad.verificador_palabras_plurales(txtnombre.Text.ToLower().Trim());
                            command_insersion.ExecuteNonQuery();
                            command_insersion.Parameters.Clear();
                            refrezcar_ventana(0);
                        }
                    }
                }
                else //dueño
                {
                    bool ocupacion = false;
                    using (NpgsqlCommand command = new NpgsqlCommand("Select Verificar_Ocupacion_Duenio(@nom);", conexion))
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
                    if (ocupacion == true) MessageBox.Show("El nombre del dueño ya existe.", "Error");
                    else
                    {
                        using (NpgsqlCommand command_insersion = new NpgsqlCommand("Select Ingresar_duenio(@nom);", conexion))
                        {
                            command_insersion.Parameters.Add(new NpgsqlParameter("@nom", NpgsqlDbType.Varchar));
                            command_insersion.Prepare();
                            command_insersion.Parameters[0].Value = txtnombre.Text.ToLower().Trim();
                            command_insersion.ExecuteNonQuery();
                            command_insersion.Parameters.Clear();
                            refrezcar_ventana(1);
                        }
                    }
                }
            }
        }

        private void tecla_presionada(object sender, KeyPressEventArgs e)
        {
            nueva_utilidad.verificador_campo_nombre(sender, e);
        }

        private void ventana_activada(object sender, EventArgs e)
        {
            txtnombre.Focus();
        }
    }
}
