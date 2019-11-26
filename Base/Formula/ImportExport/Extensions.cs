using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;
using System.Reflection;
using System.Linq.Expressions;

namespace Formula.ImportExport
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 验证Excel导入的数据是否有效
        /// </summary>
        /// <param name="data">Excel的数据信息</param>
        /// <param name="CellValidation">单元格的验证事件</param>
        /// <returns></returns>
        public static IEnumerable<CellErrorInfo> Vaildate(this ExcelData data, Action<CellValidationArgs> CellValidation)
        {
            var errors = new List<CellErrorInfo>();

            // 循环表格
            foreach (var table in data.Tables)
            {
                // 循环表格行
                var dt = table.ToDataTable();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    // 循环表格列
                    var row = table.Rows[i];
                    foreach (var cell in row.Cells)
                    {
                        // 构造验证单元格事件的参数
                        var args = new CellValidationArgs
                        {
                            ColIndex = cell.Structure.ColIndex,
                            EnumKey = cell.Structure.EnumKey,
                            FieldName = cell.FieldName,
                            Record = dt.Rows[i],
                            RowIndex = row.RowIndex,
                            Value = cell.Value,
                        };

                        // 触发验证单元格事件
                        CellValidation(args);

                        // 如果验证不通过，则反写Excel，红色背景白色字体加书签说明
                        if (!args.IsValid)
                        {
                            errors.Add(new CellErrorInfo
                            {
                                ColIndex = args.ColIndex,
                                RowIndex = args.RowIndex,
                                ErrorText = args.ErrorText,
                            });
                        }
                    }
                }
            }

            // 检查变量
            foreach (var cell in data.Variables)
            {
                // 构造验证单元格事件的参数
                var args = new CellValidationArgs
                {
                    ColIndex = cell.Structure.ColIndex,
                    EnumKey = cell.Structure.EnumKey,
                    FieldName = cell.FieldName,
                    Record = null,
                    RowIndex = cell.Structure.RowIndex,
                    Value = cell.Value,
                };

                // 触发验证单元格事件
                CellValidation(args);

                // 如果验证不通过，则反写Excel，红色背景白色字体加书签说明
                if (!args.IsValid)
                {
                    errors.Add(new CellErrorInfo
                    {
                        ColIndex = args.ColIndex,
                        RowIndex = args.RowIndex,
                        ErrorText = args.ErrorText,
                    });
                }
            }

            return errors;
        }

        #region 实体与DataSet、DataTable之间的转化
        /// <summary>
        /// 将IList<T>转化为DataTable后，追加到DataSet中。
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="ds">DataSet</param>
        /// <param name="list">实体列表</param>
        /// <param name="tableName">可选，DataTable的名称</param>
        /// <returns></returns>
        public static DataSet AddList<T>(this DataSet ds, T entity, string tableName = null) where T : class
        {
            if (entity == null)
                throw new ArgumentNullException("entity", "数据实体不能为Null");

            return AddList<T>(ds, new List<T> { entity }, tableName);

        }

        /// <summary>
        /// 将IList<T>转化为DataTable后，追加到DataSet中。
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="ds">DataSet</param>
        /// <param name="list">实体列表</param>
        /// <param name="tableName">可选，DataTable的名称</param>
        /// <returns></returns>
        public static DataSet AddList<T>(this DataSet ds, IList<T> list, string tableName = null) where T : class
        {
            if (ds == null)
                throw new ArgumentNullException("ds", "DataSet不能为Null");

            tableName = tableName ?? typeof(T).Name;
            if (!ds.Tables.Contains(tableName))
            {
                var table = list.ToDataTable(tableName);
                ds.Tables.Add(table);
            }

            return ds;
        }

        /// <summary>
        /// 将嵌套的IList<T>转化为主从的DataTable后，追加到DataSet中，并追加主从关联关系
        /// </summary>
        /// <typeparam name="Parent">父实体类型</typeparam>
        /// <typeparam name="Children">子实体类型</typeparam>
        /// <param name="ds">DataSet</param>
        /// <param name="parentlist">父实体列表</param>
        /// <param name="childlist">子实体列表</param>
        /// <param name="parentColumnName">获取父列名的表达式树</param>
        /// <param name="childColumnName">获取子列名的表达式树</param>
        /// <param name="parentTableName">可选，父DataTable的名称</param>
        /// <param name="childTableName">可选，子DataTable的名称</param>
        /// <returns></returns>
        public static DataSet AddNestedList<Parent, Children>(this DataSet ds, IList<Parent> parentlist, IList<Children> childlist, Expression<Func<Parent, object>> parentColumnName, Expression<Func<Children, object>> childColumnName, string parentTableName = null, string childTableName = null)
            where Parent : class
            where Children : class
        {
            if (ds == null)
                throw new ArgumentNullException("ds", "DataSet不能为Null");

            parentTableName = parentTableName ?? typeof(Parent).Name;
            childTableName = childTableName ?? typeof(Children).Name;
            if (!ds.Tables.Contains(parentTableName))
            {
                var table = parentlist.ToDataTable(parentTableName);
                ds.Tables.Add(table);
            }
            if (!ds.Tables.Contains(childTableName))
            {
                var table = childlist.ToDataTable(childTableName);
                ds.Tables.Add(table);
            }

            ds.AddRelation(GetColumnName(parentColumnName), GetColumnName(childColumnName), parentTableName, childTableName);

            return ds;
        }

        /// <summary>
        /// 给DataSet添加主从关系
        /// </summary>
        /// <param name="ds">DataSet</param>
        /// <param name="parentTableName">父表的表名</param>
        /// <param name="parentColumnName">父表的列名</param>
        /// <param name="childTableName">子表的表名</param>
        /// <param name="childColumnName">子表的列表</param>
        /// <returns></returns>
        public static DataSet AddRelation(this DataSet ds, string parentColumnName, string childColumnName, string parentTableName, string childTableName)
        {
            if (ds == null)
                throw new ArgumentNullException("ds", "DataSet不能为Null");

            var relationName = string.Format("{0}-{1}", parentTableName, childTableName);
            if (!ds.Relations.Contains(relationName))
            {
                if (!ds.Tables.Contains(parentTableName))
                {
                    throw new ArgumentException("parentTableName", string.Format("DataSet不存在名称为（{0}）的表", parentTableName));
                }
                else
                {
                    if (!ds.Tables[parentTableName].Columns.Contains(parentColumnName))
                        throw new ArgumentException("parentColumnName", string.Format("名称为（{0}）的表不存在列名为（{1}）的列", parentTableName, parentColumnName));
                }

                if (!ds.Tables.Contains(childTableName))
                {
                    throw new ArgumentException("childTableName", string.Format("DataSet不存在名称为（{0}）的表", childTableName));
                }
                else
                {
                    if (!ds.Tables[childTableName].Columns.Contains(childColumnName))
                        throw new ArgumentException("childColumnName", string.Format("名称为（{0}）的表不存在列名为（{1}）的列", childTableName, childColumnName));
                }

                ds.Relations.Add(relationName, ds.Tables[parentTableName].Columns[parentColumnName], ds.Tables[childTableName].Columns[childColumnName]);
            }

            return ds;
        }

        /// <summary>
        /// 将IList<T>转换为DataTable
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="list">实体列表</param>
        /// <param name="tableName">可选，DataTable的名称</param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IList<T> list, string tableName = null)
        {
            DataTable table = CreateTable<T>(tableName);
            Type entityType = typeof(T);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (T item in list)
            {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }

                table.Rows.Add(row);
            }

            return table;
        }

        /// <summary>
        /// 根据实体类型生成DataTable表结构
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="tableName">可选，DataTable的名称</param>
        /// <returns></returns>
        private static DataTable CreateTable<T>(string tableName = null)
        {
            Type entityType = typeof(T);
            DataTable table = new DataTable(tableName ?? entityType.Name);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (PropertyDescriptor prop in properties)
            {
                var colType = prop.PropertyType;

                // 判断是否为可空类型。
                if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    colType = colType.GetGenericArguments()[0];
                }

                table.Columns.Add(prop.Name, colType);
            }

            return table;
        }

        /// <summary>
        /// 获取表达式树的属性名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static string GetColumnName<T>(Expression<Func<T, object>> expression) where T : class
        {
            try
            {
                MemberExpression body = (MemberExpression)expression.Body;
                return body.Member.Name;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("非法的使用Bind方法，当前表达式不可解析", ex);
            }
        }
        #endregion

    }
}
