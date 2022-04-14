using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Objects
{
    public class BlackListedPairs
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string Base { get; set; }
        public string Quote { get; set; }
        public override string ToString()
        {
            return $"{Base}/{Quote}";
        }
    }
}
