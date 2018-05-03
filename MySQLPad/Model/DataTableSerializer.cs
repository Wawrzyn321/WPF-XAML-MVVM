using System;
using System.Data;
using System.IO;
using System.Windows;
using CsvHelper;
using Microsoft.Win32;

namespace Model
{
    /// <summary>
    /// Class for saving DataTables to XML and CSV.
    /// </summary>
    public static class DataTableSerializer
    {
        public static bool SerializeAsXML(DataTable table)
        {
            string path = GetSavePath("XML (*.xml)|*.xml");
            if (path == null) return false;

            try
            {
                table.WriteXml(path);
                return true;
            }
            catch (Exception err)
            {
                MessageBox.Show("Could not save XML file, reason: " + err.Message + ".", "SQL Pad");
                return false;
            }
        }

        public static bool SerializeAsCSV(DataTable table)
        {
            string path = GetSavePath("CSV (*.csv)|*.csv");
            if (path == null) return false;

            try
            {
                using (TextWriter writer = File.CreateText(path))
                {
                    var csvWriter = new CsvWriter(writer);
                    foreach (DataColumn dataColumn in table.Columns)
                    {
                        csvWriter.WriteField(dataColumn.ColumnName);
                    }
                    csvWriter.NextRecord();
                    foreach (DataRow dataRow in table.Rows)
                    {
                        foreach (var o in dataRow.ItemArray)
                        {
                            csvWriter.WriteField(o);
                        }
                        csvWriter.NextRecord();
                    }
                    csvWriter.Flush();
                }

                return true;
            }
            catch (Exception err)
            {
                MessageBox.Show("Could not save CSV file, reason: " + err.Message + ".", "SQL Pad");
                return false;
            }
        }

        private static string GetSavePath(string filter)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog {Filter = filter};

            if (saveFileDialog.ShowDialog() == true)
            {
                return saveFileDialog.FileName;
            }

            return null;
        }
    }
}
