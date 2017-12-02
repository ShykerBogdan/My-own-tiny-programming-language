using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Freel.Entities
{
  public  class Constant : Token
    {        
        public Constant(string value, int index, int row, int? classIndex) : base(value, index, row,classIndex)
        {
            ClassIndex = classIndex;
        }
    }
}
