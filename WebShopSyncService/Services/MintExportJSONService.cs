using Delos.Contexts;
using Delos.Model;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Delos.Services
{
    public class MintExportJSONService : IExportService
    {
        public override async Task ExportAsync()
        {
            using (var dbContext = new DelosDbContext(ConnectionString))
            {
                var artikli = dbContext.artikal.Where(a=>dbContext.kategorija.Where(k=>k.aktivna==true).Select(k=>k.naziv).Contains(a.kategorija) && a.aktivan==true).ToList();
                var json = JsonSerializer.Serialize(artikli);
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
            }
        }

        private bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
