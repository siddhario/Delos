﻿using Delos.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Delos.Services
{
    public class AsbisSyncService : ISyncService
    {
        public override Task<List<artikal>> SyncAsync()
        {
            List<artikal> artikli = new List<artikal>();

            String URLString = "https://services.it4profit.com/product/bs/756/PriceAvail.xml?USERNAME=info@mintict.com&PASSWORD=6S3260b7w8";
            XmlTextReader reader = new XmlTextReader(URLString);

            artikal artikal = null;
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: // The node is an element.
                        {
                            if (reader.Name == "PRICE")
                            {
                                artikal = new artikal() { dobavljac = this.Description };
                                artikal.sifra = artikal.dobavljac + "_" + artikal.dobavljac_sifra;
                                artikli.Add(artikal);
                            }
                            if (reader.Name == "GROUP_NAME")
                            {
                                if (artikal.vrste == null)
                                    artikal.vrste = new List<string>();
                                artikal.vrste.Add(reader.ReadInnerXml().Trim());
                            }
                            if (reader.Name == "WIC")
                            {
                                artikal.dobavljac_sifra = reader.ReadInnerXml().Trim();
                                artikal.sifra = artikal.dobavljac + "_" + artikal.dobavljac_sifra;
                            }
                            if (reader.Name == "DESCRIPTION")
                                artikal.naziv = reader.ReadInnerXml().Trim();
                            if (reader.Name == "MY_PRICE")
                            {
                                string c = reader.ReadInnerXml().Trim().Replace(",", ".");
                                decimal cijena;
                                decimal.TryParse(c, System.Globalization.NumberStyles.Any, new CultureInfo("en-US"), out cijena);
                                artikal.cijena_sa_rabatom = cijena;
                            }
                        }
                        if (reader.Name == "AVAIL")
                        {
                            artikal.dostupnost = reader.ReadInnerXml().Trim();
                        }
                        break;
                }
            }

            string catalogUrl = "https://services.it4profit.com/product/hr/756/ProductList.xml?USERNAME=info@mintict.com&PASSWORD=6S3260b7w8";
            //foreach (var art in artikli)
            //    Console.WriteLine(art.dobavljac + " " + art.dobavljac_sifra + " " + art.naziv + " " + art.kolicina + " " + art.cijena_sa_rabatom);
            reader = new XmlTextReader(catalogUrl);
        
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: // The node is an element.
                        {
                            if (reader.Name == "ProductCode")
                            {
                                var sifra = reader.ReadInnerXml().Trim();
                                artikal = artikli.FirstOrDefault(a => a.dobavljac == this.Description && a.dobavljac_sifra == sifra);
                            }
                            if (reader.Name == "Image" && artikal!=null)
                            {
                                if (artikal.slike == null)
                                    artikal.slike = new List<string>();
                                artikal.slike.Add(reader.ReadInnerXml().Trim());
                            }
                        }
                        break;
                }
            }
            return Task.FromResult(artikli);
        }
    }
}