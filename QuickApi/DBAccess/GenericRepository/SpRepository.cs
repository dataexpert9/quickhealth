using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Configuration;
using DBAccess;
//using FOX.DataModels.Context;

namespace FOX.DataModels.GenericRepository
{
    /// <summary>
    /// Generic Repository class for Entity Operations
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public static class SpRepository<TEntity> where TEntity : class
    {
        #region Private member variables...
       // internal static DbContext Context =new DbContextSP();
       // internal static DbSet<TEntity> DbSet= Context.Set<TEntity>();
        //private object 
        #endregion

        #region Public Constructor...
        
        static SpRepository()
        {
        }
        #endregion

        #region Public member methods...


        /// <summary>
        /// generic Execute SP
        /// </summary>
        /// <returns></returns>
        public static List<TEntity> GetListWithStoreProcedure(string query, params object[] parameters)
        {
            using (DbContext Context = new DbContextSP())
            {
                Context.Database.CommandTimeout = 300;
                return Context.Database.SqlQuery<TEntity>(query, parameters).ToList<TEntity>();
            }
            
        }

        public static TEntity GetSingleObjectWithStoreProcedure(string query, params object[] parameters)
        {
            using (DbContext Context = new DbContextSP())
            {
                Context.Database.CommandTimeout = 300;
                return Context.Database.SqlQuery<TEntity>(query, parameters).FirstOrDefault<TEntity>();
            }
        }
        //sql dataAdapter for dataset
        public static SqlDataAdapter getSpSqlDataAdapter(string query)
        {
            using (DbContext Context = new DbContextSP())
            {
                Context.Database.CommandTimeout = 300;
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, Context.Database.Connection.ConnectionString);
                return dataAdapter;
            }
        }
        #endregion
    }
   
}
