using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo //
{
    public enum TodoStatus 
    {
        Todo,
        InProgress,
        Done
    }

    class TodoItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public TodoStatus Status { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
