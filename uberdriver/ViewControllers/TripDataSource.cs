using System;
using System.Collections.Generic;
using Foundation;
using uberdriver.Datamodels;
using UIKit;

namespace uberdriver
{
    internal class TripDataSource : UITableViewSource
    {
        private List<RideHistory> rideHistories;

        public TripDataSource(List<RideHistory> rideHistories)
        {
            this.rideHistories = rideHistories;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (TripCell)tableView.DequeueReusableCell("tripCell", indexPath);

            var history = rideHistories[indexPath.Row];

            cell.UpdateCell(history);

            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return rideHistories.Count;
        }
    }
}