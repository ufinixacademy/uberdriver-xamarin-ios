using Foundation;
using System;
using uberdriver.Datamodels;
using UIKit;

namespace uberdriver
{
    public partial class TripCell : UITableViewCell
    {
        public TripCell (IntPtr handle) : base (handle)
        {
        }

        internal void UpdateCell(RideHistory history)
        {
            pickupText.Text = history.PickupAddress;
            destinationText.Text = history.DestinationAddress;
            dateText.Text = history.Created_At.ToString("hh:mm tt, dddd, dd mmmm, yyyy");
            faresAmountText.Text = "$" + history.Fares.ToString();

            if(history.Status == "ended")
            {
                statusText.Text = "Completed";
            }
        }
    }
}