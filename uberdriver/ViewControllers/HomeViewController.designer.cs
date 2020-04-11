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
    [Register ("HomeViewController")]
    partial class HomeViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView callButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView cancelButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView centerMarker { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton collectPayementButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView colletPaymentView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel faresAmountText { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Google.Maps.MapView googleMap { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton goOnlineButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView overlay { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView progressBar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel progressStatusText { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView rideDetailsView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel riderNameText { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton tripButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (callButton != null) {
                callButton.Dispose ();
                callButton = null;
            }

            if (cancelButton != null) {
                cancelButton.Dispose ();
                cancelButton = null;
            }

            if (centerMarker != null) {
                centerMarker.Dispose ();
                centerMarker = null;
            }

            if (collectPayementButton != null) {
                collectPayementButton.Dispose ();
                collectPayementButton = null;
            }

            if (colletPaymentView != null) {
                colletPaymentView.Dispose ();
                colletPaymentView = null;
            }

            if (faresAmountText != null) {
                faresAmountText.Dispose ();
                faresAmountText = null;
            }

            if (googleMap != null) {
                googleMap.Dispose ();
                googleMap = null;
            }

            if (goOnlineButton != null) {
                goOnlineButton.Dispose ();
                goOnlineButton = null;
            }

            if (overlay != null) {
                overlay.Dispose ();
                overlay = null;
            }

            if (progressBar != null) {
                progressBar.Dispose ();
                progressBar = null;
            }

            if (progressStatusText != null) {
                progressStatusText.Dispose ();
                progressStatusText = null;
            }

            if (rideDetailsView != null) {
                rideDetailsView.Dispose ();
                rideDetailsView = null;
            }

            if (riderNameText != null) {
                riderNameText.Dispose ();
                riderNameText = null;
            }

            if (tripButton != null) {
                tripButton.Dispose ();
                tripButton = null;
            }
        }
    }
}