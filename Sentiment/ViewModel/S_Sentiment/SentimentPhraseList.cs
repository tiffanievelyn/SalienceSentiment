using Lexalytics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.ViewModel
{
    public class SentimentPhraseList : ObservableCollection<S_Phrase>
    {
        public SentimentPhraseList() : base() {}
    }

    public class S_Phrase
    {
        public float sp_score { get; set; }
        public string sp_phrase { get; set; }
        
    }
    
}
