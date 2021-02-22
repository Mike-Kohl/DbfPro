namespace DbfPro
{
    interface IDbfWriter
    {
        void CreateFile();
        void WriteHeader();
        void WriteHeaderRecords();
        void AddRecords();
        void FinalProcessing();
    }
}
