using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Runtime.Serialization;

namespace Lexalytics
{
#region MANAGED .NET LAYER

    /// <summary>
    /// Mode definitions added in Salience 5.1
    /// Provides shortcut for setting multiple options relevant to specific use profile.
    /// DEFAULT: leaves all options at default values
    /// SHORTFORM: TextThreshold = 40%, CalculateListAndTables = false, UsePolarityModel = true, ProcessAsOneSentence = true 
    /// </summary>
    public enum SALIENCE_MODE
    {
        DEFAULT,
        SHORTFORM
    }

    /// <summary>
    /// Data class encapsulating startup information
    /// </summary>
    public class SalienceStartup
    {
        /// <summary>
        /// Path to write startup information for diagnostics, if requested.
        /// </summary>
	    public string sLogPath;
        /// <summary>
        /// Path to the Lexalytics data directory to use for the session, default is [Lexalytics root]\data
        /// </summary>
	    public string sDataDirectory;
        /// <summary>
        /// Path to a user directory of customizations, default is [path to data directory]\user
        /// </summary>
	    public string sUserDirectory;
        /// <summary>
        /// Added in Salience 5.1, mode switch that automatically sets multiple options appropriate to use case.
        /// </summary>
        public SALIENCE_MODE nMode;
    }

    /// <summary>
    /// Class encapsulating a phrase extracted from the content.
    /// Lists of phrases relevant to results are included in other result structures such as document sentiment or theme mentions.
    /// </summary>
    public class SaliencePhrase
    {
        /// <summary>
        /// The text of the phrase
        /// </summary>
        public string sText;
        public int nDocument;
        public int nSentence;
        public int nWord;
        public int nLength;
        public int nByte;
        public int nByteLength;
        /// <summary>
        /// Added in Salience 5.1, indicates if phrase has been negated
        /// </summary>
        public int nIsNegated;
        /// <summary>
        /// Added in Salience 5.1, the token responsible for negating the phrase
        /// </summary>
        public string sNegator;
        public int nType;
        public int nSection;
        public int nRow;
        public int nColumn;
    }

    /// <summary>
    /// Support for learning customer's domain
    /// Added in Salience 5.2
    /// </summary>
    public class SalienceAlternateTheme
    {
        public string sAlternateTheme;
        public float fScore;
    }

    public class SalienceTheme
    {
        public string sTheme;
        public string sStemmedTheme;
        public string sNormalizedTheme;
        public int nThemeType;
        public float fScore;
        public float fSentiment;
        public int nEvidence;
        public bool bAbout;
        public string sSummary;
        public List<SaliencePhrase> ThemeMentions = new List<SaliencePhrase>();
        /// <summary>
        /// Introduced in Salience 5.1, contains a list of topics that the theme co-occurred with.
        /// </summary>
        public List<SalienceTopic> Topics;
        public List<SalienceAlternateTheme> AlternateThemes = new List<SalienceAlternateTheme>();
        public List<SaliencePhrase> ChildMentions = new List<SaliencePhrase>();
        public List<SaliencePhrase> RelatedMentions = new List<SaliencePhrase>();
    }

    public class SalienceSentiment
    {
        public float fScore;
        public List<SalienceSentimentPhrase> Phrases = new List<SalienceSentimentPhrase>();
        public List<SalienceModelSentiment> ModelSentiment = new List<SalienceModelSentiment>();
        /// <summary>
        /// DEPRECATED: Added in Salience 5.1 to convey experimental emotion information about sentiment detected.
        /// </summary>
        public List<SalienceTopic> Emotions = new List<SalienceTopic>();
    }

    public class SalienceSentimentPhrase
    {
        public SaliencePhrase Phrase;
        public string sSource;
        public float fScore;
        public int nType;
        public int nModified;
        public List<SaliencePhrase> SupportingPhrases;
    }

    public class SalienceModelSentiment
    {
        public string sName;
        public int nBest;
        public float fPositive;
        public float fNegative;
        public float fMixed;
        public float fNeutral;
    }
    
    public class SalienceMention
    {
        public SaliencePhrase Phrase;
        public int nType;
        public float fScore;
    }
    
    public class SalienceEntity
    {
        public string sNormalizedForm;
        public string sType;
        public string sLabel;
        public float fSentimentScore;
        public int nEvidence;
        public int nConfident;
        public int nAbout;
        public string sSummary;
        public int nCount;
        public int nFirstPos;
        public List<SalienceMention> Mentions;
        public List<SalienceTheme> Themes;
        public List<SalienceSentimentPhrase> SentimentPhrases= new List<SalienceSentimentPhrase>();
        /// <summary>
        /// Introduced in Salience 5.1, a list of topics that the entity co-occurred with.
        /// </summary>
        public List<SalienceTopic> Topics;
    }

    public class SalienceCollectionEntity
    {
        public string sNormalizedForm;
        public string sType;
        public string sLabel;
        public int nCount;
        public int nPositiveCount;
        public int nNegativeCount;
        public int nNeutralCount;
        public List<SaliencePhrase> oMentions;
    }

    public class SalienceEntityParams
    {
        public int nRequiredThreshold = 55;
        public int nSummaryLength = 2;
        public int nTimeout = 5;
    }

    public class SalienceAttribute
    {
        public string sAttribute;
        public int nCount;
        public int nPositiveCount;
        public int nNegativeCount;
        public int nNeutralCount;
        public List<SaliencePhrase> Mentions;
        public List<SaliencePhrase> PositiveMentions;
        public List<SaliencePhrase> NegativeMentions;
        public List<SaliencePhrase> NeutralMentions;
    }

    public class SalienceFacet
    {
        public string sFacet;
        public int nCount;
        public int nPositiveCount;
        public int nNegativeCount;
        public int nNeutralCount;
        public List<SalienceAttribute> Attributes;
        public int nSubjectLength;
        public List<SaliencePhrase> Mentions;
        public List<SaliencePhrase> PositiveMentions;
        public List<SaliencePhrase> NegativeMentions;
        public List<SaliencePhrase> NeutralMentions;
    }

    public class SalienceToken
    {
        public string sToken;
        public int nCount;
    }

    public class SalienceWord
    {
        public string sToken;
	    public string sPOSTag;
        public string sStem;
	    public bool bInvert;
        public float fSentiment;
        public int nSentimentType;
        public string sEntityType;
        public int nID;
        public int nSecondaryID;
        public bool bPostFixed;
    }

    public class SalienceChunk
    {
        public string sLabel;
	    public int nSentence;
        public List<SalienceWord> Tokens;
        public float fSentiment;
    }

    public class SalienceSentence
    {
        public List<SalienceWord> Tokens;
        public List<SalienceChunk> Chunks;
        public int nLength;
        public int nChunkCount;
        /// <summary>
        /// Added to .Net wrapper in Salience 5.1, indicates if the sentence is flagged as subjective or objective. (0=objective, 1=subjective)
        /// </summary>
        public int nSubjective;
        /// <summary>
        /// Added in Salience 5.1, indicates if the sentence is flagged as conveying sentiment polarity or not. (0=normal, 1=polar)
        /// </summary>
        public int nPolar;
        /// <summary>
        /// Added in Salience 5.1, indicates if the sentence is flagged as imperative (a call to action). (0=normal, 1=imperative)
        /// </summary>
        public int nImperative;
        /// <summary>
        ///  The rank of the sentence with relation to summary. Need to process summary for this to be initialized.
        /// </summary>
        public int nSummaryRank;
        /// <summary>
        /// Added in Salience 5.1, the textual content of this sentence.
        /// </summary>
        public String sText;
        /// <summary>
        /// Added in Salience 6.0, the sentiment score of this sentence.
        /// </summary>
        public float fSentiment;
    }

    public class SalienceDocument
    {
	    List<SalienceSentence> pSentences;
    }

    public class SalienceRow
    {
        public List<SalienceDocument> Columns;
    }

    public class SalienceSection
    {
        public string sText;
        public string sFingerprint;
        public string sHeader;
        public int nWordCount;
        public int nSentenceCount;
        public int nObjectiveCount;
        public int nSubjectiveCount;
        public int nParsedCount;
        public List<SalienceToken> TermFrequency;
        public List<SalienceToken> TaggedTermFrequency;
        public List<SalienceToken> BiGramFrequency;
        public List<SalienceToken> TaggedBiGramFrequency;
        public List<SalienceToken> TriGramFrequency;
        public List<SalienceToken> TaggedTriGramFrequency;
        public List<SalienceToken> QuadGramFrequency; //ideogram languages only
        public List<SalienceToken> oNegators;
        public List<SalienceToken> oIntensifiers;
        public List<SalienceSentence> Sentences;
        public List<SalienceChunk> Chunks;
        public List<SalienceRow> Rows; 
    }

    public class SalienceDocumentDetails
    {
        public List<SalienceSection> Sections;
    }

    public class SalienceCollectionDetails
    {
        public int nSize;
    }

    public class SalienceRelationship
    {
        public float fScore;
        public string sLabel;
        public string sExtra;
        public List<SalienceEntity> Entities;
    }

    public class SalienceOpinion
    {
	    public SalienceEntity Speaker;
	    public SalienceEntity EntityTopic;
	    public SalienceTheme ThemeTopic;
        public string sQuotation;
        public float fSentiment;
        public bool bThemeOpinion;
     }

    public class SalienceIntention
    {
        public string sWhat;
        public string sWho;
        public string sEvidence;
        public string sType;
        public SalienceChunk oWhat;
        public SalienceChunk oWho;
        public SalienceChunk oEvidence;
    }

    public class SalienceTopic
    {
        public string sTopic;
        public int nHits;
        /// <summary>
        /// Changed in Salience 5.1, standardizing use across concept-defined and query-defined topics. Topic score provides decimal match weight.
        /// </summary>
        public float fScore;
        /// <summary>
        /// Added in Salience 5.1, previously topic sentiment was reported as the "score". This field added for clarity during standardization for concept-defined and query-defined topics.
        /// </summary>
        public float fSentiment;
        /// <summary>
        /// Added in Salience 5.1.1, indicates whether the topic is a concept or a query topic.
        /// </summary>
        public int nType;
        public string sSummary;
        public List<string> vDocuments;
        /// <summary>
        /// Added in Salience 5.1.1, to support taxonomies in DocumentCategories
        /// </summary>
        public List<SalienceTopic> vChildren;
        /// <summary>
        /// Added in Salience 6.0, to support query topics defined with entities
        /// </summary>
        public List<SalienceEntity> vEntities;
    }

    public class SalienceSummary
    {
        public string sSummary;
        public List<SalienceSentence> vSentences;
        public string sAlternateSummary;
        public List<SalienceSentence> vAlternateSentences;
    }

    public class SalienceCollectionDocument
    {
        public string sIdentifier;
        public string sText;
    }

    /// <summary>
    /// Data structure redundant with API class. Removed pending further need.
    /// </summary>
    //public class SalienceCollection
    //{
    //    public string sName;
    //    public int nSize;
    //    public List<SalienceCollectionDocument> Documents;
    //}

#endregion // MANAGED .NET LAYER

#region UNMANAGED C LAYER (CONNECTION TO UNDERLYING SALIENCE C API)

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int umSalienceCallback(void* pParam, int nStatus, IntPtr acMessage);

    [StructLayout(LayoutKind.Sequential)]
    unsafe internal struct umSalienceStartup
    {
        [MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 1024)]
        public string acError;
        public int nStartupLog;
        public IntPtr acLogPath;
        [MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 260)]
        public string acDataDirectory;
        [MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 260)]
        public string acUserDirectory;
        public int nMode;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct umSalienceOption
    {
        public int nOption;         //The option being set
        public IntPtr acValue;      //Use this member if the set option requires a string
        public int nValue;          //!< Use this member if the set option requires an integer
        public float fValue;        //!< Use this member if the set option requires a float
    }


    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSaliencePhrase
    {
        public IntPtr acText;
        public int nDocument;
        public int nSentence;
        public int nWord;
        public int nLength;
        public int nByte;
        public int nByteLength;
        public int nIsNegated;
        public IntPtr acNegator;
        public int nType;
        public int nSection;
        public int nRow;
        public int nColumn;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSaliencePhraseList
    {
        public umSaliencePhrase* pPhrases;
        public int nLength;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceSuggestedTheme
    {
        public IntPtr acTheme;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceAlternateTheme
    {
        public IntPtr acAlternateTheme;
        public float fScore;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceAlternateThemeList
    {
        public umSalienceAlternateTheme* pAlternateThemes;
        public int nLength;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceTheme
    {
        public IntPtr acTheme;
        public IntPtr acStemmedTheme;
        public IntPtr acNormalizedTheme;
        public int nThemeType;
        public float fScore;
        public float fSentiment;
        public int nEvidence;
        public int nAbout;
        public IntPtr acSummary;
        public umSaliencePhraseList oMentions;
        public umSalienceTopicList oTopics;
        public umSalienceAlternateThemeList oAlternateThemes;
        public umSaliencePhraseList oChildMentions;
        public umSaliencePhraseList oRelatedMentions;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceThemeList
    {
        public umSalienceTheme* pThemes;
        public int nLength;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceAttribute
    {
        public IntPtr acAttribute;
        public int nCount;
        public int nPositiveCount;
        public int nNegativeCount;
        public int nNeutralCount;
        public umSaliencePhraseList oMentions;
        public umSaliencePhraseList oPositiveMentions;
        public umSaliencePhraseList oNegativeMentions;
        public umSaliencePhraseList oNeutralMentions;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceFacet
    {
        public IntPtr acFacet;
        public IntPtr acSubFacetList;
        public int nCount;
        public int nPositiveCount;
        public int nNegativeCount;
        public int nNeutralCount;
        public umSalienceAttribute* pAttributes;
        public int nSubjectLength;
        public umSaliencePhraseList oMentions;
        public umSaliencePhraseList oPositiveMentions;
        public umSaliencePhraseList oNegativeMentions;
        public umSaliencePhraseList oNeutralMentions;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceFacetList
    {
        public umSalienceFacet* pFacetList;
        public int nLength;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceSentimentPhrase
    {
        public umSaliencePhrase oPhrase;
        public float fScore;
        public int nType;
        public IntPtr acSource;
        public int nModified;
        public umSaliencePhraseList oSupportingPhrases;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceSentimentPhraseList
    {
        public umSalienceSentimentPhrase* pPhrases;
        public int nLength;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceSentimentModel
    {
        public int nBest;
        public float fPositive;
        public float fNegative;
        public float fMixed;
        public float fNeutral;
        public IntPtr acModelName;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceSentimentResult
    {
        public float fScore;
        public umSalienceSentimentPhraseList oPhrases;
        public umSalienceSentimentModel* pModel;
        public int nModelCount;
        public umSalienceTopicList oEmotions;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceMention
    {
        public umSaliencePhrase oPhrase;
        public int nType;
        public float fScore;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceMentionList
    {
        public umSalienceMention* pMentions;
        public int nLength;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceEntity
    {
        public IntPtr acNormalizedForm;
        public IntPtr acType;
        public IntPtr acLabel;
        public float fSentimentScore;
        public int nEvidence;
        public int nConfident;
        public int nAbout;
        public IntPtr acSummary;
        public umSalienceMentionList oMentions;
        public umSalienceThemeList oThemes;
        public umSalienceSentimentPhraseList oSentimentPhrases;
        public umSalienceTopicList oTopics;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceCollectionEntity
    {
        public IntPtr acNormalizedForm;
        public IntPtr acType;
        public IntPtr acLabel;
        public int nCount;
        public int nPositiveCount;
        public int nNegativeCount;
        public int nNeutralCount;
        public umSaliencePhraseList oMentions;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceEntityList
    {
        public umSalienceEntity* pEntities;
        public int nLength;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceCollectionEntityList
    {
        public umSalienceCollectionEntity* pCollectionEntities;
        public int nLength;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceRelation
    {
        public umSalienceEntityList oEntities;
        public IntPtr acType;
        public float fConfidence;
        public IntPtr acExtra;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceRelationList
    {
        public umSalienceRelation* pRelations;
        public int nLength;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceOpinion
    {
        public umSalienceEntity oSpeaker;
        public umSalienceEntity oEntityTopic;
        public umSalienceTheme oThemeTopic;
        public IntPtr acQuotation;
        public float fSentiment;
        public int nHasTheme;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceOpinionList
    {
        public umSalienceOpinion* pOpinions;
        public int nLength;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceIntention
    {
	    public IntPtr acWhat;
        public IntPtr acWho;
        public IntPtr acEvidence;
        public IntPtr acType;
        public umSalienceChunk oWhat;
        public umSalienceChunk oWho;
        public umSalienceChunk oEvidence;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceIntentionList
    {
	    public umSalienceIntention* pIntentions;
	    public int nLength;
    }


    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceToken
    {
        public IntPtr acToken;
        public int nCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceTokenList
    {
        public umSalienceToken* pTokens;
        public int nLength;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceWord
    {
        public IntPtr acToken;
        public IntPtr acPOSTag;
        public IntPtr acStem;
        public int nInvert;
        public float fSentiment;
        public int nSentimentType;
        public IntPtr acEntityType;
        public int nId;
        public int nSecondaryId;
        public int nPostFixed;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceChunk
    {
        public umSalienceWord* pTokens;
        public int nLength;
        public IntPtr acLabel;
        public int nSentence;
        public float fSentiment;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceSentence
    {
        public umSalienceWord* pTokens;
        public umSalienceChunk* pChunks;
        public int nLength;
        public int nChunkCount;
        public int nSubjective;
        public int nPolar;
        public int nImperative;
        public int nSummaryRank;
        public IntPtr acText;
        public float fSentiment;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceDocument
    {
        public int nSentenceCount;
        public umSalienceSentence* pSentences;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceRowList
    {
	    public int nRowCount;
	    public IntPtr* pRows;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceSection
    {
        public int nWordCount;                      //!< The number of tokens in the document.
        public int nSentenceCount;                  //!< The number of sentences in the document.
        public int nObjectiveCount;                 //!< The number of objective sentences in the document.
        public int nSubjectiveCount;                //!< The number of subjective sentences in the document.
        public int nParsedCount;                    //!< The number of sentences that grammatically parsed.
        public umSalienceTokenList oTermFrequency;
        public umSalienceTokenList oTaggedTermFrequency;
        public umSalienceTokenList oBiGrams;
        public umSalienceTokenList oTaggedBiGrams;
        public umSalienceTokenList oTriGrams;
        public umSalienceTokenList oTaggedTriGrams;
        public umSalienceTokenList oQuadGrams;      //!< Only filled in for Chinese
        public umSalienceTokenList oNegators;
        public umSalienceTokenList oIntensifiers;
        public umSalienceSentence* pSentences;      //!< Pointer to a list of the individual sentences.
        public IntPtr acInternalRepresentation;
        public IntPtr acFingerprint;
        public umSalienceRowList oRows;
        public IntPtr acHeader;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceDocumentDetails
    {
        public int nSectionCount;
        public IntPtr oSections;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceCollectionDetails
    {
        public int nSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceTopic
    {
        public IntPtr acTopic;
        public int nHits;
        public float fScore;
        public float fSentiment;
        public IntPtr acAdditional;
        public int nType;
        public IntPtr pChildren;
        public umSalienceEntityList oEntities;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceTopicList
    {
        public umSalienceTopic* pTopics;
        public int nLength;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceSummaryResult
    {
        public IntPtr acSummary;
        public umSalienceDocument* pDocument;
        public IntPtr acAlternateSummary;
        public umSalienceDocument* pAlternateDocument;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceCollectionDocument
    {
	    public IntPtr acIndentifier;
        public int nIsText;
        public int nSplitByLine;
	    public IntPtr acText;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct umSalienceCollection
    {
        public IntPtr acName;
    	public int nSize;
	    public umSalienceCollectionDocument* pDocuments;
    }


#endregion // UNMANAGED C LAYER (CONNECTION TO UNDERLYING SALIENCE C API)
}
