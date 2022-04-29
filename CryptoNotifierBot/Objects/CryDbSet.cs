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
        [DataType(DataType.Date)]
        public string Date { get; set; }
        [DataType(DataType.Time)]
        public string Time { get; set; }

        public string Exchange { get; set; }

        public DateTime GetDateTime()
        {
            return Convert.ToDateTime($"{Date} {Time}");
        }

        public CryDbSet()
        {

        }
        public CryDbSet(DateTime dateTime, string exchange)
        {
            Date = dateTime.ToShortDateString();
            Exchange = exchange;
            Time = dateTime.ToLongTimeString();
        }
    }
}
