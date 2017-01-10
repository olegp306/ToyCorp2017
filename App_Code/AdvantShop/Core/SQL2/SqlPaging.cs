using System.Data.SqlClient;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AdvantShop.Core.SQL2
{
    //public enum SqlPagingSort
    //{
    //    None,
    //    ASC,
    //    DESC
    //}
    //public enum SqlPagingOperation
    //{
    //    None,
    //    AND,
    //    OR
    //}

    public class SqlParam
    {
        public string ParameterName;
        public object Value;

        public SqlParam(string parameterName, object value)
        {
            ParameterName = parameterName;
            Value = value;
        }
    }

    public class SqlPaging
    {
        private const string Advparam = "@p";
        private readonly List<string> _whereCondition;
        private readonly List<SqlParam> _listParams;
        private readonly List<string> _selectFields;
        private readonly List<string> _orderFields;
        private string _tablename;
        private readonly List<string> _joinTable;
        public int ItemsPerPage { get; set; }
        public int CurrentPageIndex { get; set; }


        public SqlPaging()
            : this(1, 10)
        {
        }

        public SqlPaging(int currentPageIndex, int itemsPerPage)
        {
            CurrentPageIndex = currentPageIndex;
            ItemsPerPage = itemsPerPage;
            _whereCondition = new List<string>();
            _listParams = new List<SqlParam>();
            _selectFields = new List<string>();
            _orderFields = new List<string>();
            _tablename = string.Empty;
            _joinTable = new List<string>();
        }

        public int PageCount(int rowsCount)
        {
            return (int)(Math.Ceiling((double)rowsCount / ItemsPerPage));
        }

        public int TotalRowsCount
        {
            get
            {
                var query = "SELECT COUNT( " + _selectFields.First() + ") FROM "
                                        + _tablename
                                        + _joinTable.Aggregate(" ", (a, b) => a + " " + b)
                                        + _whereCondition.Aggregate(" WHERE ", (a, b) => a + " " + b);

                var totalRowsCount = SQLDataAccess.ExecuteScalar<int>(query, CommandType.Text, _listParams.Select(x => new SqlParameter { Value = x.Value, ParameterName = x.ParameterName }).ToArray());
                return totalRowsCount;
            }
        }

        public List<T> ItemsIds<T>(string idName) where T : IConvertible
        {

            var query = "SELECT " + idName + " FROM "
                                  + _tablename
                                  + _joinTable.Aggregate(" ", (a, b) => a + " " + b)
                                  + _whereCondition.Aggregate(" WHERE ", (a, b) => a + " " + b);

            var itemsIds = SQLDataAccess.ExecuteReadList(query, CommandType.Text, reader => SQLDataHelper.GetValue<T>(reader, "ID"), _listParams.Select(x => new SqlParameter { Value = x.Value, ParameterName = x.ParameterName }).ToArray());
            return itemsIds;
        }

        public DataTable PageItems
        {
            get
            {
                var needRow = CurrentPageIndex * ItemsPerPage;
                var keyid = (CurrentPageIndex - 1) * ItemsPerPage;

                var selecStr = _selectFields.Aggregate((a, b) => a + ", " + b);
                var fieldsToAdd = _orderFields.Where(field => _selectFields.All(selectField => selectField.ToLower().Split(" as ").Last().Trim() != field.ToLower().Replace("asc", "").Replace("desc", "").Trim())).ToList();
                selecStr = fieldsToAdd.Select(x => x.ToLower().Replace("asc", "").Replace("desc", "").Trim()).Aggregate(selecStr, (a, b) => a + ", " + b);

                var firstColum = _selectFields.First();
                firstColum = (firstColum.Contains(".")
                    ? firstColum.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[1]
                    : firstColum);

                var order = _orderFields.Any() ? _orderFields.Select(x => x.Contains(" as ") || x.Contains(" AS ") ? x.Split(new[] { " as ", " AS " }, StringSplitOptions.RemoveEmptyEntries)[1] : x)
                                    .Select(x => x.Contains(".") ? x.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[1] : x)
                                    .Aggregate((a, b) => a + ", " + b) + ", " + firstColum + " ASC" : firstColum + " ASC";

                var query = string.Format("WITH TEMP " +
                             "AS ( " +
                                 "SELECT {0} " +
                                 "FROM {1} {2} {3}" +
                                 ")" +
                              "SELECT * " +
                              "FROM ( " +
                                    "SELECT TOP ({4}) Row_Number() OVER ( " +
                                            "ORDER BY {5} " +
                                            ") AS RowNum " +
                                        ",* " +
                                    "FROM TEMP " +
                                    ") AS t " +
                            "WHERE RowNum > {6} ",
                             selecStr,
                            _tablename,
                            _joinTable.Aggregate(" ", (a, b) => a + " " + b),
                            _whereCondition.Aggregate(" WHERE ", (a, b) => a + " " + b),
                            needRow,
                            order,
                            keyid);
                var tbl = SQLDataAccess.ExecuteTable(query, CommandType.Text, _listParams.Select(x => new SqlParameter { Value = x.Value, ParameterName = x.ParameterName }).ToArray());
                return tbl;
            }
        }

        public List<T> GetCustomData<T>(string selectFields, string newCondition, Func<IDataReader, T> getFromReader, bool useDistinct, string jointable = "")
        {
            var query = String.Format("SELECT {5}{0} FROM {1} {2} {3} {4}",
                selectFields, _tablename,
                _joinTable.Aggregate(" ", (a, b) => a + " " + b) + " " + jointable,
                _whereCondition.Aggregate(" WHERE ", (a, b) => a + " " + b),
                newCondition,
                useDistinct ? "Distinct " : ""
                );
            var table = SQLDataAccess.ExecuteReadList(query, CommandType.Text, getFromReader, _listParams.Select(x => new SqlParameter { Value = x.Value, ParameterName = x.ParameterName }).ToArray());
            return table;
        }

        private string GetParamString(params object[] args)
        {
            var returnStr = string.Empty;
            if (args == null) return returnStr;
            foreach (var arg in args)
            {
                var argArr = arg as Array;
                if (argArr != null)
                    foreach (var argItem in argArr)
                    {
                        var temp = Advparam + _listParams.Count;
                        _listParams.Add(new SqlParam(temp, argItem));
                        if (!string.IsNullOrEmpty(returnStr))
                            returnStr += ",";
                        returnStr += temp;
                    }
                else
                {
                    var temp = Advparam + _listParams.Count;
                    _listParams.Add(new SqlParam(temp, arg));
                    if (!string.IsNullOrEmpty(returnStr))
                        returnStr += ",";
                    returnStr += temp;
                }

            }
            return returnStr;
        }

        public void Select(params string[] field)
        {
            _selectFields.AddRange(field);
        }

        public void Where(string condition, params object[] args)
        {
            var temp = GetParamString(args);
            _whereCondition.Add(string.Format(condition, temp));
        }

        public void OrderBy(params string[] condition)
        {
            _orderFields.AddRange(condition.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        private void Join(string joinTable, params object[] args)
        {
            var temp = GetParamString(args);
            _joinTable.Add(string.Format(joinTable, temp));
        }

        public void Inner_Join(string joinTable, params object[] args)
        {
            Join("Inner Join " + joinTable, args);
        }

        public void Left_Join(string joinTable, params object[] args)
        {
            Join("Left Join " + joinTable, args);
        }

        public void From(string tablename)
        {
            _tablename = tablename;
        }

    }

}