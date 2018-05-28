using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using DictionaryLibrary;
using Android.Widget;
using static Android.Views.View;
using Android.Graphics.Drawables;

namespace HangmanXamarin
{
    [Activity(Label = "HangmanGame1Player", WindowSoftInputMode = SoftInput.StateAlwaysVisible)]
    public class HangmanGame1Player : Activity, IOnKeyListener
    {
        private Dictionary _curDictionary;
        private string _curWord;
        private List<EditText> _characters; // Each EditText contains a letter or empty space
        private List<char> _charactersGuessed; // which characters have been guessed so far (correct and incorrect)

        private TextView _lettersToShowOnUI; // shows incorrect letter(s) guessed by user
        private TransitionDrawable _drawable; // animated img to handle the hangman image
        private Toast _userInputToast;

        private int _numGuesses = 0;
        private TextView _numGuessesTextView;
        private int _totalGuessesAvail;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.HangmanGame1PlayerLayout);

            _lettersToShowOnUI = FindViewById<TextView>(Resource.Id.guessedLetters);

            // get the image view
            ImageView button = FindViewById<ImageView>(Resource.Id.imageView1);
            _drawable = (TransitionDrawable)button.Drawable;

            _numGuessesTextView = FindViewById<TextView>(Resource.Id.textNumGuessesLeft);

            LinearLayout onePlayerBackground = FindViewById<LinearLayout>(Resource.Id.llPlayer1Game);
            onePlayerBackground.Click += ReshowKeyboardIfHidden;
            onePlayerBackground.SetOnKeyListener(this);

            if (savedInstanceState == null)
            {
                StartNewGame();
            }

            /* // TODO: Create a toolbar to use to switch languages, get a new word, etc 
            //SetActionBar(new Toolbar())
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "My Toolbar";
            // InputMethodManager inputManager = (InputMethodManager)GetSystemService(Context.InputMethodService);
            //inputManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
            */
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutAll(outState);
            base.OnSaveInstanceState(outState);
        }
        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
        }

        /// <summary>
        /// If the keyboard is hidden for some reason, we need a way to reshow it, or
        /// else the user will be unable to continue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReshowKeyboardIfHidden(object sender, EventArgs e)
        {
            InputMethodManager inputManager = (InputMethodManager)GetSystemService(Context.InputMethodService);
            inputManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pView"></param>
        public void ShowKeyboard(View pView)
        {
            pView.RequestFocus();

            InputMethodManager inputMethodManager = (InputMethodManager)GetSystemService(Context.InputMethodService);
            inputMethodManager.ShowSoftInput(pView, ShowFlags.Forced);
            inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
        }


        /// <summary>
        /// Simple toast to give user info on what the result of their guess was. 
        /// TODO: to be either removed or some other functionality to show user, as this is very
        /// laggy when the user chooses multiple letters at once.
        /// </summary>
        /// <param name="message"></param>
        private void CreateToastCenterScreen(string message)
        {
            // create timer todo to remove text from screen

            _userInputToast = Toast.MakeText(this, message, ToastLength.Short);
            _userInputToast.SetGravity(GravityFlags.Center, 0, 0);
            _userInputToast.Show();
        }

        /// <summary>
        /// This method is called when we have detected a valid character being entered.
        /// Currently only supports english characters.
        /// </summary>
        /// <param name="character"></param>
        public void GuessCharacterInWord(string character)
        {
            if (character.Length == 1)
            {
                char c = character[0];
                c = char.ToUpper(c);

                // Check to see if guessedCharacter was already chosen
                if (_charactersGuessed.Contains(c))
                {
                    // Toast to user about duplicated attempt
                    CreateToastCenterScreen("You already entered this letter");
                }
                else
                {
                    // Add guessed character to list
                    AddGuessedCharacterToList(c);

                    if (_curWord.Contains(c))
                    {
                        int numLettersMatching = 0;

                        // loop through the list of text views, setting the text for each matching letter
                        // so we can show the characters matching the entered character
                        for (int indx = 0; indx < _curWord.Length; indx++)
                        {
                            if (_curWord[indx] == c)
                            {
                                _characters[indx].Text = c.ToString();
                                numLettersMatching++;
                            }
                        }

                        // TODO: fix this, horribly inefficient
                        // check if we have guessed all characters:
                        bool guessedAllChars = true;
                        for (int i = 0; i < _characters.Count; i++)
                        {
                            if (_characters[i].Text == string.Empty)
                                guessedAllChars = false;
                        }

                        if (guessedAllChars)
                            CreateMessageBox("Game WON!", "Would you like to start a new game?", "Yes", "No");
                        else
                            CreateToastCenterScreen(string.Format("Number of characters found was {0}!", numLettersMatching));
                    }
                    else
                    {
                        _numGuesses++;
                        int guessesLeft = _totalGuessesAvail - _numGuesses;
                        _numGuessesTextView.Text = guessesLeft.ToString();

                        // not found
                        CreateToastCenterScreen(string.Format("Character not found in string! {0} attempts left!", guessesLeft));

                        // Draw the next shape to show invalid answers
                        _drawable.StartTransition(500);

                        if (guessesLeft == 0)
                        {
                            // GAME OVER
                            CreateToastCenterScreen("Game Over!");

                            // Show messagebox/finish word on screen, add reload/new game option
                            CompleteWordTextFields();

                            CreateMessageBox("Game Over", "Would you like to start a new game?", "Yes", "No");
                        }
                    }
                }
            }
            else
            {
                // TODO: error handling, this should never happen, as the user should not be able to enter 
                // multiple characters at once.
                Console.WriteLine("character length != 1!");
                Console.WriteLine("character: " + character);
            }
        }

        /// <summary>
        /// This function adds the passed in character to the list of characters guesses.
        /// There will be no duplicate letters in this list.
        /// </summary>
        /// <param name="c"></param>
        private void AddGuessedCharacterToList(char c)
        {
            _charactersGuessed.Add(c);

            // Add this entry to the UI list
            if (_lettersToShowOnUI != null)
            {
                if (!string.IsNullOrEmpty(_lettersToShowOnUI.Text))
                    _lettersToShowOnUI.Text += "," + c;
                else
                    _lettersToShowOnUI.Text += c;
            }
        }

        /// <summary>
        /// Method to create an N editText fields for the unguessed word.
        /// This allows the user to see empty _ where letters should be
        /// </summary>
        public void GenerateWordTextFields()
        {
            LinearLayout layout = FindViewById<LinearLayout>(Resource.Id.llWordToGuess);

            for (int curLetterIndex = 0; curLetterIndex < _curWord.Length; curLetterIndex++)
            {
                // Create a text field for each character to be guessed
                EditText e = new EditText(this);
                e.Focusable = false;
                e.FocusableInTouchMode = false;
                e.Clickable = false;
                e.SetPadding(0, 0, 0, 0);
                e.Gravity = GravityFlags.CenterHorizontal; // try to line the character up with where the underline was

                // TODO: Make the text field jump out a little, issues with different themes, shelving this idea for now
                //   e.SetBackgroundColor(Color.Black);
                //   e.SetTextColor(Color.White);

                _characters.Add(e);
                layout.AddView(e);
            }

            // now set the number of guesses allowed: 
            // TODO: Move this to new method, and allow user to change this upon starting new game
            _totalGuessesAvail = 5;
            _numGuessesTextView.Text = _totalGuessesAvail.ToString();
        }

        /// <summary>
        /// Called on Game Over to reveal the word the user failed to guess.
        /// </summary>
        private void CompleteWordTextFields()
        {
            for (int curLetterIndex = 0; curLetterIndex < _characters.Count; curLetterIndex++)
            {
                if (string.IsNullOrEmpty(_characters[curLetterIndex].Text))
                {
                    _characters[curLetterIndex].Text = _curWord[curLetterIndex].ToString();
                }
            }
        }

        /// <summary>
        /// Helper method to show a simple messagebox
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="btnPosText"></param>
        /// <param name="btnNegText"></param>
        private void CreateMessageBox(string title, string message, string btnPosText, string btnNegText)
        {
            var builder = new AlertDialog.Builder(this);
            builder.SetTitle(title);
            builder.SetMessage(message);
            builder.SetPositiveButton(btnPosText, (senderAlert, args) =>
            {
                StartNewGame();
            });
            builder.SetNegativeButton(btnNegText, delegate
            {
                CreateToastCenterScreen("Game Over!");
            });
            RunOnUiThread(() =>
            {
                builder.Show();
            });
        }

        /// <summary>
        /// Called when starting a new game either from finishing the last game, or app start.
        /// </summary>
        private void StartNewGame()
        {
            // Clear up old data from a previous game(if any)
            _lettersToShowOnUI.Text = string.Empty;
            LinearLayout layout = FindViewById<LinearLayout>(Resource.Id.llWordToGuess);
            layout.RemoveAllViews();
            CreateToastCenterScreen(string.Empty);

            // Reset some variables
            _characters = new List<EditText>();
            _charactersGuessed = new List<char>();
            _numGuesses = 0;

            // If we have already loaded the dictionary, don't do so again
            if (_curDictionary == null)
                _curDictionary = new Dictionary("LanguageDictionaries/EnglishDictionary.txt", this.Assets); // TODO: more language support

            _curWord = _curDictionary.RetrieveRandomWordFromDictionary();

            // Create UI elements needed
            GenerateWordTextFields();
        }

        /// <summary>
        /// This method is called when the user presses a key on the keyboard. 
        /// This method then sanitizes the input, and sends it to the method
        /// to handle the letter guessed.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="keyCode"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        bool IOnKeyListener.OnKey(View v, Keycode keyCode, KeyEvent e)
        {
            if (e.Action == KeyEventActions.Up)
            {
                Console.WriteLine(keyCode.ToString());
                Console.WriteLine(e.ToString());
                Console.WriteLine(e.UnicodeChar);

                bool bEnteredChar = char.IsLetter((char)e.UnicodeChar);

                if (bEnteredChar)
                {
                    GuessCharacterInWord(keyCode.ToString());
                    return true;
                }
                else
                {
                    Console.WriteLine("Unknown key entered");
                    //Toast.MakeText(this,
                    //    string.Format("Unknown key {0} entered!", keyCode.ToString()),
                    //    ToastLength.Long).Show();

                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }

}