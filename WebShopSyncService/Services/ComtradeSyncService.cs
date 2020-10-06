using ComtradeService;
using Delos.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static ComtradeService.CTProductsInStockSoapClient;

namespace Delos.Services
{
    public class ComtradeSyncService : IImportService
    {
        public override async Task<List<artikal>> SyncAsync()
        {
            List<artikal> artikli = new List<artikal>();

            CTProductsInStockSoapClient cl = new CTProductsInStockSoapClient(EndpointConfiguration.CTProductsInStockSoap12);

            var result = await cl.GetCTProducts_WithAttributesAsync(Config.Username, Config.Password, null, null, null).ConfigureAwait(false);
            foreach (var res in result.Body.GetCTProducts_WithAttributesResult)
            {
               
                artikal a = new artikal() {prioritet = this.Config.Priority };
                a.dobavljac_sifra = res.CODE;
                a.dobavljac = this.Config.Description;
                a.sifra = this.Config.Id.ToString().PadLeft(3, '0') + "_" + a.dobavljac_sifra;
                a.naziv = res.NAME;
                if (a.sifra == "002_NV70K1340BB/OL")
                {
                    int tt = 0;
                }
                decimal cijena;
                decimal.TryParse(res.PRICE.Replace(",", "."), System.Globalization.NumberStyles.Any, new CultureInfo("en-US"), out cijena);
                a.cijena_sa_rabatom = cijena;
                a.dostupnost = res.QTTYINSTOCK;
                a.slike = res.IMAGE_URLS;
                a.vrste = new List<string>() { res.PRODUCTGROUPCODE };
                decimal kolicina;
                decimal.TryParse(res.QTTYINSTOCK, System.Globalization.NumberStyles.Any, new CultureInfo("en-US"), out kolicina);
                a.barkod = res.BARCODE.Trim()!=""?res.BARCODE.Trim() : null;
                a.brend = res.MANUFACTURER.Trim() != ""?res.MANUFACTURER.Trim() : null;
                a.garancija = res.WARRANTY.Trim() != "" ? res.WARRANTY.Trim() : null;
                a.opis = res.SHORT_DESCRIPTION.Trim() != "" ? res.SHORT_DESCRIPTION.Trim() : null ;
                artikli.Add(a);
            }

            return artikli;
        }
    }
}
