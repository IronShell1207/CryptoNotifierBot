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
    public class KuTickerDB
    {
        [Key]
        public int Id { get; set; }
        [NotMapped]
        public KuTicker Parent;
        public string symbol { get; set; }
        public string symbolName { get; set; }
        public string buy { get; set; }
        public string sell { get; set; }
        public string changeRate { get; set; }
        public string changePrice { get; set; }
        public string high { get; set; }
        public string low { get; set; }
        public string vol { get; set; }
        public string volValue { get; set; }
        public string last { get; set; }
        public string averagePrice { get; set; }
        [NotMapped]
        public string takerFeeRate { get; set; }
        [NotMapped]
        public string makerFeeRate { get; set; }
        public string takerCoefficient { get; set; }
        public string makerCoefficient { get; set; }
        [NotMapped]
        public string Price => last;

        public KuTickerDB() { }

        public KuTickerDB(KuTicker parent)
        {
            Parent = parent;

            foreach (PropertyInfo prop in parent.GetType().GetProperties())
                try
                {
                    GetType().GetProperty(prop.Name)?.SetValue(this, prop?.GetValue(parent, null), null);
                }
                catch (ArgumentException ex)
                {
                    continue;
                }
                catch (System.NullReferenceException ex)
                {

                }
        }
    }
}
