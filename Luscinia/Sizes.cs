using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luscinia
{
    static class Sizes
    {
        private static uint _Height;
        private static uint _Width;
        public static double Height 
        {
            get { return _Height; } 
        }
        public static double Width 
        {
            get { return _Width; }
        }

        static Sizes()
        { 
            _Height = 580;
            _Width = 540;
        }
    }

}
