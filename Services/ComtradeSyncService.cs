﻿using ComtradeService;
using Delos.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using static ComtradeService.CTProductsInStockSoapClient;

namespace Delos.Services
{
    public class ComtradeSyncService : ISyncService
    {
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
                    a.sifra = a.dobavljac + "_" + a.dobavljac_sifra;
                    a.naziv = res.NAME;
                    decimal cijena;
                    decimal.TryParse(res.RETAILPRICE.Replace(",", "."), System.Globalization.NumberStyles.Any, new CultureInfo("en-US"), out cijena);
                    a.cijena_sa_rabatom = cijena;
                    a.dostupnost = res.QTTYINSTOCK;
                    a.slike = res.IMAGE_URLS;
                    a.vrste = new List<string>() { res.PRODUCTGROUPCODE };
                    decimal kolicina;
                    decimal.TryParse(res.QTTYINSTOCK, System.Globalization.NumberStyles.Any, new CultureInfo("en-US"), out kolicina);
                    artikli.Add(a);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return artikli;
        }
    }
}