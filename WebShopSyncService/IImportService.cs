using Delos.Contexts;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delos.Model
{

    public abstract class IImportService
    {
        public ImportServiceConfig Config { get; set; }
        public abstract Task<List<artikal>> SyncAsync();

        private void SetKategorija(DelosDbContext dbContext,artikal a)
        {
            foreach (var kat in dbContext.kategorija)
            {
                if (kat.kategorije_dobavljaca != null)
                {
                    foreach (var kd in kat.kategorije_dobavljaca)
                    {
                        if (a.vrste != null)
                        {
                            foreach (var vrsta in a.vrste)
                            {
                                if (("[" + a.dobavljac + "] " + vrsta).ToLower() == kd.ToLower())
                                    a.kategorija = kat.naziv;
                            }
                        }
                    }
                }
            }
        }
        public bool UdpateDb(DelosDbContext dbContext, List<artikal> artikli)
        {
            decimal pdvStopa = 17;
            try
            {
                foreach (var a in artikli)
                {
                    this.SetKategorija(dbContext, a);
                    var kat = dbContext.kategorija.FirstOrDefault(k => k.naziv == a.kategorija);
                    a.kalkulacija = Config.CalculatePrice;

                    if (a.kalkulacija == true && kat != null && kat.marza != null && (a.cijena_prodajna == null || a.cijena_prodajna == 0))
                    {
                        a.cijena_prodajna = a.cijena_sa_rabatom + Math.Round(a.cijena_sa_rabatom * kat.marza.Value / 100, 2);
                    }

                    a.cijena_mp = a.cijena_prodajna * (1 + pdvStopa / 100);

                    var art = dbContext.artikal.FirstOrDefault(ar => a.dobavljac_sifra == ar.dobavljac_sifra && a.dobavljac == ar.dobavljac);

                    if (art == null)
                    {
                        a.zadnje_ucitavanje = DateTime.Now;
                        dbContext.Add(a);
                    }
                    else
                    {
                        art.zadnje_ucitavanje = DateTime.Now;
                        art.cijena_sa_rabatom = a.cijena_sa_rabatom;
                        art.cijena_prodajna = a.cijena_prodajna;
                        art.cijena_mp = a.cijena_mp;
                        art.kolicina = a.kolicina;
                        art.dostupnost = a.dostupnost;
                        art.naziv = a.naziv;
                        art.slike = a.slike;
                        art.vrste = a.vrste;
                        art.sifra = a.sifra;
                        art.barkod = a.barkod;
                        art.garancija = a.garancija;
                        art.brend = a.brend;
                        art.opis = a.opis;
                    }

                }

                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
