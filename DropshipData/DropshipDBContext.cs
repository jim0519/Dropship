using DropshipCommon.Models;
//using DropshipData.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using DropshipCommon.Models.Item;
//using DropshipCommon.Models.AdvanceShipNotice;
//using DropshipCommon.Models.PurchaseOrder;
//using DropshipCommon.Models.SalesOrder;
using DropshipCommon;
using DropshipData.Mapping;

namespace DropshipData
{
    public class DropshipDBContext : DbContext, IDbContext
    {
        public DropshipDBContext()
            : base(DropshipConfig.Instance.ConnectionString)
        {
#if Debug
            this.Database.Log = (logMsg) =>
            {
                Trace.WriteLine(logMsg);
            };
#endif
            Database.SetInitializer<DropshipDBContext>(null);
            this.Database.CommandTimeout = 180;
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Configurations.Add(new DocOrderHeaderMap());
            //modelBuilder.Configurations.Add(new BASSKUMap());
            //modelBuilder.Configurations.Add(new DocOrderDetailsMap());
            //modelBuilder.Configurations.Add(new BASCustomerMap());
            //modelBuilder.Configurations.Add(new AusPostManifestModelMap());
            //modelBuilder.Configurations.Add(new AlliedManifestModelMap());
            //modelBuilder.Configurations.Add(new DocOrderCleansedAddressMap());
            //modelBuilder.Configurations.Add(new GoogleMapAddressMap());

            modelBuilder.Configurations.Add(new D_ImageMap());
            modelBuilder.Configurations.Add(new D_ItemMap());
            modelBuilder.Configurations.Add(new D_ListingMap());
            modelBuilder.Configurations.Add(new M_ItemImageMap());
            modelBuilder.Configurations.Add(new T_CategoryMap());
            modelBuilder.Configurations.Add(new T_ListingChannelMap());
            modelBuilder.Configurations.Add(new T_ListingDescriptionTemplateMap());
            modelBuilder.Configurations.Add(new T_PostageRuleMap());
            modelBuilder.Configurations.Add(new T_PostageRuleLineMap());
            modelBuilder.Configurations.Add(new T_SupplierMap());
            modelBuilder.Configurations.Add(new T_ValueRuleMap());
            modelBuilder.Configurations.Add(new T_ValueRuleLineMap());
            modelBuilder.Configurations.Add(new T_UserMap());
            modelBuilder.Configurations.Add(new T_StatusMap());
            modelBuilder.Configurations.Add(new T_SettingMap());

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Get DbSet
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>DbSet</returns>
        public new IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }


        /// <summary>
        /// Creates a raw SQL query that will return elements of the given generic type.  The type can be any type that has properties that match the names of the columns returned from the query, or can be a simple primitive type. The type does not have to be an entity type. The results of this query are never tracked by the context even if the type of object returned is an entity type.
        /// </summary>
        /// <typeparam name="TElement">The type of object returned by the query.</typeparam>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <returns>Result</returns>
        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            return this.Database.SqlQuery<TElement>(sql, parameters);
        }


        /// <summary>
        /// Execute stores procedure and load a list of entities at the end
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="commandText">Command text</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Entities</returns>
        public IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters)// where TEntity : BaseEntity, new()
        {
            //HACK: Entity Framework Code First doesn't support doesn't support output parameters
            //That's why we have to manually create command and execute it.
            //just wait until EF Code First starts support them
            //
            //More info: http://weblogs.asp.net/dwahlin/archive/2011/09/23/using-entity-framework-code-first-with-stored-procedures-that-have-output-parameters.aspx

            bool hasOutputParameters = false;
            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    var outputP = p as DbParameter;
                    if (outputP == null)
                        continue;

                    if (outputP.Direction == ParameterDirection.InputOutput ||
                        outputP.Direction == ParameterDirection.Output)
                        hasOutputParameters = true;
                }
            }



            var context = ((IObjectContextAdapter)(this)).ObjectContext;
            if (!hasOutputParameters)
            {
                //no output parameters
                //var result = this.Database.SqlQuery<TEntity>(commandText, parameters).ToList();
                //for (int i = 0; i < result.Count; i++)
                //    result[i] = AttachEntityToContext(result[i]);

                //return result;

                var result = context.ExecuteStoreQuery<TEntity>(commandText, parameters).ToList();
                //foreach (var entity in result)
                //    Set<TEntity>().Attach(entity);
                return result;
            }
            else
            {

                //var connection = context.Connection;
                var connection = this.Database.Connection;
                //Don't close the connection after command execution


                //open the connection for use
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                //create a command object
                using (var cmd = connection.CreateCommand())
                {
                    //command to execute
                    cmd.CommandText = commandText;
                    cmd.CommandType = CommandType.StoredProcedure;

                    // move parameters to command object
                    if (parameters != null)
                        foreach (var p in parameters)
                            cmd.Parameters.Add(p);

                    //database call
                    var reader = cmd.ExecuteReader();
                    //return reader.DataReaderToObjectList<TEntity>();
                    var result = context.Translate<TEntity>(reader).ToList();
                    //for (int i = 0; i < result.Count; i++)
                    //    result[i] = AttachEntityToContext(result[i]);
                    //close up the reader, we're done saving results
                    reader.Close();
                    return result;
                }

            }
        }

    }
}
