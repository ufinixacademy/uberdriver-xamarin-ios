using AVFoundation;
using CoreLocation;
using Firebase.Database;
using Foundation;
using Google.Maps;
using System;
using System.Diagnostics;
using uberdriver.Datamodels;
using uberdriver.EventListeners;
using uberdriver.Helpers;
using uberrider.Helpers;
using UIKit;
using UserNotifications;

namespace uberdriver
{
    public partial class HomeViewController : UIViewController
    {
        CLLocationManager locationManager = new CLLocationManager();
        private CLLocationCoordinate2D currentLocation;
        private bool avalability;
        private AvailabilityListener availabiltyListener;
        private RideDetails newRideDetails;
        private UIAlertController alertNewRide;
        AVAudioPlayer player;
        private NewTripEventListener newtripListener;
        private MapFunctionHelper mapHelper;
        private string status;
        private Stopwatch durationCounter;

        public HomeViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.LightContent;

            goOnlineButton.Layer.BorderWidth = 2;
            goOnlineButton.Layer.BorderColor = UIColor.FromRGB(206, 206, 206).CGColor;
            goOnlineButton.TouchUpInside += GoOnlineButton_TouchUpInside;

            locationManager.RequestAlwaysAuthorization();
            locationManager.RequestWhenInUseAuthorization();
            locationManager.DesiredAccuracy = CLLocation.AccuracyBest;
            locationManager.LocationsUpdated += LocationManager_LocationsUpdated;
            locationManager.StartUpdatingLocation();

            rideDetailsView.Layer.BorderColor = UIColor.FromRGB(206, 206, 206).CGColor;
            rideDetailsView.Layer.BorderWidth = 1;
            rideDetailsView.Layer.ShadowOpacity = 0.1f;
            rideDetailsView.Layer.ShadowRadius = 2;
            rideDetailsView.Layer.ShadowColor = UIColor.FromRGB(206, 206, 206).CGColor;

            tripButton.TouchUpInside += TripButton_TouchUpInside;

            callButton.UserInteractionEnabled = true;
            callButton.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                string riderPhone = newRideDetails.RiderPhone;
                var url = new NSUrl("tel:" + riderPhone);

                if (!UIApplication.SharedApplication.OpenUrl(url))
                {
                    var alert = UIAlertController.Create("Not supported", "call not supported", UIAlertControllerStyle.Alert);
                    alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
                    PresentViewController(alert, true, null);
                }

            }));
        }

        private async void TripButton_TouchUpInside(object sender, EventArgs e)
        {
            if(status == "ACCEPTED")
            {
                tripButton.BackgroundColor = UIColor.FromRGB(37, 178, 79);
                tripButton.SetTitle("START TRIP", UIControlState.Normal);

                status = "ARRIVED";
                newtripListener.UpdateStatus("arrived");


                ShowProgressBar("Plotting route...");
                var pickupLatLng = new CLLocationCoordinate2D(newRideDetails.PickupLat, newRideDetails.PickLng);
                var destinationLatLng = new CLLocationCoordinate2D(newRideDetails.DestinationLat, newRideDetails.DestinationLng);

                //Fetch direction data from pickup to destination
                string directionJson = await mapHelper.GetDirectionJsonAsync(pickupLatLng, destinationLatLng);
                HideProgressBar();

                // Clear Map
                googleMap.Clear();
                mapHelper.DrawTripToDestination(directionJson);
            }
            else if (status == "ARRIVED")
            {
                var alert = UIAlertController.Create("START TRIP", "You are about to start this trip", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, (obj) =>
                {

                    status = "ONTRIP";
                    newtripListener.UpdateStatus("ontrip");

                    tripButton.BackgroundColor = UIColor.FromRGB(196, 45, 45);
                    tripButton.SetTitle("DROP OFF RIDER", UIControlState.Normal);

                    durationCounter = new Stopwatch();
                    durationCounter.Start();

                }));

                PresentViewController(alert, true, null);
            }
            else if (status == "ONTRIP")
            {
                var alert = UIAlertController.Create("END TRIP", "Drop-off Passender", UIAlertControllerStyle.Alert);
                UIAlertAction alertActionCancel = UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null);

                alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, (obj) =>
                {
                    status = "ENDED";
                    tripButton.SetTitle("TRIP ENDED", UIControlState.Normal);
                    TripEnd();
                }));
                alert.AddAction(alertActionCancel);
                PresentViewController(alert, true, null);
            }
        }


        public void TripEnd()
        {
            durationCounter.Stop();
            double durationMins = int.Parse(durationCounter.ElapsedMilliseconds.ToString()) / 60000;

            double fares = mapHelper.CalculateFares(durationMins);
            newtripListener.EndTrip(fares);

            // Reset App After Trip;
            googleMap.Clear();
            status = "";
            rideDetailsView.Hidden = true;
            centerMarker.Hidden = false;

            tripButton.BackgroundColor = UIColor.FromRGB(24, 191, 242);
            tripButton.SetTitle("ARRIVED PICKUP", UIControlState.Normal);

            goOnlineButton.BackgroundColor = UIColor.FromRGB(7, 175, 18);
            goOnlineButton.SetTitle("GO OFFLINE", UIControlState.Normal);
            goOnlineButton.Enabled = true;

            availabiltyListener.ReActivate();

            faresAmountText.Text = "$" + fares.ToString();
            overlay.Hidden = false;
            colletPaymentView.Hidden = false;
            collectPayementButton.TouchUpInside += (o, obj) =>
            {
                colletPaymentView.Hidden = true;
                overlay.Hidden = true;
                TopUpEarning(fares);
            };
        }

        void TopUpEarning(double fares)
        {
            DatabaseReference earningRef = Database.DefaultInstance.GetRootReference().GetChild("drivers/" + AppDataHelper.GetDriverID() + "/earnings");
            earningRef.ObserveSingleEvent(DataEventType.Value, (DataSnapshot snapshot) =>
            {
                if(snapshot.GetValue<NSObject>() != NSNull.Null)
                {
                    string value = snapshot.GetValue<NSObject>().ToString();
                    double totalEarningsBefore = double.Parse(value);
                    double totalEarningsAfter = totalEarningsBefore + fares;
                    earningRef.SetValue((NSString)totalEarningsAfter.ToString());
                }
                else
                {
                    earningRef.SetValue((NSString)fares.ToString());
                }
            });
        }

        private void GoOnlineButton_TouchUpInside(object sender, EventArgs e)
        {
            if (!avalability)
            {
                var alert = UIAlertController.Create("Go Online", "You are about to GO ONLINE", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, (args) => GoOnline()));
                alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Default, null));
                PresentViewController(alert, true, null);
            }
            else
            {
                var alert = UIAlertController.Create("Go Offline", "You are about to GO OFFLINE", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, (args) => GoOffline()));
                alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Default, null));
                PresentViewController(alert, true, null);
            }
            
           
        }

        public void GoOffline()
        {
            if(availabiltyListener != null)
            {
                avalability = false;
                availabiltyListener.RemoveListener();
                goOnlineButton.SetTitle("GO ONLINE", UIControlState.Normal);
                goOnlineButton.BackgroundColor = UIColor.FromRGB(0, 180, 255);
            }
        }

        public void GoOnline()
        {

            goOnlineButton.BackgroundColor = UIColor.FromRGB(7, 175, 18);
            goOnlineButton.SetTitle("GO OFFLINE", UIControlState.Normal);

            avalability = true;
            availabiltyListener = new AvailabilityListener();
            availabiltyListener.Create(currentLocation);
            availabiltyListener.RideTimedOut += AvailabiltyListener_RideTimedOut;
            availabiltyListener.RideCancelled += AvailabiltyListener_RideCancelled;
            availabiltyListener.RideDetailsFound += AvailabiltyListener_RideDetailsFound;

           
        }

        private void AvailabiltyListener_RideDetailsFound(object sender, AvailabilityListener.RideDetailsEventArgs e)
        {
            newRideDetails = e.RideDetails;

            alertNewRide = UIAlertController.Create("New Ride Request", $"New Trip to {e.RideDetails.DestinationAddress}", UIAlertControllerStyle.Alert);

            alertNewRide.AddAction(UIAlertAction.Create("Reject", UIAlertActionStyle.Cancel, null));

            UIAlertAction alertOk = UIAlertAction.Create("Accept", UIAlertActionStyle.Default, (args) => Accepted(e.RideDetails));

            alertNewRide.AddAction(alertOk);
            alertNewRide.PreferredAction = alertOk;
            PresentViewController(alertNewRide, true, null);

            UIApplicationState state = UIApplication.SharedApplication.ApplicationState;

            if(state == UIApplicationState.Active)
            {
                TriggerAlert();
            }
            else if(state == UIApplicationState.Background)
            {
                SendNotification(e.RideDetails);
            }
        }

        void Accepted(RideDetails rideDetails)
        {

            // stop playing alert
            if(player != null)
            {
                player.Stop();
                player = null;  
            }

            newtripListener = new NewTripEventListener(newRideDetails.RideId, currentLocation);
            newtripListener.Create(newRideDetails.RideId);

            TripReady();
        }


        public void SendNotification(RideDetails rideDetails)
        {
            UNMutableNotificationContent content = new UNMutableNotificationContent();
            content.Title = "New Trip Request";
            content.Body = $"Trip to {rideDetails.DestinationAddress}";
            content.Badge = 0;
            content.Sound = UNNotificationSound.GetSound("alertios.aiff");

            var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(1, false);
            var requestID = "notication";
            var request = UNNotificationRequest.FromIdentifier(requestID, content, trigger);

            UNUserNotificationCenter.Current.AddNotificationRequest(request, (error) =>
            {
                if(error != null)
                {
                    // do something here.
                }
            });
        }

        public void TriggerAlert()
        {
            player = AVAudioPlayer.FromUrl(NSUrl.FromFilename("Sounds/alert.mp3"));
            player.PrepareToPlay();
            player.Play();
        }

        async void TripReady()
        {

            ShowProgressBar("Finding direction ....");
            mapHelper = new MapFunctionHelper("AIzaSyAZQBaY-ugQuwCWr4NkD-bybK7urElvNyY", googleMap);

            var pickupLatLng = new CLLocationCoordinate2D(newRideDetails.PickupLat, newRideDetails.PickLng);
            string directionJson = await mapHelper.GetDirectionJsonAsync(currentLocation, pickupLatLng);

            if (!string.IsNullOrEmpty(directionJson))
            {
                mapHelper.DrawTripOnMap(directionJson);

                status = "ACCEPTED";

                HideProgressBar();
                centerMarker.Hidden = true;
                goOnlineButton.BackgroundColor = UIColor.FromRGB(227, 16, 48);
                goOnlineButton.SetTitle("ON TRIP", UIControlState.Normal);
                goOnlineButton.Enabled = false;

                riderNameText.Text = newRideDetails.RiderName;
                rideDetailsView.Hidden = false;

                DatabaseReference historyRef = Database.DefaultInstance.GetRootReference().GetChild("drivers/" + AppDataHelper.GetDriverID() + "/trips/" + newRideDetails.RideId);
                historyRef.SetValue((NSNumber)true);
            }
            
        }


        private void AvailabiltyListener_RideCancelled(object sender, EventArgs e)
        {
            var alert = UIAlertController.Create("Alert", "New Trip Request was cancelled", UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
            PresentViewController(alert, true, null);
        }

        private void AvailabiltyListener_RideTimedOut(object sender, EventArgs e)
        {
            var alert = UIAlertController.Create("Alert", "Unfortunately, New Trip Request TimedOut", UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
            PresentViewController(alert, true, null);
        }

        private void LocationManager_LocationsUpdated(object sender, CLLocationsUpdatedEventArgs e)
        {
            CameraPosition cp = CameraPosition.FromCamera(e.Locations[0].Coordinate, 15);
            currentLocation = e.Locations[0].Coordinate;

            if (string.IsNullOrEmpty(status))
            {
                googleMap.Animate(cp);
            }
           
            // Updates drivers current location as location changes while he waits for a request
            if(availabiltyListener != null)
            {
                if (avalability)
                {
                    availabiltyListener.UpdateLocation(currentLocation);
                }
            }

            if(status == "ACCEPTED")
            {
                //Update and Animate driver movement to pick up location
                var pickupLatLng = new CLLocationCoordinate2D(newRideDetails.PickupLat, newRideDetails.PickLng);
                mapHelper.UpdateMovement(currentLocation, pickupLatLng, "Rider");
                newtripListener.UpdateLocation(currentLocation);
            }
            else if (status == "ONTRIP")
            {
                var destinationLatLng = new CLLocationCoordinate2D(newRideDetails.DestinationLat, newRideDetails.DestinationLng);

                // Update and Animate driver movement to destination
                mapHelper.UpdateMovement(currentLocation, destinationLatLng, "destination");

                //Update Location on Firebase
                newtripListener.UpdateLocation(currentLocation);
            }

        }


        public void ShowProgressBar(string status)
        {
            progressStatusText.Text = status;
            overlay.Hidden = false;
            progressBar.Hidden = false;
        }

       public void HideProgressBar()
        {
            overlay.Hidden = true;
            progressBar.Hidden = true;
        }
    }
}
