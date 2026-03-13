using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace MeGsvWrapper
{

    public sealed class GsvDevice : IDisposable
    {
        private bool _disposed;
        private bool _isOpen;

        public int ComNo { get; }
        public bool IsOpen => _isOpen;

        public GsvDevice(int comNo)
        {
            if (comNo < 0)
                throw new ArgumentOutOfRangeException(nameof(comNo), "COM number must be >= 0.");

            ComNo = comNo;
        }

        public void Open(int bufferSize = 32)
        {
            ThrowIfDisposed();

            if (_isOpen)
                return;

            int ret = GsvNative.GSVactivate(ComNo, bufferSize);
            EnsureSuccess(ret, "GSVactivate failed");

            _isOpen = true;
        }

        public void OpenExtended(
            int baudRate,
            int bufferSize = 32,
            int flags = 0x40000001,
            int stringPrefix = 0xFFFF)
        {
            ThrowIfDisposed();

            if (_isOpen)
                return;

            GsvExtended ext = GsvExtended.Create(
                bufferSize: bufferSize,
                flags: flags,
                baudRate: baudRate,
                stringPrefix: stringPrefix);

            int ret = GsvNative.GSVactivateExtended(ComNo, ref ext);
            EnsureSuccess(ret, "GSVactivateExtended failed");

            _isOpen = true;
        }

        public double ReadValue()
        {
            EnsureOpen();

            double value = 0.0;
            int ret = GsvNative.GSVread(ComNo, ref value);
            EnsureTrue(ret, "GSVread failed");

            return value;
        }

        public bool TryReadValue(out double value)
        {
            EnsureOpen();

            value = 0.0;
            int ret = GsvNative.GSVread(ComNo, ref value);
            return ret ==1;
        }

        public double ReadValueRetry(int maxAttempts = 100, int delayMs = 5)
        {
            EnsureOpen();

            if (maxAttempts <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxAttempts));

            for (int i = 0; i < maxAttempts; i++)
            {
                if (TryReadValue(out double value))
                    return value;

                if (delayMs > 0)
                    Thread.Sleep(delayMs);
            }

            throw new GsvException("Unable to read a value after retries", ComNo, null, GetLastError());
        }

        public double[] ReadMultiple(int count)
        {
            EnsureOpen();

            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            double[] buffer = new double[count];
            int valuesRead = 0;
            int maxRetry = 500;

            int ret = -1;
            ret = GsvNative.GSVreadMultiple(ComNo, ref buffer[0], count, ref valuesRead);

            if (ret == -1)
            {
                int err = GsvNative.GSVgetLastError(ComNo);
                throw new GsvException("GSVreadMultiple failed", ComNo, ret, err);
            }else if (ret == 0)
            {
                int delayMs = 5;
                while (ret != 1 && maxRetry >= 0) { 
                    ret = GsvNative.GSVreadMultiple(ComNo, ref buffer[0], count, ref valuesRead);
                    maxRetry -= 1;
                    if (delayMs > 0)
                        Thread.Sleep(delayMs);
                }
          
            }
            if (valuesRead < 0) valuesRead = 0;
            if (valuesRead > count) valuesRead = count;

            double[] result = new double[valuesRead];
            Array.Copy(buffer, result, valuesRead);
            return result;
        }
        public void SetZero()
        {
            EnsureOpen();

            int ret = GsvNative.GSVsetZero(ComNo);
            EnsureSuccess(ret, "GSVsetZero failed");
        }

        public void SetMeasurementValueProperty(int propertyType)
        {
            EnsureOpen();

            int ret = GsvNative.GSVsetMeasValProperty(ComNo, propertyType);
            EnsureSuccess(ret, "GSVsetMeasValProperty failed");
        }

        public void FlushBuffer()
        {
            EnsureOpen();

            int ret = GsvNative.GSVflushBuffer(ComNo);
            EnsureSuccess(ret, "GSVflushBuffer failed");
        }

        public double GetNorm()
        {
            EnsureOpen();
            return GsvNative.GSVDispGetNorm(ComNo);
        }

        public double GetSamplingFrequency()
        {
            EnsureOpen();
            return GsvNative.GSVreadSamplingFrequency(ComNo);
        }

        public long GetScale()
        {
            EnsureOpen();
            return GsvNative.GSVgetScale(ComNo);
        }

        public int GetLastError()
        {
            ThrowIfDisposed();
            return GsvNative.GSVgetLastError(ComNo);
        }

        public void SendAsciiCommand(string command)
        {
            EnsureOpen();

            byte[] bytes = BuildAsciiCommand(command);
            IntPtr pBuffer = Marshal.AllocHGlobal(bytes.Length);

            try
            {
                Marshal.Copy(bytes, 0, pBuffer, bytes.Length);

                int ret = GsvNative.MEsendBytes(ComNo, pBuffer, bytes.Length);
                EnsureSuccess(ret, "MEsendBytes failed");
            }
            finally
            {
                Marshal.FreeHGlobal(pBuffer);
            }
        }

        public string ReadAsciiString(int responseBufferSize = 256)
        {
            EnsureOpen();

            if (responseBufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(responseBufferSize));

            var output = new StringBuilder(responseBufferSize);
            int stillInBuffer = 0;

            int ret = GsvNative.AsciiComReadString(ComNo, output, ref stillInBuffer);

            if (ret == -1)
                throw new GsvException("AsciiComReadString failed", ComNo, ret, GetLastError());

            if (ret == 0)
                return string.Empty;

            return output.ToString().Trim();
        }

        public string QueryAscii(string command, int waitMs = 200, int responseBufferSize = 256)
        {
            SendAsciiCommand(command);

            if (waitMs > 0)
                Thread.Sleep(waitMs);

            return ReadAsciiString(responseBufferSize);
        }


        public void Close()
        {
            if (!_isOpen)
                return;

            GsvNative.GSVrelease(ComNo);

            _isOpen = false;
        }

        private void EnsureOpen()
        {
            ThrowIfDisposed();

            if (!_isOpen)
                throw new InvalidOperationException("Device is not open.");
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(GsvDevice));
        }

        private void EnsureSuccess(int nativeReturnCode, string message)
        {
            if (nativeReturnCode == -1)
                throw new GsvException(message, ComNo, nativeReturnCode, SafeGetLastError());
        }

        private void EnsureTrue(int nativeReturnCode, string message)
        {
            if (nativeReturnCode != 1)
                throw new GsvException(message, ComNo, nativeReturnCode, SafeGetLastError());
        }

        private int? SafeGetLastError()
        {
            try
            {
                return GsvNative.GSVgetLastError(ComNo);
            }
            catch
            {
                return null;
            }
        }

        private static byte[] BuildAsciiCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentException("Command must not be null or empty.", nameof(command));

            if (!command.EndsWith("\r\n", StringComparison.Ordinal))
                command += "\r\n";

            return Encoding.ASCII.GetBytes(command);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            try
            {
                if (_isOpen)
                {
                    GsvNative.GSVrelease(ComNo);
                    _isOpen = false;

               
                }
            }
            finally
            {
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}
