using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using Android.Support.V7.Widget;
using FacePostApp.Adapters;
using System.Collections.Generic;
using FacePostApp.DataModels;
using FacePostApp.Activities;
using FacePostApp.EventListeners;
using System.Linq;
using FacePostApp.Helpers;
using Firebase.Storage;
using FacePostApp.Fragments;

namespace FacePostApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        Android.Support.V7.Widget.Toolbar toolbar;
        RecyclerView postRecyclerView;
        PostAdapter postAdapter;
        List<Post> ListOfPost;

        RelativeLayout layStatus;
        ImageView camera;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            toolbar = (Android.Support.V7.Widget.Toolbar)FindViewById(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            // recycler of view of post decalration
            postRecyclerView = (RecyclerView)FindViewById(Resource.Id.postRecycleView);

            layStatus = (RelativeLayout)FindViewById(Resource.Id.layStatus);
            layStatus.Click += LayStatus_Click;
            camera = (ImageView)FindViewById(Resource.Id.camera);
            camera.Click += LayStatus_Click;

            // Retereive fullname login

            FullnameListener fullnameListener = new FullnameListener();
            fullnameListener.FetchUser();

            //CreateData();
            FetchPost();
        }

        private void LayStatus_Click(object sender, System.EventArgs e)
        {
            StartActivity(typeof(CreatePostActivity));
        }

        void FetchPost()
        {
            PostEventListener postEventListener = new PostEventListener();
            postEventListener.FetchPost();
            postEventListener.OnPostReterive += PostEventListener_OnPostReterive;
        }

        private void PostEventListener_OnPostReterive(object sender, PostEventListener.PostEventArgs e)
        {
            ListOfPost = new List<Post>();
            ListOfPost = e.Posts;

            if (ListOfPost != null)
            {
                ListOfPost = ListOfPost.OrderByDescending(x => x.PostDate).ToList();
            }
            SetupRecyclerView();
        }

        void CreateData()
        {
            ListOfPost = new List<Post>();
            ListOfPost.Add(new Post { PostBody = "mohamed mohamed mohamed mohamed v mohamed mohamed mohamed v mohamed mohamed  gkcekkuwef kuwqfgkudgqw wgkuqggkudqwdhvdwhhd ggdqwgkdwq uukgqwgd gkfqgwkugdwkjdwku", Author = "Muhammad Yasser", LikeCount = 32 });
            ListOfPost.Add(new Post { PostBody = "mohamed mohamed mohamed mohamed v mohamed mohamed mohamed v mohamed mohamed  gkcekkuwef kuwqfgkudgqw wgkuqggkudqwdhvdwhhd ggdqwgkdwq uukgqwgd gkfqgwkugdwkjdwku ", Author = "Yousef Yasser", LikeCount = 32 });
            ListOfPost.Add(new Post { PostBody = "mohamed mohamed mohamed mohamed v mohamed mohamed mohamed v mohamed mohamed  gkcekkuwef kuwqfgkudgqw wgkuqggkudqwdhvdwhhd ggdqwgkdwq uukgqwgd gkfqgwkugdwkjdwku ", Author = "Adel Yasser", LikeCount = 32 });
            ListOfPost.Add(new Post { PostBody = "mohamed mohamed mohamed mohamed v mohamed mohamed mohamed v mohamed mohamed  gkcekkuwef kuwqfgkudgqw wgkuqggkudqwdhvdwhhd ggdqwgkdwq uukgqwgd gkfqgwkugdwkjdwku ", Author = "mahmoud Yasser", LikeCount = 32 });
        }
        void SetupRecyclerView()
        {
            postRecyclerView.SetLayoutManager(new Android.Support.V7.Widget.LinearLayoutManager(postRecyclerView.Context));
            postAdapter = new PostAdapter(ListOfPost);
            postRecyclerView.SetAdapter(postAdapter);
            postAdapter.ItemLongClick += PostAdapter_ItemLongClick;
        }

        private void PostAdapter_ItemLongClick(object sender, PostAdapterClickEventArgs e)
        {
            string postID = ListOfPost[e.Position].ID;
            string ownerID = ListOfPost[e.Position].OwnerId;

            if (AppDataHelper.GetFirebaseAuth().CurrentUser.Uid == ownerID )
            {
                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                alert.SetTitle("Edit or Delete Post");
                alert.SetMessage("Are you sure");

                alert.SetNegativeButton("Edit Post", (o, args) =>
                {
                    EditPostFragment editPostFragment = new EditPostFragment(ListOfPost[e.Position]);
                    var trans = SupportFragmentManager.BeginTransaction();
                    editPostFragment.Show(trans, "edit");

                });

                alert.SetPositiveButton("Delete", (o, args) =>
                {
                    AppDataHelper.GetFirestore().Collection("posts").Document(postID).Delete();

                    StorageReference storageReference = FirebaseStorage.Instance.GetReference("postImages/" + postID);
                    storageReference.Delete();
                });
                alert.Show();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.feed_menu , menu);
            return true;
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Resource.Id.action_logout)
            {
                Toast.MakeText(this, "Click on logout", ToastLength.Short).Show();
            }
            if (id == Resource.Id.action_refresh)
            {
                Toast.MakeText(this, "Click on Refresh", ToastLength.Short).Show();
            }
            return base.OnOptionsItemSelected(item);
        }

    }
}