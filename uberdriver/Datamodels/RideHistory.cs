using System;
namespace uberdriver.Datamodels
{
    public class RideHistory
    {
       public string PickupAddress { get; set; }
        public string DestinationAddress { get; set; }
        public DateTime Created_At { get; set; }
        public string Status;
        public double Fares { get; set; }
    }
}
