using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SobelFilter
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine("sobel");

            UserParams userParams = new UserParams();
            if(!userParams.ReadArguments(args))
            {
                return 1;
            }

            return 0;
        }
    }
}
