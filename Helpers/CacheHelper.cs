using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace EDS.Helpers
{
    public static class CacheHelper
    {

        public static void AddToCache(string strKey, string expirationTime, object objValue)
        {
            double timeSpanMins = Convert.ToDouble(expirationTime);
            HttpContext.Current.Cache.Insert(strKey, objValue, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(timeSpanMins));
        }

        public static object GetFromCache(string strKey)
        {
            return HttpContext.Current.Cache.Get(strKey);
        }
    }
}