using System.Runtime.InteropServices;

namespace MeGsvWrapper
{

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct GsvExtended
    {
        public int actex_size;
        public int actex_buffersize;
        public int actex_flags;
        public int actex_baudrate;
        public int actex_strprefix;

        public static GsvExtended Create(
            int bufferSize = 32,
            int flags = 0x40000001,
            int baudRate = 9600,
            int stringPrefix = 0xFFFF)
        {
            return new GsvExtended
            {
                actex_size = Marshal.SizeOf<GsvExtended>(),
                actex_buffersize = bufferSize,
                actex_flags = flags,
                actex_baudrate = baudRate,
                actex_strprefix = stringPrefix
            };
        }
    }
}
