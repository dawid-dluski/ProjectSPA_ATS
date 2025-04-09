using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSPA_ATS.Helpers
{
    internal class ConsoleEncodingHelper
    {
        public static void SetIBM852Encoding()
        {
            var ibm852 = Encoding.GetEncoding(852);
            Console.InputEncoding = ibm852;
            Console.OutputEncoding = ibm852;
        }
    }
}
