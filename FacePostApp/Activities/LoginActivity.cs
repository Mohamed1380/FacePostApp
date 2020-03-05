﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using FacePostApp.EventListeners;
using FacePostApp.Fragments;
using FacePostApp.Helpers;
using Firebase.Auth;

namespace FacePostApp.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class LoginActivity : AppCompatActivity
    {
        TextInputLayout emailText, passwordText;
        Button loginButton;
        FirebaseAuth mAuth;
        TaskCompletionListeners taskCompletionListeners = new TaskCompletionListeners();
        ProgressDialogueFragment progressDialogue;
        TextView clickToRegister;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.login);

            // Create your application here
            emailText = (TextInputLayout)FindViewById(Resource.Id.emailLoginText);
            passwordText = (TextInputLayout)FindViewById(Resource.Id.passwordLoginText);
            loginButton = (Button)FindViewById(Resource.Id.loginButton);
            clickToRegister = (TextView)FindViewById(Resource.Id.clickToRegister);
            clickToRegister.Click += ClickToRegister_Click;

            loginButton.Click += LoginButton_Click;
            mAuth = AppDataHelper.GetFirebaseAuth();
        }

        private void ClickToRegister_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(RegistrationActivity));
            Finish();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            string email, password;
            email = emailText.EditText.Text;
            password = passwordText.EditText.Text;

            if (!email.Contains("@"))
            {
                Toast.MakeText(this, "Please provide a valid email address", ToastLength.Short).Show();
                return;
            }
            else if (password.Length <8)
            {
                Toast.MakeText(this, "Please provide a valid password", ToastLength.Short).Show();
                return;
            }
            // progress par before authentaction

            ShowProgressDialogue("Verifying you ...");

            mAuth.SignInWithEmailAndPassword(email, password).AddOnSuccessListener(taskCompletionListeners)
                .AddOnFailureListener(taskCompletionListeners);

            taskCompletionListeners.Success += (success, args) =>
            {
                CloseProgressDialogue();
                StartActivity(typeof(MainActivity));
                Toast.MakeText(this, "Login Success", ToastLength.Short).Show();
            };

            taskCompletionListeners.Failure += (failure, args) =>
            {
                CloseProgressDialogue();
                Toast.MakeText(this, "Login Faild: "+args.cause, ToastLength.Short).Show();
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