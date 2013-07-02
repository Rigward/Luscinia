using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioCloud
{
    class _sizes
    {
        private double _Height;
        private double _Width;
        public double Height 
        {
            set { ;} 
            get { return _Height; } 
        }
        public double Width 
        {
            set { ;}
            get { return _Width; }
        }

        public _sizes()
        { 
            _Height = 580;
            _Width = 540;
        }
    }

}
