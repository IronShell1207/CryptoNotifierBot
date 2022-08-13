using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CryptoApi.Objects.ExchangesPairs;

namespace CryptoApi.Objects
{
    public class KuTickerDB : KuTicker
    {
        [Key]
        public int Id { get; set; }
        [NotMapped]
        public KuTicker Parent;


        public KuTickerDB() { }
        public KuTickerDB(KuTicker parent)
        {
            Parent = parent;

            foreach (PropertyInfo prop in parent.GetType().GetProperties())
                try
                {
                    GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(parent, null), null);
                }
                catch (ArgumentException ex)
                {
                    continue;
                }
        }
    }
}
