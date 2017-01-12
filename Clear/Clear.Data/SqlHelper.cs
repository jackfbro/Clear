using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;//程序端数据集 DataSet DataTable DataRow...可以将从数据库中查询的数据放在程序缓存中,并提供一系列操作
using System.Data.SqlClient;//.NET Framework 数据提供程序 Connection DataAdapter DataReader Command 用来直接操作数据库
using System.Configuration;
using System.Collections;
namespace Clear.Data
{
    /// <summary>
    /// Microsoft Sql Server数据库操作通用类
    /// </summary>
    public static class SqlHelper
    {
        #region 从web.confng中读取数据库连接字符串
        /// <summary>
        /// 从web.confng中读取数据库连接字符串
        /// </summary>
        /// 
        private static readonly string connStrDefault = ConfigurationManager.ConnectionStrings["ConnStrDefault"].ConnectionString.Trim();
        private static readonly string connStrBasicInfo = ConfigurationManager.ConnectionStrings["ConnStrBasicInfo"].ConnectionString.Trim();
        // Hashtable to store cached parameters
        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());
        #endregion

        #region 查询返回结果集
        /// <summary>
        /// 查询返回结果集,查询语句类型是SQL文本
        /// </summary>
        /// <param name="sqlText">查询语句</param>
        /// <returns>查询返回结果集</returns>
        public static DataTable ExecuteDataTable(string connectionString, string sqlText)
        {
            return ExecuteDataTable(connectionString, sqlText, CommandType.Text, null);
        }
        /// <summary>
        /// 查询返回结果集,查询语句类型是SQL文本
        /// </summary>
        /// <param name="sqlText">查询语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns>查询返回结果集</returns>
        public static DataTable ExecuteDataTable(string connectionString, string sqlText, params SqlParameter[] parameters)
        {
            return ExecuteDataTable(connectionString, sqlText, CommandType.Text, parameters);
        }
        /// <summary>
        /// 查询返回结果集
        /// </summary>
        /// <param name="sqlText">查询语句</param>
        /// <param name="cmdType">查询语句类型，是SQL文本还是存储过程</param>
        /// <param name="parameters">参数数组</param>
        /// <returns>查询返回结果集</returns>
        public static DataTable ExecuteDataTable(string connectionString, string sqlText, CommandType cmdType, params SqlParameter[] parameters)
        {
            //实例化数据集，用于装载DataTable
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, sqlText, parameters);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt); //填充DataTable
                cmd.Parameters.Clear();// 清除参数集,以便再次使用    
                return dt;
            }
        }

        #endregion

        #region 返回SqlDataReader高速输出
        /// <summary>
        /// 返回SqlDataReader高速输出
        /// </summary>
        /// <param name="sqlText">查询语句</param>
        /// <returns>返回SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string connectionString, string sqlText)
        {
            return ExecuteReader(connectionString, sqlText, CommandType.Text, null);
        }
        /// <summary>
        /// 返回SqlDataReader高速输出
        /// </summary>
        /// <param name="sqlText">查询语句</param>
        /// <param name="Paramter">参数</param>
        /// <returns>返回SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string connectionString, string sqlText, params SqlParameter[] parameters)
        {
            return ExecuteReader(connectionString, sqlText, CommandType.Text, parameters);
        }
        /// <summary>
        /// 返回SqlDataReader高速输出
        /// </summary>
        /// <param name="sqlText">查询语句</param>
        /// <param name="cmdType">查询语句类型类型，是文本还是存储</param>
        /// <param name="Paramter">参数</param>
        /// <returns>返回SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string connectionString, string sqlText, CommandType cmdType, params SqlParameter[] parameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, Connection, null, cmdType, sqlText, parameters);
                cmd.Parameters.Clear();// 清除参数集,以便再次使用
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }

        }
        #endregion

        #region 执行查询返回第一行第一列
        /// <summary>
        /// 执行查询返回第一行第一列,查询语句类型是SQL文本
        /// </summary>
        /// <param name="sqlText">查询语句</param>
        /// <returns>返回第一行第一列</returns>
        public static Object ExecuteScalar(string connectionString, string sqlText)
        {
            return ExecuteScalar(connectionString, sqlText, CommandType.Text, null);
        }
        /// <summary>
        /// 执行查询返回第一行第一列,查询语句类型是SQL文本
        /// </summary>
        /// <param name="sqlText">查询语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回第一行第一列</returns>
        public static Object ExecuteScalar(string connectionString, string sqlText, params SqlParameter[] parameters)
        {
            return ExecuteScalar(connectionString, sqlText, CommandType.Text, parameters);
        }
        /// <summary>
        /// 执行查询返回第一行第一列
        /// </summary>
        /// <param name="sqlText">查询语句</param>
        /// <param name="cmdType">语句类型，是文本还是存储过程</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回第一行第一列</returns>
        public static Object ExecuteScalar(string connectionString, string sqlText, CommandType cmdType, params SqlParameter[] parameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(connectionString))//实例化Connection
            {
                PrepareCommand(cmd, conn, null, cmdType, sqlText, parameters);
                object obj = cmd.ExecuteScalar();//执行操作，返回结果
                cmd.Parameters.Clear();// 清除参数集,以便再次使用
                return obj;//返回对象                
            }

        }

        #endregion

        #region 对数据库进行增删改返回受影响行数
        /// <summary>
        /// 对数据库进行增删改返回受影响行数
        /// </summary>
        /// <param name="sqlText">查询语句</param>
        /// <returns>返回受影响行数</returns>
        public static int ExecuteNonQuery(string connectionString, string sqlText)
        {
            return ExecuteNonQuery(connectionString, sqlText, CommandType.Text, null);
        }
        /// <summary>
        /// 对数据库进行增删改返回受影响行数
        /// </summary>
        /// <param name="sqlText">查询语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回受影响行数</returns>
        public static int ExecuteNonQuery(string connectionString, string sqlText, params SqlParameter[] parameters)
        {
            return ExecuteNonQuery(connectionString, sqlText, CommandType.Text, parameters);
        }
        /// <summary>
        /// 对数据库进行增删改返回受影响行数
        /// </summary>
        /// <param name="sqlText">查询语句</param>
        /// <param name="cmdType">语句类型，是文本还是存储过程</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回受影响行数</returns>
        public static int ExecuteNonQuery(string connectionString, string sqlText, CommandType cmdType, params SqlParameter[] parameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(connectionString))//实例化connection
            {
                PrepareCommand(cmd, conn, null, cmdType, sqlText, parameters);
                int val = cmd.ExecuteNonQuery();//执行操作，返回受影响行数
                cmd.Parameters.Clear();
                return val;
            }
        }
        /// <summary>
        /// 对数据库进行增删改返回受影响行数
        /// </summary>
        /// <param name="trans">事务</param>
        /// <param name="sqlText">查询语句</param>
        /// <returns>返回受影响行数</returns>
        public static int ExecuteNonQuery(string connectionString, SqlTransaction trans, string sqlText)
        {
            return ExecuteNonQuery(connectionString, trans, sqlText, CommandType.Text, null);
        }
        /// <summary>
        /// 对数据库进行增删改返回受影响行数
        /// </summary>
        /// <param name="trans">事务</param>
        /// <param name="sqlText">查询语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回受影响行数</returns>
        public static int ExecuteNonQuery(string connectionString, SqlTransaction trans, string sqlText, params SqlParameter[] parameters)
        {
            return ExecuteNonQuery(connectionString, trans, sqlText, CommandType.Text, parameters);
        }
        /// <summary>
        /// 对数据库进行增删改返回受影响行数
        /// </summary>
        /// <param name="trans">事务</param>
        /// <param name="sqlText">查询语句</param>
        /// <param name="cmdType">语句类型，是文本还是存储过程</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回受影响行数</returns>
        public static int ExecuteNonQuery(string connectionString, SqlTransaction trans, string sqlText, CommandType cmdType,params SqlParameter[] parameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, trans, cmdType, sqlText, parameters);
                int val = cmd.ExecuteNonQuery();//执行操作，返回受影响行数
                cmd.Parameters.Clear();
                return val;
            }


        }
        #endregion

        //批量添加/复制系列

        #region DataReader批量添加相比之下，效率高(有事务)
        /// <summary>
        /// SqlDataReader批量添加(有事务)
        /// </summary>
        /// <param name="Reader">数据源</param>
        /// <param name="Mapping">定义数据源和目标源列的关系集合</param>
        /// <param name="DestinationTableName">目标表</param>
        public static bool SqlBulkCopy(string connectionString, SqlDataReader Reader, SqlBulkCopyColumnMapping[] Mapping, string DestinationTableName)
        {
            bool Bool = true;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlTransaction Tran = con.BeginTransaction())//指定事务
                {
                    using (SqlBulkCopy copy = new SqlBulkCopy(con, SqlBulkCopyOptions.KeepIdentity, Tran))
                    {
                        copy.DestinationTableName = DestinationTableName;//设置要添加的表名
                        if (Mapping != null)
                        {
                            //如果有匹配
                            foreach (SqlBulkCopyColumnMapping Mapp in Mapping)
                            {
                                copy.ColumnMappings.Add(Mapp);
                            }
                        }
                        try
                        {
                            copy.WriteToServer(Reader);//批量添加
                            Tran.Commit();//提交事务
                        }
                        catch
                        {
                            Tran.Rollback();//回滚事务
                            Bool = false;
                        }
                        finally
                        {
                            Reader.Close();//关闭
                        }
                    }
                }
            }
            return Bool;//返回结果
        }
        #endregion

        #region DataTable批量添加相比之下,灵活度高(有事务)
        /// <summary>
        /// DataTable批量添加(有事务)
        /// </summary>
        /// <param name="Table">数据源</param>
        /// <param name="Mapping">定义数据源和目标源列的关系集合</param>
        /// <param name="DestinationTableName">目标表</param>
        public static bool SqlBulkCopy(string connectionString, DataTable Table, SqlBulkCopyColumnMapping[] Mapping, string DestinationTableName)
        {
            bool Bool = true;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlTransaction Tran = con.BeginTransaction())
                {
                    using (SqlBulkCopy Copy = new SqlBulkCopy(con, SqlBulkCopyOptions.KeepIdentity, Tran))
                    {
                        Copy.DestinationTableName = DestinationTableName;//指定目标表
                        if (Mapping != null)
                        {
                            //如果有数据
                            foreach (SqlBulkCopyColumnMapping Map in Mapping)
                            {
                                Copy.ColumnMappings.Add(Map);
                            }
                        }
                        try
                        {
                            Copy.WriteToServer(Table);//批量添加
                            Tran.Commit();//提交事务
                        }
                        catch (Exception ex)
                        {
                            Tran.Rollback();//回滚事务
                            Bool = false;
                        }
                    }
                }
            }
            return Bool;
        }
        #endregion


        #region 提取SqlCommand
        /// <summary>
        /// 提取SqlCommand
        /// </summary>
        /// <param name="cmd">sql命令</param>
        /// <param name="conn">Sql连接</param>
        /// <param name="trans">Sql事务</param>
        /// <param name="cmdType">命令类型例如 存储过程或者文本</param>
        /// <param name="sqlText">sql命令文本,例如：Select * from Products</param>
        /// <param name="parameters">执行命令的参数</param>
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string sqlText, SqlParameter[] parameters)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = sqlText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (parameters != null)
            {
                foreach (SqlParameter parm in parameters)
                {
                    parm.Value = parm.Value ?? DBNull.Value;
                    cmd.Parameters.Add(parm);
                }
            }
        }
        #endregion

        #region DataTable反射转类
        public static List<T> MapEntity<T>(DataTable dt) where T : class,new()
        {
            var props = typeof(T).GetProperties();
            var list = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                var temp = new T();
                foreach (var p in props)
                {
                    if (p.CanWrite)
                    {
                        var data = row[p.Name];
                        if (data != DBNull.Value)
                        {
                            p.SetValue(temp, Convert.ChangeType(data, p.PropertyType), null);
                        }
                    }
                }
                list.Add(temp);
            }
            return list;
        }
        /// <summary>
        /// 反射：将一个SqlDataReader实例化为一个实体对象
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="reader">当前指向的reader</param>
        /// <returns>实体对象</returns>
        public static T MapEntity<T>(SqlDataReader reader) where T : class,new()
        {
            var props = typeof(T).GetProperties();
            var entity = new T();
            foreach (var p in props)
            {
                if (p.CanWrite)
                {
                    try
                    {
                        var index = reader.GetOrdinal(p.Name);
                        var data = reader.GetValue(index);
                        var d = reader[p.Name];
                        if (data != DBNull.Value)
                        {
                            p.SetValue(entity, Convert.ChangeType(data, p.PropertyType), null);
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            return entity;

        }
        #endregion

        #region 缓存参数 
        /// <summary>
        /// add parameter array to the cache
        /// </summary>
        /// <param name="cacheKey">Key to the parameter cache</param>
        /// <param name="cmdParms">an array of SqlParamters to be cached</param>
        public static void CacheParameters(string cacheKey, params SqlParameter[] commandParameters)
        {
            parmCache[cacheKey] = commandParameters;
        }

        /// <summary>
        /// Retrieve cached parameters
        /// </summary>
        /// <param name="cacheKey">key used to lookup parameters</param>
        /// <returns>Cached SqlParamters array</returns>
        public static SqlParameter[] GetCachedParameters(string cacheKey)
        {
            SqlParameter[] cachedParms = (SqlParameter[])parmCache[cacheKey];

            if (cachedParms == null)
                return null;

            SqlParameter[] clonedParms = new SqlParameter[cachedParms.Length];

            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms[i] = (SqlParameter)((ICloneable)cachedParms[i]).Clone();

            return clonedParms;
        }
        #endregion
    }
}


