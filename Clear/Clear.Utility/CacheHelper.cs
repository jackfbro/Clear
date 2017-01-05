using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Clear.Utility
{
    public class CacheHelper
    {
        public static void SetCache(string key, object value)
        {
            HttpRuntime.Cache[key] = value;
        }
        public static object GetCache(string key)
        {
            return HttpRuntime.Cache[key];
        }
        public static void Remove(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }
    }
}
