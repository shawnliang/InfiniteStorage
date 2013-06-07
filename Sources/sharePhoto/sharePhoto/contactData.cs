using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wpf_testHTTP
{
    class contactData
    {
        public static void IncreaseArray(ref string[] values, int increment)
        {
            string[] array = new string[values.Length + increment];

            values.CopyTo(array, 0);
            values = array;
        }

        public static void inc(int _count)
        {
            IncreaseArray(ref States, States.Length + _count);
        }

        public static string[] States = {
            "Alabama","aaaaaa","bbbbbbbbbbbbb"
        };
    }
}
