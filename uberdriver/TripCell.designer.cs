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
    [Register ("TripCell")]
    partial class TripCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel dateText { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel destinationText { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel faresAmountText { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel pickupText { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel statusText { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (dateText != null) {
                dateText.Dispose ();
                dateText = null;
            }

            if (destinationText != null) {
                destinationText.Dispose ();
                destinationText = null;
            }

            if (faresAmountText != null) {
                faresAmountText.Dispose ();
                faresAmountText = null;
            }

            if (pickupText != null) {
                pickupText.Dispose ();
                pickupText = null;
            }

            if (statusText != null) {
                statusText.Dispose ();
                statusText = null;
            }
        }
    }
}