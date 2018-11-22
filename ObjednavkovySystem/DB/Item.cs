using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjednavkovySystem
{
    class Item : DBitem
    {
        public string name { get; set; }
        public string description { get; set; }
        public int price { get; set; }

    }
}
