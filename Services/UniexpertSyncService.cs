using Delos.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Delos.Services
{
    public class UniexpertSyncService : ISyncService
    {
        public override Task<List<artikal>> SyncAsync()
        {
            String URLString = "https://www.ue.ba/ekupi.xml";
            XmlTextReader reader = new XmlTextReader(URLString);

            List<artikal> artikli = new List<artikal>();
            artikal artikal = null;
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: // The node is an element.
                        {
                            if (reader.Name == "item")
                            {
                                artikal = new artikal() { dobavljac = this.Description };
                                artikal.sifra = artikal.dobavljac + "_" + artikal.dobavljac_sifra;
                                artikli.Add(artikal);
                            }
                            if (reader.Name == "Sifra")
                            {
                                artikal.dobavljac_sifra = reader.ReadInnerXml().Split("<![CDATA[")[1].Split("]]>")[0].Trim();
                                artikal.sifra = artikal.dobavljac + "_" + artikal.dobavljac_sifra;
                            }
                            if (reader.Name == "Naziv")
                                artikal.naziv = reader.ReadInnerXml().Split("<![CDATA[")[1].Split("]]>")[0].Trim();
                            if (reader.Name == "Nabavna-cijena")
                            {
                                string c = reader.ReadInnerXml().Trim();
                                c = c.Replace(",", ".");
                                decimal cijena;
                                decimal.TryParse(c, System.Globalization.NumberStyles.Any, new CultureInfo("en-US"), out cijena);
                                artikal.cijena_sa_rabatom = cijena;
                            }
                            if (reader.Name == "Kolicina")
                            {
                                string k = reader.ReadInnerXml().Trim();
                                artikal.dostupnost = k;
                                decimal kolicina;
                                decimal.TryParse(k, out kolicina);
                                artikal.kolicina = kolicina;
                            }
                            if (reader.Name.StartsWith("Slika"))
                            {
                                var slikaUrl = reader.ReadInnerXml().Trim();
                                if (slikaUrl != "")
                                {
                                    slikaUrl = slikaUrl.Split("<![CDATA[")[1].Split("]]>")[0];
                                    if (artikal.slike == null)
                                        artikal.slike = new List<string>();
                                    artikal.slike.Add(slikaUrl);
                                }
                            }

                            if (reader.Name.StartsWith("Kategorija"))
                            {
                                var kategorija = reader.ReadInnerXml().Trim();
                                if (kategorija != "")
                                {
                                    kategorija = kategorija.Split("<![CDATA[")[1].Split("]]>")[0];
                                    if (artikal.vrste == null)
                                        artikal.vrste = new List<string>();
                                    artikal.vrste.Add(kategorija);
                                }
                            }
                            break;
                        }
                }
            }


            return Task.FromResult(artikli);
        }
    }
}
