using Firebase.Database;
using Foundation;
using System;
using System.Collections.Generic;
using uberrider.Helpers;
using UIKit;

namespace uberdriver
{
    public partial class EarningsviewController : UIViewController
    {

        List<string> KeyList;

        public EarningsviewController (IntPtr handle) : base (handle)
        {
        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            tripsView.Layer.BorderWidth = 1;
            tripsView.Layer.BorderColor = UIColor.FromRGB(226, 226, 226).CGColor;

            tripsView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                TripsViewController tripsViewCont = this.Storyboard.InstantiateViewController("TripsViewController") as TripsViewController;
                tripsViewCont.tripKeys = KeyList;

                tripsViewCont.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(tripsViewCont, true, null);

            }));

            GetEarnings();
            GetTripCount();
        }

        void GetEarnings()
        {
            DatabaseReference earningsRef = Database.DefaultInstance.GetRootReference().GetChild("drivers/" + AppDataHelper.GetDriverID() + "/earnings");
            earningsRef.ObserveEvent(DataEventType.Value, (DataSnapshot snapshot) =>
            {
                if(snapshot.GetValue<NSObject>() != NSNull.Null)
                {
                    string value = snapshot.GetValue<NSObject>().ToString();
                    tripsEarningsText.Text = "$" + value;
                }
            });
        }

        void GetTripCount()
        {
            DatabaseReference tripsRef = Database.DefaultInstance.GetRootReference().GetChild("drivers/" + AppDataHelper.GetDriverID() + "/trips");
            tripsRef.ObserveEvent(DataEventType.Value, (DataSnapshot snapshot) =>
            {
                if(snapshot.GetValue<NSObject>() != NSNull.Null)
                {
                    var snapShotData = snapshot.GetValue<NSDictionary>();
                    string keycount = snapshot.ChildrenCount.ToString();
                    tripCountText.Text = keycount;

                    KeyList = new List<string>();
                    foreach (NSString key in snapShotData.Keys)
                    {
                        KeyList.Add(key);
                    }

                }
            });
        }
    }
}