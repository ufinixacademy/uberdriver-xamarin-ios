using CoreGraphics;
using Firebase.Auth;
using Firebase.Database;
using Foundation;
using System;
using UIKit;

namespace uberdriver
{
    public partial class RegisterViewController : UIViewController
    {
        public RegisterViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            emailText.ShouldReturn = delegate {

                emailText.ResignFirstResponder();
                return true;
            };

            phoneText.ShouldReturn = delegate {

                phoneText.ResignFirstResponder();
                return true;
            };

            fullnameText.ShouldReturn = delegate {

                fullnameText.ResignFirstResponder();
                return true;
            };

            passwordText.ShouldReturn = delegate {

                passwordText.ResignFirstResponder();
                return true;
            };

            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, keyWillChange);
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, keyWillChange);

            registerButton.TouchUpInside += RegisterButton_TouchUpInside;
            clickToLogin.UserInteractionEnabled = true;
            clickToLogin.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                LoginViewController loginViewController = this.Storyboard.InstantiateViewController("LoginViewController") as LoginViewController;
                loginViewController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(loginViewController, true, null);
            }));
        }

        void keyWillChange(NSNotification notification)
        {
            if (notification.Name == UIKeyboard.WillShowNotification)
            {
                var keyboard = UIKeyboard.FrameBeginFromNotification(notification);

                CGRect frame = View.Frame;
                frame.Y = -keyboard.Height;
                View.Frame = frame;

            }

            if (notification.Name == UIKeyboard.WillHideNotification)
            {
                CGRect frame = View.Frame;
                frame.Y = 0;
                View.Frame = frame;
            }
        }


        private void RegisterButton_TouchUpInside(object sender, EventArgs e)
        {
            string fullname, phone, email, password;

            fullname = fullnameText.Text;
            phone = phoneText.Text;
            email = emailText.Text;
            password = passwordText.Text;

            if(fullname.Length < 4)
            {
                var alert = UIAlertController.Create("Alert", "Please enter a valid name", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
                PresentViewController(alert, true, null);

                return;
            }
            else if (phone.Length < 9)
            {
                var alert = UIAlertController.Create("Alert", "Please enter a valid phone number", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
                PresentViewController(alert, true, null);

                return;
            }
            else if (!email.Contains("@"))
            {
                var alert = UIAlertController.Create("Alert", "Please enter a valid email", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
                PresentViewController(alert, true, null);

                return;
            }
            else if (password.Length < 8)
            {
                var alert = UIAlertController.Create("Alert", "Please enter a password upto 8 characters", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
                PresentViewController(alert, true, null);

                return;
            }


            ShowProgressBar("Registering you ...");
            Auth.DefaultInstance.CreateUser(email, password, (AuthDataResult authDataResult, NSError error) =>
            {
                if(error == null)
                {
                    var driver = authDataResult.User.Uid;

                    if(driver != null)
                    {
                        var driverDictionary = new NSDictionary
                        (
                            "fullname", fullname,
                            "email", email,
                            "phone", phone
                         );

                        //save driver details to Firebase Database
                        DatabaseReference driverRef = Database.DefaultInstance.GetRootReference().GetChild("drivers/" + authDataResult.User.Uid);
                        driverRef.SetValue<NSDictionary>(driverDictionary);

                        //Save driver details to NSUser Defaults
                        var userdefaults = NSUserDefaults.StandardUserDefaults;
                        userdefaults.SetString(phone, "phone");
                        userdefaults.SetString(email, "email");
                        userdefaults.SetString(fullname, "fullname");
                        userdefaults.SetString(authDataResult.User.Uid, "driver_id");

                        HideProgressBar();

                        MainTabController tabController = this.Storyboard.InstantiateViewController("MainTabController") as MainTabController;
                        tabController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                        PresentViewController(tabController, true, null);


                    }
                }
                else
                {
                    HideProgressBar();
                    var alert = UIAlertController.Create("Error", error.LocalizedDescription, UIAlertControllerStyle.Alert);
                    alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
                    PresentViewController(alert, true, null);

                }
            });

        }

        void ShowProgressBar( string status)
        {
            progressStatusText.Text = status;
            overlay.Hidden = false;
            progressBar.Hidden = false;
        }

        void HideProgressBar()
        {
            overlay.Hidden = true;
            progressBar.Hidden = true;
        }
    }
}
