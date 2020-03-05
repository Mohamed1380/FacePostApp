package crc642c8480e2e4a99586;


public class PostAdapterViewHolder
	extends android.support.v7.widget.RecyclerView.ViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("FacePostApp.Adapters.PostAdapterViewHolder, FacePostApp", PostAdapterViewHolder.class, __md_methods);
	}


	public PostAdapterViewHolder (android.view.View p0)
	{
		super (p0);
		if (getClass () == PostAdapterViewHolder.class)
			mono.android.TypeManager.Activate ("FacePostApp.Adapters.PostAdapterViewHolder, FacePostApp", "Android.Views.View, Mono.Android", this, new java.lang.Object[] { p0 });
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
