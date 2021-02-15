using Npgsql;
using System;
using System.IO;
using System.Windows.Forms;

namespace Interfaz_Proyecto
{
    public partial class ventana_ingreso_db : Form
    {
        public struct credenciales
        {
            public string server, puerto, nombre, id, pass;
        }
        utilidades nueva_utilidad = new utilidades();
        credenciales nuevo_ingreso;
        NpgsqlConnection conexion = new NpgsqlConnection();
        string CadenaConexion, errores;
        public ventana_ingreso_db()
        {
            InitializeComponent();
            using (FileStream fs = new FileStream("dbcrecenciales.bin", FileMode.OpenOrCreate))
            {
                File.Decrypt("dbcrecenciales.bin");
                if (fs.Length != 0)
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        txtserver.Text = br.ReadString();
                        txtpuerto.Text = br.ReadString();
                        txtnombre.Text = br.ReadString();
                        txtuser.Text = br.ReadString();
                        txtpass.Text = br.ReadString();
                        chkrecordar.Checked = true;
                    }
                }
            }
            File.Encrypt("dbcrecenciales.bin");
        }

        private void btncancear_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void tecla_precionada(object sender, KeyPressEventArgs e)
        {
            nueva_utilidad.verificador_campo_numero_nodecimal(sender, e);
        }

        //Retorna la conexion con la bd
        public NpgsqlConnection retornar_conexion()
        {
            return conexion;
        }

        private bool listar_errores()
        {
            bool hay_errores = false;
            errores = "Lista de errores:" + Environment.NewLine;
            if (txtnombre.Text.Trim() == "" || txtpass.Text == "" || txtpuerto.Text.Trim() == "" || txtserver.Text.Trim() == "" || txtuser.Text.Trim() == "")
            {
                errores += "*Debe completar todos los campos obligatorios (*) antes de ingresar al programa." + Environment.NewLine;
                hay_errores = true;
            }
            return hay_errores;
        }

        //Al hacer click en conectar, verifico todos los campos y llamo a la funcion que realiz al conexion
        private void btnaceptar_Click(object sender, EventArgs e)
        {
            if (listar_errores() == true) MessageBox.Show(errores, "Error");
            else
            {
                nuevo_ingreso.server = txtserver.Text.Trim();
                nuevo_ingreso.puerto = txtpuerto.Text.Trim();
                nuevo_ingreso.nombre = txtnombre.Text.Trim().ToLower();
                nuevo_ingreso.id = txtuser.Text.Trim().ToLower();
                nuevo_ingreso.pass = txtpass.Text;
                //Si quiere recordar los datos
                if (chkrecordar.Checked == true)
                {
                    using (FileStream fs = new FileStream("dbcrecenciales.bin", FileMode.Create))
                    {
                        using (BinaryWriter bw = new BinaryWriter(fs))
                        {
                            // Escritura de datos
                            bw.Write(nuevo_ingreso.server);
                            bw.Write(nuevo_ingreso.puerto);
                            bw.Write(nuevo_ingreso.nombre);
                            bw.Write(nuevo_ingreso.id);
                            bw.Write(nuevo_ingreso.pass);
                        }
                    }
                    File.Encrypt("dbcrecenciales.bin");
                }
                else File.Delete("dbcrecenciales.bin");
                realizar_conexion();
                if (conexion.State.ToString() == "Open") this.DialogResult = DialogResult.OK;
            }
        }

        //Con los datos ingresados, establezco la conexion con la bd
        private void realizar_conexion()
        {
            CadenaConexion = "Server = " + nuevo_ingreso.server + "; Port = " + nuevo_ingreso.puerto + "; Database = " + nuevo_ingreso.nombre + ";";
            CadenaConexion += "User ID = " + nuevo_ingreso.id + ";";
            CadenaConexion += "; Password = " + nuevo_ingreso.pass;
            conexion.ConnectionString = CadenaConexion;
            try
            {
                conexion.Open();
            }
            catch
            {
                MessageBox.Show("Hubo un problema al establecer conexion con la Base de Datos.", "Error de conexion");
                conexion.Close();
                txtserver.Focus();
            }
        }
    }
}
