using System.IO;

namespace DbfPro
{
    interface IDbfWriter
    {
        void CreateFile();
        void WriteHeader(FileStream fileStream, BinaryWriter binaryWriter);
        void WriteHeaderRecords(FileStream fileStream, BinaryWriter binaryWriter);
        void AddRecords(BinaryWriter binaryWriter);
        void FinalProcessing(BinaryWriter binaryWriter);
    }
}
