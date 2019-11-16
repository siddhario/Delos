using System.Collections.Generic;
using System.Linq;

namespace Delos
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
    }
}
