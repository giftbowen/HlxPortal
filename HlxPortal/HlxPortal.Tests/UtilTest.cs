using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LeSan.HlxPortal.Common;
using System.Drawing;
using System.IO;
using System.Configuration;
using System.Drawing.Imaging;
namespace HlxPortal.Tests
{
    [TestClass]
    public class UtilTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            string path = "c:\\abc\\abcd\\12.jpg";
            string res = path.Remove(path.LastIndexOf("\\"));
            Directory.CreateDirectory(res);
            

            byte[] aa = new byte[] { 0xF1, 0xF2, 0xF3, 0xF4 };
            var bb = BigEndianBitConverter.ToUInt32(aa, 0);
            var cc = BitConverter.ToUInt32(aa, 0);
            var b1 = BigEndianBitConverter.ToUInt16(aa, 0);
            var c1 = BitConverter.ToUInt16(aa, 0);

            UInt16 a = 0xF1F2;
            var b3 = BitConverter.GetBytes(a);
            var b4 = BigEndianBitConverter.GetBytes(a);
        }

        [TestMethod]
        public void TestMethod2()
        {
            Image img = Image.FromFile(@"C:\1.jpg");
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] bitmap = ms.ToArray();

            //bitmap = new byte[10 * 1024];
            using (ms = new MemoryStream(bitmap))
            {
                Image image = Image.FromStream(ms);
                image.Save("c:\\temp\\1.1.jpg", ImageFormat.Jpeg);
            }
        }

        [TestMethod]
        public void TestMethod3()
        {
            string strDt = "2015-01-02 03:04:05";
            DateTime Dt = DateTime.Parse(strDt);

            string path = Util.GetRadiationImagePath(@"c:\abc\", 5, Dt);

            Assert.IsTrue(path == @"c:\abc\005\2015\01\02\20150102030405_005_RC.jpg");
        }

    }
}
