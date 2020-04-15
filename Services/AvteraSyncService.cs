using Delos.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Delos.Services
{
    public class AvteraSyncService : ISyncService
    {
        public override Task<List<artikal>> SyncAsync()
        {
            String URLString = Config.Url[0];
            XmlTextReader reader = new XmlTextReader(URLString);

            List<artikal> artikli = new List<artikal>();
            artikal artikal = null;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: // The node is an element.
                        {
                            if (reader.Name == "izdelek")
                            {
                                artikal = new artikal() { dobavljac = this.Config.Description };
                                artikli.Add(artikal);
                            }
                            if (reader.Name == "izdelekID")
                            {
                                artikal.dobavljac_sifra = reader.ReadInnerXml().Trim();
                                artikal.sifra = artikal.dobavljac + "_" + artikal.dobavljac_sifra;
                            }
                            if (reader.Name == "slikaVelika")
                            {
                                string url = reader.ReadInnerXml().Trim();
                                if (url != "")
                                {
                                    url = url.Split("<![CDATA[")[1].Split("]]>")[0];
                                    artikal.slike = new List<string>() { url };
                                }
                            }
                          
                            if (reader.Name == "skupinaIzdelka" || reader.Name == "kategorija")
                            {
                                string vrsta = reader.ReadInnerXml().Trim();
                                if (vrsta != "")
                                {
                                    //vrsta = vrsta.Split("<![CDATA[")[1].Split("]]>")[0];
                                    if (artikal.vrste == null)
                                        artikal.vrste = new List<string>();
                                    artikal.vrste.Add(vrsta);
                                }
                            }
                            if (reader.Name == "izdelekIme")
                            {
                                artikal.naziv = reader.ReadInnerXml().Split("<![CDATA[")[1].Split("]]>")[0].Trim();
                            }
                            if (reader.Name == "nabavnaCena")
                            {
                                string c = reader.ReadInnerXml().Trim();
                                c = c.Replace(",", ".");
                                decimal cijena;
                                decimal.TryParse(c, System.Globalization.NumberStyles.Any, new CultureInfo("en-US"), out cijena);
                                artikal.cijena_sa_rabatom = cijena;
                            }
                            if (reader.Name == "zaloga")
                            {
                                string k = reader.ReadInnerXml().Trim();
                                artikal.dostupnost = k;
                                decimal kolicina;
                                decimal.TryParse(k, out kolicina);
                                artikal.kolicina = kolicina;
                            }

                            break;
                        }
                }
            }


            return Task.FromResult(artikli);
        }
    }
}
