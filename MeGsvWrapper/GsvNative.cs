using System.Runtime.InteropServices;
using System.Text;

namespace MeGsvWrapper
{
    internal static class GsvNative
    {
        private const string DllName = "MEGSV.dll";

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int GSVactivate(int no, int buffersize);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int GSVactivateExtended(int no, ref GsvExtended actex);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int GSVinitialize(int no);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int GSVflushBuffer(int no);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int GSVread(int no, ref double ad);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int GSVreadMultiple(int no, ref double ad, int count, ref int valsread);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern void GSVrelease(int no);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int GSVsetZero(int no);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern double GSVDispGetNorm(int no);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern long GSVgetScale(int no);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern double GSVreadSamplingFrequency(int no);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int GSVgetLastError(int no);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern int GSVgetLastErrorText(int no, StringBuilder errText);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int GSVstartTransmit(int no);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int GSVstopTransmit(int no);



        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int GSVsetMeasValProperty(int comNo, int propType);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int AsciiComReadString(int comNo, StringBuilder output, ref int stillInBuffer);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int MEsendBytes(int comNo, IntPtr pbuf, int count);
    }
}