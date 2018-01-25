using Lexalytics;
using Sentiment.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ViewModel
{
    public class MainViewModel : ObservableObject
    {
        private string _licensePath = "C:/Program Files (x86)/Lexalytics/License.v5";
        private string _dataPath = "C:/Program Files (x86)/Lexalytics/data";

        private string _inputText = "This is happy all about getting on with it, delivering the things that we said we would do, I'm very pleased to say ";

        #region Attribute
        private ObservableCollection<S_Phrase> _phraseList = new ObservableCollection<S_Phrase>();

        private float _documentSentiment;
        private float _score;
        private string _phrasesView;
        private string _modelSentiment;
        private string _emotions;

        public float DocumentSentiment
        {
            get { return _documentSentiment; }
            set
            {
                _documentSentiment = value;
                RaisePropertyChangedEvent(nameof(DocumentSentiment));
            }
        }

        public ObservableCollection<Sentiment.ViewModel.S_Phrase> PhraseList
        {
            get
            {
                return _phraseList;
            }
        }

        public string PhraseView
        {
            get { return _phrasesView; }
            set
            {
                _phrasesView = value;
                RaisePropertyChangedEvent(nameof(PhraseView));
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

        #endregion

        #region ICommmand

        public ICommand UpdateCommand
        {
            get { return new DelegateCommand(Update); }
        }

        public ICommand ResetCommand
        {
            get { return new DelegateCommand(Reset); }
        }

        #endregion
        
        Salience Engine = null;
        private float score; //LOOK
        private string phrase;

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
        }
        
        private void Reset()
        {
            LicensePath = DataPath = TextInput = string.Empty;
        }

        private void Update()
        {
            if (!string.IsNullOrEmpty(TextInput))
            {
                int nRet = Engine.PrepareText(_inputText); //returns 0 if less than 1000 words
                Console.WriteLine(nRet);
                if (nRet == 0)
                {
                    SalienceSentiment mySentiment = Engine.GetDocumentSentiment(true, String.Empty);
                    DocumentSentiment = mySentiment.fScore;
                    foreach (var a in mySentiment.Phrases.ToArray())
                    {
                        S_Phrase p = new S_Phrase { sp_score = a.fScore, sp_phrase = a.Phrase.sText };
                        PhraseList.Add(p);
                        RaisePropertyChangedEvent(nameof(PhraseList));
                    }
                    
                }
            }
        }

    }
}
