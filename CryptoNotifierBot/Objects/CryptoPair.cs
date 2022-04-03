using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace CryptoApi.Objects
{
    public class CryptoPair
    {
        public string Name { get; set; }
        public string Quote { get; set; }

        public CryptoPair(){}
        public CryptoPair(string name, string quote)
        {
            this.Name = name;
            this.Quote = quote;
        }
    }
}
