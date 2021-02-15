using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Interfaz_Proyecto
{
    public partial class Form5 : Form
    {
        //Actualiza el grafico para mostrar los resultados del resumen 1
        private void primer_resumen()
        {
            using (NpgsqlCommand command = new NpgsqlCommand("SELECT resumen_peso_producto_diario(@fecha);", conexion))
            {
                command.Parameters.Add(new NpgsqlParameter("@fecha", NpgsqlDbType.Date));
                command.Prepare();
                command.Parameters[0].Value = datepicker.SelectionStart;
                command.ExecuteNonQuery();
                command.Parameters.Clear();
            }
            Comando_a_usar = ("SELECT * from Auxiliar");
            using (NpgsqlCommand new_command = new NpgsqlCommand(Comando_a_usar, conexion))
            {
                NpgsqlDataReader reader = new_command.ExecuteReader();
                while (reader.Read())
                {
                    series.Add(reader[11].ToString());
                    valores.Add(float.Parse(reader[12].ToString()));
                }
                reader.Close();
            }
            grafico_resumen_peso_producto_diario.Titles.Add("Peso diario de cada producto");
            if (series.Count() > 0)
            {
                for (int i = 0; i < series.Count(); i++)
                {
                    //Titulo
                    Series nombres = grafico_resumen_peso_producto_diario.Series.Add(series[i] + " - " + valores[i].ToString());
                    //Valor
                    nombres.Label = valores[i].ToString();
                    nombres.Points.Add(valores[i]);
                }
            }
            else
            {
                Series nombres = grafico_resumen_peso_producto_diario.Series.Add("Sin Datos");
                nombres.Label = "0";
                nombres.Points.Add(0.0);
            }
        }

        //Actualiza el grafico para mostrar los resultados del resumen 2 o 3 (se usa la misma funcion porque los resultados son similares)
        private void segundo_y_tercer_resumen(int elec)
        {
            string comando_a_usar;
            string titulo;
            if (elec == 0) //segundo resumen - semestral
            {
                comando_a_usar = "SELECT resumen_peso_total_donante_semestre(@fecha);";
                titulo = "Peso semestral donado por cada negocio";
            }
            else
            {
                comando_a_usar = "SELECT resumen_peso_total_donante_anio(@fecha);";
                titulo = "Peso anual donado por cada negocio";
            }
            using (NpgsqlCommand command = new NpgsqlCommand(comando_a_usar, conexion))
            {
                command.Parameters.Add(new NpgsqlParameter("@fecha", NpgsqlDbType.Date));
                command.Prepare();
                command.Parameters[0].Value = datepicker.SelectionStart;
                command.ExecuteNonQuery();
                command.Parameters.Clear();
            }
            Comando_a_usar = ("SELECT * from Auxiliar");
            using (NpgsqlCommand new_command = new NpgsqlCommand(Comando_a_usar, conexion))
            {
                NpgsqlDataReader reader = new_command.ExecuteReader();
                while (reader.Read())
                {
                    series.Add(reader[5].ToString());
                    valores.Add(float.Parse(reader[12].ToString()));
                }
                reader.Close();
            }
            if (series.Count() > 0)
            {
                grafico_resumen_peso_producto_diario.Titles.Add(titulo);
                for (int i = 0; i < series.Count(); i++)
                {
                    //Titulo
                    Series nombres = grafico_resumen_peso_producto_diario.Series.Add(series[i] + " - " + valores[i].ToString());
                    //Valor
                    nombres.Label = valores[i].ToString();
                    nombres.Points.Add(valores[i]);
                }
            }
            else
            {
                Series nombres = grafico_resumen_peso_producto_diario.Series.Add("Sin Datos");
                nombres.Label = "0";
                nombres.Points.Add(0.0);
            }
        }

        //Actualiza el grafico para mostrar los resultados del resumen 4
        private void cuarto_resumen()
        {
            using (NpgsqlCommand command = new NpgsqlCommand("SELECT resumen_promedio_peso_total_mes(@fecha);", conexion))
            {
                command.Parameters.Add(new NpgsqlParameter("@fecha", NpgsqlDbType.Date));
                command.Prepare();
                command.Parameters[0].Value = datepicker.SelectionStart;
                command.ExecuteNonQuery();
                command.Parameters.Clear();
            }
            Comando_a_usar = ("SELECT * from Auxiliar");
            using (NpgsqlCommand new_command = new NpgsqlCommand(Comando_a_usar, conexion))
            {
                NpgsqlDataReader reader = new_command.ExecuteReader();
                while (reader.Read())
                {
                    //series.Add(meses[i]);
                    if (reader[12].ToString() == DBNull.Value.ToString()) valores.Add(0);
                    else valores.Add(float.Parse(reader[12].ToString()));
                }
                reader.Close();
            }
            grafico_resumen_peso_producto_diario.Titles.Add("Promedio de Kg. recolectados por cada mes");
            if (valores.Count() > 0)
            {
                for (int i = 0; i < valores.Count(); i++)
                {
                    //Titulo
                    Series nombres = grafico_resumen_peso_producto_diario.Series.Add(meses[i] + " - " + valores[i].ToString());
                    //Valor
                    nombres.Label = valores[i].ToString();
                    nombres.Points.Add(valores[i]);
                }
            }
            else
            {
                Series nombres = grafico_resumen_peso_producto_diario.Series.Add("Sin Datos");
                nombres.Label = "0";
                nombres.Points.Add(0.0);
            }
        }

        NpgsqlConnection conexion;
        string Comando_a_usar;
        DateTime fecha = new DateTime();
        List<string> series = new List<string>();
        List<float> valores = new List<float>();
        string[] meses = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
        public Form5(NpgsqlConnection conexion_recibida)
        {
            InitializeComponent();
            conexion = conexion_recibida;
            lstresumen.SelectedIndex = 0;
            datepicker.SelectionStart = DateTime.Now;
        }

        //Se ejecuta al elegir un dia del calendario
        private void seleccion_dia(object sender, DateRangeEventArgs e)
        {
            fecha = datepicker.SelectionStart;
            actualizar_datos();
        }

        private void actualizar_datos()
        {
            //Vacio todo antes de volver a llenarlo, sino se repite
            series.Clear();
            valores.Clear();
            grafico_resumen_peso_producto_diario.Titles.Clear();
            grafico_resumen_peso_producto_diario.Series.Clear();
            //Actualizo el valor de peso total de donaciones para ese dia
            using (NpgsqlCommand command = new NpgsqlCommand("SELECT resumen_peso_total_diario(@fecha);", conexion))
            {
                command.Parameters.Add(new NpgsqlParameter("@fecha", NpgsqlDbType.Date));
                command.Prepare();
                command.Parameters[0].Value = fecha;
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader[0].ToString() != "")
                    {
                        lbltotal_conseguido.Text = reader[0].ToString() + " Kg.";
                    }
                    else lbltotal_conseguido.Text = "0 Kg.";
                }
                reader.Close();
                command.Parameters.Clear();
            }

            //Llamo a los posibles resumenes para que actualicen el grafico
            if (lstresumen.SelectedIndex == 0)
            {
                primer_resumen();
            }
            else if (lstresumen.SelectedIndex == 1)
            {
                segundo_y_tercer_resumen(0);
            }
            else if (lstresumen.SelectedIndex == 2)
            {
                segundo_y_tercer_resumen(1);
            }
            else
            {
                cuarto_resumen();
            }
            ajustar_colores();
            grafico_resumen_peso_producto_diario.ResetAutoValues();
            grafico_resumen_peso_producto_diario.Update();
        }

        //Se llama cuando se cambia el resumen seleccionado, de manera que actualice la informacion del grafico para dicho resumen
        private void seleccion_resumen(object sender, EventArgs e)
        {
            actualizar_datos();
        }

        private void ajustar_colores()
        {
            grafico_resumen_peso_producto_diario.Titles[0].ForeColor = Color.FromKnownColor(KnownColor.ButtonHighlight);
            grafico_resumen_peso_producto_diario.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.FromKnownColor(KnownColor.ButtonHighlight);
            grafico_resumen_peso_producto_diario.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.FromKnownColor(KnownColor.ButtonHighlight);
        }
    }
}
