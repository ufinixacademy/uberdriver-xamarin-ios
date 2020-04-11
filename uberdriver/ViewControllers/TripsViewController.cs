using Firebase.Database;
using Foundation;
using System;
using System.Collections.Generic;
using uberdriver.Datamodels;
using UIKit;

namespace uberdriver
{
    public partial class TripsViewController : UIViewController
    {
        public List<string> tripKeys;
        List<RideHistory> rideHistories = new List<RideHistory>();

        public TripsViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            historyTableView.Source = new TripDataSource(rideHistories);
            historyTableView.ReloadData();

            GetRideHistory();

            exitButton.UserInteractionEnabled = true;
            exitButton.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                DismissModalViewController(true);
            }));
        }

        public void GenerateData()
        {
            rideHistories.Add(new RideHistory { Status = "ended", Created_At = DateTime.Now, DestinationAddress = "SPAR PH", Fares = 40, PickupAddress = "Genesis Cinema" });
            rideHistories.Add(new RideHistory { Status = "ended", Created_At = DateTime.Now, DestinationAddress = "Stamford University", Fares = 100, PickupAddress = "New York Times" });

        }

        public void GetRideHistory()
        {
            if (tripKeys.Count == 0)
            {
                return;
            }


            foreach (string key in tripKeys)
            {
                DatabaseReference tripRef = Database.DefaultInstance.GetRootReference().GetChild("rideRequest/" + key);
                tripRef.ObserveSingleEvent(DataEventType.Value, (DataSnapshot snapshot) =>
                {

                    if(snapshot.GetValue<NSObject>() != NSNull.Null)
                    {
                        if(snapshot.GetChildSnapshot("status").GetValue<NSObject>() != NSNull.Null)
                        {
                            var rideItem = new RideHistory();

                            rideItem.Status = snapshot.GetChildSnapshot("status").GetValue<NSObject>().ToString();
                            rideItem.DestinationAddress = snapshot.GetChildSnapshot("destination_address").GetValue<NSObject>().ToString();
                            rideItem.PickupAddress = snapshot.GetChildSnapshot("pickup_address").GetValue<NSObject>().ToString();
                            rideItem.Created_At = DateTime.Parse(snapshot.GetChildSnapshot("created_at").GetValue<NSObject>().ToString());

                            if(snapshot.GetChildSnapshot("fares").GetValue<NSObject>() != NSNull.Null)
                            {
                                rideItem.Fares = double.Parse(snapshot.GetChildSnapshot("fares").GetValue<NSObject>().ToString());
                            }
                            else
                            {
                                rideItem.Fares = 0;
                            }

                            rideHistories.Add(rideItem);
                            historyTableView.ReloadData();
                        }
                    }

                });
            }
        }
    }
}