using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Freel.Entities
{
 public class Error
    {
        public Error(int number, int? line, string desctiption)
        {
            Number = number;
            Line = line;
            Description = desctiption;            
        }

        public int Number
        {
            get;
            protected set;
        }

        public int? Line
        {
            get;
            protected set;
        }  

        public  string Description
        {
            get;
        }

      
    }
}
