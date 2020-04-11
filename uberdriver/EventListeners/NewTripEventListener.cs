using System;
using CoreLocation;
using Firebase.Database;
using Foundation;
using uberrider.Helpers;

namespace uberdriver.EventListeners
{
    public class NewTripEventListener
    {
        string rideID;
        CLLocationCoordinate2D currentLocation;
        DatabaseReference tripRef;
        private bool isAccepted;

        public NewTripEventListener(string _rideid, CLLocationCoordinate2D _currentLocation)
        {
            rideID = _rideid;
            currentLocation = _currentLocation;
        }

        public void Create(string rideid)
        {
            tripRef = Database.DefaultInstance.GetRootReference().GetChild("rideRequest/" + rideid);
            tripRef.ObserveEvent(DataEventType.Value, (DataSnapshot snapshot) =>
            {

                if(snapshot.GetValue<NSObject>() != NSNull.Null)
                {
                    if (!isAccepted)
                    {
                        isAccepted = true;
                        Accept();
                    }
                }

            });
        }


        public void Accept()
        {
            var locationCordinate = new NSDictionary
                (
                 "latitude", currentLocation.Latitude.ToString(),
                 "longitude", currentLocation.Longitude.ToString()
                );

            tripRef.GetChild("driver_location").SetValue<NSDictionary>(locationCordinate);
            tripRef.GetChild("driver_name").SetValue((NSString)AppDataHelper.GetFullName());
            tripRef.GetChild("driver_id").SetValue((NSString)AppDataHelper.GetDriverID());
            tripRef.GetChild("driver_phone").SetValue((NSString)AppDataHelper.GetPhone());
            tripRef.GetChild("status").SetValue((NSString)"accepted");
        }

        public void UpdateLocation(CLLocationCoordinate2D currentLocation)
        {
            var locationCordinate = new NSDictionary
             
                (
                "latitude", currentLocation.Latitude.ToString(),
                "longitude", currentLocation.Longitude.ToString()
                );

            tripRef.GetChild("driver_location").SetValue<NSDictionary>(locationCordinate);
        }


        public void UpdateStatus(string status)
        {
            tripRef.GetChild("status").SetValue((NSString)status);
        }

        public void EndTrip(double fares)
        {
            if(tripRef != null)
            {
                tripRef.GetChild("fares").SetValue((NSString)fares.ToString());
                tripRef.GetChild("status").SetValue((NSString)"ended");
                tripRef.RemoveAllObservers();
                tripRef = null;
            }
        }
    }
}
