using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SobelFilter
{
    class UserParams
    {
        private string _inputFileName;
        public string InputFileName
        {
            get { return _inputFileName; }
        }

        public bool ReadArguments(string[] args)
        {
            if(args.Length <1 )
            {
                return false;
            }

            _inputFileName = args[0];
            return true;
        }
    }
}
