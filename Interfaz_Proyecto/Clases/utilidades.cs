using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Interfaz_Proyecto
{
    class utilidades
    {
        //Se encarga de hacer singular una palabra en caso que sea plural
        public string verificador_palabras_plurales(string text_ingresado)
        {
            string texto_salida = "";
            if (text_ingresado.Contains("albaricoque") || text_ingresado.Contains("aguacate") || text_ingresado.Contains("alquejenje") || text_ingresado.Contains("tomate") || text_ingresado.Contains("jitomate"))
            {
                if (text_ingresado.EndsWith("s")) texto_salida = text_ingresado.Remove(text_ingresado.Count() - 1, 1);
                else texto_salida = text_ingresado;
            }
            else
            {
                if (text_ingresado.EndsWith("es")) texto_salida = text_ingresado.Remove(text_ingresado.Count() - 2, 2);
                else if (text_ingresado.EndsWith("s")) texto_salida = text_ingresado.Remove(text_ingresado.Count() - 1, 1);
                else texto_salida = text_ingresado;
            }
            return texto_salida.ToLower();
        }

        //Se usa para validar que se haya ingresado un mail con formato correcto
        public bool verificacion_mail(String email)
        {
            String expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, String.Empty).Length == 0) return true;
                else return false;
            }
            else return false;
        }
        public bool contador_puntos(string texto)
        {
            int counter = 0;
            for (int i = 0; i < texto.Length; i++)
            {
                if (texto[i] == '.') counter++;
            }
            if (counter > 0) return true; //ya hay un punto
            return false; //no hay un punto
        }
        public void verificador_campo_numero(KeyPressEventArgs e, string texto)
        {
            if (Char.IsNumber(e.KeyChar)) e.Handled = false;
            else if (e.KeyChar == '.' && contador_puntos(texto) == false) e.Handled = false;
            else if (Char.IsControl(e.KeyChar)) //Al pulsar teclas como Borrar y eso.
            {
                e.Handled = false; //Se acepta (todo OK)
            }
            else //Para todo lo demas
            {
                e.Handled = true; //No se acepta (si pulsas cualquier otra cosa pues no se envia)
            }
        }

        public string remp_coma_punto(string texto)
        {
            return texto.Replace('.', ',');
        }

        public void verificador_campo_numero_nodecimal(object sender, KeyPressEventArgs e)
        {
            if (Char.IsNumber(e.KeyChar)) e.Handled = false;
            else if (Char.IsControl(e.KeyChar)) e.Handled = false;
            else e.Handled = true;
        }

        public void verificador_campo_nombre(object sender, KeyPressEventArgs e) //solo deja poner letras y espacios
        {
            if (Char.IsLetter(e.KeyChar)) e.Handled = false;
            else if (Char.IsControl(e.KeyChar)) e.Handled = false;
            else if (Char.IsWhiteSpace(e.KeyChar)) e.Handled = false;
            else e.Handled = true;
        }
    }
}
