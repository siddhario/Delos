using Delos.Model;
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
    public class KimtecSyncService : ISyncService
    {
        
        public override async Task<List<artikal>> SyncAsync()
        {
            List<artikal> artikli = new List<artikal>();
            artikal artikal = null;
            X509Certificate2 cert = new X509Certificate2(Directory.GetCurrentDirectory() + "\\Certificates\\B2BMint4504.p12", "4504");
            string url;
            HttpWebRequest httpWebRequest;
            XmlReader reader;

            //PRODUCT LIST
            url = "https://b2b.kimtec.ba/B2BService/HTTP/Product/GetProductsList.aspx";
            httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/xml";
            httpWebRequest.Method = "GET";
            httpWebRequest.ClientCertificates.Add(cert);

            using (var response = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(stream))
                    {
                        var content = sr.ReadToEnd();
                        var settings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment, IgnoreWhitespace = true, IgnoreComments = true };
                        reader = XmlTextReader.Create(new StringReader(content), settings);
                        //PRODUCT LIST
                        using (Stream s = new FileStream(Directory.GetCurrentDirectory() + "\\TestData\\ProductList.xml", FileMode.Open))
                        {
                            using (
                                reader
                                //reader = new XmlTextReader(s)
                                )
                            {
                                while (reader.Read())
                                {
                                    switch (reader.NodeType)
                                    {
                                        case XmlNodeType.Element: // The node is an element.
                                            {
                                                if (reader.Name == "Table")
                                                {
                                                    artikal = new artikal() { dobavljac = this.Description };
                                                    artikli.Add(artikal);
                                                }
                                                if (reader.Name == "ProductCode")
                                                {
                                                    artikal.dobavljac_sifra = reader.ReadInnerXml().Trim();
                                                    artikal.sifra = artikal.dobavljac + "_" + artikal.dobavljac_sifra;
                                                }
                                                if (reader.Name == "ProductName")
                                                    artikal.naziv = reader.ReadInnerXml().Trim();
                                                if (reader.Name == "ProductImageUrl")
                                                    artikal.slike = new List<string>() { reader.ReadInnerXml().Trim().Replace("https:","http:") };
                                                break;
                                            }
                                    }
                                }
                            }
                        }
                    }
                }
            }


            //PRODUCT PRICE AND AVAILABILITY
            url = "https://b2b.kimtec.ba/B2BService/HTTP/Product/GetProductsPriceList.aspx";
            httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/xml";
            httpWebRequest.Method = "GET";
            httpWebRequest.ClientCertificates.Add(cert);

            using (var response = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(stream))
                    {
                        var content = sr.ReadToEnd();
                        var settings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment, IgnoreWhitespace = true, IgnoreComments = true };
                        reader = XmlTextReader.Create(new StringReader(content), settings);
                        using (Stream s = new FileStream(Directory.GetCurrentDirectory() + "\\TestData\\ProductPriceList.xml", FileMode.Open))
                        {
                            using ( 
                                reader
                                //XmlTextReader reader = new XmlTextReader(s)
                                )
                            {
                                while (reader.Read())
                                {
                                    switch (reader.NodeType)
                                    {
                                        case XmlNodeType.Element: // The node is an element.
                                            {
                                                if (reader.Name == "ProductCode")
                                                {
                                                    var sifra = reader.ReadInnerXml().Trim();
                                                    artikal = artikli.FirstOrDefault(aa => aa.dobavljac_sifra == sifra);
                                                }
                                                if (reader.Name == "ProductPartnerPrice" && artikal != null)
                                                {
                                                    decimal cijena;
                                                    decimal.TryParse(reader.ReadInnerXml().Trim().Replace(",", "."), System.Globalization.NumberStyles.Any, new CultureInfo("en-US"), out cijena);
                                                    artikal.cijena_sa_rabatom = cijena;
                                                }
                                                if (reader.Name == "ProductAvailability" && artikal != null)
                                                {
                                                    artikal.dostupnost = reader.ReadInnerXml().Trim();
                                                }
                                                break;
                                            }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return artikli;
        }
    }
}



