using Lexalytics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.ViewModel
{
    
    public class SentimentModelSentimentList : ObservableCollection<S_ModelSentiment>
    {
        public SentimentModelSentimentList() : base() { }
    }

    public class S_ModelSentiment
    {
        //MODEL SENTIMENT
        //ms_best should either be 0=positive, 1=negative, 2=mixed, 3=neutral
        //should also match category of greatest model score
        public string ms_name { get; set; }
        public int ms_best { get; set; }
        public float ms_positive { get; set; }
        public float ms_negative { get; set; }
        public float ms_mixed { get; set; }
        public float ms_neutral { get; set; }
    }
    
}
