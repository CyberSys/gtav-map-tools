using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTools
{
    class ResourceBuilder
    {
        public static byte[] Compress(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                DeflateStream ds = new DeflateStream(ms, CompressionMode.Compress, true);
                ds.Write(data, 0, data.Length);
                ds.Close();
                byte[] deflated = ms.GetBuffer();
                byte[] outbuf = new byte[ms.Length]; //need to copy to the right size buffer...
                Array.Copy(deflated, outbuf, outbuf.Length);
                return outbuf;
            }
        }
        public static byte[] Decompress(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress);
                MemoryStream outstr = new MemoryStream();
                ds.CopyTo(outstr);
                byte[] deflated = outstr.GetBuffer();
                byte[] outbuf = new byte[outstr.Length]; //need to copy to the right size buffer...
                Array.Copy(deflated, outbuf, outbuf.Length);
                return outbuf;
            }
        }

        /*
         byte[] data = File.ReadAllBytes(file.Name);
                                uint rsc7 = BitConverter.ToUInt32(data, 0);
                                if (rsc7 == 0x37435352)
                                {
                                    int version = BitConverter.ToInt32(data, 4);
                                    uint SystemFlags = BitConverter.ToUInt32(data, 8);
                                    uint GraphicsFlags = BitConverter.ToUInt32(data, 12);
                                    if (data.Length > 16)
                                    {
                                        int newlen = data.Length - 16; //trim the header from the data passed to the next step.
                                        byte[] newdata = new byte[newlen];
                                        Buffer.BlockCopy(data, 16, newdata, 0, newlen);
                                        data = newdata;
                                    }
                                }
                                data = ResourceBuilder.Decompress(data);
                                foreach(byte b in data)
                                    Console.WriteLine(b.ToString());
         */
    }
}
