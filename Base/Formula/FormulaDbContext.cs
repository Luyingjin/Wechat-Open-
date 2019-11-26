using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data;
using Formula.Helper;
using System.Transactions;

namespace Formula
{
    public class FormulaDbContext : DbContext
    {
        protected string ConnName = "";

        public FormulaDbContext(string connectionString)
            : base(connectionString)
        {
            ConnName = connectionString;
        }

        public int SaveChangesWithoutContext()
        {
            return base.SaveChanges();
        }

        public override int SaveChanges()
        {

            if (Config.Constant.DataAddLog.Where(c => c.StartsWith(ConnName)).Count() == 0
                && Config.Constant.DataDeleteLog.Where(c => c.StartsWith(ConnName)).Count() == 0
                && Config.Constant.DataModifyLog.Where(c => c.StartsWith(ConnName)).Count() == 0)
            {
                return base.SaveChanges();
            }

            int result = 0;

            Action action = () =>
            {

                this.ChangeTracker.DetectChanges();
                var ObjectStateManager = ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager;
                var modifiedEntities = ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Modified | EntityState.Deleted);

                var service = FormulaHelper.GetService<IDataLogService>();
                foreach (var item in modifiedEntities)
                {
                    string entityName = item.EntityKey.EntitySetName;


                    if (item.State == EntityState.Added)
                    {
                        if (Config.Constant.DataAddLog.Contains(ConnName + "." + entityName))
                        {
                            service.LogDataModify(ConnName, entityName, item.State.ToString(), "", JsonHelper.ToJson(item.Entity), "");
                        }
                    }
                    else if (item.State == EntityState.Modified)
                    {
                        if (Config.Constant.DataModifyLog.Contains(ConnName + "." + entityName))
                        {
                            string entityKey = item.EntityKey.EntityKeyValues.GetValue(0).ToString();
                            var modifiedProperties = item.GetModifiedProperties();
                            Dictionary<string, object> originalValus = new Dictionary<string, object>();
                            Dictionary<string, object> currentValus = new Dictionary<string, object>();
                            foreach (var pty in modifiedProperties)
                            {
                                originalValus.Add(pty, item.OriginalValues[pty]);
                                currentValus.Add(pty, item.CurrentValues[pty]);
                            }
                            service.LogDataModify(ConnName, entityName,item.State.ToString(), entityKey, JsonHelper.ToJson(currentValus), JsonHelper.ToJson(originalValus));
                        }
                    }
                    else if (item.State == EntityState.Deleted)
                    {
                        if (Config.Constant.DataDeleteLog.Contains(ConnName + "." + entityName))
                        {
                            string entityKey = item.EntityKey.EntityKeyValues.GetValue(0).ToString();
                            service.LogDataModify(ConnName, entityName, item.State.ToString(), entityKey, JsonHelper.ToJson(item.Entity), "");
                        }
                    }
                }

                result = base.SaveChanges();

            };

            if (System.Configuration.ConfigurationManager.AppSettings["UseMsdtc"].ToLower() == "true")
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    action();
                    ts.Complete();
                }
            }
            else
            {
                action();
            }

            return result;
        }       
    }
}
