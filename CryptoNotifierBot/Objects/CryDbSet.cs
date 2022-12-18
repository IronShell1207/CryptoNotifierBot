using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CryptoApi.Objects
{
    /// <summary>
    /// Набор данных выбранной биржи.
    /// </summary>
    public class CryDbSet
    {
        public int Id { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateTime { get; set; }

        public Guid IdGuid { get; set; }
        public string Exchange { get; set; }
        public List<PricedTradingPair> pairs { get; set; } = new();

        public CryDbSet()
        {
        }

        public CryDbSet(DateTime dt, string exch, Guid guid)
        {
            this.DateTime = dt;
            Exchange = exch;
            IdGuid = guid;
        }
    }
}