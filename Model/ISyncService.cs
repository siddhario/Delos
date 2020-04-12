using Delos.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delos.Model
{
    public class SyncServiceConfig
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int IntervalInMinutes { get; set; }
        public string Implementation { get; set; }
    }
    public abstract class ISyncService
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int IntervalInMinutes { get; set; }
        public abstract Task<List<artikal>> SyncAsync();
        public bool UdpateDb(DelosDbContext dbContext,List<artikal> artikli)
        {
            foreach (var a in artikli)
            {
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
                    art.kolicina = a.kolicina;
                    art.naziv = a.naziv;
                    art.slike = a.slike;
                }
              
            }
            
            dbContext.SaveChanges();
            return true;
        }
    }
}
