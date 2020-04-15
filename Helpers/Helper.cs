using ClosedXML.Excel;
using Delos.Model;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Delos.Helpers
{
    public static class Helper
    {
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
        public static string Stampa(ponuda p)
        {
            var culture = new System.Globalization.CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = culture;

            //string dir = Environment.SpecialFolder.MyDocuments + "\\ServisDB\\";

            //string dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "temp");
            string dir = "C:\\temp";

            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }

            XLWorkbook doc = new XLWorkbook("PONUDA.xlsm");

            var sheet = doc.Worksheet(1);

            string rednibroj = p.broj;
            DateTime datum = p.datum.Date;
            string partner_naziv = p.partner_naziv;
            string partner_jib = p.partner_jib;
            string partner_adresa = p.partner_adresa;

            string valuta = p.valuta_placanja;
            string opcija_ponude = p.rok_vazenja;
            string rok_isporuke = p.rok_isporuke;
            string paritet = p.paritet;
            string paritet_kod = p.paritet_kod;
            string radnik = p.radnik;


            string iznos_bez_pdv = p.iznos_bez_rabata.ToString();
            string rabat = p.rabat.ToString();
            string iznos_sa_rabatom = p.iznos_sa_rabatom.ToString();
            string pdv = p.pdv.ToString();
            string iznos_sa_pdv = p.iznos_sa_pdv.ToString();

            string broj = rednibroj;
            string[] parts = rednibroj.Split('/');
            string rb = parts[0];
            string year = parts[1];
            int rrb = int.Parse(rb);
            rednibroj = rrb.ToString("D4") + "/" + year;
            sheet.Cells("A3").Value = "PONUDA BROJ: " + rednibroj;
            sheet.Cells("A3").DataType = XLDataType.Text;

            sheet.Cells("G2").Value = datum;
            sheet.Cells("G2").DataType = XLDataType.DateTime;
            sheet.Cells("G2").Style.NumberFormat.Format = "dd.MM.yyyy";

            sheet.Cells("B7").Value = partner_naziv;
            sheet.Cells("B7").DataType = XLDataType.Text;

            sheet.Cells("B8").Value = partner_jib;
            sheet.Cells("B8").DataType = XLDataType.Text;

            sheet.Cells("B9").Value = partner_adresa;
            sheet.Cells("B9").DataType = XLDataType.Text;

            sheet.Cells("F7").Value = valuta;
            sheet.Cells("F7").DataType = XLDataType.Text;

            sheet.Cells("F8").Value = opcija_ponude;
            sheet.Cells("F8").DataType = XLDataType.Text;

            sheet.Cells("F9").Value = rok_isporuke;
            sheet.Cells("F9").DataType = XLDataType.Text;

            sheet.Cells("F10").Value = paritet_kod + " " + paritet;
            sheet.Cells("F10").DataType = XLDataType.Text;

            //List<PonudaStavka> stavke = PersistanceManager.ReadPonudaStavka(broj);
            int index = 0;
            string rowIndex = (14 + index).ToString();
            int brojStavki = p.stavke.Count();
            foreach (var stavka in p.stavke)
            {
                rowIndex = (14 + index).ToString();

                sheet.Row(14 + index).Height = 20;
                sheet.Row(14 + index).Style.Font.FontName = "Arial";
                sheet.Row(14 + index).Style.Font.FontSize = 10;
                sheet.Row(14 + index).Style.Font.FontColor = XLColor.Black;
                sheet.Row(14 + index).Style.Font.Bold = false;
                sheet.Row(14 + index).Style.Alignment.Vertical = XLAlignmentVerticalValues.Bottom;

                sheet.Cells("A" + rowIndex).Value = (index + 1).ToString();
                sheet.Cells("A" + rowIndex).DataType = XLDataType.Text;
                sheet.Cells("A" + rowIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                sheet.Cells("A" + rowIndex).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                sheet.Cells("A" + rowIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                sheet.Cells("A" + rowIndex).Style.Border.OutsideBorderColor = XLColor.FromArgb(216, 228, 188);


                sheet.Cells("B" + rowIndex).Value = stavka.artikal_naziv.ToString() + Environment.NewLine + stavka.opis;
                sheet.Cells("B" + rowIndex).DataType = XLDataType.Text;
                sheet.Cells("B" + rowIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                sheet.Cells("B" + rowIndex).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                sheet.Cells("B" + rowIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                sheet.Cells("B" + rowIndex).Style.Border.OutsideBorderColor = XLColor.FromArgb(216, 228, 188);

                sheet.Cells("B" + rowIndex).Style.Alignment.WrapText = true;
                sheet.Row(14 + index).AdjustToContents();
                sheet.Row(14 + index).ClearHeight();

                sheet.Cells("C" + rowIndex).Value = stavka.jedinica_mjere.ToString();
                sheet.Cells("C" + rowIndex).DataType = XLDataType.Text;
                sheet.Cells("C" + rowIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                sheet.Cells("C" + rowIndex).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                sheet.Cells("C" + rowIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                sheet.Cells("C" + rowIndex).Style.Border.OutsideBorderColor = XLColor.FromArgb(216, 228, 188);

                sheet.Cells("D" + rowIndex).Value = stavka.kolicina.ToString();
                sheet.Cells("D" + rowIndex).DataType = XLDataType.Number;
                sheet.Cells("D" + rowIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                sheet.Cells("D" + rowIndex).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                sheet.Cells("D" + rowIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                sheet.Cells("D" + rowIndex).Style.Border.OutsideBorderColor = XLColor.FromArgb(216, 228, 188);

                sheet.Cells("E" + rowIndex).Value = stavka.cijena_bez_pdv.ToString();
                sheet.Cells("E" + rowIndex).DataType = XLDataType.Number;
                sheet.Cells("E" + rowIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                sheet.Cells("E" + rowIndex).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                sheet.Cells("E" + rowIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                sheet.Cells("E" + rowIndex).Style.Border.OutsideBorderColor = XLColor.FromArgb(216, 228, 188);
                sheet.Cells("E" + rowIndex).Style.NumberFormat.Format = "#,##0.00 \"KM\"";

                sheet.Cells("F" + rowIndex).Value = stavka.rabat_procenat.ToString();
                sheet.Cells("F" + rowIndex).DataType = XLDataType.Number;
                sheet.Cells("F" + rowIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                sheet.Cells("F" + rowIndex).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                sheet.Cells("F" + rowIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                sheet.Cells("F" + rowIndex).Style.Border.OutsideBorderColor = XLColor.FromArgb(216, 228, 188);
                sheet.Cells("F" + rowIndex).Style.NumberFormat.Format = "0.00%";

                sheet.Cells("G" + rowIndex).Value = stavka.iznos_bez_pdv_sa_rabatom.ToString();
                sheet.Cells("G" + rowIndex).DataType = XLDataType.Number;
                sheet.Cells("G" + rowIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                sheet.Cells("G" + rowIndex).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                sheet.Cells("G" + rowIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                sheet.Cells("G" + rowIndex).Style.Border.OutsideBorderColor = XLColor.FromArgb(216, 228, 188);
                sheet.Cells("G" + rowIndex).Style.NumberFormat.Format = "#,##0.00 \"KM\"";
                sheet.Cells("G" + rowIndex).Style.Font.Bold = false;

                index++;
            }

            sheet.Row(14 + brojStavki + 2).Height = 20;
            sheet.Row(14 + brojStavki + 3).Height = 20;
            sheet.Row(14 + brojStavki + 4).Height = 20;
            sheet.Row(14 + brojStavki + 5).Height = 20;
            sheet.Row(14 + brojStavki + 6).Height = 20;

            sheet.Cells("A" + (14 + brojStavki + 2).ToString()).Value = "UKUPAN IZNOS BEZ RABATA";
            sheet.Cells("A" + (14 + brojStavki + 2).ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            sheet.Cells("A" + (14 + brojStavki + 2).ToString()).Style.Font.Bold = false;
            sheet.Cells("A" + (14 + brojStavki + 2).ToString()).Style.Font.FontName = "Arial";
            sheet.Cells("A" + (14 + brojStavki + 2).ToString()).Style.Font.FontSize = 10;
            sheet.Cells("A" + (14 + brojStavki + 2).ToString()).Style.Font.FontColor = XLColor.Black;
                                  
            sheet.Cells("A" + (14 + brojStavki + 3).ToString()).Value = "IZNOS RABATA";
            sheet.Cells("A" + (14 + brojStavki + 3).ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            sheet.Cells("A" + (14 + brojStavki + 3).ToString()).Style.Font.Bold = false;
            sheet.Cells("A" + (14 + brojStavki + 3).ToString()).Style.Font.FontName = "Arial";
            sheet.Cells("A" + (14 + brojStavki + 3).ToString()).Style.Font.FontSize = 10;
            sheet.Cells("A" + (14 + brojStavki + 3).ToString()).Style.Font.FontColor = XLColor.Black;
                                   
            sheet.Cells("A" + (14 + brojStavki + 4).ToString()).Value = "UKUPAN IZNOS BEZ PDV";
            sheet.Cells("A" + (14 + brojStavki + 4).ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            sheet.Cells("A" + (14 + brojStavki + 4).ToString()).Style.Font.Bold = false;
            sheet.Cells("A" + (14 + brojStavki + 4).ToString()).Style.Font.FontName = "Arial";
            sheet.Cells("A" + (14 + brojStavki + 4).ToString()).Style.Font.FontSize = 10;
            sheet.Cells("A" + (14 + brojStavki + 4).ToString()).Style.Font.FontColor = XLColor.Black;
                                 
            sheet.Cells("A" + (14 + brojStavki + 5).ToString()).Value = "PDV";
            sheet.Cells("A" + (14 + brojStavki + 5).ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            sheet.Cells("A" + (14 + brojStavki + 5).ToString()).Style.Font.Bold = false;
            sheet.Cells("A" + (14 + brojStavki + 5).ToString()).Style.Font.FontName = "Arial";
            sheet.Cells("A" + (14 + brojStavki + 5).ToString()).Style.Font.FontSize = 10;
            sheet.Cells("A" + (14 + brojStavki + 5).ToString()).Style.Font.FontColor = XLColor.Black;
                                   
            sheet.Cells("A" + (14 + brojStavki + 6).ToString()).Value = "UKUPAN IZNOS SA PDV";
            sheet.Cells("A" + (14 + brojStavki + 6).ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            sheet.Cells("A" + (14 + brojStavki + 6).ToString()).Style.Font.Bold = false;
            sheet.Cells("A" + (14 + brojStavki + 6).ToString()).Style.Font.FontName = "Arial";
            sheet.Cells("A" + (14 + brojStavki + 6).ToString()).Style.Font.FontSize = 10;
            sheet.Cells("A" + (14 + brojStavki + 6).ToString()).Style.Font.FontColor = XLColor.Black;
                                  
            sheet.Range("A" + (14 + brojStavki + 2).ToString() + ":" + "F" + (14 +  brojStavki + 2).ToString()).Row(1).Merge();
            sheet.Range("A" + (14 + brojStavki + 3).ToString() + ":" + "F" + (14 +  brojStavki + 3).ToString()).Row(1).Merge();
            sheet.Range("A" + (14 + brojStavki + 4).ToString() + ":" + "F" + (14 +  brojStavki + 4).ToString()).Row(1).Merge();
            sheet.Range("A" + (14 + brojStavki + 5).ToString() + ":" + "F" + (14 +  brojStavki + 5).ToString()).Row(1).Merge();
            sheet.Range("A" + (14 + brojStavki + 6).ToString() + ":" + "F" + (14 +  brojStavki + 6).ToString()).Row(1).Merge();
                                                                                    
            sheet.Range("A" + (14 + brojStavki + 2).ToString() + ":" + "F" + (14 +  brojStavki + 2).ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.None;
            sheet.Range("A" + (14 + brojStavki + 3).ToString() + ":" + "F" + (14 +  brojStavki + 3).ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.None;
            sheet.Range("A" + (14 + brojStavki + 4).ToString() + ":" + "F" + (14 +  brojStavki + 4).ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.None;
            sheet.Range("A" + (14 + brojStavki + 5).ToString() + ":" + "F" + (14 +  brojStavki + 5).ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.None;
            sheet.Range("A" + (14 + brojStavki + 6).ToString() + ":" + "F" + (14 + brojStavki + 6).ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.None;
                                  
                                  
            sheet.Cells("G" + (14 + brojStavki + 2).ToString()).Value = iznos_bez_pdv;
            sheet.Cells("G" + (14 + brojStavki + 2).ToString()).DataType = XLDataType.Number;
            sheet.Cells("G" + (14 + brojStavki + 2).ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            sheet.Cells("G" + (14 + brojStavki + 2).ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            sheet.Cells("G" + (14 + brojStavki + 2).ToString()).Style.Border.OutsideBorderColor = XLColor.FromArgb(216, 228, 188);
            sheet.Cells("G" + (14 + brojStavki + 2).ToString()).Style.NumberFormat.Format = "#,##0.00 \"KM\"";
            sheet.Cells("G" + (14 + brojStavki + 2).ToString()).Style.Font.Bold = true;
            sheet.Cells("G" + (14 + brojStavki + 2).ToString()).Style.Font.FontName = "Arial";
            sheet.Cells("G" + (14 + brojStavki + 2).ToString()).Style.Font.FontSize = 10;
            sheet.Cells("G" + (14 + brojStavki + 2).ToString()).Style.Font.FontColor = XLColor.Black;
                                  
            sheet.Cells("G" + (14 + brojStavki + 3).ToString()).Value = rabat;
            sheet.Cells("G" + (14 + brojStavki + 3).ToString()).DataType = XLDataType.Number;
            sheet.Cells("G" + (14 + brojStavki + 3).ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            sheet.Cells("G" + (14 + brojStavki + 3).ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            sheet.Cells("G" + (14 + brojStavki + 3).ToString()).Style.Border.OutsideBorderColor = XLColor.FromArgb(216, 228, 188);
            sheet.Cells("G" + (14 + brojStavki + 3).ToString()).Style.NumberFormat.Format = "#,##0.00 \"KM\"";
            sheet.Cells("G" + (14 + brojStavki + 3).ToString()).Style.Font.Bold = true;
            sheet.Cells("G" + (14 + brojStavki + 3).ToString()).Style.Font.FontName = "Arial";
            sheet.Cells("G" + (14 + brojStavki + 3).ToString()).Style.Font.FontSize = 10;
            sheet.Cells("G" + (14 + brojStavki + 3).ToString()).Style.Font.FontColor = XLColor.Black;
                                   
            sheet.Cells("G" + (14 + brojStavki + 4).ToString()).Value = iznos_sa_rabatom;
            sheet.Cells("G" + (14 + brojStavki + 4).ToString()).DataType = XLDataType.Number;
            sheet.Cells("G" + (14 + brojStavki + 4).ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            sheet.Cells("G" + (14 + brojStavki + 4).ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            sheet.Cells("G" + (14 + brojStavki + 4).ToString()).Style.Border.OutsideBorderColor = XLColor.FromArgb(216, 228, 188);
            sheet.Cells("G" + (14 + brojStavki + 4).ToString()).Style.NumberFormat.Format = "#,##0.00 \"KM\"";
            sheet.Cells("G" + (14 + brojStavki + 4).ToString()).Style.Font.Bold = true;
            sheet.Cells("G" + (14 + brojStavki + 4).ToString()).Style.Font.FontName = "Arial";
            sheet.Cells("G" + (14 + brojStavki + 4).ToString()).Style.Font.FontSize = 10;
            sheet.Cells("G" + (14 + brojStavki + 4).ToString()).Style.Font.FontColor = XLColor.Black;
                                  
            sheet.Cells("G" + (14 + brojStavki + 5).ToString()).Value = pdv;
            sheet.Cells("G" + (14 + brojStavki + 5).ToString()).DataType = XLDataType.Number;
            sheet.Cells("G" + (14 + brojStavki + 5).ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            sheet.Cells("G" + (14 + brojStavki + 5).ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            sheet.Cells("G" + (14 + brojStavki + 5).ToString()).Style.Border.OutsideBorderColor = XLColor.FromArgb(216, 228, 188);
            sheet.Cells("G" + (14 + brojStavki + 5).ToString()).Style.NumberFormat.Format = "#,##0.00 \"KM\"";
            sheet.Cells("G" + (14 + brojStavki + 5).ToString()).Style.Font.Bold = true;
            sheet.Cells("G" + (14 + brojStavki + 5).ToString()).Style.Font.FontName = "Arial";
            sheet.Cells("G" + (14 + brojStavki + 5).ToString()).Style.Font.FontSize = 10;
            sheet.Cells("G" + (14 + brojStavki + 5).ToString()).Style.Font.FontColor = XLColor.Black;
                                   
            sheet.Cells("G" + (14 + brojStavki + 6).ToString()).Value = iznos_sa_pdv;
            sheet.Cells("G" + (14 + brojStavki + 6).ToString()).DataType = XLDataType.Number;
            sheet.Cells("G" + (14 + brojStavki + 6).ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            sheet.Cells("G" + (14 + brojStavki + 6).ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            sheet.Cells("G" + (14 + brojStavki + 6).ToString()).Style.Border.OutsideBorderColor = XLColor.FromArgb(216, 228, 188);
            sheet.Cells("G" + (14 + brojStavki + 6).ToString()).Style.NumberFormat.Format = "#,##0.00 \"KM\"";
            sheet.Cells("G" + (14 + brojStavki + 6).ToString()).Style.Font.Bold = true;
            sheet.Cells("G" + (14 + brojStavki + 6).ToString()).Style.Font.FontName = "Arial";
            sheet.Cells("G" + (14 + brojStavki + 6).ToString()).Style.Font.FontSize = 10;
            sheet.Cells("G" + (14 + brojStavki + 6).ToString()).Style.Font.FontColor = XLColor.Black;
                                  
            sheet.Cells("E" + (14 + brojStavki + 8).ToString()).Style.Font.FontName = "Arial";
            sheet.Cells("E" + (14 + brojStavki + 8).ToString()).Style.Font.FontSize = 10;
            sheet.Cells("E" + (14 + brojStavki + 8).ToString()).Style.Font.FontColor = XLColor.Black;
                                   
            sheet.Cells("E" + (14 + brojStavki + 8)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            sheet.Cells("E" + (14 + brojStavki + 8).ToString()).Value = "Dokument sastavio: " + p.Korisnik.ime + " " + p.Korisnik.prezime;
            sheet.Range("E" + (14 + brojStavki + 8) + ":G" + (14 + brojStavki + 8)).Row(1).Merge();
                                   
            sheet.Cells("F" + (14 + brojStavki + 12).ToString()).Value = "M.P.";
            sheet.Cells("A" + (14 + brojStavki + 16).ToString()).Value = "Hvala na povjerenju!";
            sheet.Range("A" + (14 + brojStavki + 16).ToString() + ":" + "G" + (14 + brojStavki + 16).ToString()).Row(1).Merge();
            sheet.Cells("A" + (14 + brojStavki + 16).ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var imagePath = "footer.jpg";

            var image = sheet.AddPicture(imagePath)
                .MoveTo(sheet.Cell("A" + (14 + brojStavki + 16).ToString())).Scale(0.1);

            string fileName = dir + "\\MINTICT_Ponuda_" + rednibroj.Replace("/", "-") + ".xlsm";
            if (File.Exists(fileName) == true)
            { 
                try
                {
                    File.Delete(fileName);
                }
                catch (Exception ex)
                {
                    return null;
                }
                doc.SaveAs(fileName);
            }
            else
            {
                doc.SaveAs(fileName);
            }

            return fileName;
        }
        public static void CopyPropertiesTo<T, TU>(this T source, TU dest)
        {
            var sourceProps = typeof(T).GetProperties().Where(x => x.CanRead).ToList();
            var destProps = typeof(TU).GetProperties()
                    .Where(x => x.CanWrite)
                    .ToList();

            foreach (var sourceProp in sourceProps)
            {
                if (sourceProp.Name != "stavke")
                {
                    if (destProps.Any(x => x.Name == sourceProp.Name))
                    {
                        var p = destProps.First(x => x.Name == sourceProp.Name);
                        if (p.CanWrite)
                        { // check if the property can be set or no.
                            p.SetValue(dest, sourceProp.GetValue(source, null), null);
                        }
                    }
                }
            }

        }


        public static void LogException(Exception ex)
        {
            string logDirectory = "C:\\Delos\\log";
            DirectoryInfo d = new DirectoryInfo(logDirectory);
            if (d.Exists == false)
                d.Create();
            using (StreamWriter sw = new StreamWriter(Path.Combine(logDirectory, "log.txt"), true))
            {
                string msg = null;

                var exception = ex;
                msg = exception.Message + "\t\t" + exception.StackTrace + "\t\t" + exception.Source + "\t\t" + exception.TargetSite + "\t\t" + exception.Data;
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                    msg += Environment.NewLine + exception.Message + "\t\t" + exception.StackTrace + "\t\t" + exception.Source + "\t\t" + exception.TargetSite + "\t\t" + exception.Data;
                }
                sw.WriteLine(DateTime.Now);
                sw.WriteLine(msg);
                sw.Close();
            }
        }
    }
}
