using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_BGPU.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; } // "Наличные", "Карта", "Сберегательный счёт"
        public decimal Balance { get; set; }
    }
}
