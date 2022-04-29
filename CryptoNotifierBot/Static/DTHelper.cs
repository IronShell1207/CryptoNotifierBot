using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoApi.Static
{
    public class DTHelper
    {
        public static bool DatesNearEqual(string date1, string date2) =>
            DatesNearEqual(Convert.ToDateTime(date1), Convert.ToDateTime(date2));
        public static bool DatesNearEqual(DateTime date1, DateTime date2)
        {
            if (date1 == date2)
                return true;
            if (date1.Month == date2.Month && date1.Day == date2.Day &&
                date1.Hour == date2.Hour &&
                date1.Minute == date2.Minute)
            {
                if (date1.Second - 10 < date2.Second && date2.Second < date1.Second + 10) return true;
                if (date2.Second - 10 < date1.Second && date1.Second < date2.Second + 10) return true;
            }

            return false;
        }
    }
}
