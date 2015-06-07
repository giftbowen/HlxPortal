using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeSan.HlxPortal.Common
{
    public static class Util
    {
        public static string GetCpfImagePath(string root, byte siteId, DateTime time)
        {
            return string.Format(@"{0}{1:000}\{2:yyyy\\MM\\dd\\yyyyMMddHHmmss}_{1:000}_CPF.jpg", root, siteId, time);
        }

        public static string GetLpnImagePath(string root, byte siteId, DateTime time)
        {
            return string.Format(@"{0}{1:000}\{2:yyyy\\MM\\dd\\yyyyMMddHHmmss}_{1:000}_LPN.jpg", root, siteId, time);
        }
        public static string GetRadiationImagePath(string root, byte siteId, string sn)
        {
            return GetRadiationImagePath(root, siteId, FromSN(sn));
        }

        public static string GetRadiationImagePath(string root, byte siteId, DateTime time)
        {
            return string.Format(@"{0}{1:000}\{2:yyyy\\MM\\dd\\yyyyMMddHHmmss}_{1:000}_RC.jpg", root, siteId, time);
        }

        public static string GetSN(DateTime time)
        {
            return time.ToString("yyyyMMddHHmmss");
        }

        public static DateTime FromSN(string sn)
        {
            return DateTime.ParseExact(sn, "yyyyMMddHHmmss", null, DateTimeStyles.AssumeLocal);
        }

        public static void CreateDirectoryOfFilePath(string fileFullPath)
        {
            string path = fileFullPath.Remove(fileFullPath.LastIndexOf("\\"));
            Directory.CreateDirectory(path);
        }

        public static int IndexOfPattern(this byte[] array, byte[] pattern, int offset = 0)
        {
            if (array == null || pattern == null)
            {
                throw new ArgumentNullException();
            }
            if (array.Length == 0 || pattern.Length == 0)
            {
                throw new ArgumentException("stream.Length == 0 or pattern.Length == 0 !");
            }

            int success = 0;
            for (int i = offset; i < array.Length; i++)
            {
                if (array[i] == pattern[success])
                {
                    success++;
                }
                else
                {
                    success = 0;
                }

                if (pattern.Length == success)
                {
                    return i - pattern.Length + 1;
                }
            }
            return -1;
        }
    }

    public class BigEndianBitConverter
    {
        //
        // Summary:
        //     Returns a 16-bit unsigned integer converted from two bytes at a specified
        //     position in a byte array.
        //
        // Parameters:
        //   value:
        //     The array of bytes.
        //
        //   startIndex:
        //     The starting position within value.
        //
        // Returns:
        //     A 16-bit unsigned integer formed by two bytes beginning at startIndex.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     startIndex equals the length of value minus 1.
        //
        //   System.ArgumentNullException:
        //     value is null.
        //
        //   System.ArgumentOutOfRangeException:
        //     startIndex is less than zero or greater than the length of value minus 1.
        [CLSCompliant(false)]
        public static ushort ToUInt16(byte[] value, int startIndex)
        {
            return (UInt16)(value[startIndex] << 8 | value[startIndex + 1]);
        }

        //
        // Summary:
        //     Returns a 32-bit unsigned integer converted from four bytes at a specified
        //     position in a byte array.
        //
        // Parameters:
        //   value:
        //     An array of bytes.
        //
        //   startIndex:
        //     The starting position within value.
        //
        // Returns:
        //     A 32-bit unsigned integer formed by four bytes beginning at startIndex.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     startIndex is greater than or equal to the length of value minus 3, and is
        //     less than or equal to the length of value minus 1.
        //
        //   System.ArgumentNullException:
        //     value is null.
        //
        //   System.ArgumentOutOfRangeException:
        //     startIndex is less than zero or greater than the length of value minus 1.
        [CLSCompliant(false)]
        public static uint ToUInt32(byte[] value, int startIndex)
        {
            return (UInt32)(value[startIndex] << 24 | value[startIndex + 1] << 16 | value[startIndex + 2] << 8 | value[startIndex + 3]);
        }

        //
        // Summary:
        //     Returns the specified 16-bit unsigned integer value as an array of bytes.
        //
        // Parameters:
        //   value:
        //     The number to convert.
        //
        // Returns:
        //     An array of bytes with length 2.
        [CLSCompliant(false)]
        public static byte[] GetBytes(ushort value)
        {
            return new byte[] { (byte)(value >> 8), (byte)value };
        }
    }
}
