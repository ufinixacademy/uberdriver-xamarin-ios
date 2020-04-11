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
    [Register ("EarningsviewController")]
    partial class EarningsviewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel tripCountText { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel tripsEarningsText { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView tripsView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (tripCountText != null) {
                tripCountText.Dispose ();
                tripCountText = null;
            }

            if (tripsEarningsText != null) {
                tripsEarningsText.Dispose ();
                tripsEarningsText = null;
            }

            if (tripsView != null) {
                tripsView.Dispose ();
                tripsView = null;
            }
        }
    }
}