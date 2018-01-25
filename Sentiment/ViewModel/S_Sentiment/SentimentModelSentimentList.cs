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
        public string ms_name { get; set; }
        public int ms_best { get; set; }
        public float ms_positive { get; set; }
        public float ms_negative { get; set; }
        public float ms_mixed { get; set; }
        public float ms_neutral { get; set; }
    }
    
}
