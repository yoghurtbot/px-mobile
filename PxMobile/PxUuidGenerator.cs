using System;
using px_mobile.Extensions;

namespace px_mobile.PxMobile
{
    public static class UuidLsbGenerator
    {
        private static string f2873a = null;
        private static long f2874b;

        static UuidLsbGenerator()
        {
            long j;
            f2874b = long.MinValue;
            Random random = new Random();
            string str = f2873a;
            if (str != null)
            {
                j = long.Parse(str, System.Globalization.NumberStyles.HexNumber) | f2874b;
            }
            else
            {
                byte[] b = m4090b();
                long j2 = f2874b | (((long)(b[0] << 24)) & 4278190080L);
                f2874b = j2;
                long j3 = j2 | ((long)((b[1] << 16) & 16711680));
                f2874b = j3;
                long j4 = j3 | ((long)((b[2] << 8) & 65280));
                f2874b = j4;
                j = j4 | ((long)(b[3] & byte.MaxValue));
            }
            f2874b = j;
            f2874b |= ((long)(random.NextDouble() * 16383.0d)) << 48;
        }

        public static long m4089a()
        {
            return f2874b;
        }

        private static byte[] m4090b()
        {
            byte[] bArr = new byte[4];
            new Random().NextBytes(bArr);
            return bArr;
        }
    }

    /// <summary>
    /// Generates a random Uuid needed for PX mobile. Uses the Java Uuid implementation that accepts LSB and MSB as the constructor
    /// </summary>
    public class PxUuidGenerator
    {
        private static long f2872a = long.MinValue;

        public static Uuid GeneratePxUuid()
        {
            return GeneratePxUuid(DateExtensions.UnixTimeNowMs());
        }

        private static Uuid GeneratePxUuid(long j)
        {
            return new Uuid(m4088d(m4086b(m4087c(j))), UuidLsbGenerator.m4089a());
        }

        private static long m4086b(long j)
        {
            lock (typeof(PxUuidGenerator))
            {
                long j2 = f2872a;
                if (j > j2)
                {
                    f2872a = j;
                    return j;
                }
                long j3 = j2 + 1;
                f2872a = j3;
                return j3;
            }
        }

        private static long m4087c(long j)
        {
            return (j * 10000) + 122192928000000000L;
        }

        private static long m4088d(long j)
        {
            return ((j & -281474976710656L) >>> 48) | (j << 32) | 4096 | ((281470681743360L & j) >>> 16);
        }
    }
}
