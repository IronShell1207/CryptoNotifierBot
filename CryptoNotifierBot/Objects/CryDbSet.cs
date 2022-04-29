using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CryptoApi.Objects
{
    public class CryDbSet
    {
        
        public int Id { get; set; }
        [DataType(DataType.DateTime)]
        public string DateTime { get; set; }

        public string Exchange { get; set; }

        public DateTime GetDateTime()
        {
            return Convert.ToDateTime(DateTime);
        }

        public CryDbSet()
        {

        }
        public CryDbSet(DateTime dateTime, string exchange)
        { 
            this.DateTime = dateTime.ToString();
            Exchange = exchange;
        }
    }
}
