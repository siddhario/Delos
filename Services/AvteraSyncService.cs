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
            String URLString = "https://www.avtera.ba/XmlExport/mSYR1FItWIWeI4YzW1mJkX5xrCmKUMmGoPkEoaf2MwECP1ZYnr1Yr60vvfgwFMGGBz0seC5d43VF9hc1oCOxM3m71qxM6gaixQFG/8OACnAQ0lYS508ac6IDe6ThG_UMNg_2_CBFbjHA2VCB8_eZ8Wtha9SRdL0KDi3prYZgY3Ph7Vth11p6Q-JjZAMT4OifVmEEmrE8Fh6VBkNY5bjXna59UsRlZMRQrpLh4/ZALAvteraMINTICT";
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
                                artikal = new artikal() { dobavljac = this.Description };
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
