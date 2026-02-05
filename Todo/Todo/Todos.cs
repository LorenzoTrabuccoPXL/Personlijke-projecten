using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo
{
    class Todos
    {
        public string name { get; set; }
        public string omschrijving { get; set; }
        public DateTime deadlineDate { get; set; }
        public string status { get; set; }

        public override string ToString()
        {
            return name;
        }
    }
}
