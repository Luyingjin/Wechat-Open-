using System;
namespace Formula
{
    public interface IDataLogService : ISingleton
    {
        //数据修改日志
        void LogDataModify(string connName, string tableName,string modifyMode, string entityKey, string CurrentValue, string OriginalValue);
    }
}
