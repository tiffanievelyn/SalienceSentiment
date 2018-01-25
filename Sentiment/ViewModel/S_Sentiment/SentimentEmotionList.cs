using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.ViewModel
{
    public class SentimentEmotionList : ObservableCollection<S_Emotion>
    {
        public SentimentEmotionList() : base() { }
    }

    public class S_Emotion
    {
        public string e_topic { get; set; }
        public int e_hit { get; set; }
        public float e_score { get; set; }
        public string s_summary { get; set; }
    }
}
