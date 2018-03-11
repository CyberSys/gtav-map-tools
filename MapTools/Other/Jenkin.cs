using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTools.Other
{
    static class Jenkin
    {
        public static uint GenHash(string text)
        {
            uint h = 0;
            for (int i = 0; i < text.Length; i++)
            {
                h += (byte)text[i];
                h += (h << 10);
                h ^= (h >> 6);
            }
            h += (h << 3);
            h ^= (h >> 11);
            h += (h << 15);

            return h;
        }
    }
}
