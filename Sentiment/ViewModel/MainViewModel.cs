﻿using Lexalytics;
using Sentiment.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Input;

namespace ViewModel
{
    public class MainViewModel : ObservableObject
    {
        private string _licensePath = "C:/Program Files (x86)/Lexalytics/License.v5";
        private string _dataPath = "C:/Program Files (x86)/Lexalytics/data";

        private string _inputText = "This is happy all about getting on with it, delivering the things that we said we would do, I'm very pleased to say ";

        #region Attribute
        private float _documentSentiment;
        private ObservableCollection<S_Phrase> _phraseList = new ObservableCollection<S_Phrase>();
        private ObservableCollection<S_ModelSentiment> _modelSentimentList = new ObservableCollection<S_ModelSentiment>();
        private ObservableCollection<S_Emotion> _emotiontList = new ObservableCollection<S_Emotion>();

        private int _docScore = 0;
        private List<KeyValuePair<string, int>> _pieChartItems = new List<KeyValuePair<string, int>>();

        public int DocScore
        {
            get { return _docScore; }
            set
            {
                _docScore = value;
                RaisePropertyChangedEvent(nameof(DocScore));
            }
        }
        public List<KeyValuePair<string, int>> PieChartItems
        {
            get { return _pieChartItems; }
            set
            {
                _pieChartItems = value;
                RaisePropertyChangedEvent(nameof(PieChartItems));
            }
        }

        public float DocumentSentiment
        {
            get { return _documentSentiment; }
            set
            {
                _documentSentiment = value;
                RaisePropertyChangedEvent(nameof(DocumentSentiment));
            }
        }

        public ObservableCollection<S_Phrase> PhraseList
        {
            get
            {
                return _phraseList;
            }
        }

        public ObservableCollection<S_ModelSentiment> ModelSentimentList
        {
            get { return _modelSentimentList; }
        }

        public ObservableCollection<S_Emotion> EmotionList
        {
            get { return _emotiontList; }
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

            PieChartItems.Add(new KeyValuePair<string, int>("Positive", 25));
            PieChartItems.Add(new KeyValuePair<string, int>("Negative", 25));
            PieChartItems.Add(new KeyValuePair<string, int>("Mixed", 25));
            PieChartItems.Add(new KeyValuePair<string, int>("Neutral", 25));
        }
        private void resetList()
        {
            EmotionList.Clear(); PhraseList.Clear(); ModelSentimentList.Clear();
        }
        
        private void Reset()
        {
            //LicensePath = DataPath = 
            TextInput = string.Empty;
            resetList();
        }

        private void Update()
        {
            if (!string.IsNullOrEmpty(TextInput))
            {
                int nRet = Engine.PrepareText(_inputText); //returns 0 if less than 1000 words
                Console.WriteLine(nRet);
                
                if (nRet == 0 || nRet == 6) //not sure what 0 or 6 means but that's what Salience set the returns for non-error as
                {
                    SalienceSentiment mySentiment = Engine.GetDocumentSentiment(true, String.Empty);
                    resetList();
                    DocumentSentiment = mySentiment.fScore * 100;
                    DocScore = (int)DocumentSentiment;

                    foreach (var a in mySentiment.Phrases.ToArray())
                    {
                        S_Phrase p = new S_Phrase {
                            sp_score = a.fScore,
                            sp_phrase = a.Phrase.sText
                        };
                        PhraseList.Add(p);
                    }
                    foreach (var b in mySentiment.ModelSentiment.ToArray())
                    {
                        S_ModelSentiment m = new S_ModelSentiment
                        {
                            ms_name = b.sName,
                            ms_best = b.nBest,
                            ms_positive = b.fPositive,
                            ms_mixed = b.fMixed,
                            ms_negative = b.fNegative,
                            ms_neutral = b.fNeutral
                        };
                        ModelSentimentList.Add(m);
                    }

                    List<KeyValuePair<string, int>> MyValue = new List<KeyValuePair<string, int>>();
                    MyValue.Add(new KeyValuePair<string, int>("Positive", (int) (ModelSentimentList[0].ms_positive *100)));
                    MyValue.Add(new KeyValuePair<string, int>("Negative", (int) (ModelSentimentList[0].ms_negative *100)));
                    MyValue.Add(new KeyValuePair<string, int>("Mixed", (int) (ModelSentimentList[0].ms_mixed *100)));
                    MyValue.Add(new KeyValuePair<string, int>("Neutral", (int) (ModelSentimentList[0].ms_neutral *100)));

                    PieChartItems = MyValue;
                    /*
                    foreach (var c in mySentiment.Emotions.ToArray())
                    {
                        S_Emotion e = new S_Emotion
                        {
                            e_topic = c.sTopic,
                            e_hit = c.nHits,
                            e_score = c.fScore,
                            s_summary = c.sSummary
                        };
                        EmotionList.Add(e);
                    }
                    */
                }
            }
        }

    }
}
