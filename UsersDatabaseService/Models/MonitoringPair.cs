using System.ComponentModel.DataAnnotations.Schema;

namespace UsersDatabaseService.Models
{
    public class MonitoringPair : CryptoPairModel
    {
        #region Public Properties

        public bool GainOrFall { get; set; }
        public bool IsTriggered { get; set; } = false;
        public bool IsTriggerOnce { get; set; } = false;
        public bool NotifyEnabled { get; set; }

        public double TriggerPrice { get; set; }

        #endregion Public Properties

        #region Public Constructors

        public MonitoringPair()
        { }

        public MonitoringPair(int owner)
        {
            OwnerId = owner;
        }

        #endregion Public Constructors
    }
}