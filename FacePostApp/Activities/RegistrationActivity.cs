using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using FacePostApp.EventListeners;
using FacePostApp.Fragments;
using FacePostApp.Helpers;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Java.Util;

namespace FacePostApp.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class RegistrationActivity : AppCompatActivity
    {
        FirebaseFirestore database;
        FirebaseAuth mAuth;
        Button registerButton;
        TextInputLayout fullnameText, emailText, passwordText, confirmPasswordText;
        string fullname, email, password, confirm;
        TaskCompletionListeners taskCompletionListeners = new TaskCompletionListeners();
        ProgressDialogueFragment progressDialogue;
        TextView clickHereToLogin;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.register);

            fullnameText = (TextInputLayout)FindViewById(Resource.Id.fullNameRegText);
            emailText = (TextInputLayout)FindViewById(Resource.Id.emailRegText);
            passwordText = (TextInputLayout)FindViewById(Resource.Id.passwordRegText);
            confirmPasswordText = (TextInputLayout)FindViewById(Resource.Id.confirmpasswordRegText);
            clickHereToLogin = (TextView)FindViewById(Resource.Id.clickToLogin);
            clickHereToLogin.Click += ClickHereToLogin_Click;

            // Create your application here
            registerButton = (Button)FindViewById(Resource.Id.RegisterButton);
            registerButton.Click += RegisterButton_Click;
            database = AppDataHelper.GetFirestore();
            mAuth = AppDataHelper.GetFirebaseAuth();
        }

        private void ClickHereToLogin_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(LoginActivity));
            Finish();
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            /*
            HashMap userMap = new HashMap();
            userMap.Put("email", "mabdalhady138@gmail.com");
            userMap.Put("fullname", "Mohamed abdalhady");
            DocumentReference userReference = database.Collection("users").Document();
            userReference.Set(userMap);
            */
            fullname = fullnameText.EditText.Text;
            email = emailText.EditText.Text;
            password = passwordText.EditText.Text;
            confirm = confirmPasswordText.EditText.Text;

            if (fullname.Length < 4 )
            {
                Toast.MakeText(this, "Please enter a valid name", ToastLength.Short).Show();
                return;
            }
            else if (!email.Contains("@"))
            {
                Toast.MakeText(this, "Please enter a valid email address", ToastLength.Short).Show();
                return;
            }
            else if (password.Length < 8)
            {
                Toast.MakeText(this, "Please enter a password upto 8 characters", ToastLength.Short).Show();
                return;
            }
            else if (password != confirm)
            {
                Toast.MakeText(this, "Password does not match, Please make correction", ToastLength.Short).Show();
                return;
            }

            // progress par 
            ShowProgressDialogue("Registering you ...");

            mAuth.CreateUserWithEmailAndPassword(email, password).AddOnSuccessListener(this, taskCompletionListeners)
                .AddOnFailureListener(this, taskCompletionListeners);

            // Registeration Success Callback Func
            taskCompletionListeners.Success += (success, args) =>
             {
                 HashMap userMap = new HashMap();
                 userMap.Put("email", email);
                 userMap.Put("fullname", fullname);

                 DocumentReference userReference = database.Collection("users").Document(mAuth.CurrentUser.Uid);
                 userReference.Set(userMap);

                 CloseProgressDialogue();
                 StartActivity(typeof(MainActivity));
                 Finish();
             };

            // Registeration Failure Callback Func 
            taskCompletionListeners.Failure += (failure, args) =>
             {
                 CloseProgressDialogue();
                 Toast.MakeText(this, "Registeration Failed: " + args.cause, ToastLength.Short).Show();
             };
        }

        void ShowProgressDialogue(string status)
        {
            progressDialogue = new ProgressDialogueFragment(status);
            var trans = SupportFragmentManager.BeginTransaction();
            progressDialogue.Cancelable = false;
            progressDialogue.Show(trans, "Progress");
        }

        void CloseProgressDialogue()
        {
            if (progressDialogue != null)
            {
                progressDialogue.Dismiss();
                progressDialogue = null;
            }
        }


    }
}