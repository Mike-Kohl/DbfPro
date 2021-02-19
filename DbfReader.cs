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
        public List<string> GetColumnLengths(string path) 
        {            
            Int32 columnCount = 0;
            Int32 position = 0;

            var columnLengths = new List<string>();

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

                            //To Do:
                            //changed binaryReader.ReadByte() to binaryReader.ReadString()
                            // this needs to be tested
                            columnLengths.Add(binaryReader.ReadString());
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
        public Int32 GetTotalColumnLengths(List<string> columnLengths) 
        {
            Int32 cnt = 0;

            for (int i = 0; i < columnLengths.Count; i++)
            {
                cnt += Convert.ToInt32(columnLengths[i]);
            }

            return cnt;
        }

        public DataTable ReadDbf(DataTable tb, string path)
        {
            Int32 headerLength = 0;
            Int32 recordCount = 0;
        }
    }
} 
