// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace uberdriver
{
    [Register ("TripsViewController")]
    partial class TripsViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView exitButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView historyTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (exitButton != null) {
                exitButton.Dispose ();
                exitButton = null;
            }

            if (historyTableView != null) {
                historyTableView.Dispose ();
                historyTableView = null;
            }
        }
    }
}