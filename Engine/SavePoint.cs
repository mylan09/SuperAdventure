using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class SavePoint
    {
        public string Name { get; set; }


        public SavePoint(string name)
        {
            Name = name;
            
        }
    }
}
