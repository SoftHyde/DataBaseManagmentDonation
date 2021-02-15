using System;
using System.Windows.Forms;

namespace Interfaz_Proyecto
{
    class exportacion
    {
        public void exportar_tabla(DataGridView tabla)
        {
            try
            {
                //creo una ventana de dialogo para que escoja donde guardar el archivo exportado
                SaveFileDialog fichero = new SaveFileDialog();
                fichero.Filter = "Excel (* .xls)|* .xls";
                fichero.FileName = "TablaDonaciones";
                if (fichero.ShowDialog() == DialogResult.OK)
                {
                    //creo los distintos elementos que conforman un archivo excel (area de trabajo, una hoja, etc). Luego para cada celda, completo el valor correspondiente
                    Microsoft.Office.Interop.Excel.Application aplicacion;
                    Microsoft.Office.Interop.Excel.Workbook trabajo;
                    Microsoft.Office.Interop.Excel.Worksheet hoja;
                    aplicacion = new Microsoft.Office.Interop.Excel.Application();
                    trabajo = aplicacion.Workbooks.Add();
                    hoja = (Microsoft.Office.Interop.Excel.Worksheet)trabajo.Worksheets.get_Item(1);
                    for (int i = 0; i < tabla.Rows.Count - 1; i++)
                    {
                        for (int j = 0; j < tabla.Columns.Count; j++)
                        {
                            if (tabla.Rows[i].Cells[j].Value != null)
                            {
                                hoja.Cells[i + 2, j + 1] = tabla.Rows[i].Cells[j].Value.ToString();
                            }
                        }
                    }
                    for (int i = 0; i < tabla.Columns.Count; i++)
                    {
                        hoja.Cells[1, i + 1] = tabla.Columns[i].Name;
                    }
                    trabajo.SaveAs(fichero.FileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
                    trabajo.Close(true);
                    aplicacion.Quit();
                }
            }
            catch
            {
                MessageBox.Show("Hubo un error al realizar la exportación.", "Error");
            }
        }
    }
}
