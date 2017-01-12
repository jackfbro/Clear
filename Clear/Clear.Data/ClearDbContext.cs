using Clear.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clear.Data
{
    public class ClearDbContext:DbContext
    {
        public ClearDbContext()
            : base("name=ConnectionString")
        {
            //this.Configuration.AutoDetectChangesEnabled = false;//关闭自动跟踪对象的属性变化 false,TryUpdateModel将不可用
            base.Configuration.ValidateOnSaveEnabled = false;
            //System.Data.Entity.Database.SetInitializer<MyWebContext>(null);
            this.Configuration.UseDatabaseNullSemantics = true; //关闭数据库null比较行为
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // prevents table names from being pluralized
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>(); 
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<UserInfo> UserInfo { get; set; }
    }
}
