using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjednavkovySystem
{
    class User : DBitem
    {
        public string nick { get; set; }
        public string password { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
    }
}
