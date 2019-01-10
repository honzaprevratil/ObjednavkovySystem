using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjednavkovySystem
{
    class Order : DBitem
    {
        public DateTime date { get; set; }
        public int visible { get; set; }
        public List<Item> items { get; set; }
    }
}
