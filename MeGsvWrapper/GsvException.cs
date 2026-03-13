using System.Runtime.Serialization;

namespace MeGsvWrapper
{

    [Serializable]
    public class GsvException : Exception
    {
        public int ComNo { get; }
        public int? NativeReturnCode { get; }
        public int? LastError { get; }

        public GsvException(string message)
            : base(message)
        {
        }

        public GsvException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public GsvException(string message, int comNo, int? nativeReturnCode = null, int? lastError = null)
            : base(BuildMessage(message, comNo, nativeReturnCode, lastError))
        {
            ComNo = comNo;
            NativeReturnCode = nativeReturnCode;
            LastError = lastError;
        }

        protected GsvException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ComNo = info.GetInt32(nameof(ComNo));
            NativeReturnCode = (int?)info.GetValue(nameof(NativeReturnCode), typeof(int?));
            LastError = (int?)info.GetValue(nameof(LastError), typeof(int?));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(ComNo), ComNo);
            info.AddValue(nameof(NativeReturnCode), NativeReturnCode, typeof(int?));
            info.AddValue(nameof(LastError), LastError, typeof(int?));
        }

        private static string BuildMessage(string message, int comNo, int? nativeReturnCode, int? lastError)
        {
            return $"{message} (COM={comNo}, ReturnCode={nativeReturnCode?.ToString() ?? "n/a"}, LastError={lastError?.ToString() ?? "n/a"})";
        }
    }
}
