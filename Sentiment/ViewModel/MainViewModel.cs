using Lexalytics;
using System;
using System.Windows.Input;

namespace ViewModel
{
    public class MainViewModel : ObservableObject
    {
        private string _licensePath = "C:/Program Files (x86)/Lexalytics/License.v5";
        private string _dataPath = "C:/Program Files (x86)/Lexalytics/data";
        private string _inputText = "This is all about getting on with it, delivering the things that we said we would do, I'm very pleased to say that we are ahead of time and this project includes not just level crossing removals, but power upgrades, signal upgrades, new stations well over and above the original commitment we made";
        private float _score;
        private string _phrases;

        Salience Engine = null;
        
        public MainViewModel()
        {
            try
            {
                //We declared Engine before initializing it so we could put this in a try/catch block, and 
                //have the rest of the code outside.
                Engine = new Salience(_licensePath, _dataPath);
            }
            catch (SalienceException e)
            {
                /*If the SalienceEngine constructor throws an error, one of these is likely to be true:
                 * 1) The license file is missing/invalid/out of date
                 * 2) The data directory was missing or contained incorrect files
                 * 3) Salience6.dll could not be found. */
                System.Console.WriteLine("Error Loading SalienceEngine: " + e.Message);
                return;
            }

            int nRet = Engine.PrepareText(_inputText);
            if (nRet == 0)
            {
                SalienceSentiment mySentiment = Engine.GetDocumentSentiment(true, String.Empty);
                Score = mySentiment.fScore;
                Phrase = mySentiment.ToString();
                
            }

        }

        public string Phrase
        {
            get { return _phrases; }
            set
            {
                _phrases = value;
                RaisePropertyChangedEvent(nameof(Phrase));
            }
        }

        public float Score
        {
            get { return _score; }
            set
            {
                _score = value;
                RaisePropertyChangedEvent(nameof(Score));
            }
        }

        public string LicensePath
        {
            get { return _licensePath; }
            set
            {
                _licensePath = value;
                RaisePropertyChangedEvent(nameof(LicensePath));
            }
        }

        public string DataPath
        {
            get { return _dataPath; }
            set
            {
                _dataPath = value;
                RaisePropertyChangedEvent(nameof(DataPath));
            }
        }

        public string TextInput
        {
            get { return _inputText; }
            set
            {
                _inputText = value;
                RaisePropertyChangedEvent(nameof(TextInput));
            }
        }

        public ICommand ResetCommand
        {
            get { return new DelegateCommand(Reset); }
        }

        private void Reset()
        {
            if (string.IsNullOrWhiteSpace(LicensePath)) return;
            LicensePath = DataPath = TextInput = string.Empty;
        }

    }
}
