using ListManagement.interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListManagement.models
{
    public class ToDo: Item
    {
        public DateTime Deadline { get; set; }
        public bool IsCompleted { get; set; }

        public override string ToString()
        {
            return $"Type: ToDo Name:{Name} Description:{Description} Completed:{IsCompleted}";
        }
    }
}
