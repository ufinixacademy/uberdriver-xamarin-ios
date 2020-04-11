using System;
using CoreLocation;
using Firebase.Auth;
using Firebase.Database;
using Foundation;
using uberdriver.Datamodels;
using uberrider.Helpers;

namespace uberdriver.EventListeners
{
    public class AvailabilityListener
    {
        DatabaseReference driverAvailableRef;
        private bool rideAssigned;

        public event EventHandler RideCancelled;
        public event EventHandler RideTimedOut;

        public class RideDetailsEventArgs : EventArgs
        {
            public RideDetails RideDetails { get; set; }
        }

        public event EventHandler<RideDetailsEventArgs> RideDetailsFound;
        public event EventHandler RideDetailsNotFound;

        public AvailabilityListener()
        {
        }

        public void Create(CLLocationCoordinate2D location)
        {
            var locationCordinate = new NSDictionary
                (
                "latitude", location.Latitude.ToString(),
                "longitude", location.Longitude.ToString()
                ) ;

            var driverInfo = new NSDictionary
                (
                "location", locationCordinate,
                "ride_id", "waiting"
                );

            driverAvailableRef = Database.DefaultInstance.GetRootReference().GetChild("driversAvailable/" + AppDataHelper.GetDriverID());
            driverAvailableRef.SetValue<NSDictionary>(driverInfo);

            driverAvailableRef.ObserveEvent(DataEventType.Value, (DataSnapshot snapshot) =>
            {

                if(snapshot.GetValue<NSObject>() != NSNull.Null)
                {
                    if(snapshot.GetChildSnapshot("ride_id").GetValue<NSObject>() != NSNull.Null)
                    {
                        string ride_id = snapshot.GetChildSnapshot("ride_id").GetValue<NSObject>().ToString();

                        if(ride_id != "waiting" && ride_id != "timeout" && ride_id != "cancelled")
                        {
                            // Ride has been assigned to us
                            if (!rideAssigned)
                            {
                                GetRideDetails(ride_id);
                                rideAssigned = true;
                            }
                        }
                        else if (ride_id == "timeout")
                        {
                            // timeout
                            RideTimedOut?.Invoke(this, new EventArgs());
                            rideAssigned = false;
                            ReActivate();
                        }
                        else if(ride_id == "cancelled")
                        {
                            // cancelled;
                            RideCancelled?.Invoke(this, new EventArgs());
                            rideAssigned = false;
                            ReActivate();
                        }
                    }
                }

            });
        }


        public void ReActivate()
        {
            driverAvailableRef.GetChild("ride_id").SetValue((NSString)"waiting");
            rideAssigned = false;
        }

        public void RemoveListener()
        {
            driverAvailableRef.RemoveValue();
            driverAvailableRef.RemoveAllObservers();
            driverAvailableRef = null;
        }


        public void GetRideDetails(string ride_id)
        {
            DatabaseReference rideDetailsRef = Database.DefaultInstance.GetRootReference().GetChild("rideRequest/" + ride_id);
            rideDetailsRef.ObserveSingleEvent(DataEventType.Value, (DataSnapshot snapshot) =>
            {
                if(snapshot.GetValue<NSObject>() != NSNull.Null)
                {
                    RideDetails rideDetails = new RideDetails();
                    rideDetails.DestinationAddress = snapshot.GetChildSnapshot("destination_address").GetValue<NSObject>().ToString();
                    rideDetails.DestinationLat = double.Parse(snapshot.GetChildSnapshot("destination").GetChildSnapshot("latitude").GetValue<NSObject>().ToString());
                    rideDetails.DestinationLng = double.Parse(snapshot.GetChildSnapshot("destination").GetChildSnapshot("longitude").GetValue<NSObject>().ToString());
                    rideDetails.PickupAddress = snapshot.GetChildSnapshot("pickup_address").GetValue<NSObject>().ToString();
                    rideDetails.PickupLat = double.Parse(snapshot.GetChildSnapshot("location").GetChildSnapshot("latitude").GetValue<NSObject>().ToString());
                    rideDetails.PickLng = double.Parse(snapshot.GetChildSnapshot("location").GetChildSnapshot("longitude").GetValue<NSObject>().ToString());

                    rideDetails.RideId = snapshot.Key;
                    rideDetails.RiderName = snapshot.GetChildSnapshot("rider_name").GetValue<NSObject>().ToString();
                    rideDetails.RiderPhone = snapshot.GetChildSnapshot("rider_phone").GetValue<NSObject>().ToString();

                    RideDetailsFound?.Invoke(this, new RideDetailsEventArgs { RideDetails = rideDetails });
                }
                else
                {
                    RideDetailsNotFound?.Invoke(this, new EventArgs());
                }

            });
        }


        public void UpdateLocation(CLLocationCoordinate2D currentLocation)
        {
            var locationCordinate = new NSDictionary
               (
               "latitude", currentLocation.Latitude.ToString(),
               "longitude", currentLocation.Longitude.ToString()
               );

            driverAvailableRef.GetChild("location").SetValue<NSDictionary>(locationCordinate);
        }
    }
}
