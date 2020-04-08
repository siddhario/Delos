using ComtradeService;
using Delos.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Threading.Tasks;
using System.Xml;
using static ComtradeService.CTProductsInStockSoapClient;

namespace Delos.Services
{
    public class ComtradeSyncService : ISyncService
    {
        public interface ISoapDemoApi
        {
            Task<ComtradeService.CTProductsInStockSoapClient> GetInstanceAsync();
            Task<object> GetArtikli(string username, string password);
        }
        public class SoapDemoApi : ISoapDemoApi
        {
            public readonly string serviceUrl = "https://www.crcind.com:443/csp/samples/SOAP.Demo.cls";
            public readonly EndpointAddress endpointAddress;
            public readonly BasicHttpBinding basicHttpBinding;

            public SoapDemoApi()
            {
                endpointAddress = new EndpointAddress(serviceUrl);

                basicHttpBinding =
                    new BasicHttpBinding(endpointAddress.Uri.Scheme.ToLower() == "http" ?
                                BasicHttpSecurityMode.None : BasicHttpSecurityMode.Transport);

                //Please set the time accordingly, this is only for demo
                basicHttpBinding.OpenTimeout = TimeSpan.MaxValue;
                basicHttpBinding.CloseTimeout = TimeSpan.MaxValue;
                basicHttpBinding.ReceiveTimeout = TimeSpan.MaxValue;
                basicHttpBinding.SendTimeout = TimeSpan.MaxValue;
            }

            public async Task<CTProductsInStockSoapClient> GetInstanceAsync()
            {
                return await Task.Run(() => new CTProductsInStockSoapClient(basicHttpBinding, endpointAddress));
            }


            public async Task<object> GetArtikli(string username, string password)
            {
                var client = await GetInstanceAsync();
                ComtradeService.GetCTProducts_WithAttributesRequest inValue = new ComtradeService.GetCTProducts_WithAttributesRequest();
                inValue.Body = new ComtradeService.GetCTProducts_WithAttributesRequestBody();
                inValue.Body.username = username;
                inValue.Body.password = password;
                //inValue.Body.productGroupCode = productGroupCode;
                //inValue.Body.manufacturerCode = manufacturerCode;
                //inValue.Body.searchphrase = searchphrase;
                return await ((ComtradeService.CTProductsInStockSoap)(this)).GetCTProducts_WithAttributesAsync(inValue);

                //var response = await client.GetCTProducts_WithAttributesAsync(username,password,null,null,null);
                //return response;
            }
        }

        public override async Task<List<artikal>> SyncAsync()
        {

            List<artikal> artikli = new List<artikal>();
         
                try
                {
                    CTProductsInStockSoapClient cl = new CTProductsInStockSoapClient(EndpointConfiguration.CTProductsInStockSoap12);

                    var result = await cl.GetCTProducts_WithAttributesAsync("mint", "marko2020", null, null, null).ConfigureAwait(false);
                    foreach (var res in result.Body.GetCTProducts_WithAttributesResult)
                    {
                        artikal a = new artikal();
                        a.dobavljac_sifra = res.CODE;
                        a.dobavljac = this.Description;
                        a.naziv = res.NAME;
                        decimal cijena;
                        decimal.TryParse(res.RETAILPRICE.Replace(",", "."), System.Globalization.NumberStyles.Any, new CultureInfo("en-US"), out cijena);
                        a.cijena_sa_rabatom = cijena;
                        a.dostupnost = res.QTTYINSTOCK;
                        decimal kolicina;
                        decimal.TryParse(res.QTTYINSTOCK, System.Globalization.NumberStyles.Any, new CultureInfo("en-US"), out kolicina);
                        artikli.Add(a);
                    }

                }
                catch (MessageSecurityException ex)
                {
                    //error caught here
                    throw;
                }

          
            foreach (var art in artikli)
                Console.WriteLine(art.dobavljac + " " + art.dobavljac_sifra + " " + art.naziv + " " + art.kolicina + " " + art.cijena_sa_rabatom);

            return artikli;
        }
    }
}
