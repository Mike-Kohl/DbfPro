﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace DbfPro
{
    public class DbfWriter : IDbfWriter
    {
        DbfWriterInfo _dbfWriterInfo;

        public DbfWriter(DbfWriterInfo dbfWriterInfo) 
        {
            _dbfWriterInfo = dbfWriterInfo; 
        }

        public void AddRecords()
        {
            throw new NotImplementedException();
        }

        public void CreateFile()
        {
            try
            {
                WriteHeader();
                WriteHeaderRecords();
                AddRecords();
                FinalProcessing();
            }
            catch (Exception)
            {
                throw new Exception("Dbf Writer - Problem creating a new dbf file");
            }
        }

        public void FinalProcessing()
        {
            byte[] recordBytes;
            Int32 num;

            using (var filestream = new FileStream(_dbfWriterInfo.Path, FileMode.Open, FileAccess.Write)) 
            {
                using (var binaryWriter = new BinaryWriter(filestream)) 
                {
                    binaryWriter.Seek(4, SeekOrigin.Begin);
                    num = _dbfWriterInfo.RecordCount;
                    recordBytes = BitConverter.GetBytes(num);
                    binaryWriter.Write(recordBytes, 0, 4);
                }
            }

        }

        public void WriteHeader()
        {
            using (var fileStream = new FileStream(_dbfWriterInfo.Path, FileMode.Open, FileAccess.Write)) 
            {
                var headerBytes = new byte[32];
                byte[] b;
                const string version = "3"; // dbf version
                var value = string.Empty;
                Int32 num;
                Int32 pos;

                using (var binaryWriter = new BinaryWriter(fileStream)) 
                {
                    //dbf version
                    headerBytes[0] = Convert.ToByte(version);

                    //year
                    value = DateTime.Now.Year.ToString().Substring(2, 2);
                    headerBytes[1] = Convert.ToByte(value);

                    //month
                    value = DateTime.Now.Month.ToString();
                    headerBytes[2] = Convert.ToByte(value);

                    //day 
                    value = DateTime.Now.ToString();
                    headerBytes[3] = Convert.ToByte(value);

                    //number of records in the table (default to zeros)
                    headerBytes[4] = 0;
                    headerBytes[5] = 0;
                    headerBytes[6] = 0;
                    headerBytes[7] = 0;

                    //number of bytes in the header
                    num = (_dbfWriterInfo.ColumnNames.Count + 1) * 32 + 1;
                    pos = num;

                    b = BitConverter.GetBytes(num);
                    headerBytes[8] = b[0];
                    headerBytes[9] = b[1];

                    //length of each record
                    num = _dbfWriterInfo.ColumnLengthTotal + 1;
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
            }
        }

        public void WriteHeaderRecords()
        {
            var recordBytes = new byte[32];
            Int32 num;
            string columnName;
            Int32 columnLength;

            using (var fileStream = new FileStream(_dbfWriterInfo.Path, FileMode.Open, FileAccess.Write)) 
            {
                using (var binaryWriter = new BinaryWriter(fileStream)) 
                {
                    //move to position 32
                    fileStream.Seek(32, SeekOrigin.Begin);

                    for (int i = 0; i < _dbfWriterInfo.ColumnNames.Count; i++)
                    {
                        columnName = _dbfWriterInfo.ColumnNames[i];
                        columnLength = Convert.ToInt32(_dbfWriterInfo.ColumnLengths[i]);
                        num = columnName.Length;                        

                        columnName.ToCharArray(0, columnName.Length - 1);

                        //note column names cannot be longer than 10
                        for (int n = 0; n < 10; n++)
                        {
                            recordBytes[n] = 0;
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
    }
}