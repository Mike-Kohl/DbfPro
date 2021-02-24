using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DbfPro
{
    public class DbfWriter : IDbfWriter
    {
        string _path;
        List<string> _columnNames;
        List<byte> _columnLengths;
        int _columnLengthsTotal;
        int _recordCount;
        DataTable _dataTable;
        string _fileName;

        public DbfWriter(
            string path, 
            List<string> columnNames, 
            List<byte> columnLengths, 
            int columnLengthsTotal, 
            int recordCount, 
            DataTable dataTable, 
            string fileName) 
        {
            _path = path;
            _columnNames = columnNames;
            _columnLengths = columnLengths;
            _columnLengthsTotal = columnLengthsTotal;
            _recordCount = recordCount;
            _dataTable = dataTable;
            _fileName = fileName;
        }

        public void CreateFile()
        {
            try
            {
                using (var fileStream = new FileStream(_path, FileMode.Create, FileAccess.Write))
                {
                    using (var binaryWriter = new BinaryWriter(fileStream))
                    {
                        WriteHeader(fileStream, binaryWriter);
                        WriteHeaderRecords(fileStream, binaryWriter);
                        AddRecords(binaryWriter);
                        FinalProcessing(binaryWriter);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public void AddRecords(BinaryWriter binaryWriter)
        {
            Int32 num;
            byte[] b = new byte[1];

            try
            {
                for (int i = 0; i < _recordCount; i++)
                {
                    string value = string.Empty;
                    b[0] = 32;
                    binaryWriter.Write(b, 0, 1);

                    for (int n = 0; n < _columnNames.Count; n++)
                    {
                        num = Convert.ToInt32(_columnLengths[n]);

                        byte[] columnByte = new byte[num];
                        value = _dataTable.Rows[i][n].ToString();

                        if (string.IsNullOrEmpty(value))
                        {
                            value = "";
                        }

                        //create a character array
                        value.ToCharArray();

                        //conver to bytes
                        for (int x = 0; x < num; x++)
                        {
                            if (x > value.Length - 1)
                            {
                                columnByte[x] = 32;
                            }
                            else
                            {
                                columnByte[x] = Convert.ToByte(Convert.ToSByte(value[x]));
                            }
                        }

                        //write bytes
                        binaryWriter.Write(columnByte, 0, columnByte.Length);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }               

        public void FinalProcessing(BinaryWriter binaryWriter)
        {
            byte[] recordBytes;
            Int32 num;

            binaryWriter.Seek(4, SeekOrigin.Begin);
            num = _recordCount;
            recordBytes = BitConverter.GetBytes(num);
            binaryWriter.Write(recordBytes, 0, 4); 
        }

        public void WriteHeader(FileStream fileStream, BinaryWriter binaryWriter)
        {
            try
            {
                var headerBytes = new byte[32];
                byte[] b;
                const string version = "3"; // dbf version
                var value = string.Empty;
                Int32 num;
                Int32 pos;

                //dbf version
                headerBytes[0] = Convert.ToByte(version);

                //year
                value = DateTime.Now.Year.ToString().Substring(2, 2);
                headerBytes[1] = Convert.ToByte(value);

                //month
                value = DateTime.Now.Month.ToString();
                headerBytes[2] = Convert.ToByte(value);

                //day 
                value = DateTime.Now.Day.ToString();
                headerBytes[3] = Convert.ToByte(value);

                //number of records in the table (default to zeros)
                headerBytes[4] = 0;
                headerBytes[5] = 0;
                headerBytes[6] = 0;
                headerBytes[7] = 0;

                //number of bytes in the header
                num = (_columnNames.Count + 1) * 32 + 1;
                pos = num;

                b = BitConverter.GetBytes(num);
                headerBytes[8] = b[0];
                headerBytes[9] = b[1];

                //length of each record
                num = _columnLengthsTotal + 1;
                b = BitConverter.GetBytes(num);
                headerBytes[10] = b[0];
                headerBytes[11] = b[1];

                headerBytes[12] = 0;
                headerBytes[13] = 0;
                headerBytes[14] = 0;
                headerBytes[15] = 0;
                headerBytes[16] = 0;
                headerBytes[17] = 0;
                headerBytes[18] = 0;
                headerBytes[19] = 0;
                headerBytes[20] = 0;
                headerBytes[21] = 0;
                headerBytes[22] = 0;
                headerBytes[23] = 0;
                headerBytes[24] = 0;
                headerBytes[25] = 0;
                headerBytes[26] = 0;
                headerBytes[27] = 0;
                headerBytes[28] = 0;
                headerBytes[29] = 0;
                headerBytes[30] = 0;
                headerBytes[31] = 0;
                binaryWriter.Write(headerBytes, 0, headerBytes.Length);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public void WriteHeaderRecords(FileStream fileStream, BinaryWriter binaryWriter)
        {
            var recordBytes = new byte[32];
            Int32 num;
            string columnName;
            Int32 columnLength;

            //move to position 32
            fileStream.Seek(32, SeekOrigin.Begin);

            for (int i = 0; i < _columnNames.Count; i++)
            {
                columnName = _columnNames[i];
                columnLength = Convert.ToInt32(_columnLengths[i]);
                num = columnName.Length;

                //note column names cannot be longer than 10
                if (num > 10)
                {
                    num = 10;
                }

                columnName.ToCharArray(0, columnName.Length - 1);

                for (int n = 0; n < num; n++)
                {
                    recordBytes[n] = Convert.ToByte(Convert.ToSByte(columnName[n]));
                }

                string value = "C";
                value.ToCharArray();

                recordBytes[11] = Convert.ToByte(Convert.ToSByte(value[0]));
                recordBytes[12] = 0;
                recordBytes[13] = 0;
                recordBytes[14] = 0;
                recordBytes[15] = 0;
                recordBytes[16] = Convert.ToByte(columnLength);
                recordBytes[17] = 0;
                recordBytes[18] = 0;
                recordBytes[19] = 0;
                recordBytes[20] = 0;
                recordBytes[21] = 0;
                recordBytes[22] = 0;
                recordBytes[23] = 0;
                recordBytes[24] = 0;
                recordBytes[25] = 0;
                recordBytes[26] = 0;
                recordBytes[27] = 0;
                recordBytes[28] = 0;
                recordBytes[29] = 0;
                recordBytes[30] = 0;
                recordBytes[31] = 0;

                binaryWriter.Write(recordBytes, 0, recordBytes.Length);

                //clear values in byte array
                for (int j = 0; j < 32; j++)
                {
                    recordBytes[j] = 0;
                }
            }

            byte[] terminator = new byte[1];
            terminator[0] = 13;
            binaryWriter.Write(terminator, 0, 1);
        }
    }
}
