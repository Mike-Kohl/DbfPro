using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace DbfPro
{
    public class DbfReader : IDbfReader
    {
        public Int32 GetRecordCount(string path)
        {
            Int32 recordCount = 0;

            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                using (var binaryReader = new BinaryReader(fileStream))
                {
                    //move to the position that contains the record count
                    fileStream.Seek(4, SeekOrigin.Begin);
                    recordCount = binaryReader.ReadInt32();
                }
            }

            return recordCount;
        }
        public List<string> GetColumnNames(string path)
        {
            Int32 columnCount = 0;
            Int32 position = 0;

            var columnNames = new List<string>();
            var encoding = new ASCIIEncoding();

            try
            {
                using (var fileStream = new FileStream(path, FileMode.Open))
                {
                    using (var binaryReader = new BinaryReader(fileStream))
                    {
                        //move to position that contains record count
                        fileStream.Seek(4, SeekOrigin.Begin);
                        int recordCount = binaryReader.ReadInt16();

                        //move to position that contains header length
                        fileStream.Seek(8, SeekOrigin.Begin);
                        int headerLength = binaryReader.ReadInt16();

                        //get the number of columns
                        columnCount = Convert.ToInt32((headerLength - 32) / 32);

                        for (int i = 0; i < columnCount; i++)
                        {
                            position += 32;
                            //move to the next column
                            fileStream.Seek(position, SeekOrigin.Begin);

                            //get column name
                            char[] c = binaryReader.ReadChars(10);
                            var columnValue = new string(c);
                            Int32 cnt = 0;

                            byte[] b = encoding.GetBytes(columnValue);
                            for (int n = 0; n < 10; n++)
                            {
                                if (b[n] != 0)
                                {
                                    cnt += 1;
                                }
                            }

                            var columnName = columnValue.Substring(0, cnt);
                            columnNames.Add(columnName);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("DBF Reader error - Getting column names");
            }

            return columnNames;
        }
        public List<Byte> GetColumnLengths(string path)
        {
            Int32 columnCount = 0;
            Int32 position = 0;

            var columnLengths = new List<Byte>();

            try
            {
                using (var fileStream = new FileStream(path, FileMode.Open))
                {
                    using (var binaryReader = new BinaryReader(fileStream))
                    {
                        // move to position that contains record count
                        fileStream.Seek(4, SeekOrigin.Begin);
                        int recordCount = binaryReader.ReadInt16();

                        // move to position that contains header length
                        fileStream.Seek(8, SeekOrigin.Begin);
                        int headerLength = binaryReader.ReadInt16();

                        //get number of columns
                        columnCount = Convert.ToInt32((headerLength - 32) / 32);

                        for (int i = 0; i < columnCount; i++)
                        {
                            position += 32;
                            //move to next column
                            fileStream.Seek(position, SeekOrigin.Begin);

                            //get column name
                            char[] c = binaryReader.ReadChars(10);

                            int offSet = position + 16;
                            fileStream.Seek(offSet, SeekOrigin.Begin);

                            columnLengths.Add(binaryReader.ReadByte());
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw new Exception("DBF Reader error - Getting column lengths");
            }
            return columnLengths;
        }
        public Int32 GetTotalColumnLengths(List<byte> columnLengths)
        {
            Int32 cnt = 0;

            for (int i = 0; i < columnLengths.Count; i++)
            {
                cnt += Convert.ToInt32(columnLengths[i]);
            }

            return cnt;
        }
        public void ReadDbf(DataTable tb, string path)
        {
            Int32 headerLength = 0;
            Int32 recordCount = 0;
            Int32 columnCount = 0;
            Int32 position = 0;
            Int32 offSet = 0;
            long location = 0;
            var columnLengths = new List<Byte>();
            var encoding = new ASCIIEncoding();

            try
            {
                using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    using (var binaryReader = new BinaryReader(fileStream))
                    {
                        // move to position that contains record count
                        fileStream.Seek(4, SeekOrigin.Begin);

                        recordCount = binaryReader.ReadInt32();

                        //move to position that contains header length
                        fileStream.Seek(8, SeekOrigin.Begin);

                        headerLength = binaryReader.ReadInt16();

                        //get the number of fields
                        columnCount = Convert.ToInt32((headerLength - 32) / 32);

                        //add columns to datatable
                        for (int i = 0; i < columnCount; i++)
                        {
                            position += 32;

                            //move to the next column name
                            fileStream.Seek(position, SeekOrigin.Begin);

                            //get column name
                            char[] c = binaryReader.ReadChars(10);
                            var columnValue = new string(c);
                            Int32 cnt = 0;

                            //trim off the trailing bytes
                            byte[] b = encoding.GetBytes(columnValue);

                            for (int n = 0; n < 10; n++)
                            {
                                if (b[n] != 0)
                                {
                                    cnt += 1;
                                }
                            }

                            string columnName = columnValue.Substring(0, cnt);
                            tb.Columns.Add(columnName);

                            //get column length
                            offSet = position + 16;
                            fileStream.Seek(offSet, SeekOrigin.Begin);
                            columnLengths.Add(binaryReader.ReadByte());
                        }


                        //move to first record
                        fileStream.Seek(headerLength, SeekOrigin.Begin);

                        //populate table with data
                        DataRow row;
                        for (int i = 0; i < recordCount; i++)
                        {
                            location = fileStream.Position;

                            //record deletion value
                            binaryReader.ReadChar();

                            row = tb.NewRow();

                            for (int ii = 0; ii < columnCount; ii++)
                            {
                                var columnValue = string.Empty;
                                var len = Convert.ToInt32(columnLengths[ii]);

                                columnValue = encoding.GetString(binaryReader.ReadBytes(len), 0, len).Trim();
                                row[ii] = columnValue;
                            }

                            tb.Rows.Add(row);
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

        }        
    }
} 
