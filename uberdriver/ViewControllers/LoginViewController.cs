using CoreGraphics;
using Firebase.Auth;
using Firebase.Database;
using Foundation;
using System;
using UIKit;

namespace uberdriver
{
    public partial class LoginViewController : UIViewController
    {
        public LoginViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //Keyboard Observers
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, keyWillChange);
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, keyWillChange);

            emailText.ShouldReturn = delegate {

                emailText.ResignFirstResponder();
                return true;
            };

            passwordText.ShouldReturn = delegate {

                passwordText.ResignFirstResponder();
                return true;

            };

            loginButton.TouchUpInside += LoginButton_TouchUpInside;

            clickToRegister.UserInteractionEnabled = true;
            clickToRegister.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                RegisterViewController registerController = this.Storyboard.InstantiateViewController("RegisterViewController") as RegisterViewController;
                registerController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(registerController, true, null);
                
            }));
        }

        private void LoginButton_TouchUpInside(object sender, EventArgs e)
        {
            string email, password;

            email = emailText.Text;
            password = passwordText.Text;


            // Verify Inputs
            if (!email.Contains("@"))
            {
                var alert = UIAlertController.Create("Alert", "Please provide a valid email", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
                PresentViewController(alert, true, null);

            }
            else if (password.Length < 8)
            {
                var alert = UIAlertController.Create("Alert", "Please provide a valid password", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
                PresentViewController(alert, true, null);
            }

            ShowProgressBar("Logging you in...");
            Auth.DefaultInstance.SignInWithPassword(email, password, (AuthDataResult authDataResult, NSError error) =>
            {
                if(error == null)
                {
                    if(authDataResult.User.Uid != null)
                    {
                        string id = authDataResult.User.Uid;

                        // Login successful
                        FetchDriverInfo(id);
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


        void FetchDriverInfo(string id)
        {
            DatabaseReference dbref = Database.DefaultInstance.GetRootReference().GetChild("drivers/" + id);

            dbref.ObserveSingleEvent(DataEventType.Value, (DataSnapshot snapshot) =>
            {
                if(snapshot.GetValue<NSObject>() != NSNull.Null)
                {
                    string email, fullname, phone;

                    if(snapshot.GetChildSnapshot("email").GetValue<NSObject>() != NSNull.Null)
                    {
                        email = snapshot.GetChildSnapshot("email").GetValue<NSObject>().ToString();
                        fullname = snapshot.GetChildSnapshot("fullname").GetValue<NSObject>().ToString();
                        phone = snapshot.GetChildSnapshot("phone").GetValue<NSObject>().ToString();

                        //Save driver details to NSUser Defaults
                        var userdefaults = NSUserDefaults.StandardUserDefaults;
                        userdefaults.SetString(phone, "phone");
                        userdefaults.SetString(email, "email");
                        userdefaults.SetString(fullname, "fullname");
                        userdefaults.SetString(id, "driver_id");

                        HideProgressBar();

                        MainTabController tabController = this.Storyboard.InstantiateViewController("MainTabController") as MainTabController;
                        tabController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                        PresentViewController(tabController, true, null);
                        
                     
                    }
                    
                }
                else
                {
                    HideProgressBar();
                    /// dont proceed.
                     var alert = UIAlertController.Create("Alert", "Login was not successful", UIAlertControllerStyle.Alert);
                    alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
                    PresentViewController(alert, true, null);
                }
                
               
            });
        }

        void ShowProgressBar(string status)
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


    }
}