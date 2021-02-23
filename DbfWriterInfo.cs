using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DbfPro
{
    public class DbfWriterInfo
    {
        public List<string> ColumnNames { get; set; }
        public List<byte> ColumnLengths { get; set; }
        public Int32 ColumnLengthTotal { get; set; }
        public Int32 ColumnCount { get; set; }
        public DataTable TableRecords { get; set; }
        public string Path { get; set; }
        public Int32 RecordCount { get; set; }
    }
}
