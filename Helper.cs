﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delos
{
    public static class Helper
    {
        public static void CopyPropertiesTo<T, TU>(this T source, TU dest)
        {
            var sourceProps = typeof(T).GetProperties().Where(x => x.CanRead).ToList();
            var destProps = typeof(TU).GetProperties()
                    .Where(x => x.CanWrite)
                    .ToList();

            foreach (var sourceProp in sourceProps)
            {
                if (sourceProp.Name != "stavke")
                {
                    if (destProps.Any(x => x.Name == sourceProp.Name))
                    {
                        var p = destProps.First(x => x.Name == sourceProp.Name);
                        if (p.CanWrite)
                        { // check if the property can be set or no.
                            p.SetValue(dest, sourceProp.GetValue(source, null), null);
                        }
                    }
                }
            }

        }
    }
}
