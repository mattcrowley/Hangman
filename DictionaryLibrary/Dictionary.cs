using Android.Content.Res;
using System;
using System.Collections.Generic;
using System.IO;

namespace DictionaryLibrary
{
    public class Dictionary
    {
        private List<string> _dictionaryWords;

        /// <summary>
        /// 
        /// </summary>
        public Dictionary()
        {
            _dictionaryWords = new List<string>();
        }
        public Dictionary(string filename, AssetManager assets)
        {
            _dictionaryWords = new List<string>();
            LoadFile(filename, assets);
        }

        /// <summary>
        /// Loads the new dictionary into memory. Called upon app startup/when changing languages while running.
        /// </summary>
        /// <param name="dicFile"></param>
        public void LoadFile(string dicFile, AssetManager assets)
        {
            // Delete old list just in case
            if (_dictionaryWords.Count > 0)
                _dictionaryWords.Clear();

            // Checking different solutions to generate the dictionary list
            /* 
            Console.WriteLine("Starting file open read, then the loop to get all words");
            using (StreamReader sr = new StreamReader(assets.Open(dicFile)))
            {
                for (int curIndx = 0; !sr.EndOfStream; curIndx++)
                {
                    string line = sr.ReadLine();
                    _dictionaryWords.Add(line);
                }
            }
            Console.WriteLine("Finished file open read, then the loop to get all words");
            _dictionaryWords.Clear();
            */

            Console.WriteLine("Starting file read, using BufferedStream:");
            using (Stream fs = assets.Open(dicFile, Access.Streaming))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    _dictionaryWords.Add(line);
                }
            }
            Console.WriteLine("Done file read, using BufferedStream:");

            Console.WriteLine("Loaded {0} items from dictionary: {1}", _dictionaryWords.Count, dicFile);
        }

        /// <summary>
        /// Called to get a letter from the dictionary.
        /// This function should be called after loading the dictionary. 
        /// </summary>
        /// <returns></returns>
        public string RetrieveRandomWordFromDictionary()
        {
            string randomWord = string.Empty;

            if (_dictionaryWords != null && _dictionaryWords.Count > 0)
            {
                Random rGen = new Random();

                int rNum = rGen.Next(0, _dictionaryWords.Count + 1);

                randomWord = _dictionaryWords[rNum];
                Console.WriteLine("Loaded {0} from dictionary:", randomWord);
            }
            else
            {
                Console.WriteLine("ERROR! Must load dictionary before calling this method!");
            }

            return randomWord;
        }
    }
}
