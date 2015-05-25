using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var t1 = new List<int> { };

            var t2 = t1.Where(a => a == 1);

            foreach (var item in t2)
            {
                
            }
        }
    }
}
