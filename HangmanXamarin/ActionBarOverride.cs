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

namespace HangmanXamarin
{
    public class ActionBarOverride : Activity
    {
        // This function overrides the passed in ActionBar with a image button and a custom title
        public void CreateActionBarWithTitleAndMenuButton(ref ActionBar myCurActionBar, Context from, string title)
        {
            LayoutInflater mInflater = LayoutInflater.From(from);

            View mCustomView = mInflater.Inflate(Resource.Layout.toolbar, null);
            TextView aTitle = mCustomView.FindViewById<TextView>(Resource.Id.toolbar_TitleText);
            aTitle.Text = title;

            myCurActionBar.SetCustomView(mCustomView, new ActionBar.LayoutParams(WindowManagerLayoutParams.MatchParent, WindowManagerLayoutParams.MatchParent));

            myCurActionBar.SetDisplayShowCustomEnabled(true);
            myCurActionBar.SetDisplayShowTitleEnabled(false);

            myCurActionBar.Title = title;
        }
    }
}