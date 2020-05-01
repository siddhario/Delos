using Delos.Helpers;
using Delos.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml;

namespace Delos.Services
{
    public class MintSyncService : IImportService
    {

        public override async Task<List<artikal>> SyncAsync()
        {
            List<artikal> artikli = new List<artikal>();
            artikal artikal = null;

            StreamReader sr = new StreamReader(Config.Path);
            var content = sr.ReadToEnd();
            content = content.Replace("&", "&amp;");
            sr.Close();

            StreamWriter sw = new StreamWriter(Config.Path);
            sw.Write(content);
            sw.Close();

            using (Stream s = new FileStream(Config.Path, FileMode.Open))
            {
                using (
                    XmlTextReader reader = new XmlTextReader(s)
                    )
                {
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element: // The node is an element.
                                {
                                    if (reader.Name == "Red")
                                    {
                                        artikal = new artikal() { dobavljac = this.Config.Description };
                                        artikli.Add(artikal);
                                    }
                                    if (reader.Name == "Id")
                                    {
                                        artikal.dobavljac_sifra = reader.ReadInnerXml().Trim();
                                        artikal.sifra = this.Config.Id.ToString().PadLeft(3,'0') + "_" + artikal.dobavljac_sifra;
                                    }
                                    if (reader.Name == "Artikal")
                                    {
                                        artikal.naziv = reader.ReadInnerXml().Trim();
                                    }

                                    if (reader.Name == "BARKOD")
                                    {
                                        artikal.barkod = reader.ReadInnerXml().Trim();
                                        artikal.barkod = artikal.barkod.Trim() == "" ? null : artikal.barkod.Trim();
                                    }

                                    if (reader.Name == "OPIS")
                                    {
                                        artikal.opis = reader.ReadInnerXml().Trim();
                                        artikal.opis = artikal.opis.Trim() == "" ? null : artikal.opis.Trim();
                                    }

                                    if (reader.Name == "Vrsta")
                                    {
                                        var vrste = reader.ReadInnerXml().Trim();
                                        if (vrste != "")
                                            artikal.vrste = new List<string> { vrste };
                                    }
                                    if (reader.Name == "Nabavnacena")
                                    {
                                        decimal cijena;
                                        decimal.TryParse(reader.ReadInnerXml().Trim().Replace(",", "."), System.Globalization.NumberStyles.Any, new CultureInfo("en-US"), out cijena);
                                        artikal.cijena_sa_rabatom = cijena;
                                    }
                                    if (reader.Name == "Prodajnacena")
                                    {
                                        decimal cijena;
                                        decimal.TryParse(reader.ReadInnerXml().Trim().Replace(",", "."), System.Globalization.NumberStyles.Any, new CultureInfo("en-US"), out cijena);
                                        artikal.cijena_prodajna = cijena;
                                    }
                                    if (reader.Name == "Kolicina")
                                    {
                                        decimal kolicina;
                                        decimal.TryParse(reader.ReadInnerXml().Trim().Replace(",", "."), System.Globalization.NumberStyles.Any, new CultureInfo("en-US"), out kolicina);
                                        artikal.kolicina = kolicina;
                                        artikal.dostupnost = artikal.kolicina == 0 ? "0" : artikal.kolicina.ToString();
                                    }
                                    break;
                                }
                        }
                    }
                }
            }

            return artikli;
        }
    }
}



