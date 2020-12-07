using ClosedXML.Excel;
using Delos.Contexts;
using Delos.Model;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Delos.Helpers
{
    public static class Helper
    {


        public static string StampaPrijave(prijava prijava)
        {
            //string dir = Environment.SpecialFolder.MyDocuments + "\\ServisDB\\";

            string dir = "C:\\temp";

            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }


            XLWorkbook doc = new XLWorkbook("PRIJEMNICA NA SERVIS.xlsx");
            //doc.Worksheets.Add("PRIJAVA");

            var sheet = doc.Worksheet(1);


            sheet.Cells("C17").Value = prijava.broj;
            sheet.Cells("C17").DataType = XLDataType.Text;
            sheet.Cells("C15").Value = prijava.broj;
            sheet.Cells("C15").Style.Font.FontName = "Free 3 of 9 Extended";
            sheet.Cells("C15").Style.Font.FontSize = 28;
            sheet.Cells("C18").Value = prijava.broj_garantnog_lista;
            sheet.Cells("C21").Value = prijava.datum;
            sheet.Cells("C24").Value = prijava.kupac_ime;
            sheet.Cells("C25").DataType = XLDataType.Text;
            sheet.Cells("C25").Style.NumberFormat.Format = "@";
            sheet.Cells("C25").Value = prijava.kupac_telefon;
            sheet.Cells("C28").Value = prijava.model;
            sheet.Cells("C29").Value = prijava.serijski_broj;
            sheet.Cells("C31").Value = prijava.dodatna_oprema;
            sheet.Cells("C32").Value = prijava.predmet;

            sheet.Cells("G48").Value = prijava.serviser_primio;

            string fileName = dir + "\\" + prijava.broj.Replace("/", "-") + ".xlsx";

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

            sheet.Range("A" + (14 + brojStavki + 2).ToString() + ":" + "F" + (14 + brojStavki + 2).ToString()).Row(1).Merge();
            sheet.Range("A" + (14 + brojStavki + 3).ToString() + ":" + "F" + (14 + brojStavki + 3).ToString()).Row(1).Merge();
            sheet.Range("A" + (14 + brojStavki + 4).ToString() + ":" + "F" + (14 + brojStavki + 4).ToString()).Row(1).Merge();
            sheet.Range("A" + (14 + brojStavki + 5).ToString() + ":" + "F" + (14 + brojStavki + 5).ToString()).Row(1).Merge();
            sheet.Range("A" + (14 + brojStavki + 6).ToString() + ":" + "F" + (14 + brojStavki + 6).ToString()).Row(1).Merge();

            sheet.Range("A" + (14 + brojStavki + 2).ToString() + ":" + "F" + (14 + brojStavki + 2).ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.None;
            sheet.Range("A" + (14 + brojStavki + 3).ToString() + ":" + "F" + (14 + brojStavki + 3).ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.None;
            sheet.Range("A" + (14 + brojStavki + 4).ToString() + ":" + "F" + (14 + brojStavki + 4).ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.None;
            sheet.Range("A" + (14 + brojStavki + 5).ToString() + ":" + "F" + (14 + brojStavki + 5).ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.None;
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
        public static string StampaUgovora(ugovor ugovor)
        {

            //string dir = Environment.SpecialFolder.MyDocuments + "\\ServisDB\\";

            string dir = "C:\\temp";

            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }


            XLWorkbook doc = new XLWorkbook("Ugovor.xlsx");
            //doc.Worksheets.Add("PRIJAVA");

            var sheet = doc.Worksheet(1);


            sheet.Cells("A5").Value = ugovor.kupac_naziv;
            sheet.Cells("A5").DataType = XLDataType.Text;

            sheet.Cells("A6").Value = ugovor.kupac_adresa;
            sheet.Cells("A6").DataType = XLDataType.Text;


            sheet.Cells("A7").Value = "LK:" + ugovor.kupac_broj_lk;
            sheet.Cells("A7").DataType = XLDataType.Text;


            sheet.Cells("A8").Value = "JMBG:" + ugovor.kupac_maticni_broj;
            sheet.Cells("A8").DataType = XLDataType.Text;

            sheet.Cells("A17").Value = sheet.Cell("A17").Value.ToString().Replace("$IZNOS$", ugovor.iznos_sa_pdv.ToString("N2"));
            sheet.Cells("A17").DataType = XLDataType.Text;
            decimal iznosRate = Math.Round((ugovor.iznos_sa_pdv - ugovor.inicijalno_placeno) / ugovor.broj_rata, 2, MidpointRounding.AwayFromZero);
            sheet.Cells("A21").Value = sheet.Cell("A21").Value.ToString()
                .Replace("$UPLACENO$", ugovor.inicijalno_placeno.ToString("N2"))
                .Replace("$BROJ_RATA$", ugovor.broj_rata.ToString("N0"))
                .Replace("$IZNOS_RATE$", iznosRate.ToString("N2"));

            sheet.Cells("A21").DataType = XLDataType.Text;

            var rate = ugovor.rate.ToList();
            for (int i = 0; i < rate.Count; i++)
            {
                sheet.Cells("A" + (55 + i).ToString()).Value = (i + 1).ToString() + ". do " + rate[i].rok_placanja.ToString("dd.MM.yyyy") + " - iznos: " + rate[i].iznos.ToString("N2") + " KM";
            }


            string fileName = dir + "\\Ugovor_" + ugovor.broj.Replace("/", "-") + ".xlsx";

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

        public static string StampaPotvrdeOPlacanju(ugovor ugovor,int brojRate)
        {

            //string dir = Environment.SpecialFolder.MyDocuments + "\\ServisDB\\";
            var rata = ugovor.rate.FirstOrDefault(r => r.broj_rate == brojRate);
            string dir = "C:\\temp";

            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }


            XLWorkbook doc = new XLWorkbook("PotvrdaOPlacanju.xlsx");
            //doc.Worksheets.Add("PRIJAVA");

            var sheet = doc.Worksheet(1);


            sheet.Cells("A9").Value = ugovor.kupac_naziv;
            sheet.Cells("A9").DataType = XLDataType.Text;

            sheet.Cells("A10").Value = ugovor.kupac_adresa;
            sheet.Cells("A10").DataType = XLDataType.Text;


            sheet.Cells("A11").Value = "LK:" + ugovor.kupac_broj_lk;
            sheet.Cells("A11").DataType = XLDataType.Text;


            sheet.Cells("A12").Value = "JMBG:" + ugovor.kupac_maticni_broj;
            sheet.Cells("A12").DataType = XLDataType.Text;

            sheet.Cells("A16").Value = sheet.Cell("A16").Value.ToString()
                .Replace("$DATUM_UPLATE$", rata.datum_placanja.Value.ToString("dd.MM.yyyy."))
                .Replace("$BROJ_RATE$", rata.broj_rate.ToString())
                .Replace("$UPLACENO$", rata.uplaceno.Value.ToString("N2"))
                ;
            sheet.Cells("A16").DataType = XLDataType.Text;

            sheet.Cells("A17").Value = sheet.Cell("A17").Value.ToString()
                .Replace("$BROJ_RACUNA$", ugovor.broj_racuna);

            sheet.Cells("A18").Value = sheet.Cell("A18").Value.ToString()
                .Replace("$BROJ_UGOVORA$", ugovor.broj);


            decimal iznosRate = Math.Round((ugovor.iznos_sa_pdv - ugovor.inicijalno_placeno) / ugovor.broj_rata, 2, MidpointRounding.AwayFromZero);
            sheet.Cells("A20").Value = sheet.Cell("A20").Value.ToString()
                .Replace("$INICIJALNO_UPLACENO$", ugovor.inicijalno_placeno.ToString("N2"));

            sheet.Cells("A21").Value = sheet.Cell("A21").Value.ToString()
            .Replace("$SUMA_UPLATA$", ugovor.suma_uplata.ToString("N2"));

            sheet.Cells("A22").Value = sheet.Cell("A22").Value.ToString()
            .Replace("$PREOSTALO_ZA_UPLATU$", ugovor.preostalo_za_uplatu.ToString("N2"));

            var rate = ugovor.rate.ToList();
            int i = 0;
            foreach (var r in rate.Where(rr => rr.uplaceno != 0))
            {
                sheet.Cells("A" + (57 + i).ToString()).Value = r.broj_rate.ToString() + ". rata - uplaćeno ("+r.datum_placanja.Value.ToString("dd.MM.yyyy")+"): " + r.uplaceno.Value.ToString("N2")+ " KM";
                i++;
            }

            i = 0;
            foreach (var r in rate.Where(rr => rr.uplaceno == 0))
            {
                sheet.Cells("E" + (57 + i).ToString()).Value = r.broj_rate.ToString() + ". rata - iznos: " + r.iznos.ToString("N2")+" KM";
                i++;
            }


            string fileName = dir + "\\Ugovor_" + ugovor.broj.Replace("/", "-") + ".xlsx";

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

        public static string StampaRadnogNalogaExcel(prijava prijava)
        {

            //string dir = Environment.SpecialFolder.MyDocuments + "\\ServisDB\\";

            string dir = "C:\\temp";

            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }


            XLWorkbook doc = new XLWorkbook("RadniNalog.xlsx");
            //doc.Worksheets.Add("PRIJAVA");

            var sheet = doc.Worksheet(1);


            sheet.Cells("C10").Value = prijava.broj_naloga;
            sheet.Cells("C10").DataType = XLDataType.Text;
            sheet.Cells("G10").Value = prijava.datum.Value.ToString("dd.MM.yyyy");
            sheet.Cells("G10").DataType = XLDataType.DateTime;
            sheet.Cells("C13").Value = prijava.kupac_ime;
            sheet.Cells("C13").DataType = XLDataType.Text;
            sheet.Cells("C14").Value = prijava.datum.Value.ToString("dd.MM.yyyy");
            sheet.Cells("C14").DataType = XLDataType.DateTime;
            sheet.Cells("A23").Value = prijava.predmet;
            sheet.Cells("A23").DataType = XLDataType.Text;
            sheet.Cells("D37").Value = prijava.serviser;
            sheet.Cells("D37").DataType = XLDataType.Text;


            string fileName = dir + "\\" + prijava.broj.Replace("/", "-") + ".xlsx";

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

        public static void UpdateCijenaIKategorijaArtikala(DelosDbContext dbContext, kategorija kategorija)
        {
            decimal pdvStopa = 17;
            var artikli = dbContext.artikal.Where(a => a.kategorija == kategorija.naziv).ToList();
            if (artikli != null)
            {
                foreach (var art in artikli)
                {
                    art.aktivan = kategorija.aktivna.Value;
                    if (kategorija.marza.HasValue)
                    {
                        if (art.kalkulacija == true && art.cijena_sa_rabatom != 0 && art.cijena_sa_rabatom != 0)
                        {
                            art.cijena_prodajna = art.cijena_sa_rabatom + Math.Round(art.cijena_sa_rabatom * kategorija.marza.Value / 100, 2);
                            art.cijena_mp = art.cijena_prodajna * (1 + pdvStopa / 100);
                        }
                    }
                }
            }
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

        public static void UpdateKategorije(DelosDbContext dbContext)
        {
            decimal pdvStopa = 17;
            var artikli = dbContext.artikal.ToList();
            foreach (var kat in dbContext.kategorija)
            {
                if (kat.kategorije_dobavljaca != null)
                {
                    foreach (var kd in kat.kategorije_dobavljaca)
                    {
                        foreach (var art in artikli)
                        {
                            if (art.vrste != null)
                            {
                                string vrsta = art.vrsteString.Substring(0, art.vrsteString.Length - 1);
                                //foreach (var vrsta in art.vrste)
                                //{
                                if (("[" + art.dobavljac + "] " + vrsta).ToLower() == kd.ToLower())
                                {
                                    art.kategorija = kat.naziv;

                                    if (art.kalkulacija == true && art.cijena_sa_rabatom != 0 && art.cijena_sa_rabatom != 0)
                                    {
                                        art.cijena_prodajna = art.cijena_sa_rabatom + Math.Round(art.cijena_sa_rabatom * (kat.marza == null ? 0 : kat.marza.Value) / 100, 2);
                                        art.cijena_mp = art.cijena_prodajna * (1 + pdvStopa / 100);
                                    }
                                }
                                //}
                            }
                        }
                    }
                }
            }
            dbContext.SaveChanges();
            //Thread.Sleep(10000);
            //foreach (var kat in dbContext.kategorija)
            //    Helper.UpdateCijene(dbContext, kat);



        }
        public enum RecurringModeEnum { NONE = 0, MINUTELY = 1, HOURLY = 2, DAILY = 3, WEEKLY = 4 }

        public static DateTime NextRunOn(ImportServiceConfig updateSchedule)
        {
            DateTime startOfSchedule = DateTime.Now;
            var currentTime = DateTime.Now;
            switch (updateSchedule.RecurringMode)
            {
                case RecurringModeEnum.MINUTELY:
                    {
                        startOfSchedule = currentTime.Date;
                        startOfSchedule = startOfSchedule.AddHours(updateSchedule.StartsOn.Hours);
                        startOfSchedule = startOfSchedule.AddMinutes(updateSchedule.StartsOn.Minutes);
                        startOfSchedule = startOfSchedule.AddSeconds(updateSchedule.StartsOn.Seconds);
                        while (startOfSchedule < DateTime.Now)
                            startOfSchedule = startOfSchedule.AddMinutes(updateSchedule.RecurringInterval);
                        break;
                    }
                case RecurringModeEnum.HOURLY:
                    {
                        startOfSchedule = currentTime.Date;
                        startOfSchedule = startOfSchedule.AddHours(updateSchedule.StartsOn.Hours);
                        startOfSchedule = startOfSchedule.AddMinutes(updateSchedule.StartsOn.Minutes);
                        startOfSchedule = startOfSchedule.AddSeconds(updateSchedule.StartsOn.Seconds);
                        while (startOfSchedule <= DateTime.Now)
                            startOfSchedule = startOfSchedule.AddHours(updateSchedule.RecurringInterval);
                        break;
                    }

                case RecurringModeEnum.DAILY:
                    {
                        startOfSchedule = currentTime.Date;
                        startOfSchedule = startOfSchedule.AddHours(updateSchedule.StartsOn.Hours);
                        startOfSchedule = startOfSchedule.AddMinutes(updateSchedule.StartsOn.Minutes);
                        startOfSchedule = startOfSchedule.AddSeconds(updateSchedule.StartsOn.Seconds);
                        if (startOfSchedule < DateTime.Now)
                            startOfSchedule = startOfSchedule.AddDays(updateSchedule.RecurringInterval);
                        break;
                    }
            }
            return startOfSchedule;
        }
        public static DateTime NextRunOn(ExportServiceConfig updateSchedule)
        {
            DateTime startOfSchedule = DateTime.Now;
            var currentTime = DateTime.Now;
            switch (updateSchedule.RecurringMode)
            {
                case RecurringModeEnum.MINUTELY:
                    {
                        startOfSchedule = currentTime.Date;
                        startOfSchedule = startOfSchedule.AddHours(updateSchedule.StartsOn.Hours);
                        startOfSchedule = startOfSchedule.AddMinutes(updateSchedule.StartsOn.Minutes);
                        startOfSchedule = startOfSchedule.AddSeconds(updateSchedule.StartsOn.Seconds);
                        while (startOfSchedule < DateTime.Now)
                            startOfSchedule = startOfSchedule.AddMinutes(updateSchedule.RecurringInterval);
                        break;
                    }
                case RecurringModeEnum.HOURLY:
                    {
                        startOfSchedule = currentTime.Date;
                        startOfSchedule = startOfSchedule.AddHours(updateSchedule.StartsOn.Hours);
                        startOfSchedule = startOfSchedule.AddMinutes(updateSchedule.StartsOn.Minutes);
                        startOfSchedule = startOfSchedule.AddSeconds(updateSchedule.StartsOn.Seconds);
                        while (startOfSchedule <= DateTime.Now)
                            startOfSchedule = startOfSchedule.AddHours(updateSchedule.RecurringInterval);
                        break;
                    }

                case RecurringModeEnum.DAILY:
                    {
                        startOfSchedule = currentTime.Date;
                        startOfSchedule = startOfSchedule.AddHours(updateSchedule.StartsOn.Hours);
                        startOfSchedule = startOfSchedule.AddMinutes(updateSchedule.StartsOn.Minutes);
                        startOfSchedule = startOfSchedule.AddSeconds(updateSchedule.StartsOn.Seconds);
                        if (startOfSchedule < DateTime.Now)
                            startOfSchedule = startOfSchedule.AddDays(updateSchedule.RecurringInterval);
                        break;
                    }
            }
            return startOfSchedule;
        }
        public static void LogException(Exception ex)
        {
            string logDirectory = "C:\\Delos\\log";
            DirectoryInfo d = new DirectoryInfo(logDirectory);
            if (d.Exists == false)
                d.Create();
            using (StreamWriter sw = new StreamWriter(Path.Combine(logDirectory, "log" + DateTime.Now.ToString("yyyyMMdd_HHmmss.fff") + ".txt"), true))
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
