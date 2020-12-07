using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Shared.Helpers
{
    public class ReportBuilder
    {
        public static string BuildReport(string reportName,string reportTitle, DataSet dataSet, List<string> columnsToSum)
        {
            try
            {
                if (Directory.Exists(System.IO.Path.GetTempPath() + "\\TMS\\") == false)
                    Directory.CreateDirectory(System.IO.Path.GetTempPath() + "\\TMS\\");
                string fileName = System.IO.Path.GetTempPath() + "\\TMS\\" + reportName + "_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".xlsx";

                XLWorkbook xLWorkbook = new XLWorkbook();


                int k = 1;
                foreach (DataTable dt in dataSet.Tables)
                {
                    object sumObject;
                 
                    Dictionary<string, object> sume = new Dictionary<string, object>();
                    foreach(string c in columnsToSum)
                    {
                       sume.Add(c, sumObject = dt.Compute("Sum("+ c + ")", string.Empty));
                    }


                   
                    //dt.TableName = reportName+"_"+k.ToString();
                    var ws = xLWorkbook.Worksheets.Add(reportName);
                    ws.Cell("A1").Value = reportTitle;
                    ws.Cell("A1").Style.Font.Bold = true;
                    ws.Cell("A1").Style.Font.FontSize = 16;
                    ws.Cell("A3").InsertTable(dt);
                    ws.Range("A1:D1").Merge();
                    //update hyperlinks

                    //var ws = xLWorkbook.Worksheet(1);
                    int rowcount = ws.RangeUsed().RowCount();
                    int columncount = ws.RangeUsed().ColumnCount();

                    for (int i = 0; i < columncount; i++)
                    {
                        ws.Column(i+1).AdjustToContents();
                        string colName = ws.Cell(1, i + 1).Value.ToString();
                        if (colName.Contains("_Wx"))
                        {
                            string columnName = colName.Substring(0, colName.LastIndexOf("_Wx"));
                            double width = double.Parse(colName.Substring(colName.LastIndexOf("_Wx") + 3));
                            ws.Column(i + 1).Width = width;
                        }
                    }


                    for (int i = 0; i < columncount; i++)
                    {
                        var is_color_col = ws.Cell(1, i + 1).Value.ToString().Contains("_color");
                        if (is_color_col)
                        {
                            string columnName = ws.Cell(1, i + 1).Value.ToString().Substring(0, ws.Cell(1, i + 1).Value.ToString().IndexOf('_'));
                            var column = dt.Columns[columnName].Ordinal;
                            for (int j = 0; j < rowcount; j++)
                            {
                                if (ws.Cell(j + 1, i + 1).Value.ToString() != "" && ws.Cell(j + 1, column + 1).Value.ToString() != "")
                                    ws.Cell(j + 1, column + 1).Style.Fill.BackgroundColor = XLColor.FromName(ws.Cell(j + 1, i + 1).Value.ToString());
                            }
                            ws.Column(i + 1).Hide();
                        }


                    }

                    for (int i = 0; i < columncount; i++)
                    {
                        var is_url_col = ws.Cell(1, i + 1).Value.ToString().Contains("_url");
                        if (is_url_col)
                        {
                            string columnName = ws.Cell(1, i + 1).Value.ToString().Substring(0, ws.Cell(1, i + 1).Value.ToString().IndexOf('_'));
                            var column = dt.Columns[columnName].Ordinal;
                            for (int j = 0; j < rowcount; j++)
                            {
                                if (ws.Cell(j + 1, i + 1).Value.ToString() != "" && ws.Cell(j + 1, column + 1).Value.ToString() != "")
                                    ws.Cell(j + 1, column + 1).Hyperlink = new XLHyperlink(ws.Cell(j + 1, i + 1).Value.ToString());
                            }
                            ws.Column(i + 1).Hide();
                        }


                    }
                    k++;

                    foreach(KeyValuePair<string,object> kvp in  sume)
                    {
                        int column = dt.Columns[kvp.Key].Ordinal+1;
                        int row = dt.Rows.Count+4;
                        ws.Cell(row, column).Value = kvp.Value;
                        ws.Cell(row, column).Style.Font.Bold = true;
                    }
                }


                MemoryStream ms = new MemoryStream();
                xLWorkbook.SaveAs(ms);
                FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                ms.WriteTo(file);
                file.Close();
                ms.Close();
                return fileName;
            }
            catch (Exception e)
            {
                //Logger.Exception(e);
                return null;
            }
        }
        public static string BuildReport(string reportName, List<DataTable> dts)
        {
            try
            {
                if (Directory.Exists(System.IO.Path.GetTempPath() + "\\TMS\\") == false)
                    Directory.CreateDirectory(System.IO.Path.GetTempPath() + "\\TMS\\");
                string fileName = System.IO.Path.GetTempPath() + "\\TMS\\" + reportName + "_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".xlsx";

                XLWorkbook xLWorkbook = new XLWorkbook();

                int k = 1;
                foreach (var dt in dts)
                {
                    //dt.TableName = reportName+"_"+k.ToString();
                    var ws = xLWorkbook.Worksheets.Add(dt);


                    //update hyperlinks

                    //var ws = xLWorkbook.Worksheet(1);
                    int rowcount = ws.RangeUsed().RowCount();
                    int columncount = ws.RangeUsed().ColumnCount();

                    for (int i = 0; i < columncount; i++)
                    {
                        string colName = ws.Cell(1, i + 1).Value.ToString();
                        if (colName.Contains("_Wx"))
                        {
                            string columnName = colName.Substring(0, colName.LastIndexOf("_Wx"));
                            double width = double.Parse(colName.Substring(colName.LastIndexOf("_Wx") + 3));
                            ws.Column(i + 1).Width = width;
                        }
                    }

                    for (int i = 0; i < columncount; i++)
                    {
                        var is_url_col = ws.Cell(1, i + 1).Value.ToString().Contains("_url");
                        if (is_url_col)
                        {
                            string columnName = ws.Cell(1, i + 1).Value.ToString().Substring(0, ws.Cell(1, i + 1).Value.ToString().IndexOf('_'));
                            var column = dt.Columns[columnName].Ordinal;
                            for (int j = 0; j < rowcount; j++)
                            {
                                if (ws.Cell(j + 1, i + 1).Value.ToString() != "" && ws.Cell(j + 1, column + 1).Value.ToString() != "")
                                    ws.Cell(j + 1, column + 1).Hyperlink = new XLHyperlink(ws.Cell(j + 1, i + 1).Value.ToString());
                            }
                            ws.Column(i + 1).Hide();
                        }
                    }
                    k++;
                }

                MemoryStream ms = new MemoryStream();
                xLWorkbook.SaveAs(ms);
                FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                ms.WriteTo(file);
                file.Close();
                ms.Close();
                return fileName;
            }
            catch (Exception e)
            {
                //Logger.Exception(e);
                return null;
            }
        }
    }
}
