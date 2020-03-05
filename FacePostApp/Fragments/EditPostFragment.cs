using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using FacePostApp.DataModels;
using FacePostApp.Helpers;
using FFImageLoading;
using Firebase.Firestore;

namespace FacePostApp.Fragments
{
    public class EditPostFragment : Android.Support.V4.App.DialogFragment
    {
        Post thispost;
        EditText postEditText;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public EditPostFragment(Post post)
        {
            thispost = post;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view= inflater.Inflate(Resource.Layout.editpost, container, false);

            Button editButton = (Button)view.FindViewById(Resource.Id.editButton);
            ImageView postImageView = (ImageView)view.FindViewById(Resource.Id.postImageView);
            postEditText = (EditText)view.FindViewById(Resource.Id.postEditText);
            postEditText.Text = thispost.PostBody;
            GetImage(thispost.DownloadUrl, postImageView);
            editButton.Click += EditButton_Click;

            return view;
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            DocumentReference reference = AppDataHelper.GetFirestore().Collection("posts").Document(thispost.ID);
            reference.Update("post_body", postEditText.Text);
            this.Dismiss();
        }

        void GetImage(string url, ImageView imageView)
        {
            ImageService.Instance.LoadUrl(url)
                .Retry(3, 200)
                .DownSample(400, 400)
                .Into(imageView);
        }
    }
}