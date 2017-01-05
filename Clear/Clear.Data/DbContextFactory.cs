using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Clear.Data
{  /// <summary>
    /// EF上下文对象唯一
    /// </summary>
    public class DbContextFactory
    {
        public static DbContext CreateCurrentDbContext()
        {
            DbContext dbContext = (DbContext)CallContext.GetData("DbContext");
            if (dbContext == null)
            {
                dbContext = new ClearDbContext();

                CallContext.SetData("db", dbContext);
            }
            return dbContext;
        }
    }
}
