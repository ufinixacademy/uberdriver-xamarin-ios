using System;
namespace uberdriver.Datamodels
{
    public class RideDetails
    {
        public string PickupAddress { get; set; }
        public string DestinationAddress { get; set; }
        public string RiderName { get; set; }
        public string RiderPhone { get; set; }
        public double PickupLat { get; set; }
        public double PickLng { get; set; }
        public double DestinationLat { get; set; }
        public double DestinationLng { get; set; }
        public string RideId { get; set; }
    }
}
