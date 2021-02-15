using System;
using System.Windows.Forms;

namespace Interfaz_Proyecto
{
    public partial class Form7 : Form
    {
        public Form7()
        {
            InitializeComponent();
        }

        //abro automaticamente la aplicacion predeterminada de correo con mi mail como destinatario al hacer click en el
        private void link_precionado(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                linkLabel1.LinkVisited = true;
                System.Diagnostics.Process.Start("mailto:Luucas125@outlook.com.ar");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hubo un error al procesar su solicitud. Error: " + ex.Message.ToString(), "Error");
            }
        }
    }
}
