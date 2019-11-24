using Delos.Model;
using System.Collections.Generic;
using System.Linq;

namespace Delos.Helpers
{
    public static class ExtensionMethods
    {
        public static IEnumerable<korisnik> WithoutPasswords(this IEnumerable<korisnik> users)
        {
            return users.Select(x => x.WithoutPassword());
        }

        public static korisnik WithoutPassword(this korisnik user)
        {
            user.lozinka = null;
            return user;
        }

        public static ponuda_dokument WithoutData(this ponuda_dokument dokument)
        {
            dokument.dokument = null;
            return dokument;
        }

        public static IEnumerable<ponuda_dokument> WithoutDatas(this IEnumerable<ponuda_dokument> dokument)
        {
            return dokument.Select(x => x.WithoutData());
        }
    }
}
