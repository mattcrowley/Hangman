using Android.App;
using Android.Widget;
using Android.OS;

namespace HangmanXamarin
{
    [Activity(Label = "HangmanXamarin", MainLauncher = true, Icon = "@drawable/StickMan")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);
            Button btnStart1PlayerGame = FindViewById<Button>(Resource.Id.btnStart1PlayerGame);
            btnStart1PlayerGame.Click += Start1PlayerGame;
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutAll(outState);
            base.OnSaveInstanceState(outState);
        }

        public void Start1PlayerGame(object sender, System.EventArgs e)
        {
            // Start new activity to show the Hangman game/options page
            StartActivity(typeof(HangmanGame1Player));
        }
    }
}

