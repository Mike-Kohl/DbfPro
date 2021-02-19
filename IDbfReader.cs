using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DbfPro
{
    interface IDbfReader
    {
        Int32 GetRecordCount(string path);
        List<string> GetColumnNames(string path);
        List<string> GetColumnLengths(string path);
        Int32 GetTotalColumnLengths(List<string> columnLengths);
        DataTable ReadDbf(DataTable tb, string path);
    }
}
