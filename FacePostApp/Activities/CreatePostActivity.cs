using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using FacePostApp.EventListeners;
using FacePostApp.Fragments;
using FacePostApp.Helpers;
using Firebase.Firestore;
using Firebase.Storage;
using Java.Util;
using Plugin.Media;
using static Android.Renderscripts.Element;

namespace FacePostApp.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class CreatePostActivity : AppCompatActivity
    {
        Android.Support.V7.Widget.Toolbar toolbar;
        ImageView postImage;
        Button submitButton;
        EditText postEditText;

        readonly string[] permessionGroup = 
        {
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage,
            Manifest.Permission.Camera
        };

        byte[] fileBytes;

        TaskCompletionListeners taskCompletionListeners = new TaskCompletionListeners();
        TaskCompletionListeners downloadUrlListener = new TaskCompletionListeners();

        StorageReference storageReference;
        ProgressDialogueFragment progressDialogue;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.create_post);
            // Create your application here
            toolbar = (Android.Support.V7.Widget.Toolbar)FindViewById(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Create Post";

            Android.Support.V7.App.ActionBar actionBar = SupportActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(true);
            actionBar.SetHomeAsUpIndicator(Resource.Drawable.outline_arrowback);
            postEditText = (EditText)FindViewById(Resource.Id.postText);

            postImage = (ImageView)FindViewById(Resource.Id.newPostImage);
            postImage.Click += PostImage_Click;

            submitButton = (Button)FindViewById(Resource.Id.submitButton);
            submitButton.Click += SubmitButton_Click;

            RequestPermissions(permessionGroup, 0);

        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            HashMap postMap = new HashMap();
            postMap.Put("author", AppDataHelper.GetFullName());
            postMap.Put("owner_id", AppDataHelper.GetFirebaseAuth().CurrentUser.Uid);
            postMap.Put("post_date", DateTime.Now.ToString());
            postMap.Put("post_body", postEditText.Text);

            DocumentReference newPostRef = AppDataHelper.GetFirestore().Collection("posts").Document();
            string postKey = newPostRef.Id;

            postMap.Put("image_id", postKey);

            ShowProgressDialogue("Saving Information ...");


            // Save Post Image to firbase storage 
            StorageReference storageReference = null;
            if (fileBytes != null)
            {
                storageReference = FirebaseStorage.Instance.GetReference("postImages/"+ postKey);
                storageReference.PutBytes(fileBytes)
                    .AddOnSuccessListener(taskCompletionListeners)
                    .AddOnFailureListener(taskCompletionListeners);
            }
            taskCompletionListeners.Success += (obj, args) =>
            {
                //Toast.MakeText(this, "Image was Uploaded successfully", ToastLength.Short).Show();
                if (storageReference != null)
                {
                    storageReference.DownloadUrl.AddOnSuccessListener(downloadUrlListener);
                }
            };

            // Image Download URL Callback
            downloadUrlListener.Success += (obj, args) =>
            {
                string downloadUrl = args.Result.ToString();
                postMap.Put("download_url", downloadUrl);

                // save post to firebase firestore
                newPostRef.Set(postMap);

                CloseProgressDialogue();
                Finish();
            };
            taskCompletionListeners.Failure += (obj, args) =>
            {
                Toast.MakeText(this, "Upload was not completed", ToastLength.Short).Show();
            };
        }

        private void PostImage_Click(object sender, EventArgs e)
        {
            Android.Support.V7.App.AlertDialog.Builder photoAlert = new Android.Support.V7.App.AlertDialog.Builder(this);
            photoAlert.SetMessage("Change Photo");
            photoAlert.SetNegativeButton("Take Photo", (thisalert, args) =>
            {
                // Capture the image FrOM Camera
                TakePhoto();
            });
            photoAlert.SetPositiveButton("Upload Photo", (a, b) =>
            {
                // select photo from Gallary
                SelectPhoto();
            });
            photoAlert.Show();
        }
        async void TakePhoto()
        {
            await CrossMedia.Current.Initialize();
            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                CompressionQuality = 20,
                Directory ="Sample",
                Name = GenerateRandomString(6) + "facepost.jpg"

            }); 

            if ( file == null)
            {
                return;
            }

            byte[] imageArray = System.IO.File.ReadAllBytes(file.Path);
            fileBytes = imageArray;
            Bitmap bitmap = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
            postImage.SetImageBitmap(bitmap);
        }

        async void SelectPhoto()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                Toast.MakeText(this, "Upload not supported", ToastLength.Short).Show();
                return;
            }

            var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                CompressionQuality=30
            });

            if (file ==null)
            {
                return;
            }
            byte[] imageArray = System.IO.File.ReadAllBytes(file.Path);
            fileBytes = imageArray;
            Bitmap bitmap = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
            postImage.SetImageBitmap(bitmap);
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
        public string GenerateRandomString(int lenght)
        {
            System.Random rand = new System.Random();
            char[] allchars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
            string sResult = "";
            for (int i=0; i<lenght;i++)
            {
                sResult += allchars[rand.Next(0, allchars.Length)];
            }
            return sResult;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }
}