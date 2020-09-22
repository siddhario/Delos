using Delos.Contexts;
using Delos.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Delos.Services
{
    public class MintExportJSONService : IExportService
    {

        int duplicateByPermuation = 0;
        public bool MatchArtikal(artikal a,artikal b)
        {
            if (a.barkod != null && a.barkod == b.barkod || a.naziv == b.naziv)
                return true;
            else
            {
                var awords = a.naziv.Split(" ");
                var bwords = b.naziv.Split(" ");
                if(awords.Length==bwords.Length)
                {
                    foreach (var wa in awords)
                        if (bwords.FirstOrDefault(w=>w.ToLower()
                        .Replace("š","s").Replace("č", "c").Replace("ć", "c").Replace("ž", "z").Replace("đ", "d")
                        == wa.ToLower()
                        .Replace("š", "s").Replace("č", "c").Replace("ć", "c").Replace("ž", "z").Replace("đ", "d")
                        ) == null)
                            return false;
                    duplicateByPermuation++;
                    return true;
                }
                return false;
            }
        }
        public override async Task ExportAsync()
        {

            using (var dbContext = new DelosDbContext(ConnectionString))
            {

                DirectoryInfo di = new DirectoryInfo(Config.Path);
                var toDelete = di.GetFiles().Where(f => DateTime.Now.Subtract(f.LastWriteTime).TotalDays > Config.KeepFileDays);
                foreach (var file in toDelete)
                    file.Delete();

                var artikli = from a in dbContext.artikal.Where(a => dbContext.kategorija.
                              Where(k => k.aktivna == true).Select(k => k.naziv).Contains(a.kategorija) && a.aktivan == true
                              && a.dostupnost != null && a.dostupnost != "0" && a.cijena_prodajna != null && a.cijena_prodajna != 0).ToList()
                              select
                              a;

                List<artikal> arts = new List<artikal>();
                int duplicates = 0;
                foreach (var a in artikli)
                {
                    var existing = arts.FirstOrDefault(x => MatchArtikal(x,a));
                    if (existing != null)
                    {
                        duplicates++;
                        if ((existing.prioritet!=null?existing.prioritet:1000) > (a.prioritet!=null?a.prioritet:1000) 
                            || (existing.prioritet==a.prioritet && existing.cijena_mp>a.cijena_mp))
                        {
                            arts.Remove(existing);
                            arts.Add(a);
                        }
                    }
                    else
                        arts.Add(a);
                }
                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                var json = JsonSerializer.Serialize(from a in arts
                                                    select new { a.sifra, a.naziv, a.barkod,
                                                        a.cijena_mp, a.dostupnost, a.kategorija, a.opis, a.slike }, options);
                string fileName = "ArtikliWEBSHOP_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".json";
                string localFilePath = Path.Combine(Config.Path, fileName);
                StreamWriter sw = new StreamWriter(localFilePath);
                await sw.WriteAsync(json);
                sw.Close();

                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateCertificate);
                // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Path.Combine(Config.FtpAddress, fileName));
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // This example assumes the FTP site uses anonymous logon.
                request.Credentials = new NetworkCredential(Config.FtpUsername, Config.FtpPassword);

                // Copy the contents of the file to the request stream.
                byte[] fileContents;
                using (StreamReader sourceStream = new StreamReader(localFilePath))
                {
                    fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                }

                request.ContentLength = fileContents.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileContents, 0, fileContents.Length);
                }

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
                }
                if (Config.KeepFileDays == 0)
                    File.Delete(localFilePath);
            }
        }

        private bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
