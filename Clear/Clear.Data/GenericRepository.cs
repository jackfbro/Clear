using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Clear.Data
{
    class GenericRepository<T> where T : class
    {
        private DbContext context = DbContextFactory.CreateCurrentDbContext();
        private DbSet<T> dbSet;
        public GenericRepository(ClearDbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<T>();
        }

        #region Insert
        public virtual void Insert(T entity)
        {
            dbSet.Add(entity);
        }
        public virtual void Insert(List<T> entityList)
        {
            dbSet.AddRange(entityList);
        }
        #endregion

        #region Delete
        public virtual void Delete(params object[] ids)
        {
            T entityToDelete = dbSet.Find(ids);
            this.Delete(entityToDelete);
        }
        public virtual void Delete(T entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Deleted)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }
        public virtual void Delete(Expression<Func<T, bool>> delWhere)
        {
            List<T> listToDelete = dbSet.Where(delWhere).ToList();
           dbSet.RemoveRange(listToDelete);
        }
        /// <summary>
        /// 批量物理删除数据,也可以用作单个物理删除--此方法适用于id为int类型的表--性能会比先查询后删除快
        /// </summary>
        /// <param name="ids">ID集合1,2,3</param>
        /// <returns>是否成功</returns>
        public bool DeletePhysics(string ids)
        {
            var tableName = typeof(T).Name;//获取表名   
            var sql = string.Format("delete from {0} where id in({1})", tableName, ids);
            return context.Database.ExecuteSqlCommand(sql) > 0;
        }
        #endregion

        #region Update
        public virtual void Update(T entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }
        public virtual void Update(List<T> listToUpdate)
        {
            listToUpdate.ForEach(entity => {
                dbSet.Attach(entity);
                context.Entry(entity).State = EntityState.Modified;
            });
        }
        /// <summary>
        /// 修改指定列的值
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertyNames"></param>
        public virtual void Update(T entity, params string[] propertyNames)
        {
            DbEntityEntry entry = context.Entry<T>(entity);
            if (entry.State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            entry.State = EntityState.Unchanged;
            propertyNames.ToList().ForEach(p => { entry.Property(p).IsModified = true; });
        }
        #endregion

        #region Select
        public T GetByID(params object[] ids)
        {
            return dbSet.Find(ids);
        }
        public T Get(Expression<Func<T, bool>> whereLambda)
        {
            var entity = dbSet.FirstOrDefault<T>(whereLambda);
            return entity;
        }

        public IQueryable<T> GetList<S>(Expression<Func<T, bool>> whereLambda,Expression<Func<T, S>> orderLambda, bool isAsc)
        {
            var list = dbSet.Where(whereLambda);
            list = isAsc ? list.OrderBy(orderLambda) : list.OrderByDescending(orderLambda);
            return list;
        }
        public IQueryable<T> GetPageList<S>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, S>> orderLambda, bool isAsc, int pageIndex, int pageSize, out int pageCount)
        {
            var temp = context.Set<T>().Where<T>(whereLambda);
            pageCount = temp.Count();
            temp = isAsc ? temp.OrderBy(orderLambda) : temp.OrderByDescending(orderLambda);
            return temp.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="anyLambda">查询表达式</param>
        /// <returns>布尔值</returns>
        public bool Exist(Expression<Func<T, bool>> whereLambda)
        {
            //确定序列中的任何元素是否都满足条件
            return this.Get(whereLambda) != null;
        }
        public bool Any(Expression<Func<T, bool>> anyLambda)
        {
            //确定序列中的任何元素是否都满足条件
            return dbSet.Any(anyLambda);
        }
        public IEnumerable<T> SqlQuery(string query, params object[] parameters)
        {
            return dbSet.SqlQuery(query, parameters).ToList();
        }
        #endregion       
        /// <summary>
        /// Creates a raw SQL query that will return elements of the given generic type.  The type can be any type that has properties that match the names of the columns returned from the query, or can be a simple primitive type. The type does not have to be an entity type. The results of this query are never tracked by the context even if the type of object returned is an entity type.
        /// </summary>
        /// <typeparam name="TElement">The type of object returned by the query.</typeparam>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <returns>Result</returns>
        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            return context.Database.SqlQuery<TElement>(sql, parameters);
        }
        #region 待定
  
        public virtual IEnumerable<T> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }
        #endregion
    }
}
