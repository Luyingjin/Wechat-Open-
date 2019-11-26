using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Formula
{
    public class DataLogService : Formula.IDataLogService
    {


        public void LogDataModify(string connName, string tableName, string modifyMode, string entityKey, string CurrentValue, string OriginalValue)
        {
            Config.Logic.DataLog.LogDataModify(connName, tableName, modifyMode, entityKey, CurrentValue, OriginalValue);
        }


    }
}
