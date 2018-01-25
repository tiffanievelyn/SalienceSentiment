using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Runtime.Serialization;

namespace Lexalytics
{
    public class Salience6Engine : IDisposable
    {
        private const int LXA_MAX_ERROR_LENGTH = 1024;
        protected bool m_bDisposed = false;
        protected IntPtr m_pSessionHandle;
        protected IntPtr m_pLicense;
        protected String m_sCurrentOptionSession ="";
        public delegate int dEngineNotification(int nMessageId, string sMessage);
        protected umSalienceCallback fCallback;
        public dEngineNotification Callback_GetCollectionFacets;
        public dEngineNotification Callback_GetCollectionEntities;
        public dEngineNotification Callback_GetCollectionUserEntities;
        public dEngineNotification Callback_GetCollectionThemes;
        public dEngineNotification Callback_PrepareCollection;

        #region  Base options

        /// <summary>
        /// Set this option first to specify which session subsequent options should be applied to. Set to the empty string to apply to all.
        /// If you set CurrentOptionSession, it will apply to all subsequent option settings on this Salience object.
        /// </summary>
        public String CurrentOptionSession
        {
            get { return m_sCurrentOptionSession; }
            set { m_sCurrentOptionSession = value; }
        }

        private int m_TextThreshold;
        /// <summary>
        /// Integer property indicating the required threshold (between 0 and 100) of text that must be alphanumeric characters for text processing. 
        /// By default, 80% of the content sumbitted to a text preparation method must be alphanumeric. Otherwise, a "Failed to parse" exception is thrown.
        /// Common usage is to decrease this threshold for very short content, where one extra punctuation character makes big difference in percentage.
        /// </summary>
        public int TextThreshold
        {
            get { return m_TextThreshold; }
            set { if (SetSalienceOption(1000, value, m_sCurrentOptionSession) == 0) m_TextThreshold = value; }
        }

        private bool m_CalculateListsAndTables;
        /// <summary>
        /// Determine if content contains lists or tables.
        /// Defaults to 1 (true), which excludes table content from text analysis. 
        /// </summary>
        /// <remarks>Added in Salience 5.1</remarks>
        public bool CalculateListsAndTables
        {
            get { return m_CalculateListsAndTables; }
            set { if (SetSalienceOption(1001, value, m_sCurrentOptionSession) == 0) m_CalculateListsAndTables = value; }
        }

        private int m_ExecutionTimeout;
        /// <summary>
        /// Maximum time in milliseconds certain functions are allowed to run for (0=no timeout)
        /// </summary>
        public int ExecutionTimeout
        {
            get { return m_ExecutionTimeout; }
            set { if (SetSalienceOption(1002, value, m_sCurrentOptionSession) == 0) m_ExecutionTimeout = value; }
        }

        private bool m_FailOnLongSentences;
        /// <summary>
        /// When a document contains a very long sentence, should you process the rest of the document? (0=process)
        /// </summary>
        public bool FailOnLongSentences
        {
            get { return m_FailOnLongSentences; }
            set { if (SetSalienceOption(1003, value, m_sCurrentOptionSession) == 0) m_FailOnLongSentences = value; }
        }

        private string m_UserDirectory;
        /// <summary>
        /// Sets path to user directory of customizations
        /// </summary>
        public string UserDirectory
        {
            get { return m_UserDirectory; }
            set { if (SetSalienceOption(1004, value, m_sCurrentOptionSession) == 0) m_UserDirectory = value; }
        }

        private float m_ConceptSlop;
        /// <summary>
        /// Sets a threshold for the measure of similarity between two concepts for them to roll together. This is a float value between 0 and 1, default value is 0.8. We do not recommend significant changes to this option value.
        /// </summary>
        public float ConceptSlop
        {
            get { return m_ConceptSlop; }
            set { if (SetSalienceOption(1005, value, m_sCurrentOptionSession) == 0) m_ConceptSlop = value; }
        }

        private bool m_ProcessAsOneSentence;
        /// <summary>
        /// Treats the whole document as a single sentence.  Good for Twitter type content.  Defaults to 0 (false)
        /// </summary>
        /// <remarks>Added in Salience 5.1</remarks>
        public bool ProcessAsOneSentence
        {
            get { return m_ProcessAsOneSentence; }
            set { if (SetSalienceOption(1009, value, m_sCurrentOptionSession) == 0) m_ProcessAsOneSentence = value; }
        }

        private bool m_UseSharedMemory;
        /// <summary>
        /// Puts large data files into interprocess space for a smaller overall footprint.
        /// Defaults to true.
        /// </summary>
        /// <remarks>Added in Salience 5.1</remarks>
        public bool UseSharedMemory
        {
            get { return m_UseSharedMemory; }
            set { if (SetSalienceOption(1010, value, m_sCurrentOptionSession) == 0) m_UseSharedMemory = value; }
        }

        private bool m_ProcessComplexStems;
        /// <summary>
        /// Enables the processing of things like looooovvvvvveeeeee into love at a cost of processing time.
        /// Defaults to false.
        /// </summary>
        /// <remarks>Added in Salience 5.1</remarks>
        public bool ProcessComplexStems
        {
            get { return m_ProcessComplexStems; }
            set { if (SetSalienceOption(1012, value, m_sCurrentOptionSession) == 0) m_ProcessComplexStems = value; }
        }

        private bool m_FlattenAllUpperCase;
        /// <summary>
        /// If a sentence is encountered with all upper case tokens, it should get converted to lower case for better POS tagging
        /// Defaults to false.
        /// </summary>
        /// <remarks>Added in Salience 5.1</remarks>
        public bool FlattenAllUpperCase
        {
            get { return m_FlattenAllUpperCase; }
            set { if (SetSalienceOption(1013, value, m_sCurrentOptionSession) == 0) m_FlattenAllUpperCase = value; }
        }

        private bool m_ContentHTML;
        /// <summary>
        /// By default, the content provided to Salience for processing must be plain text. Enabling this option allows HTML content to be provided to text preparation methods, either as a raw well-formed HTML string, or as path to an HTML file.
        /// Defaults to false.
        /// </summary>
        /// <remarks>Added in Salience 5.1.1</remarks>
        public bool ContentHTML
        {
            get { return m_ContentHTML; }
            set { if (SetSalienceOption(1014, value, m_sCurrentOptionSession) == 0) m_ContentHTML = value; }
        }

        private bool m_AlternateForms;
        
        /// <summary>
        /// Turns on internal alternate forms, allowing Salience to predict intended word choices, e.g. to instead of too. On by default for short-mode. Off by default for long-mode.
        /// </summary>
        /// <remarks>Added in Salience 6.0</remarks>
        public bool AlternateForms
        {
            get { return m_AlternateForms; }
            set { if (SetSalienceOption(1015, value, m_sCurrentOptionSession) == 0) m_AlternateForms = value; }
        }


        private bool m_UseChainer;
        /// <summary>
        /// Allows you to turn on and off our lexical chaining algorithm, which improves accuracy at a runtime performance cost
		/// Disabling also makes the reason results were generated little clearer.
        /// Defaults to true
        /// </summary>
        /// <remarks>Added in Salience 6.0</remarks>
        public bool UseChainer
        {
            get { return m_UseChainer; }
            set { if (SetSalienceOption(1016, value, m_sCurrentOptionSession) == 0) m_UseChainer = value; }
        }

        #endregion // Base options

        #region Document Details options

        private bool m_StemDocDetails = true;

        /// <summary>
        /// Whether to return terms,bigrams and trigrams in document details in a stemmed or unstemmed form.
        /// Defaults to true.
        /// </summary>
        /// <remarks> Introduced in Salience 5.2 update</remarks>
        public bool StemDocDetails
        {
            get { return m_StemDocDetails; }
            set { if (SetSalienceOption(1500, value, m_sCurrentOptionSession) == 0) m_StemDocDetails = value; }
        }

        #endregion

        #region Concept options
        private int m_MaxConceptHits;
        /// <summary>
        /// Sets a limit on the most hits that a concept topic can contain (for collections)
		/// Min/Max range: 0-MAX INT
		/// Default value:  	
        /// </summary>
        public int MaxConceptHits
        {
            get { return m_MaxConceptHits; }
            set { if (SetSalienceOption(2000, value, m_sCurrentOptionSession) == 0) m_MaxConceptHits = value; }
        }

        private float m_MinConceptScore;
        /// <summary>
        /// Sets a lower bound on the score for a concept topic to be identified for a document
        /// Min/Max range:	0-1
        /// Default value:  	
        /// The score value for a concept topic hit must be greater than this setting in order for it to be considered a hit for the document.
        /// Setting this value higher will result in fewer concept topic matches per document, but stronger matches. Setting the value lower will bring in more weak matches.
        /// </summary>
        public float MinConceptScore
        {
            get { return m_MinConceptScore; }
            set { if (SetSalienceOption(2001, value, m_sCurrentOptionSession) == 0) m_MinConceptScore = value; }
        }

        private int m_ConceptWindowSize;
        /// <summary>
        /// Smaller values will allow small, on-topic text to match concepts for the whole document.
        /// </summary>
        public int ConceptWindowSize
        {
            get { return m_ConceptWindowSize; }
            set { if (SetSalienceOption(2002, value, m_sCurrentOptionSession) == 0) m_ConceptWindowSize = value; }
        }

        private int m_ConceptTopicJump;
        /// <summary>
        /// Smooths out the concept topic window feature at a slight performance hit. Should never be larger than concept topic window size.
        /// </summary>
        public int ConceptTopicJump
        {
            get { return m_ConceptTopicJump; }
            set { if (SetSalienceOption(2003, value, m_sCurrentOptionSession) == 0) m_ConceptTopicJump = value; }
        }

        private bool m_NongrammaticalTopics;
        /// <summary>
        /// Flag to control definition of concept topics.
        /// Set to false if your concept topics are defined by sentences, true if they are collections of words. Default behavior is to define concept topics as a set of words.
        /// </summary>
        public bool NongrammaticalTopics
        {
            get { return m_NongrammaticalTopics; }
            set { if (SetSalienceOption(2004, value, m_sCurrentOptionSession) == 0) m_NongrammaticalTopics = value; }
        }

        private String m_ConceptTopics;
        /// <summary>
        /// Sets path to file containing concept topic definitions.
        /// </summary>
        public String ConceptTopics
        {
            get { return m_ConceptTopics; }
            set { if (SetSalienceOption(2005, value, m_sCurrentOptionSession) == 0) m_ConceptTopics = value; }
        }
        #endregion // Concept options

        #region Entity options
        private string m_RequiredEntities;
        /// <summary>
        /// If set, only entities of the specified type (comma-delimited list) will be returned
        /// </summary>
        public string RequiredEntities
        {
            get { return m_RequiredEntities; }
            set { if (SetSalienceOption(3000, value, m_sCurrentOptionSession) == 0) m_RequiredEntities = value; }
        }

        private bool m_AnaphoraResolution;
        /// <summary>
        /// By default, this option is set to true. When set to true, Salience will connect entities to pronouns.
        /// When set to false, pronouns are ignored with respect to entities.
        /// </summary>
        public bool AnaphoraResolution
        {
            get { return m_AnaphoraResolution; }
            set { if (SetSalienceOption(3001, value, m_sCurrentOptionSession) == 0) m_AnaphoraResolution = value; }
        }

        private float m_ModelSensitivity;
        /// <summary>
        /// Threshold for CRF model-based entity extraction.
        /// Min/Max range   10000-1000000
        /// Default value   1000000
        /// Lower values cause entity calculations to more quickly degrade into a 'simple mode' if the document is excessively complex to keep speed up.
        /// </summary>
        public float ModelSensitivity
        {
            get { return m_ModelSensitivity; }
            set { if (SetSalienceOption(3002, value, m_sCurrentOptionSession) == 0) m_ModelSensitivity = value; }
        }

        private int m_EntityThreshold;
        /// <summary>Integer property indicating the required threshold (between 0 and 100) that an entity must meet in order to be reported in entity results.</summary>
        /// <remarks>This property replaces the nThreshold member of the SE4 EntityParams structure.</remarks>
        public int EntityThreshold
        {
            get { return m_EntityThreshold; }
            set { if (SetSalienceOption(3003, value, m_sCurrentOptionSession) == 0) m_EntityThreshold = value; }
        }

        private int m_EntitySummaryLength;
        /// <summary>Integer property indicating maximum length in sentences for entity summaries.</summary>
        /// <remarks>This property replaces both the bCalcSummary and nSummaryLength members of the SE4 EntityParams structure.</remarks>
        public int EntitySummaryLength
        {
            get { return m_EntitySummaryLength; }
            set { if (SetSalienceOption(3004, value, m_sCurrentOptionSession) == 0) m_EntitySummaryLength = value; }
        }

        private bool m_EntityOverlap;
        /// <summary>
        /// Sets whether User Entities can overlap each other, defaults to 0 (no)
        /// </summary>
        public bool EntityOverlap
        {
            get { return m_EntityOverlap; }
            set { if (SetSalienceOption(3005, value, m_sCurrentOptionSession) == 0) m_EntityOverlap = value; }
        }

        private string m_EntityList;
        /// <summary>String property specifying a file containing user-defined entities.</summary>
        /// <remarks>This property replaces the SE4 method SetSimpleEntitiesFile.</remarks>
        public string EntityList
        {
            get { return m_EntityList; }
            set { if (SetSalienceOption(3006, value, m_sCurrentOptionSession) == 0) m_EntityList = value; }
        }

        private bool m_SentimentThemeOverlap;
        /// <summary>
        /// Allows the sentiment phrases from an underlying LSF and custom HSD to overlap. This will allow sentiment phrases such as "good" and "good day" to both appear in results.
        /// By default, this option is set to false.
        /// </summary>
        public bool SentimentThemeOverlap
        {
            get { return m_SentimentThemeOverlap; }
            set { if (SetSalienceOption(3007, value, m_sCurrentOptionSession) == 0) m_SentimentThemeOverlap = value; }
        }

        private bool m_EntityTopics;
        /// <summary>
        /// Whether to calculate topic matches for all entities.
        /// By default, this option is set to true
        /// </summary>
        /// <remarks>Added in Salience 5.1</remarks>
        public bool EntityTopics
        {
            get { return m_EntityTopics; }
            set { if (SetSalienceOption(3008, value, m_sCurrentOptionSession) == 0) m_EntityTopics = value; }
        }

        private bool m_StemUserEntityContent;
        /// <summary>
        /// Whether to stem the content provided when processing user entities.
        /// By default, this option is set to true
        /// </summary>
        /// <remarks>Added in Salience 5.2</remarks>
        public bool StemUserEntityContent
        {
            get { return m_StemUserEntityContent; }
            set { if (SetSalienceOption(3009, value, m_sCurrentOptionSession) == 0) m_StemUserEntityContent = value; }
        }

        private bool m_EntityUserDirectoryOnly;
        /// <summary>
        /// When turned on, allows named entities to operate using only the files from user/salience/entities rather than both the user and the data folders.
        /// By default, this option is set to false
        /// </summary>
        /// <remarks>Added in Salience 5.2</remarks>
        public bool EntityUserDirectoryOnly
        {
            get { return m_EntityUserDirectoryOnly; }
            set { if (SetSalienceOption(3010, value, m_sCurrentOptionSession) == 0) m_EntityUserDirectoryOnly = value; }
        }

        #endregion // Entity options

        #region Sentiment options

        private string m_sSentimentModel;

        public String AddSentimentModel
        {
            get { return m_sSentimentModel; }
            set { if (SetSalienceOption(4001, value, m_sCurrentOptionSession) == 0) m_sSentimentModel = value; } 
        }

        private bool m_UsePolarityModel;
        /// <summary>
        /// Includes a measure of whether a sentence 'looks' like the sort of sentence that usually communicates sentiment in analysis.
        /// By default, this option is set to false
        /// </summary>
        /// <remarks>Added in Salience 5.1</remarks>
        public bool UsePolarityModel
        {
            get { return m_UsePolarityModel; }
            set { if (SetSalienceOption(4004, value, m_sCurrentOptionSession) == 0) m_UsePolarityModel = value; }
        }

        private bool m_SetAllSentimentPhrases;
        /// <summary>
        /// Enables the user to get all possible sentiment phrases back from GetSentiment, even ones that aren't in the dictionary.
        /// By default, this option is set to false
        /// </summary>
        /// <remarks>Added in Salience 5.1</remarks>
        public bool SetAllSentimentPhrases
        {
            get { return m_SetAllSentimentPhrases; }
            set { if (SetSalienceOption(4005, value, m_sCurrentOptionSession) == 0) m_SetAllSentimentPhrases = value; }
        }

        private float m_EmphaticModifier;
        /// <summary>
        /// Enables the user to customize the sentiment modifier for emphatic sentences such as sentences with exclamations or repeated phrases.
        /// By default, this option is set to 4.0
        /// </summary>
        /// <remarks>Added in Salience 5.1.1</remarks>
        public float EmphaticModifier
        {
            get { return m_EmphaticModifier; }
            set { if (SetSalienceOption(4006, value, m_sCurrentOptionSession) == 0) m_EmphaticModifier = value; }
        }

        private float m_SuperlativeModifier;
        /// <summary>
        /// Enables the user to customize the sentiment modifier for superlatives
        /// By default, this option is set to 1.25
        /// </summary>
        /// <remarks>Added in Salience 5.1.1</remarks>
        public float SuperlativeModifier
        {
            get { return m_SuperlativeModifier; }
            set { if (SetSalienceOption(4007, value, m_sCurrentOptionSession) == 0) m_SuperlativeModifier = value; }
        }

        private bool m_ChainEntitySentiment;
        /// <summary>
        /// Reverts to the former entity chain sentiment algorithm, from the newer parse entity sentiment algorithm.
        /// By default this option is off.
        /// </summary>
        /// <remarks> Added in Salience 6.0 </remarks>
        public bool ChainEntitySentiment
        {
            get { return m_ChainEntitySentiment; }
            set { if (SetSalienceOption(4008, value, m_sCurrentOptionSession) == 0) m_ChainEntitySentiment = value; }
        }
        #endregion // Sentiment options

        #region Topic options
        private bool m_TopicStemming;
        /// <summary>Boolean property indicating whether stemming should be applied to topic queries</summary>
        /// <remarks>This property replaces the TagStemming property introduced in SE4.</remarks>
        public bool TopicStemming
        {
            get { return m_TopicStemming; }
            set { if (SetSalienceOption(5000, value, m_sCurrentOptionSession) == 0) m_TopicStemming = value; }
        }

        private string m_TopicList;
        /// <summary>String property specifying a file containing topic queries.</summary>
        /// <remarks>This property replaces the SE4 method SetTagDefinitions.</remarks>
        public string TopicList
        {
            get { return m_TopicList; }
            set { if (SetSalienceOption(5001, value, m_sCurrentOptionSession) == 0) m_TopicList = value; }        
        }

        private int m_MaxTopicLength;

        /// <summary> Integer property representing maximum length (in bytes) of a single topic query.</summary>
        /// <remarks> Introduced in a 5.2 update</remarks> 
        public int MaxTopicLength
        {
            get { return m_MaxTopicLength; }
            set { if (SetSalienceOption(5002, value, m_sCurrentOptionSession) == 0) m_MaxTopicLength = value; }
        }

        private bool m_TopicIgnoreAccents;
        /// <summary>Boolean property indicating whether accents on letters should be ignored in topic queries</summary>
        public bool TopicIgnoreAccents
        {
            get { return m_TopicIgnoreAccents; }
            set { if (SetSalienceOption(5003, value, m_sCurrentOptionSession) == 0) m_TopicIgnoreAccents = value; }
        }

        #endregion // Topic options
        
        #region Classification options
        private int m_ClassificationTreshold;

        /// <summary>
        /// Value from 1 to 100 controlling the threshold classification results must meet  .
        /// </summary>
        public int ClassificationThreshold
        {
            get { return m_ClassificationTreshold; }
            set { if (SetSalienceOption(6500, value, m_sCurrentOptionSession) == 0) m_ClassificationTreshold = value; }
        }

        private string m_ClassificationModel;
        /// <summary>
        /// File path to classification model.
        /// </summary>
        public string ClassificationModel
        {
            get { return m_ClassificationModel; }
            set { if (SetSalienceOption(6501, value, 0, m_sCurrentOptionSession) == 0) m_ClassificationModel = value; }
        }

        public void ClassificationModelReset()
        {
            SetSalienceOption(6501, "", 1, m_sCurrentOptionSession); 
        }

        #endregion

        #region Theme options
        private bool m_ThemeTopics;
        /// <summary>
        /// Boolean flag to control whether of not to calculate topic matches for themes.
        /// </summary>
        /// <remarks>Added in Salience 5.1</remarks>
        public bool ThemeTopics
        {
            get { return m_ThemeTopics; }
            set { if (SetSalienceOption(7000, value, m_sCurrentOptionSession) == 0) m_ThemeTopics = value; }
        }


    #endregion //Theme options

        #region Category Options

        private bool m_ExplainCategories;

        public bool ExplainCategories
        {
            get { return m_ExplainCategories; }
            set { if (SetSalienceOption(8000, value, m_sCurrentOptionSession) == 0) m_ExplainCategories = value; }
        }

        #endregion

        #region Summary Options

        private string m_SummaryDelimiter;
        /// <summary>String property specifying the delimiter for summaries. Defaults to "..."</summary>
        /// <remarks>Added in Salience 5.2</remarks>
        public string SummaryDelimiter
        {
            get { return m_SummaryDelimiter; }
            set { if (SetSalienceOption(9001, value, m_sCurrentOptionSession) == 0) m_SummaryDelimiter = value; }
        }

        private bool m_SummaryQuotesIntact;
        /// <summary>Boolean property specifying whether quotations should be kept together when displayed in a summary. Default value is false.</summary>
        /// <remarks>Added in Salience 5.2</remarks>
        public bool SummaryQuotesIntact
        {
            get { return m_SummaryQuotesIntact; }
            set { if (SetSalienceOption(9002, value, m_sCurrentOptionSession) == 0) m_SummaryQuotesIntact = value; }
        }

        private int m_SummaryMinSentenceLength;
        /// <summary>Exclude sentences with fewer than this many tokens from consideration. Defaults to 4.</summary>
        /// <remarks>Added in Salience 5.2</remarks>
        public int SummaryMinSentenceLength
        {
            get { return m_SummaryMinSentenceLength; }
            set { if (SetSalienceOption(9003, value, m_sCurrentOptionSession) == 0) m_SummaryMinSentenceLength = value; }
        }

        private bool m_SummaryAllowInitialConjunction;
        /// <summary>Whether to allow sentences into summary that begin with a conjunction. Defaults to true.</summary>
        /// <remarks>Added in Salience 5.2</remarks>
        public bool SummaryAllowInitialConjunction
        {
            get { return m_SummaryAllowInitialConjunction; }
            set { if (SetSalienceOption(9004, value, m_sCurrentOptionSession) == 0) m_SummaryAllowInitialConjunction = value; }
        }

        private int m_SummaryEarlySentenceCount;
        /// <summary>Number of sentences considered \"early\" when generating summaries. Defaults to 5.</summary>
        /// <remarks>Added in Salience 5.2</remarks>
        public int SummaryEarlySentenceCount
        {
            get { return m_SummaryEarlySentenceCount; }
            set { if (SetSalienceOption(9005, value, m_sCurrentOptionSession) == 0) m_SummaryEarlySentenceCount = value; }
        }

        private float m_SummaryEarlySentenceBonus;
        /// <summary>Bonus to early sentences when generating summaries. Defaults to .8.</summary>
        /// <remarks>Added in Salience 5.2</remarks>
        public float SummaryEarlySentenceBonus
        {
            get { return m_SummaryEarlySentenceBonus; }
            set { if (SetSalienceOption(9006, value, m_sCurrentOptionSession) == 0) m_SummaryEarlySentenceBonus = value; }
        }

        private float m_SummaryPronounPenalty;
        /// <summary>Penalty for using pronouns in a sentence when selecting them for summaries. Defaults to 0.</summary>
        /// <remarks>Added in Salience 5.2</remarks>
        public float SummaryPronounPenalty
        {
            get { return m_SummaryPronounPenalty; }
            set { if (SetSalienceOption(9007, value, m_sCurrentOptionSession) == 0) m_SummaryPronounPenalty = value; }
        }

        private int m_SummaryIdealLength;
        /// <summary>Target summary sentence length. Defaults to 0 (meaning off).</summary>
        /// <remarks>Added in Salience 5.2</remarks>
        public int SummaryIdealLength
        {
            get { return m_SummaryIdealLength; }
            set { if (SetSalienceOption(9008, value, m_sCurrentOptionSession) == 0) m_SummaryIdealLength = value; }
        }

        private float m_SummaryLengthPenalty;
        /// <summary>Penalty for deviating from target summary length. Defaults to 0.</summary>
        /// <remarks>Added in Salience 5.2</remarks>
        public float SummaryLengthPenalty
        {
            get { return m_SummaryLengthPenalty; }
            set { if (SetSalienceOption(9009, value, m_sCurrentOptionSession) == 0) m_SummaryLengthPenalty = value; }
        }

        private float m_SummaryDiversity;
        /// <summary>How strongly to encourage uniqueness in the sentences in a summary. Defaults to 0.5.</summary>
        /// <remarks>Added in Salience 5.2</remarks>
        public float SummaryDiversity
        {
            get { return m_SummaryDiversity; }
            set { if (SetSalienceOption(9010, value, m_sCurrentOptionSession) == 0) m_SummaryDiversity = value; }
        }

        #endregion

        #region MANAGED .NET LAYER
        #region Constructors

        /// <summary>
        /// Standard constructor, requires path to license file and path to data directory
        /// </summary>
        /// <param name="sLicensePath">Path to a valid Lexalytics license file</param>
        /// <param name="sDataPath">Path to data directory to use for this session</param>
        /// <exception cref="SalienceException">
        /// Thrown if license file cannot be loaded or error in loading data directory (AccessViolationException), or unable to load underlying Salience Engine (DllNotFoundException)
        /// </exception>
        public Salience6Engine(string sLicensePath, string sDataPath)
        {
            SalienceStartup oStartup = new SalienceStartup() { sDataDirectory = sDataPath, sUserDirectory = sDataPath + "/user", sLogPath = String.Empty, nMode = SALIENCE_MODE.DEFAULT };
            m_pSessionHandle = IntPtr.Zero;
            m_pLicense = IntPtr.Zero;
            int nRet;
            try
            {
                nRet = lxaLoadLicense(sLicensePath, ref m_pLicense);
                openSalienceSession(oStartup);
            }
            catch (AccessViolationException e)
            {
                throw new SalienceException("Could not load license at " + sLicensePath, e);
            }
            catch (DllNotFoundException e)
            {
                throw new SalienceException("Unable to load Salience6.dll. This file must be in the same directory as the application.", e);
            }
            SetInternalCallback("");
            //Check nError.
        }

        /// <summary>
        /// Standard constructor, requires path to license file and path to data directory
        /// </summary>
        /// <param name="sLicensePath">Path to a valid Lexalytics license file</param>
        /// <param name="sDataPath">Path to data directory to use for this session</param>
        /// <param name="sUserDir">Path to a user directory of customizations to use for this session</param>
        /// <exception cref="SalienceException">
        /// Thrown if license file cannot be loaded or error in loading data directory (AccessViolationException), or unable to load underlying Salience Engine (DllNotFoundException)
        /// </exception>
        public Salience6Engine(string sLicensePath, string sDataPath, string sUserDir)
        {
            SalienceStartup oStartup = new SalienceStartup() { sDataDirectory = sDataPath, sUserDirectory = sUserDir, sLogPath = String.Empty, nMode = SALIENCE_MODE.DEFAULT };
            m_pSessionHandle = IntPtr.Zero;
            m_pLicense = IntPtr.Zero;
            int nRet;
            try
            {
                nRet = lxaLoadLicense(sLicensePath, ref m_pLicense);
                openSalienceSession(oStartup);
            }
            catch (AccessViolationException e)
            {
                throw new SalienceException("Could not load license at " + sLicensePath, e);
            }
            catch (DllNotFoundException e)
            {
                throw new SalienceException("Unable to load Salience6.dll. This file must be in the same directory as the application.", e);
            }
            SetInternalCallback("");
            //Check nError.
        }

        /// <summary>
        /// Standard constructor, requires path to license file and path to data directory
        /// </summary>
        /// <param name="sLicensePath">Path to a valid Lexalytics license file</param>
        /// <param name="sDataPath">Path to data directory to use for this session</param>
        /// <param name="sUserDir">Path to a user directory of customizations to use for this session</param>
        /// <param name="sLogPath">Path to a log file to write session in itialization information to</param>
        /// <exception cref="SalienceException">
        /// Thrown if license file cannot be loaded or error in loading data directory (AccessViolationException), or unable to load underlying Salience Engine (DllNotFoundException)
        /// </exception>
        public Salience6Engine(string sLicensePath, string sDataPath, string sUserDir, string sLogPath)
        {
            SalienceStartup oStartup = new SalienceStartup() { sDataDirectory = sDataPath, sUserDirectory = sUserDir, sLogPath = sLogPath, nMode = SALIENCE_MODE.DEFAULT };
            m_pSessionHandle = IntPtr.Zero;
            m_pLicense = IntPtr.Zero;
            int nRet;
            try
            {
                nRet = lxaLoadLicense(sLicensePath, ref m_pLicense);
                openSalienceSession(oStartup);
            }
            catch (AccessViolationException e)
            {
                throw new SalienceException("Could not load license at " + sLicensePath, e);
            }
            catch (DllNotFoundException e)
            {
                throw new SalienceException("Unable to load Salience6.dll. This file must be in the same directory as the application.", e);
            }
            SetInternalCallback("");
            //Check nError.
        }

        /// <summary>
        /// Standard constructor, requires path to license file and path to data directory
        /// </summary>
        /// <param name="sLicensePath">Path to a valid Lexalytics license file</param>
        /// <param name="sDataPath">Path to data directory to use for this session</param>
        /// <param name="sUserDir">Path to a user directory of customizations to use for this session</param>
        /// <param name="sLogPath">Path to a log file to write session in itialization information to</param>
        /// <param name="nSalienceMode">Mode switch for Salience session</param>
        /// <exception cref="SalienceException">
        /// Thrown if license file cannot be loaded or error in loading data directory (AccessViolationException), or unable to load underlying Salience Engine (DllNotFoundException)
        /// </exception>
        public Salience6Engine(string sLicensePath, string sDataPath, string sUserDir, string sLogPath, SALIENCE_MODE nSalienceMode)
        {
            SalienceStartup oStartup = new SalienceStartup() { sDataDirectory = sDataPath, sUserDirectory = sUserDir, sLogPath = sLogPath, nMode = nSalienceMode};
            m_pSessionHandle = IntPtr.Zero;
            m_pLicense = IntPtr.Zero;
            int nRet;
            try
            {
                nRet = lxaLoadLicense(sLicensePath, ref m_pLicense);
                openSalienceSession(oStartup);
            }
            catch (AccessViolationException e)
            {
                throw new SalienceException("Could not load license at " + sLicensePath, e);
            }
            catch (DllNotFoundException e)
            {
                throw new SalienceException("Unable to load Salience6.dll. This file must be in the same directory as the application.", e);
            }
            SetInternalCallback("");
            //Check nError.
        }

        /// <summary>
        /// Standard constructor, requires path to license file and path to data directory
        /// </summary>
        /// <param name="sLicensePath">Path to a valid Lexalytics license file</param>
        /// <param name="oStartup">Startup object giving other session startup parameters</param>
        /// <exception cref="SalienceException">
        /// Thrown if license file cannot be loaded or error in loading data directory (AccessViolationException), or unable to load underlying Salience Engine (DllNotFoundException)
        /// </exception>
        public Salience6Engine(string sLicensePath, SalienceStartup oStartup)
        {
            m_pSessionHandle = IntPtr.Zero;
            m_pLicense = IntPtr.Zero;
            int nRet;
            try
            {
                nRet = lxaLoadLicense(sLicensePath, ref m_pLicense);
                openSalienceSession(oStartup);
            }
            catch (AccessViolationException e)
            {
                throw new SalienceException("Could not load license at " + sLicensePath, e);
            }
            catch (DllNotFoundException e)
            {
                throw new SalienceException("Unable to load Salience6.dll. This file must be in the same directory as the application.", e);
            }
            SetInternalCallback("");
            //Check nError.
        }

    #endregion

    #region Destructor
        ~Salience6Engine()
        {
            Dispose(false);
        }
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Code to dispose the managed resources of the class
            }
            // Code to dispose the un-managed resources of the class
            if (this.m_pSessionHandle != IntPtr.Zero)
            {
                lxaCloseSalienceSession(this.m_pSessionHandle);
                this.m_pSessionHandle = IntPtr.Zero;
            }
            if (this.m_pLicense != IntPtr.Zero)
            {
                lxaFreeLicense(ref m_pLicense);
            }

            m_bDisposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
           // GC.SuppressFinalize(this);
        }
    #endregion

    #region Utility methods

        public void AddSalienceConfiguration(String userDir, String configurationID)
        {
            processAddSalienceConfiguration(userDir, configurationID);
        }

        public void RemoveSalienceConfiguration(String configurationID)
        {
            processRemoveSalienceConfiguration(configurationID);
        }

        protected static string GetDefaultSalienceLocation()
        {
            return getDefaultSalienceLocation();
        }

        /// <summary>
        /// Wrapper method to return the Salience Engine version
        /// </summary>
        /// <returns>String with Salience Engine version information</returns>
        /// <remarks>Wrapper around C API method lxaGetSalienceVersion</remarks>
        /// <exception cref="SalienceException">Thrown if version information is not available</exception>
        protected string GetSalienceVersion()
        {
            return getVersion();
        }

        /// <summary>
        /// Wrapper method to return information about the Salience Engine environment
        /// </summary>
        /// <returns>String with Salience Engine environment information</returns>
        /// <remarks>Wrapper around C API method lxaDumpEnvironment</remarks>
        protected string DumpEnvironment()
        {
            return dumpEnvironment();
        }

        /// <summary>
        /// Returns an integer error code indicating last warning encountered by Salience Engine, generally in the process of text preparation
        /// </summary>
        /// <returns>Integer return code</returns>
        /// <remarks>Wrapper around C API method lxaGetLastWarnings</remarks>
        protected int GetLastWarnings()
        {
            int nRet;
            lxaGetLastWarnings(m_pSessionHandle, out nRet);
            return nRet;
        }

        /// <summary>
        /// Adds a hand-scored dictionary to the engine for use in sentiment analysis.
        /// </summary>
        /// <param name="sDictionary">Path to a hand-scored dictionary, or string contents of a hand-scored dictionary.</param>
        /// <param name="bReset">Set to true to reset dictionaries and only use this one, set to false to add dictionary to others loaded.</param>
        /// <returns>Integer return code</returns>
        /// <remarks>Equivalent to SE4 method lxaAddSentimentDictionary, which is now an option that takes an additional parameter.</remarks>
        protected int AddSentimentDictionary(string sDictionary, bool bReset, string sConfig)
        {
            int nValue = 0;
            if (bReset)
                nValue = 1;

            return SetSalienceOption(4000, sDictionary, nValue, sConfig);
        }

        /// <summary>
        /// Sets a Salience Engine option which takes an integer value
        /// </summary>
        /// <param name="nOption">Option constant as defined in SalienceOptions.h</param>
        /// <param name="nValue">Applicable integer value for the option</param>
        /// <exception cref="SalienceException">Thrown if invalid option specified or invalid value</exception>
        /// <returns>0 on success, otherwise throws SalienceException</returns>
        /// <remarks>Wrapper around C API method lxaSetSalienceOption</remarks>
        protected int SetSalienceOption(int nOption, int nValue, String id="")
        {
            int nRet = setSalienceOption(nOption, nValue, id);
            if (nRet == 0)
            {
                return nRet;
            }
            else
            {
                string errMessage = String.Format("Error setting option #{0}", nOption);
                switch (nRet)
                {
                    case 4:
                        errMessage += "Invalid parameter provided when setting Salience option.";
                        break;
                    case 12:
                        errMessage += "Option not supported in Salience options.";
                        break;
                    default:
                        break;
                }
                SalienceException errOption = new SalienceException(errMessage);
                throw errOption;
            }
        }

        /// <summary>
        /// Sets a Salience Engine option which takes an boolean value
        /// </summary>
        /// <param name="nOption">Option constant as defined in SalienceOptions.h</param>
        /// <param name="nValue">Applicable boolean value for the option</param>
        /// <exception cref="SalienceException">Thrown if invalid option specified or invalid value</exception>
        /// <returns>0 on success, otherwise throws SalienceException</returns>
        /// <remarks>Wrapper around C API method lxaSetSalienceOption</remarks>
        protected int SetSalienceOption(int nOption, bool bValue, String id = "")
        {
            int nRet = setSalienceOption(nOption, bValue, id);
            if (nRet == 0)
            {
                return nRet;
            }
            else
            {
                string errMessage = String.Format("Error setting option #{0}", nOption);
                switch (nRet)
                {
                    case 4:
                        errMessage += "Invalid parameter provided when setting Salience option.";
                        break;
                    case 12:
                        errMessage += "Option not supported in Salience options.";
                        break;
                    default:
                        break;
                }
                SalienceException errOption = new SalienceException(errMessage);
                throw errOption;
            }
        }

        /// <summary>
        /// Sets a Salience Engine option which takes a float value
        /// </summary>
        /// <param name="nOption">Option constant as defined in SalienceOptions.h</param>
        /// <param name="nValue">Applicable float value for the option</param>
        /// <exception cref="SalienceException">Thrown if invalid option specified or invalid value</exception>
        /// <returns>0 on success, otherwise throws SalienceException</returns>
        /// <remarks>Wrapper around C API method lxaSetSalienceOption</remarks>
        protected int SetSalienceOption(int nOption, float fValue, String id = "")
        {
            int nRet = setSalienceOption(nOption, fValue, id);
            if (nRet == 0)
            {
                return nRet;
            }
            else
            {
                string errMessage = String.Format("Error setting option #{0}", nOption);
                switch (nRet)
                {
                    case 4:
                        errMessage += "Invalid parameter provided when setting Salience option.";
                        break;
                    case 12:
                        errMessage += "Option not supported in Salience options.";
                        break;
                    default:
                        break;
                }
                SalienceException errOption = new SalienceException(errMessage);
                throw errOption;
            }
        }

        /// <summary>
        /// Sets a Salience Engine option which takes a character string value
        /// </summary>
        /// <param name="nOption">Option constant as defined in SalienceOptions.h</param>
        /// <param name="sValue">String value for the option</param>
        /// <exception cref="SalienceException">Thrown if invalid option specified or invalid value</exception>
        /// <returns>0 on success, otherwise throws SalienceException</returns>
        /// <remarks>Wrapper around C API method lxaSetSalienceOption</remarks>
        protected int SetSalienceOption(int nOption, string sValue, String id = "")
        {
            int nRet = setSalienceOption(nOption, sValue, id);
            if (nRet == 0)
            {
                return nRet;
            }
            else
            {
                string errMessage = String.Format("Error setting option #{0}", nOption);
                switch (nRet)
                {
                    case 4:
                        errMessage += "Invalid parameter provided when setting Salience option.";
                        break;
                    case 12:
                        errMessage += "Option not supported in Salience options.";
                        break;
                    default:
                        break;
                }
                SalienceException errOption = new SalienceException(errMessage);
                throw errOption;
            }
        }

        /// <summary>
        /// Sets a Salience Engine option which takes a character string value and integer value
        /// </summary>
        /// <param name="nOption">Option constant as defined in SalienceOptions.h</param>
        /// <param name="sValue">String value for the option</param>
        /// <param name="nValue">Integer value for the option</param>
        /// <exception cref="SalienceException">Thrown if invalid option specified or invalid value</exception>
        /// <returns>0 on success, otherwise throws SalienceException</returns>
        /// <remarks>Wrapper around C API method lxaSetSalienceOption</remarks>
        protected int SetSalienceOption(int nOption, string sValue, int nValue, String id = "")
        {
            int nRet = setSalienceOption(nOption, sValue, nValue, id);
            if (nRet == 0)
            {
                return nRet;
            }
            else
            {
                string errMessage = String.Format("Error setting option #{0}", nOption);
                switch (nRet)
                {
                    case 4:
                        errMessage += "Invalid parameter provided when setting Salience option.";
                        break;
                    case 12:
                        errMessage += "Option not supported in Salience options.";
                        break;
                    default:
                        break;
                }
                SalienceException errOption = new SalienceException(errMessage);
                throw errOption;
            }
        }


    #endregion // Utility methods

    #region Text preparation methods

        /// <summary>
        /// Wrapper method to submit text to Salience Engine for processing
        /// </summary>
        /// <param name="sText">The text to be processed</param>
        /// <returns>Integer error return code</returns>
        /// <remarks>Wrapper around C API method lxaPrepareText</remarks>
        /// <exception cref="SalienceException">Thrown if text cannot be parsed</exception>
        protected int PrepareText(string sText)
        {
            int nRet = processPrepareText(sText);
            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }
            return nRet;
        }

        /// <summary>
        /// Wrapper method to submit contents of a file to Salience Engine for processing
        /// </summary>
        /// <param name="sFile">Full path to a readable text file</param>
        /// <returns>Integer error return code</returns>
        /// <remarks>Wrapper around C API method lxaPrepareTextFromFile</remarks>
        /// <exception cref="SalienceException">Thrown if file cannot be found or text cannot be parsed</exception>
        protected int PrepareTextFromFile(string sFile)
        {
            int nRet = processPrepareTextFromFile(sFile);
            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }
            return nRet;
        }

        /// <summary>
        /// Wrapper method to add text to Salience Engine for section processing
        /// </summary>
        /// <param name="sHeader">Header or title for the text to be processed</param>
        /// <param name="sText">The text to be processed</param>
        /// <param name="bProcess">Whether to use this text for processing or not</param>
        /// <returns>Integer error return code</returns>
        /// <remarks>Wrapper around C API method lxaAddSection</remarks>
        /// <exception cref="SalienceException">Thrown if text cannot be parsed</exception>
        protected int AddSection(string sHeader, string sText, bool bProcess)
        {
            int nRet = processAddSection(sHeader, sText, bProcess);
            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }
            return nRet;
        }

        /// <summary>
        /// Wrapper method to add contents of a file to Salience Engine for section processing
        /// </summary>
        /// <param name="sHeader">Header or title for the file to be processed</param>
        /// <param name="sFile">Full path to a readable text file</param>
        /// <param name="bProcess">Whether to use this file for processing or not</param>
        /// <returns>Integer error return code</returns>
        /// <remarks>Wrapper around C API method lxaAddSectionFromFile</remarks>
        /// <exception cref="SalienceException">Thrown if file cannot be found or text cannot be parsed</exception>
        protected int AddSectionFromFile(string sHeader, string sFile, bool bProcess)
        {
            int nRet = processAddSectionFromFile(sHeader, sFile, bProcess);
            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }
            return nRet;
        }

        /// <summary>
        /// Creates a collection from a list of strings
        /// </summary>
        /// <returns>Integer error return code</returns>
        /// <remarks>Wrapper around C API method lxaPrepareCollection</remarks>
        /// <exception cref="SalienceException">Thrown if text cannot be parsed</exception>
        protected int PrepareCollectionFromList(String Name, List<String> lstText)
        {
            List<SalienceCollectionDocument> CollectionDocuments = new List<SalienceCollectionDocument>();
            for (int i = 0; i < lstText.Count; i++)
            {
                CollectionDocuments.Add(new SalienceCollectionDocument() { sIdentifier = i.ToString(), sText = lstText[i] });
            }
            int nRet = processPrepareCollectionFromList(Name, CollectionDocuments);
            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }
            return nRet;
        }

        /// <summary>
        /// Creates a collection from a file containing content
        /// </summary>
        /// <param name="sName">Name for the collection</param>
        /// <param name="sFile">Full path to a readable text file</param>
        /// <returns>Integer error return code</returns>
        /// <remarks>Wrapper around C API method lxaPrepareCollectionFromFile</remarks>
        /// <exception cref="SalienceException">Thrown if text cannot be parsed</exception>
        protected int PrepareCollectionFromFile(string sName, string sFile)
        {
            int nRet = processPrepareCollectionFromFile(sName, sFile);
            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }
            return nRet;
        }

    #endregion // Text preparation methods

    #region Details methods
        /// <summary>
        /// Wrapper method to return details about current document
        /// </summary>
        /// <returns>SalienceDocumentDetails object</returns>
        /// <remarks>Wrapper around C API method lxaGetDocumentDetails</remarks>
        public SalienceDocumentDetails GetDocumentDetails(String id="")
        {
            return processDocumentDetails(id);
        }

        /// <summary>
        /// Wrapper method to return details about the current collection
        /// </summary>
        /// <returns>SalienceCollectionDetails object</returns>
        /// <remarks>Wrapper around C API method lxaGetCollectionDetails</remarks>
        public SalienceCollectionDetails GetCollectionDetails(String id = "")
        {
            return processCollectionDetails(id);
        }

    #endregion // Details methods

    #region Document methods
        /// <summary>
        /// Returns a summary for the document
        /// </summary>
        /// <param name="nLength">The length in sentences that the summary should be.</param>
        /// <returns>A text string containing a summary of the document up to nLength sentences.</returns>
        /// <remarks>Wrapper around C API method lxaGetSummary</remarks>
        protected SalienceSummary GetSummary(int nLength, String id = "")
        {
            return processSummary(nLength, id);
        }

        /// <summary>
        /// Returns the sentiment analysis for the document
        /// </summary>
        /// <param name="bUseChains">A boolean indicating whether or not to use lexical chains for sentiment phrase analysis</param>
        /// <returns>A SalienceSentiment object containing members with sentiment results</returns>
        /// <remarks>Wrapper around C API method lxaGetSentiment</remarks>
        protected SalienceSentiment GetDocumentSentiment(bool bUseChains, String id = "")
        {
            return processDocumentSentiment(bUseChains, id);
        }

        /// <summary>
        /// Returns a list of themes extracted from the document
        /// </summary>
        /// <returns>A List of SalienceTheme objects, each containing information about a theme extracted from the document</returns>
        /// <remarks>Wrapper around C API method lxaGetThemes</remarks>
        protected List<SalienceTheme> GetDocumentThemes(String id = "")
        {
            return processDocumentThemes(id);
        }

        /// <summary>
        /// Returns a list of intentions expressed in the document
        /// </summary>
        /// <returns>A List of SalienceIntention objects, each containing informatio about an intention expressed in the document</returns>
        /// <remarks>Wrapper around C API method lxaGetDocumentIntentions</remarks>
        protected List<SalienceIntention> GetDocumentIntentions(String id = "")
        {
            return processDocumentIntentions(id);
        }

        /// <summary>
        /// Returns a list of topics extracted from the document via tag queries
        /// </summary>
        /// <returns>A List of SalienceTopic objects, each containing information about a topic extracted from the document through the tag queries specified for the session</returns>
        /// <remarks>Wrapper around C API method lxaGetQueryDefinedTopics</remarks>
        protected List<SalienceTopic> GetQueryDefinedTopics(String id = "")
        {
            return processDocumentTopics(id);
        }

        /// <summary>
        /// Returns a list of classifications of the current document.
        /// </summary>
        /// <returns>A List of SalienceTopic objects, each containing information about a class extracted from the document through the classification model</returns>
        /// <remarks>Wrapper around C API method lxaGetDocumentClasses</remarks>
        protected List<SalienceTopic> GetDocumentClasses(String id = "")
        {
            return processDocumentClasses(id);
        }

        /// <summary>
        /// Returns a list of topics extracted from the document via the concept matrix
        /// </summary>
        /// <returns>A List of SalienceTopic objects, each containing information about a topic identified for the document through the use of the concept matrix</returns>
        /// <remarks>Wrapper around C API method lxaGetConceptDefinedTopics</remarks>
        protected List<SalienceTopic> GetConceptDefinedTopics(String id = "")
        {
            return processDocumentConceptTopics(id);
        }

        /// <summary>
        /// Returns a list of category topics extracted from the document via a topic model.
        /// </summary>
        /// <returns>A List of SalienceTopic objects, each containing information about a topic identified for the document</returns>
        /// <remarks>Wrapper around C API method lxaGetConceptDefinedTopics</remarks>
        protected List<SalienceTopic> GetDocumentCategories(String id = "")
        {
            return processDocumentCategories(id);
        }

        /// <summary>
        /// Returns a string listing the top contributing words and phrases from the current document for each Concept Topic Match.
        /// </summary>
        /// <returns>A text string with concept topic results, and contributing words/phrases.</returns>
        /// <remarks>Wrapper around C API method lxaExplainConceptMatches</remarks>
        protected string ExplainConceptMatches(String id = "")
        {
            return processExplainConceptMatches(id);
        }

    #endregion // Document methods

    #region Entity methods
        /// <summary>
        /// Returns a list of named entities extracted from the document.
        /// This is equivalent to the SE4.4 method GetEntities.
        /// </summary>
        /// <param name="Params">A structure with entity extraction options</param>
        /// <returns>A List of SalienceEntity objects, each containing information about a named entity</returns>
        /// <remarks>Wrapper around C API method lxaGetNamedEntities</remarks>
        protected List<SalienceEntity> GetNamedEntities(SalienceEntityParams Params, String id = "")
        {
            EntityThreshold = Params.nRequiredThreshold;
            EntitySummaryLength = Params.nSummaryLength;
            return GetNamedEntities(id);
        }

        /// <summary>
        /// Returns a list of named entities extracted from the document.
        /// This is equivalent to the SE4.4 method GetEntities.
        /// </summary>
        /// <returns>A List of SalienceEntity objects, each containing information about a named entity</returns>
        /// <remarks>Wrapper around C API method lxaGetNamedEntities</remarks>
        protected List<SalienceEntity> GetNamedEntities(String id = "")
        {
            return processNamedEntities(id);
        }

        /// <summary>
        /// Returns a list of user-defined entities extracted from the document.
        /// This is equivalent to the SE4.4 method GetSimpleEntities and GetQueryDefinedEntities.
        /// </summary>
        /// <returns>A List of SalienceEntity objects, each containing information about a user-defined entity</returns>
        /// <remarks>Wrapper around C API method lxaGetUserDefinedEntities</remarks>
        protected List<SalienceEntity> GetUserDefinedEntities(String id = "")
        {
            return processUserDefinedEntities(id);
        }

        /// <summary>
        /// Returns a list of relationships for named entities extracted from the document.
        /// This is equivalent to the SE4.4 method GetEntityRelationships.
        /// </summary>
        /// <returns>A List of SalienceRelationship objects, each containing information about a relationship between multiple named entities</returns>
        /// <remarks>Wrapper around C API method lxaGetNamedEntityRelationships</remarks>
        protected List<SalienceRelationship> GetNamedEntityRelationships(int nThreshold, String id = "")
        {
            EntityThreshold = nThreshold;
            return processRelationships(id);
        }

        /// <summary>
        /// Returns a list of opinions for named entities extracted from the document.
        /// This is equivalent to the SE4.4 method GetEntityOpinions.
        /// </summary>
        /// <returns>A List of SalienceOpinion objects, each containing information about an opinion related to a named entity</returns>
        /// <remarks>Wrapper around C API method lxaGetNamedEntityOpinions</remarks>
        protected List<SalienceOpinion> GetNamedEntityOpinions(int nThreshold, String id = "")
        {
            EntityThreshold = nThreshold;
            return processOpinions(id);
        }

        /// <summary>
        /// Returns a list of relationships for user-defined entities extracted from the document.
        /// This is equivalent to the SE4.4 method GetEntityRelationships for named entities, but for simple and query-defined entities.
        /// </summary>
        /// <returns>A List of SalienceRelationship objects, each containing information about a relationship between multiple user-defined entities</returns>
        /// <remarks>Wrapper around C API method lxaGetUserEntityRelationships</remarks>
        protected List<SalienceRelationship> GetUserEntityRelationships(String id = "")
        {
            return processUserEntityRelationships(id);
        }

        /// <summary>
        /// Returns a list of opinions for user-defined entities extracted from the document.
        /// This is equivalent to the SE4.4 method GetEntityOpinions for named entities, but for simple and query-defined entities.
        /// </summary>
        /// <returns>A List of SalienceOpinion objects, each containing information about an opinion related to a user-defined entity</returns>
        /// <remarks>Wrapper around C API method lxaGetUserEntityOpinions</remarks>
        protected List<SalienceOpinion> GetUserEntityOpinions(String id = "")
        {
            return processUserEntityOpinions(id);
        }

    #endregion 

    #region Collection methods
        /// <summary>
        /// Returns a list of themes extracted from the collection
        /// </summary>
        /// <returns>A List of SalienceTheme objects, each containing information about a theme extracted from the collection</returns>
        /// <remarks>Wrapper around C API method lxaGetCollectionThemes</remarks>
        protected List<SalienceTheme> GetCollectionThemes(String id = "")
        {
            return processCollectionThemes(id);
        }

        /// <summary>
        /// Returns a list of entities extracted from the collection
        /// </summary>
        /// <returns>A List of SalienceEntity objects, each containing information about an entity extracted from the collection</returns>
        /// <remarks>Wrapper around C API method lxaGetCollectionEntities</remarks>
        protected List<SalienceCollectionEntity> GetCollectionEntities(String id = "")
        {
            return processCollectionEntities(id);
        }

        /// <summary>
        /// Returns a list of user entities extracted from the collection
        /// </summary>
        /// <returns>A List of SalienceEntity objects, each containing information about a user entity extracted from the collection</returns>
        /// <remarks>Wrapper around C API method lxaGetCollectionUserEntities</remarks>
        protected List<SalienceCollectionEntity> GetCollectionUserEntities(String id = "")
        {
            return processCollectionUserEntities(id);
        }

        /// <summary>
        /// Returns a list of facets extracted from the collection
        /// </summary>
        /// <returns>A List of SalienceFacet objects, each containing information about a facet and its attributes extracted from the collection</returns>
        /// <remarks>Wrapper around C API method lxaGetCollectionFacets</remarks>
        protected List<SalienceFacet> GetCollectionFacets(String id = "")
        {
            return processCollectionFacets(id);
        }

        /// <summary>
        /// Returns a list of topics extracted from the collection via tag query definitions.
        /// </summary>
        /// <returns>A List of SalienceTopic objects, each containing information about a topic identified for at least one document in the collection</returns>
        /// <remarks>Wrapper around C API method lxaGetCollectionQueryDefinedTopics</remarks>
        protected List<SalienceTopic> GetCollectionQueryDefinedTopics(String id = "")
        {
            return processCollectionQueryTopics(id);
        }

        /// <summary>
        /// Returns a list of topics extracted from the collection via the concept matrix.
        /// </summary>
        /// <returns>A List of SalienceTopic objects, each containing information about a topic identified for at least one document in the collection</returns>
        /// <remarks>Wrapper around C API method lxaGetCollectionConceptDefinedTopics</remarks>
        protected List<SalienceTopic> GetCollectionConceptTopics(String id = "")
        {
            return processCollectionConceptTopics(id);
        }

    #endregion // Collection methods

    #region Markup methods
        protected string GetEntityTaggedText(String id = "")
        {
            return processNamedEntityTaggedText(id);
        }

        protected string GetUserEntityTaggedText(String id = "")
        {
            return processUserEntityTaggedText(id);
        }

        protected string GetPOSTaggedText(String id="")
        {
            return processPOSTaggedText(id);
        }

        protected string GetSentimentTaggedText(String id = "")
        {
            return processSentimentTaggedText(-0.3f,0.3f,id);
        }

        protected string GetUserOpinionTaggedText(String id = "")
        {
            return processUserOpinionTaggedText(id);
        }

        protected string GetNamedOpinionTaggedText(String id = "")
        {
            return processNamedOpinionTaggedText(id);
        }

    #endregion // Markup methods

    #region Reinitialize methods

        public void ReinitializeThemes()
        {
            SetSalienceOption(1008, 1);
        }

    #endregion

#endregion MANAGED .NET LAYER

        #region UNMANAGED .NET TO C LAYER (INTERNAL IMPLEMENTATION)

        #region Session methods
        private unsafe void openSalienceSession(SalienceStartup oStartup)
        {
            umSalienceStartup umStartup = new umSalienceStartup();
            umStartup.acDataDirectory = oStartup.sDataDirectory;
            umStartup.acUserDirectory = oStartup.sUserDirectory;
            umStartup.nMode = (int)oStartup.nMode;

            if (oStartup.sLogPath != String.Empty)
            {
                umStartup.nStartupLog = 1;
                umStartup.acLogPath = Marshal.StringToHGlobalAnsi(oStartup.sLogPath);
            }
            
            int nError = lxaOpenSalienceSession(m_pLicense, ref umStartup, ref m_pSessionHandle);
            if (nError != 0)
            {
               throw new SalienceException(umStartup.acError);
            }
        }

        private unsafe void processAddSalienceConfiguration(String userDir, String configurationID)
        {
            IntPtr pMarshalledId = GetUTF8String(configurationID);
            IntPtr pMarshalledUserDir = GetUTF8String(userDir);
            lxaAddSalienceConfiguration(m_pSessionHandle, pMarshalledUserDir, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledUserDir);
        }

        private unsafe void processRemoveSalienceConfiguration(String configurationID)
        {
            IntPtr pMarshalledId = GetUTF8String(configurationID);
            lxaRemoveSalienceConfiguration(m_pSessionHandle, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);

        }

    #endregion // Session methods

    #region Text preparation methods
        private unsafe int processPrepareText(string sText)
        {
            IntPtr pMarshalled = GetUTF8String(sText);
            int nRet = lxaPrepareText(m_pSessionHandle, pMarshalled);
            Marshal.FreeHGlobal(pMarshalled);
            return nRet;
        }

        private unsafe int processPrepareTextFromFile(string sFile)
        {
            return lxaPrepareTextFromFile(m_pSessionHandle, sFile);
        }

        private unsafe int processAddSection(string sHeader, string sText, bool bProcess)
        {
            IntPtr pMarshalledHeader = GetUTF8String(sHeader);
            IntPtr pMarshalledText = GetUTF8String(sText);
            int nRet = lxaAddSection(m_pSessionHandle, pMarshalledHeader, pMarshalledText, bProcess ? 1 : 0);
            Marshal.FreeHGlobal(pMarshalledHeader);
            Marshal.FreeHGlobal(pMarshalledText);
            return nRet;
        }

        private unsafe int processAddSectionFromFile(string sHeader, string sFile, bool bProcess)
        {
            IntPtr pMarshalledHeader = GetUTF8String(sHeader);
            int nRet = lxaAddSectionFromFile(m_pSessionHandle, pMarshalledHeader, sFile, bProcess ? 1 : 0);
            Marshal.FreeHGlobal(pMarshalledHeader);
            return nRet;
        }

        private unsafe int processPrepareCollectionFromList(string sCollectionName, List<SalienceCollectionDocument> lstDocs)
        {
            Callback_Current = Callback_PrepareCollection;
            umSalienceCollection Collection = new umSalienceCollection();
            Collection.nSize = lstDocs.Count;
            IntPtr pArray = Marshal.AllocHGlobal(lstDocs.Count * Marshal.SizeOf(typeof(umSalienceCollectionDocument)));
            Collection.pDocuments = (umSalienceCollectionDocument*)pArray.ToPointer();
            Collection.acName = Marshal.StringToHGlobalAnsi(sCollectionName);
            for (int i = 0; i < Collection.nSize; i++)
            {
                Collection.pDocuments[i].acIndentifier = Marshal.StringToHGlobalAnsi(lstDocs[i].sIdentifier);
                Collection.pDocuments[i].acText = GetUTF8String(lstDocs[i].sText);
                Collection.pDocuments[i].nIsText = 1;
                Collection.pDocuments[i].nSplitByLine = 0;
            }
            int nRet = lxaPrepareCollection(m_pSessionHandle, &Collection);
            for (int i = 0; i < Collection.nSize; i++)
            {
                Marshal.FreeHGlobal(Collection.pDocuments[i].acIndentifier);
                Marshal.FreeHGlobal(Collection.pDocuments[i].acText);
            }
            Marshal.FreeHGlobal(Collection.acName);
            Marshal.FreeHGlobal(pArray);
            return nRet;
        }

        private unsafe int processPrepareCollectionFromFile(string sCollectionName, string sFile)
        {
            return lxaPrepareCollectionFromFile(m_pSessionHandle, sCollectionName, sFile);
        }
    #endregion // Text preparation methods

    #region Details methods
        private unsafe SalienceSection processDocumentSection(umSalienceSection* pSection)
        {
            SalienceSection ReturnDetails = new SalienceSection();
            ReturnDetails.sText = MarshalPtrToUtf8(pSection->acInternalRepresentation);
            ReturnDetails.sFingerprint = MarshalPtrToUtf8(pSection->acFingerprint);
            ReturnDetails.sHeader = MarshalPtrToUtf8(pSection->acHeader);
            ReturnDetails.nSentenceCount = pSection->nSentenceCount;
            ReturnDetails.nWordCount = pSection->nWordCount;
            ReturnDetails.TermFrequency = processTokens(pSection->oTermFrequency);
            ReturnDetails.TaggedTermFrequency = processTokens(pSection->oTaggedTermFrequency);
            ReturnDetails.BiGramFrequency = processTokens(pSection->oBiGrams);
            ReturnDetails.TaggedBiGramFrequency = processTokens(pSection->oTaggedBiGrams);
            ReturnDetails.TriGramFrequency = processTokens(pSection->oTriGrams);
            ReturnDetails.TaggedTriGramFrequency = processTokens(pSection->oTaggedTriGrams);
            ReturnDetails.QuadGramFrequency = processTokens(pSection->oQuadGrams);
            ReturnDetails.oNegators = processTokens(pSection->oNegators);
            ReturnDetails.oIntensifiers = processTokens(pSection->oIntensifiers);
            ReturnDetails.Chunks = processChunks(pSection->pSentences, pSection->nSentenceCount);
            ReturnDetails.Sentences = processSentences(pSection->pSentences, pSection->nSentenceCount);
            umSalienceDocument* pRow = (umSalienceDocument*)pSection->oRows.pRows;
            for (int i = 0; i < pSection->oRows.nRowCount; i++, pRow++)
            {
                //todo: process rows.
            }
            return ReturnDetails;
        }

        private unsafe SalienceDocumentDetails processDocumentDetails(String id)
        {
            umSalienceDocumentDetails DocDetails = new umSalienceDocumentDetails();
            IntPtr pMarshalledId = GetUTF8String(id);
            int nRet = lxaGetDocumentDetails(m_pSessionHandle, &DocDetails, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);
            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }
            SalienceDocumentDetails ReturnDetails = new SalienceDocumentDetails();
            umSalienceSection* pSection = (umSalienceSection*)DocDetails.oSections;
            ReturnDetails.Sections = new List<SalienceSection>();
            for (int i = 0; i < DocDetails.nSectionCount; i++, pSection++)
            {
                ReturnDetails.Sections.Add(processDocumentSection(pSection));
            }
            
            lxaFreeDocumentDetails(&DocDetails);

            return ReturnDetails;
        }

        private unsafe SalienceCollectionDetails processCollectionDetails(String id)
        {
            umSalienceCollectionDetails CollectionDetails = new umSalienceCollectionDetails();
            IntPtr pMarshalledId = GetUTF8String(id);
            int nRet = lxaGetCollectionDetails(m_pSessionHandle, &CollectionDetails, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);
            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }
            SalienceCollectionDetails ReturnDetails = new SalienceCollectionDetails();
            ReturnDetails.nSize = CollectionDetails.nSize;
            return ReturnDetails;
        }

        private unsafe List<SalienceToken> processTokens(umSalienceTokenList TokenList)
        {
            List<SalienceToken> ReturnTokens = new List<SalienceToken>(TokenList.nLength);
            umSalienceToken* pToken = TokenList.pTokens;
            for (int i = 0; i < TokenList.nLength; i++, pToken++)
            {
                SalienceToken Token = new SalienceToken();
                Token.nCount = pToken->nCount;
                Token.sToken = MarshalPtrToUtf8(pToken->acToken);
                ReturnTokens.Add(Token);
            }
            return ReturnTokens;
        }

        private unsafe SalienceChunk processChunk(umSalienceChunk oChunk)
        {
            SalienceChunk Result = new SalienceChunk();
            Result.nSentence = oChunk.nSentence;
            Result.sLabel = MarshalPtrToUtf8(oChunk.acLabel);
            Result.fSentiment = oChunk.fSentiment;

            Result.Tokens = new List<SalienceWord>(oChunk.nLength);
            umSalienceWord* pToken = oChunk.pTokens;
            for (int k = 0; k < oChunk.nLength; k++, pToken++)
            {
                SalienceWord Token = new SalienceWord();
                Token.bInvert = true;
                if (pToken->nInvert == 0)
                {
                    Token.bInvert = false;
                }
                Token.bPostFixed = true;
                if (pToken->nPostFixed == 0)
                {
                    Token.bPostFixed = false;
                }
                Token.fSentiment = pToken->fSentiment;
                Token.nID = pToken->nId;
                Token.nSecondaryID = pToken->nSecondaryId;
                Token.nSentimentType = pToken->nSentimentType;
                Token.sEntityType = MarshalPtrToUtf8(pToken->acEntityType);
                Token.sPOSTag = MarshalPtrToUtf8(pToken->acPOSTag);
                Token.sStem = MarshalPtrToUtf8(pToken->acStem);
                Token.sToken = MarshalPtrToUtf8(pToken->acToken);

                Result.Tokens.Add(Token);
            }
            return Result;
        }

        private unsafe List<SalienceChunk> processChunks(umSalienceSentence* pSentences, int nLength)
        {
            List<SalienceChunk> ReturnChunks = new List<SalienceChunk>(nLength);
            for (int i = 0; i < nLength; i++, pSentences++)
            {
                for (int j = 0; j < pSentences->nChunkCount; j++)
                {
                    ReturnChunks.Add(processChunk(pSentences->pChunks[j]));
                }
            }
            return ReturnChunks;
        }

        private unsafe List<SalienceSentence> processSentences(umSalienceSentence* pSentences, int nLength)
        {
            List<SalienceSentence> ReturnSentences = new List<SalienceSentence>(nLength);
            for (int i = 0; i < nLength; i++)
            {
                umSalienceSentence oSentence = pSentences[i];
                SalienceSentence Sentence = new SalienceSentence();
                Sentence.fSentiment = oSentence.fSentiment;
                Sentence.nImperative = oSentence.nImperative;
                Sentence.nSubjective = oSentence.nSubjective;
                Sentence.nSummaryRank = oSentence.nSummaryRank;
                Sentence.sText = MarshalPtrToUtf8(oSentence.acText);
                Sentence.nPolar = oSentence.nPolar;
                Sentence.Tokens = new List<SalienceWord>(oSentence.nLength);
                umSalienceWord* pToken = oSentence.pTokens;
                for (int k = 0; k < oSentence.nLength; k++, pToken++)
                {
                    SalienceWord Token = new SalienceWord();
                    Token.bInvert = true;
                    if (pToken->nInvert == 0)
                    {
                        Token.bInvert = false;
                    }
                    Token.bPostFixed = true;
                    if (pToken->nPostFixed == 0)
                    {
                        Token.bPostFixed = false;
                    }
                    Token.fSentiment = pToken->fSentiment;
                    Token.nID = pToken->nId;
                    Token.nSecondaryID = pToken->nSecondaryId;
                    Token.nSentimentType = pToken->nSentimentType;
                    Token.sEntityType = MarshalPtrToUtf8(pToken->acEntityType);
                    Token.sPOSTag = MarshalPtrToUtf8(pToken->acPOSTag);
                    Token.sStem = MarshalPtrToUtf8(pToken->acStem);
                    Token.sToken = MarshalPtrToUtf8(pToken->acToken);

                    Sentence.Tokens.Add(Token);
                }

                ReturnSentences.Add(Sentence);
            }
            return ReturnSentences;
        }

    #endregion // Details methods

    #region Document methods
        private unsafe SalienceSummary processSummary(int nLength, String id)
        {
            umSalienceSummaryResult oResult = new umSalienceSummaryResult();
            IntPtr pMarshalledId = GetUTF8String(id);
            int nRet = lxaGetSummary(m_pSessionHandle, nLength, &oResult, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);

            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }

            SalienceSummary oRet = new SalienceSummary();
            oRet.sSummary = MarshalPtrToUtf8(oResult.acSummary);
            oRet.vSentences = processSentences(oResult.pDocument->pSentences, oResult.pDocument->nSentenceCount);
            oRet.sAlternateSummary = MarshalPtrToUtf8(oResult.acAlternateSummary);
            oRet.vAlternateSentences = processSentences(oResult.pAlternateDocument->pSentences, oResult.pAlternateDocument->nSentenceCount);
            lxaFreeSummaryResult(&oResult);
            return oRet;
        }

        private unsafe SalienceSentiment processDocumentSentiment(bool bUseChains, String id)
        {
            umSalienceSentimentResult oResult = new umSalienceSentimentResult();
            IntPtr pMarshalledId = GetUTF8String(id);
            int nRet = lxaGetSentiment(m_pSessionHandle, bUseChains ? 1 : 0, &oResult, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);

            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }

            SalienceSentiment oRet = new SalienceSentiment();
            oRet.fScore = oResult.fScore;
            oRet.Phrases = new List<SalienceSentimentPhrase>(oResult.oPhrases.nLength);
            umSalienceSentimentPhrase* pPhrase = oResult.oPhrases.pPhrases;
            for (int i = 0; i < oResult.oPhrases.nLength; i++, pPhrase++)
            {
                SalienceSentimentPhrase Phrase = new SalienceSentimentPhrase();
                Phrase.fScore = pPhrase->fScore;
                Phrase.Phrase = processPhrase(pPhrase->oPhrase);
                Phrase.nType = pPhrase->nType;
                Phrase.sSource = Marshal.PtrToStringAnsi(pPhrase->acSource);
                Phrase.nModified = pPhrase->nModified;
                Phrase.SupportingPhrases = new List<SaliencePhrase>(pPhrase->oSupportingPhrases.nLength);
                umSaliencePhrase* pSupportingPhrase = pPhrase->oSupportingPhrases.pPhrases;
                for (int j = 0; j < pPhrase->oSupportingPhrases.nLength; j++, pSupportingPhrase++)
                {
                    Phrase.SupportingPhrases.Add(processPhrase(*pSupportingPhrase));
                }
              //  Phrase.SupportingPhrase = pPhrase->nNegated == 0 ? new SaliencePhrase() : processPhrase(pPhrase->oSupportingPhrase);
                oRet.Phrases.Add(Phrase);
            }

            //Now the other bits
            for (int i = 0; i < oResult.nModelCount; i++)
            {
                SalienceModelSentiment Sentiment = new SalienceModelSentiment();
                Sentiment.fPositive = oResult.pModel[i].fPositive;
                Sentiment.fNegative = oResult.pModel[i].fNegative;
                Sentiment.fNeutral = oResult.pModel[i].fNeutral;
                Sentiment.fMixed = oResult.pModel[i].fMixed;
                Sentiment.nBest = oResult.pModel[i].nBest;
                Sentiment.sName = Marshal.PtrToStringAnsi(oResult.pModel[i].acModelName);
                oRet.ModelSentiment.Add(Sentiment);

            }

            // Added in Salience 5.1, experimental output of emotion information for sentiment
            oRet.Emotions = new List<SalienceTopic>(oResult.oEmotions.nLength);
            umSalienceTopic* pEmotion = oResult.oEmotions.pTopics;
            for (int i = 0; i < oResult.oEmotions.nLength; i++, pEmotion++)
            {
                SalienceTopic Emotion = new SalienceTopic();
                Emotion.fScore = pEmotion->fScore;
                Emotion.fSentiment = pEmotion->fSentiment;
                Emotion.nHits = pEmotion->nHits;
                Emotion.sSummary = Marshal.PtrToStringAnsi(pEmotion->acAdditional);
                Emotion.sTopic = Marshal.PtrToStringAnsi(pEmotion->acTopic);
                oRet.Emotions.Add(Emotion);
            }

            lxaFreeSentimentResult(&oResult);
            return oRet;
        }

        private unsafe List<SalienceTheme> processDocumentThemes(String id)
        {
            umSalienceThemeList ThemeList = new umSalienceThemeList();
            IntPtr pMarshalledId = GetUTF8String(id);
            int nRet = lxaGetThemes(m_pSessionHandle, &ThemeList, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);

            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }

            return processThemes(ThemeList, true);
        }

        private unsafe SalienceIntention processIntention(umSalienceIntention umIntention)
        {
            SalienceIntention Intention = new SalienceIntention();
            Intention.sWho = MarshalPtrToUtf8(umIntention.acWho);
            Intention.sWhat = MarshalPtrToUtf8(umIntention.acWhat);
            Intention.sEvidence = MarshalPtrToUtf8(umIntention.acEvidence);
            
            Intention.sType = MarshalPtrToUtf8(umIntention.acType);

            Intention.oWho = processChunk(umIntention.oWho);
            Intention.oWhat = processChunk(umIntention.oWhat);
            Intention.oEvidence = processChunk(umIntention.oEvidence);
            return Intention;
        }

        private unsafe List<SalienceIntention> processDocumentIntentions(String id)
        {
            umSalienceIntentionList IntentionList = new umSalienceIntentionList();
            IntentionList.nLength = -1;
            IntPtr pMarshalledId = GetUTF8String(id);
            int nRet = lxaGetIntentions(m_pSessionHandle, &IntentionList, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);
            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }
            List<SalienceIntention> vRet = new List<SalienceIntention>(IntentionList.nLength);
            umSalienceIntention* umIntention = IntentionList.pIntentions;
            for (int i = 0; i < IntentionList.nLength; i++, umIntention++)
            {
                vRet.Add(processIntention(*umIntention));
            }
            lxaFreeIntentionList(&IntentionList);
            return vRet;
        }

        private unsafe SalienceTopic processTopic(umSalienceTopic umTopic)
        {
            SalienceTopic Topic = new SalienceTopic();
            Topic.fScore = umTopic.fScore;
            Topic.nType = umTopic.nType;
            Topic.fSentiment = umTopic.fSentiment;
            Topic.nHits = umTopic.nHits;
            Topic.sTopic = MarshalPtrToUtf8(umTopic.acTopic);
            Topic.sSummary = umTopic.acAdditional != null ? MarshalPtrToUtf8(umTopic.acAdditional) : "";
            Topic.vChildren = new List<SalienceTopic>();
            if (umTopic.pChildren != (IntPtr)0)
            {
                for (int i = 0; i < ((umSalienceTopicList*)umTopic.pChildren)->nLength; i++)
                {
                    Topic.vChildren.Add(processTopic(((umSalienceTopicList*)umTopic.pChildren)->pTopics[i]));
                }
            }
            Topic.vEntities = processEntityList(umTopic.oEntities, false);

            return Topic;
        }

        private unsafe List<SalienceTopic> processTopics(umSalienceTopicList oTopicList, bool bFree)
        {
            List<SalienceTopic> vRet = new List<SalienceTopic>(oTopicList.nLength);
            umSalienceTopic* pTopic = oTopicList.pTopics;
            for (int i = 0; i < oTopicList.nLength; i++, pTopic++)
            {
                vRet.Add(processTopic(*pTopic));
            }
            if (bFree)
            {
                lxaFreeTopicList(&oTopicList);
            }
            return vRet;
        }

        private unsafe List<SalienceTopic> processDocumentTopics(String id)
        {
            umSalienceTopicList oTopicList = new umSalienceTopicList();
            IntPtr pMarshalledId = GetUTF8String(id);
            int nRet = lxaGetQueryDefinedTopics(m_pSessionHandle, &oTopicList, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);

            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }

            return processTopics(oTopicList,true);

        }

        private unsafe List<SalienceTopic> processDocumentConceptTopics(String id)
        {
            umSalienceTopicList oTopicList = new umSalienceTopicList();
            IntPtr pMarshalledId = GetUTF8String(id);
            int nRet = lxaGetConceptDefinedTopics(m_pSessionHandle, &oTopicList, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);
            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }
            return processTopics(oTopicList, true);
        }

        private unsafe List<SalienceTopic> processDocumentClasses(String id)
        {
            umSalienceTopicList oTopicList = new umSalienceTopicList();
            IntPtr pMarshalledId = GetUTF8String(id);
            int nRet = lxaGetDocumentClasses(m_pSessionHandle, &oTopicList, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);
            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }
            return processTopics(oTopicList, true);
        }

        private unsafe List<SalienceTopic> processDocumentCategories(String id)
        {
            umSalienceTopicList oTopicList = new umSalienceTopicList();
            IntPtr pMarshalledId = GetUTF8String(id);
            int nRet = lxaGetDocumentCategories(m_pSessionHandle, &oTopicList, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);
            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }
            return processTopics(oTopicList, true);
        }

        private unsafe string processExplainConceptMatches(String id)
        {
            IntPtr acBuffer;
            IntPtr pMarshalledId = GetUTF8String(id);
            int nRet = lxaExplainConceptMatches(m_pSessionHandle, out acBuffer, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);
            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }
            if (acBuffer == IntPtr.Zero)
            {
                throw new SalienceException("Error attempting to run lxaExplainConceptMatches");
            }
            string text = MarshalPtrToUtf8(acBuffer);
            lxaFreeString(acBuffer);
            return text;
        }

    #endregion // Document methods

    #region Entity methods

        private unsafe List<SalienceEntity> processNamedEntities(String id)
        {
            umSalienceEntityList EntityList = new umSalienceEntityList();
            IntPtr pMarshalledId = GetUTF8String(id);
            int nRet = lxaGetNamedEntities(m_pSessionHandle, &EntityList, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);
            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }
            return processEntityList(EntityList, true);
        }

        private unsafe List<SalienceEntity> processUserDefinedEntities(String id)
        {
            umSalienceEntityList EntityList = new umSalienceEntityList();
            IntPtr pMarshalledId = GetUTF8String(id);
            int nRet = lxaGetUserDefinedEntities(m_pSessionHandle, &EntityList, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);
            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }
            return processEntityList(EntityList, true);
        }

        private unsafe List<SalienceRelationship> processRelationships(String id)
        {
            umSalienceRelationList RelationList = new umSalienceRelationList();
            IntPtr pMarshalledId = GetUTF8String(id);
            int nRet = lxaGetNamedEntityRelationships(m_pSessionHandle, &RelationList, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);

            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }

            List<SalienceRelationship> vRet = new List<SalienceRelationship>(RelationList.nLength);
            umSalienceRelation* pRelation = RelationList.pRelations;
            for (int i = 0; i < RelationList.nLength; i++, pRelation++)
            {
                SalienceRelationship Relationship = new SalienceRelationship();
                Relationship.fScore = pRelation->fConfidence;
                Relationship.sLabel = MarshalPtrToUtf8(pRelation->acType);
                Relationship.sExtra = MarshalPtrToUtf8(pRelation->acExtra);
                Relationship.Entities = processEntityList(pRelation->oEntities, false);

                vRet.Add(Relationship);
            }
            lxaFreeRelationList(&RelationList);
            return vRet;
        }

        private unsafe List<SalienceOpinion> processOpinions(String id)
        {
            umSalienceOpinionList OpinionList = new umSalienceOpinionList();
            IntPtr pMarshalledId = GetUTF8String(id);
            int nRet = lxaGetNamedEntityOpinions(m_pSessionHandle, &OpinionList, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);

            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }

            List<SalienceOpinion> vRet = new List<SalienceOpinion>(OpinionList.nLength);

            umSalienceOpinion* pOpinion = OpinionList.pOpinions;
            for (int i = 0; i < OpinionList.nLength; i++, pOpinion++)
            {
                SalienceOpinion Opinion = new SalienceOpinion();
                Opinion.sQuotation = MarshalPtrToUtf8(pOpinion->acQuotation);
                Opinion.fSentiment = pOpinion->fSentiment;
                Opinion.Speaker = processEntity(&pOpinion->oSpeaker);
                if (pOpinion->nHasTheme != 0)
                {
                    Opinion.bThemeOpinion = true;
                    Opinion.ThemeTopic = processTheme(&pOpinion->oThemeTopic);
                }
                else
                {
                    Opinion.bThemeOpinion = false;
                    Opinion.EntityTopic = processEntity(&pOpinion->oEntityTopic);
                }
                vRet.Add(Opinion);
            }
            return vRet;

        }

        private unsafe List<SalienceRelationship> processUserEntityRelationships(String id)
        {
            umSalienceRelationList RelationList = new umSalienceRelationList();
            IntPtr pMarshalledId = GetUTF8String(id);
            int nRet = lxaGetUserEntityRelationships(m_pSessionHandle, &RelationList, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);

            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }

            List<SalienceRelationship> vRet = new List<SalienceRelationship>(RelationList.nLength);
            umSalienceRelation* pRelation = RelationList.pRelations;
            for (int i = 0; i < RelationList.nLength; i++, pRelation++)
            {
                SalienceRelationship Relationship = new SalienceRelationship();
                Relationship.fScore = pRelation->fConfidence;
                Relationship.sLabel = MarshalPtrToUtf8(pRelation->acType);
                Relationship.sExtra = MarshalPtrToUtf8(pRelation->acExtra);
                Relationship.Entities = processEntityList(pRelation->oEntities, false);

                vRet.Add(Relationship);
            }
            lxaFreeRelationList(&RelationList);
            return vRet;
        }

        private unsafe List<SalienceOpinion> processUserEntityOpinions(String id)
        {
            umSalienceOpinionList OpinionList = new umSalienceOpinionList();
            IntPtr pMarshalledId = GetUTF8String(id);
            int nRet = lxaGetUserEntityOpinions(m_pSessionHandle, &OpinionList, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);

            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }

            List<SalienceOpinion> vRet = new List<SalienceOpinion>(OpinionList.nLength);

            umSalienceOpinion* pOpinion = OpinionList.pOpinions;
            for (int i = 0; i < OpinionList.nLength; i++, pOpinion++)
            {
                SalienceOpinion Opinion = new SalienceOpinion();
                Opinion.sQuotation = MarshalPtrToUtf8(pOpinion->acQuotation);
                Opinion.fSentiment = pOpinion->fSentiment;
                Opinion.Speaker = processEntity(&pOpinion->oSpeaker);
                if (pOpinion->nHasTheme != 0)
                {
                    Opinion.bThemeOpinion = true;
                    Opinion.ThemeTopic = processTheme(&pOpinion->oThemeTopic);
                }
                else
                {
                    Opinion.bThemeOpinion = false;
                    Opinion.EntityTopic = processEntity(&pOpinion->oEntityTopic);
                }
                vRet.Add(Opinion);
            }
            return vRet;

        }

        private unsafe SalienceEntity processEntity(umSalienceEntity* pEntity)
        {
            SalienceEntity Entity = new SalienceEntity();
            Entity.fSentimentScore = pEntity->fSentimentScore;
            Entity.nAbout = pEntity->nAbout;
            Entity.nConfident = pEntity->nConfident;
            Entity.nEvidence = pEntity->nEvidence;
            Entity.nCount = pEntity->oMentions.nLength;
            Entity.nFirstPos = pEntity->oMentions.pMentions[0].oPhrase.nWord;
            Entity.sLabel = MarshalPtrToUtf8(pEntity->acLabel);
            Entity.sNormalizedForm = MarshalPtrToUtf8(pEntity->acNormalizedForm);
            Entity.sSummary = MarshalPtrToUtf8(pEntity->acSummary);
            Entity.sType = MarshalPtrToUtf8(pEntity->acType);

            Entity.Mentions = processMentions(pEntity->oMentions);
            Entity.Themes = processThemes(pEntity->oThemes, false);
            Entity.SentimentPhrases = processSentimentPhrases(pEntity->oSentimentPhrases);
            Entity.Topics = processTopics(pEntity->oTopics,false);
            return Entity;
        }

        private unsafe List<SalienceEntity> processEntityList(umSalienceEntityList EntityList, bool bFree)
        {
            List<SalienceEntity> ReturnEntities = new List<SalienceEntity>(EntityList.nLength);
            umSalienceEntity* pEntity = EntityList.pEntities;
            for (int i = 0; i < EntityList.nLength; i++, pEntity++)
            {
                SalienceEntity Entity = processEntity(pEntity);
                ReturnEntities.Add(Entity);
            }
            if (bFree)
            {
                lxaFreeEntityList(&EntityList);
            }
            return ReturnEntities;

        }

        private unsafe List<SalienceMention> processMentions(umSalienceMentionList MentionList)
        {
            List<SalienceMention> ReturnMentions = new List<SalienceMention>(MentionList.nLength);
            umSalienceMention* pMention = MentionList.pMentions;
            for (int i = 0; i < MentionList.nLength; i++, pMention++)
            {
                SalienceMention Mention = new SalienceMention();
                Mention.fScore = pMention->fScore;
                Mention.nType = pMention->nType;
                Mention.Phrase = processPhrase(pMention->oPhrase);
                ReturnMentions.Add(Mention);
            }
            return ReturnMentions;
        }

        private unsafe List<SaliencePhrase> processPhraseMentions(umSaliencePhraseList MentionList)
        {
            List<SaliencePhrase> ReturnPhrases = new List<SaliencePhrase>(MentionList.nLength);
            umSaliencePhrase* pPhraseMention = MentionList.pPhrases;
            for (int i = 0; i < MentionList.nLength; i++, pPhraseMention++)
            {
                SaliencePhrase PhraseMention = new SaliencePhrase();
                PhraseMention = processPhrase(MentionList.pPhrases[i]);
                ReturnPhrases.Add(PhraseMention);
            }
            return ReturnPhrases;
        }

    #endregion // Entity methods

    #region Shared Entity and Document methods
        private unsafe List<SalienceSentimentPhrase> processSentimentPhrases(umSalienceSentimentPhraseList oPhraseList)
        {
            List<SalienceSentimentPhrase> oRet = new List<SalienceSentimentPhrase>(oPhraseList.nLength);
            umSalienceSentimentPhrase* pPhrase = oPhraseList.pPhrases;
            for (int i = 0; i < oPhraseList.nLength; i++, pPhrase++)
            {
                SalienceSentimentPhrase Phrase = new SalienceSentimentPhrase();
                Phrase.fScore = pPhrase->fScore;
                Phrase.Phrase = processPhrase(pPhrase->oPhrase);
                Phrase.nType = pPhrase->nType;
                Phrase.nModified = pPhrase->nModified;
                Phrase.SupportingPhrases = new List<SaliencePhrase>(pPhrase->oSupportingPhrases.nLength);
                umSaliencePhrase* pSupportingPhrase = pPhrase->oSupportingPhrases.pPhrases;
                for (int j = 0; j < pPhrase->oSupportingPhrases.nLength; j++, pSupportingPhrase++)
                {
                    Phrase.SupportingPhrases.Add(processPhrase(*pSupportingPhrase));
                }
              //  Phrase.SupportingPhrase = pPhrase->nNegated == 0 ? new SaliencePhrase() : processPhrase(pPhrase->oSupportingPhrase);
                oRet.Add(Phrase);
            }

            return oRet;

        }

        private unsafe List<SalienceAlternateTheme> processAlternateThemes(umSalienceAlternateThemeList AlternateThemeList)
        {
            List<SalienceAlternateTheme> ReturnAlternateThemes = new List<SalienceAlternateTheme>(AlternateThemeList.nLength);
            umSalienceAlternateTheme* pAlternateTheme = AlternateThemeList.pAlternateThemes;
            for (int i = 0; i < AlternateThemeList.nLength; i++, pAlternateTheme++)
            {
                ReturnAlternateThemes.Add(processAlternateTheme(pAlternateTheme));
            }
            return ReturnAlternateThemes;
        }

        private unsafe SalienceAlternateTheme processAlternateTheme(umSalienceAlternateTheme* pAlternateTheme)
        {
            SalienceAlternateTheme AlternateTheme = new SalienceAlternateTheme();
            AlternateTheme.sAlternateTheme = MarshalPtrToUtf8(pAlternateTheme->acAlternateTheme);
            AlternateTheme.fScore = pAlternateTheme->fScore;
            return AlternateTheme;
        }

        private unsafe List<SalienceTheme> processThemes(umSalienceThemeList ThemeList, bool bFree)
        {
            List<SalienceTheme> ReturnThemes = new List<SalienceTheme>(ThemeList.nLength);
            umSalienceTheme* pTheme = ThemeList.pThemes;
            for (int i = 0; i < ThemeList.nLength; i++, pTheme++)
            {

                ReturnThemes.Add(processTheme(pTheme));
            }
            //GetEntities calls ProcessThemes to fill its Theme List, but also calls FreeEntity when it's done, which calls lxaFreeThemes.
            //Thus we call processThemes(true) only when we have a themes list that's not connected to an entity.
            if (bFree)
            {
                lxaFreeThemeList(&ThemeList);
            }
            return ReturnThemes;
        }

        private unsafe SalienceTheme processTheme(umSalienceTheme* pTheme)
        {
            SalienceTheme Theme = new SalienceTheme();
            Theme.fScore = pTheme->fScore;
            Theme.fSentiment = pTheme->fSentiment;
            Theme.nEvidence = pTheme->nEvidence;
            Theme.nThemeType = pTheme->nThemeType;
            Theme.nEvidence = pTheme->nEvidence;
            Theme.bAbout = (pTheme->nAbout == 1);
            Theme.sStemmedTheme = MarshalPtrToUtf8(pTheme->acStemmedTheme);
            Theme.sTheme = MarshalPtrToUtf8(pTheme->acTheme);
            Theme.sNormalizedTheme = MarshalPtrToUtf8(pTheme->acNormalizedTheme);
            Theme.sSummary = MarshalPtrToUtf8(pTheme->acSummary);
            Theme.ThemeMentions = processPhraseMentions(pTheme->oMentions);
            Theme.Topics = processTopics(pTheme->oTopics, false);
            Theme.AlternateThemes = processAlternateThemes(pTheme->oAlternateThemes);
            Theme.ChildMentions = processPhraseMentions(pTheme->oChildMentions);
            Theme.RelatedMentions = processPhraseMentions(pTheme->oRelatedMentions);
            return Theme;
        }

        private unsafe SaliencePhrase processPhrase(umSaliencePhrase umPhrase)
        {
            SaliencePhrase Phrase = new SaliencePhrase();
            Phrase.nByte = umPhrase.nByte;
            Phrase.nByteLength = umPhrase.nByteLength;
            Phrase.nLength = umPhrase.nLength;
            Phrase.nDocument = umPhrase.nDocument;
            Phrase.nSentence = umPhrase.nSentence;
            Phrase.nWord = umPhrase.nWord;
            Phrase.sText = MarshalPtrToUtf8(umPhrase.acText);
            Phrase.nIsNegated = umPhrase.nIsNegated;
            Phrase.sNegator = (Phrase.nIsNegated == 0) ? "" : MarshalPtrToUtf8(umPhrase.acNegator);
            Phrase.nType = umPhrase.nType;
            Phrase.nSection = umPhrase.nSection;
            Phrase.nRow = umPhrase.nRow;
            Phrase.nColumn = umPhrase.nColumn;
            return Phrase;
        }

    #endregion // Shared Entity and Document methods

    #region Collection methods
        private unsafe List<SalienceTheme> processCollectionThemes(String id)
        {
            Callback_Current = Callback_GetCollectionThemes;
            umSalienceThemeList ThemeList = new umSalienceThemeList();
            IntPtr pMarshalledId = GetUTF8String(id);
            int nRet = lxaGetCollectionThemes(m_pSessionHandle, ref ThemeList, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);
            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }
            return processThemes(ThemeList, true);
        }

        private unsafe List<SalienceCollectionEntity> processCollectionEntities(String id)
        {
            Callback_Current = Callback_GetCollectionEntities;
            umSalienceCollectionEntityList EntityList = new umSalienceCollectionEntityList();
            IntPtr pMarshalledId = GetUTF8String(id);
            int nRet = lxaGetCollectionEntities(m_pSessionHandle, &EntityList, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);
            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }
            return processCollectionEntityList(EntityList, true);
        }

        private unsafe List<SalienceCollectionEntity> processCollectionUserEntities(String id)
        {
            Callback_Current = Callback_GetCollectionUserEntities;
            umSalienceCollectionEntityList EntityList = new umSalienceCollectionEntityList();
            IntPtr pMarshalledId = GetUTF8String(id);
            int nRet = lxaGetCollectionUserEntities(m_pSessionHandle, &EntityList, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);
            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }
            return processCollectionEntityList(EntityList, true);
        }

        private unsafe List<SalienceCollectionEntity> processCollectionEntityList(umSalienceCollectionEntityList EntityList, bool bFree)
        {
            Callback_Current = Callback_GetCollectionEntities;
            List<SalienceCollectionEntity> ReturnEntities = new List<SalienceCollectionEntity>(EntityList.nLength);
            umSalienceCollectionEntity* pEntity = EntityList.pCollectionEntities;
            for (int i = 0; i < EntityList.nLength; i++, pEntity++)
            {
                SalienceCollectionEntity Entity = processCollectionEntity(pEntity);
                ReturnEntities.Add(Entity);
            }
            if (bFree)
            {
                lxaFreeCollectionEntityList(&EntityList);
            }
            return ReturnEntities;
        }

        private unsafe SalienceCollectionEntity processCollectionEntity(umSalienceCollectionEntity* pEntity)
        {
            SalienceCollectionEntity Entity = new SalienceCollectionEntity();
            Entity.nCount = pEntity->nCount;
            Entity.nNegativeCount = pEntity->nNegativeCount;
            Entity.nNeutralCount = pEntity->nNeutralCount;
            Entity.nPositiveCount = pEntity->nPositiveCount;
            Entity.sLabel = MarshalPtrToUtf8(pEntity->acLabel);
            Entity.sNormalizedForm = MarshalPtrToUtf8(pEntity->acNormalizedForm);
            Entity.sType = MarshalPtrToUtf8(pEntity->acType);
            Entity.oMentions = processPhraseMentions(pEntity->oMentions);
            return Entity;
        }

        private unsafe List<SalienceFacet> processCollectionFacets(String id)
        {
            Callback_Current = Callback_GetCollectionFacets;
            umSalienceFacetList FacetList = new umSalienceFacetList();
            IntPtr pMarshalledId = GetUTF8String(id);
            int nRet = lxaGetCollectionFacets(m_pSessionHandle, &FacetList, pMarshalledId);

            Marshal.FreeHGlobal(pMarshalledId);
            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }
            return processCollectionFacetList(FacetList, true);
        }

        private unsafe List<SalienceFacet> processCollectionFacetList(umSalienceFacetList FacetList, bool bFree)
        {
            Callback_Current = Callback_GetCollectionFacets;
            List<SalienceFacet> ReturnFacets = new List<SalienceFacet>(FacetList.nLength);
            umSalienceFacet* pFacet = FacetList.pFacetList;
            for (int i = 0; i < FacetList.nLength; i++, pFacet++)
            {
                SalienceFacet Facet = processFacet(pFacet);
                ReturnFacets.Add(Facet);
            }
            if (bFree)
            {
                lxaFreeFacetList(&FacetList);
            }
            return ReturnFacets;
        }

        private unsafe SalienceFacet processFacet(umSalienceFacet* pFacet)
        {
            SalienceFacet Facet = new SalienceFacet();
            Facet.nCount = pFacet->nCount;
            Facet.nPositiveCount = pFacet->nPositiveCount;
            Facet.nNeutralCount = pFacet->nNeutralCount;
            Facet.nNegativeCount = pFacet->nNegativeCount;
            Facet.sFacet = MarshalPtrToUtf8(pFacet->acFacet);
            Facet.Attributes = new List<SalienceAttribute>();
            Facet.Mentions = processPhraseMentions(pFacet->oMentions);
            Facet.PositiveMentions = processPhraseMentions(pFacet->oPositiveMentions);
            Facet.NeutralMentions = processPhraseMentions(pFacet->oNeutralMentions);
            Facet.NegativeMentions = processPhraseMentions(pFacet->oNegativeMentions);
            for (int i = 0; i < pFacet->nSubjectLength; i++ )
            {
                SalienceAttribute Attribute = new SalienceAttribute();
                Attribute.sAttribute = MarshalPtrToUtf8(pFacet->pAttributes[i].acAttribute);
                Attribute.nCount = pFacet->pAttributes[i].nCount;
                Attribute.nPositiveCount = pFacet->pAttributes[i].nPositiveCount;
                Attribute.nNegativeCount = pFacet->pAttributes[i].nNegativeCount;
                Attribute.nNeutralCount = pFacet->pAttributes[i].nNeutralCount;
                Attribute.Mentions = processPhraseMentions(pFacet->pAttributes[i].oMentions);
                Attribute.PositiveMentions = processPhraseMentions(pFacet->pAttributes[i].oPositiveMentions);
                Attribute.NegativeMentions = processPhraseMentions(pFacet->pAttributes[i].oNegativeMentions);
                Attribute.NeutralMentions = processPhraseMentions(pFacet->pAttributes[i].oNeutralMentions);
                Facet.Attributes.Add(Attribute);
            } 
            return Facet;
        }

        private unsafe List<SalienceTopic> processCollectionQueryTopics(String id)
        {
            umSalienceTopicList oTopicList = new umSalienceTopicList();
            IntPtr pMarshalledId = GetUTF8String(id);
            int nRet = lxaGetCollectionQueryDefinedTopics(m_pSessionHandle, &oTopicList, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);

            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }

            List<SalienceTopic> vRet = new List<SalienceTopic>(oTopicList.nLength);
            umSalienceTopic* pTopic = oTopicList.pTopics;
            for (int i = 0; i < oTopicList.nLength; i++, pTopic++)
            {
                SalienceTopic Topic = new SalienceTopic();
                Topic.fScore = pTopic->fScore;
                Topic.nHits = pTopic->nHits;
                Topic.fSentiment = pTopic->fSentiment;
                Topic.sTopic = MarshalPtrToUtf8(pTopic->acTopic);
                string sMentions = MarshalPtrToUtf8(pTopic->acAdditional);
                Topic.vDocuments = new List<string>(sMentions.Split('|'));
                vRet.Add(Topic);
            }
            lxaFreeTopicList(&oTopicList);
            return vRet;
        }

        private unsafe List<SalienceTopic> processCollectionConceptTopics(String id)
        {
            umSalienceTopicList oTopicList = new umSalienceTopicList();
            IntPtr pMarshalledId = GetUTF8String(id);
            int nRet = lxaGetCollectionConceptDefinedTopics(m_pSessionHandle, &oTopicList, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);

            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }

            List<SalienceTopic> vRet = new List<SalienceTopic>(oTopicList.nLength);
            umSalienceTopic* pTopic = oTopicList.pTopics;
            for (int i = 0; i < oTopicList.nLength; i++, pTopic++)
            {
                SalienceTopic Topic = new SalienceTopic();
                Topic.fScore = pTopic->fScore;
                Topic.nHits = pTopic->nHits;
                Topic.fSentiment = pTopic->fSentiment;
                Topic.sTopic = MarshalPtrToUtf8(pTopic->acTopic);
                string sMentions = MarshalPtrToUtf8(pTopic->acAdditional);
                Topic.vDocuments = new List<string>(sMentions.Split('|'));
                vRet.Add(Topic);
            }
            lxaFreeTopicList(&oTopicList);
            return vRet;
        }

    #endregion // Collection methods

    #region Utility methods
        private static unsafe string getDefaultSalienceLocation()
        {
            IntPtr acPath;
            lxaGetDefaultSalienceLocation(out acPath);
            if (acPath == IntPtr.Zero) throw new SalienceException("Could not find default Salience location.");
            string sPath = Marshal.PtrToStringAnsi(acPath);
            lxaFreeString(acPath);
            return sPath;
        }

        private unsafe string getVersion()
        {
            IntPtr acVersion;
            lxaGetSalienceVersion(out acVersion);
            if (acVersion == IntPtr.Zero) throw new SalienceException("Could not read version information.");
            string sVersion = Marshal.PtrToStringAnsi(acVersion);
            lxaFreeString(acVersion);
            return sVersion;
        }

        private unsafe string getLastError()
        {
            IntPtr acError;
            lxaGetSalienceErrorString(m_pSessionHandle, out acError);
            string sError = Marshal.PtrToStringAnsi(acError);
            lxaFreeString(acError);
            return sError;
        }

        private unsafe string dumpEnvironment()
        {
            IntPtr acEnvironment;
            lxaDumpEnvironment(m_pSessionHandle, out acEnvironment);
            string sEnvironment = Marshal.PtrToStringAnsi(acEnvironment);
            lxaFreeString(acEnvironment);
            return sEnvironment;
        }

        private unsafe int setSalienceOption(int nOption, int nValue, String id)
        {
            umSalienceOption umOption = new umSalienceOption();
            umOption.nOption = nOption;   
            umOption.nValue = nValue;

            IntPtr pMarshalledId = GetUTF8String(id);
            int nCode = lxaSetSalienceOption(m_pSessionHandle, &umOption, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);
            return nCode;
        }

        private unsafe int setSalienceOption(int nOption, bool bValue, String id)
        {
            umSalienceOption umOption = new umSalienceOption();
            umOption.nOption = nOption;   
            if (bValue)
                umOption.nValue = 1;
            else
                umOption.nValue = 0;

            IntPtr pMarshalledId = GetUTF8String(id);
            int nCode = lxaSetSalienceOption(m_pSessionHandle, &umOption, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);
            return nCode;
        }

        private unsafe int setSalienceOption(int nOption, float fValue, String id)
        {
            umSalienceOption umOption = new umSalienceOption();
            umOption.nOption = nOption;
            umOption.fValue = fValue;

            IntPtr pMarshalledId = GetUTF8String(id);
            int nCode = lxaSetSalienceOption(m_pSessionHandle, &umOption, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);
            return nCode;
        }

        private unsafe int setSalienceOption(int nOption, string sValue, String id)
        {
            umSalienceOption umOption = new umSalienceOption();
            umOption.nOption = nOption;
            umOption.acValue = Marshal.StringToHGlobalAnsi(sValue);

            IntPtr pMarshalledId = GetUTF8String(id);
            int nCode = lxaSetSalienceOption(m_pSessionHandle, &umOption, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);
            return nCode;
        }

        private unsafe int setSalienceOption(int nOption, string sValue, int nValue, String id)
        {
            umSalienceOption umOption = new umSalienceOption();
            umOption.nOption = nOption;
            umOption.nValue = nValue;
            umOption.acValue = Marshal.StringToHGlobalAnsi(sValue);

            IntPtr pMarshalledId = GetUTF8String(id);
            int nCode = lxaSetSalienceOption(m_pSessionHandle, &umOption, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);
            return nCode;
        }
        protected dEngineNotification Callback_Current;

        protected unsafe void SetInternalCallback(String id)
        {
            fCallback = new umSalienceCallback(handleCallbacks);
            IntPtr pCallback = Marshal.GetFunctionPointerForDelegate(fCallback);
            IntPtr pMarshalledId = GetUTF8String(id);
            lxaSetSalienceCallback(m_pSessionHandle, pCallback, null, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);
        }

        protected unsafe int handleCallbacks(void* pClass, int nId, IntPtr acMessage)
        {
            if (Callback_Current != null)
            {
                string sMessage = MarshalPtrToUtf8(acMessage);
                Callback_Current.Invoke(nId, sMessage);
            }
            return 0;
        }

    #endregion // Utility methods

    #region Marked Text methods

        private unsafe string entityTagHelper(umSalienceDocument oDoc)
        {
            StringBuilder sMarkup = new StringBuilder();
	        int nPrimaryId = -1;
	        int nSecondaryId = -1;
	        string sType = "";
	        for(int i = 0; i < oDoc.nSentenceCount; i++)
	        {
		        for(int j = 0; j < oDoc.pSentences[i].nLength; j++)
		        {
			        if(oDoc.pSentences[i].pTokens[j].nId == -1)
			        {
				        if(nPrimaryId != -1)
				        {
                            sMarkup.Append("[/");
					        sMarkup.Append(sType);
					        sMarkup.Append("]");
					        nPrimaryId = -1;
					        nSecondaryId = -1;
				        }
                        if (oDoc.pSentences[i].pTokens[j].nPostFixed == 0)
                        {
                            sMarkup.Append(" ");
                        }
				        sMarkup.Append(MarshalPtrToUtf8(oDoc.pSentences[i].pTokens[j].acToken));
			        }
			        else
			        {
				        if(nPrimaryId == oDoc.pSentences[i].pTokens[j].nId && nSecondaryId == oDoc.pSentences[i].pTokens[j].nSecondaryId)
				        {
                            if (oDoc.pSentences[i].pTokens[j].nPostFixed == 0)
                            {
                                sMarkup.Append(" ");
                            }
					        sMarkup.Append(MarshalPtrToUtf8(oDoc.pSentences[i].pTokens[j].acToken));
				        }
				        else
				        {
					        if(nPrimaryId != -1)
					        {
						        sMarkup.Append("[/");
						        sMarkup.Append(sType);
						        sMarkup.Append("]");
					        }
                            if (oDoc.pSentences[i].pTokens[j].nPostFixed == 0)
                            {
                                sMarkup.Append(" ");
                            }
					        nPrimaryId = oDoc.pSentences[i].pTokens[j].nId;
					        nSecondaryId = oDoc.pSentences[i].pTokens[j].nSecondaryId;
					        sType = MarshalPtrToUtf8(oDoc.pSentences[i].pTokens[j].acEntityType);
                            sMarkup.Append("[");
					        sMarkup.Append(sType);
					        sMarkup.Append("]");
					        sMarkup.Append(MarshalPtrToUtf8(oDoc.pSentences[i].pTokens[j].acToken));
				        }
			        }
		        }
	        }
	        if(nPrimaryId != -1)
	        {
                sMarkup.Append("[/");
		        sMarkup.Append(sType);
		        sMarkup.Append("]");
		        nPrimaryId = -1;
		        nSecondaryId = -1;
	        }
	        return sMarkup.ToString();
        }

        private unsafe string processNamedEntityTaggedText(String id)
        {
            umSalienceDocument oDoc;
            IntPtr pMarshalledId = GetUTF8String(id);
            int nRet = lxaGetNamedEntityMarkup(m_pSessionHandle, &oDoc, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);
            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }
            string sResult = entityTagHelper(oDoc);
            lxaFreeDocument(&oDoc);
            return sResult;
        }

        private unsafe string processUserEntityTaggedText(String id)
        {
            umSalienceDocument oDoc;
            IntPtr pMarshalledId = GetUTF8String(id);
            int nRet = lxaGetUserEntityMarkup(m_pSessionHandle, &oDoc, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);
            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }
            string sResult = entityTagHelper(oDoc);
            lxaFreeDocument(&oDoc);
            return sResult;
        }

        private unsafe string processPOSTaggedText(String id)
        {
            StringBuilder sReturn = new StringBuilder();
            umSalienceDocument oDoc;
            IntPtr pMarshalledId = GetUTF8String(id);
            int nRet = lxaGetPOSMarkup(m_pSessionHandle, &oDoc, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);

            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }

            for (int i = 0; i < oDoc.nSentenceCount; i++)
            {
                for (int j = 0; j < oDoc.pSentences[i].nLength; j++)
                {
                    if (oDoc.pSentences[i].pTokens[j].nPostFixed == 0)
                    {
                        sReturn.Append(" ");
                    }
                    string sTag = MarshalPtrToUtf8(oDoc.pSentences[i].pTokens[j].acPOSTag);
                    sReturn.Append("[" + sTag + "]");
                    sReturn.Append(MarshalPtrToUtf8(oDoc.pSentences[i].pTokens[j].acToken));
                    sReturn.Append("[/" + sTag + "]");
                }
            }

            lxaFreeDocument(&oDoc);
            return sReturn.ToString();
        }

        private unsafe string processSentimentTaggedText(float fNegative = -0.3f, float fPositive = 0.3f, String id="")
        {
            StringBuilder sReturn = new StringBuilder();
            umSalienceDocument oDoc;
            IntPtr pMarshalledId = GetUTF8String(id);
            int nRet = lxaGetSentimentMarkup(m_pSessionHandle, &oDoc, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);

            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }

            string sLastTag = "";
            for (int i = 0; i < oDoc.nSentenceCount; i++)
            {
                if (oDoc.pSentences[i].nPolar == 1)
                {
                    sReturn.Append("[LXA_SENTENCE_POLAR]");
                }
                
                
                for (int j = 0; j < oDoc.pSentences[i].nLength; j++)
                {
                    string sTag = "";
                    int nType = oDoc.pSentences[i].pTokens[j].nSentimentType;
                    if(nType != -1)
                    {
                        if(nType == 0)
                        {
                            sTag = "LXA_STOP";
                        }
                        else if(nType == 2)
                        {
                            sTag = "LXA_POSSIBLE";
                        }
                        else if(oDoc.pSentences[i].pTokens[j].nInvert != 0)
                        {
                            sTag = "LXA_INVERT";
                        }
                        else
                        {
                            if(nType == 1)
                            {
                                sTag = "LXA_HANDSCORE_";
                            }
                            else
                            {
                                sTag = "LXA_INTERNAL_";
                            }
                            if(oDoc.pSentences[i].pTokens[j].fSentiment < fNegative)
                            {
                                sTag += "NEGATIVE";
                            }
                            else if (oDoc.pSentences[i].pTokens[j].fSentiment > fPositive)
                            {
                                sTag += "POSITIVE";
                            }
                            else
                            {
                                sTag += "NEUTRAL";
                            }
                        }
                    }
                    if (sTag != sLastTag)
                    {
                        if (sLastTag != "")
                        {
                            sReturn.Append("[/" + sLastTag + "]");
                        }
                        if (oDoc.pSentences[i].pTokens[j].nPostFixed == 0)
                        {
                            sReturn.Append(" ");
                        }
                        if (sTag != "")
                        {
                            sReturn.Append("[" + sTag + "]");
                        }
                        sLastTag = sTag;
                        sReturn.Append(MarshalPtrToUtf8(oDoc.pSentences[i].pTokens[j].acToken));
                    }
                    else
                    {
                        if (oDoc.pSentences[i].pTokens[j].nPostFixed == 0)
                        {
                            sReturn.Append(" ");
                        }
                        sReturn.Append(MarshalPtrToUtf8(oDoc.pSentences[i].pTokens[j].acToken));
                    }
                }
                if (sLastTag != "")
                {
                    sReturn.Append("[/" + sLastTag + "]");
                }

                if (oDoc.pSentences[i].nPolar == 1)
                {
                    sReturn.Append("[/LXA_SENTENCE_POLAR]");
                }
                
            }
            lxaFreeDocument(&oDoc);
            return sReturn.ToString();
        }

        private unsafe string processNamedOpinionTaggedText(String id)
        {
            IntPtr acTaggedText;
            IntPtr pMarshalledId = GetUTF8String(id);
            int nRet = lxaGetNamedOpinionTaggedText(m_pSessionHandle, out acTaggedText, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);

            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }

            string sTaggedText = MarshalPtrToUtf8(acTaggedText);
            lxaFreeString(acTaggedText);
            return sTaggedText;
        }
    

        private unsafe string processUserOpinionTaggedText(String id)
        {
            IntPtr acTaggedText;
            IntPtr pMarshalledId = GetUTF8String(id);
            int nRet = lxaGetUserOpinionTaggedText(m_pSessionHandle, out acTaggedText, pMarshalledId);
            Marshal.FreeHGlobal(pMarshalledId);

            if (nRet != 0 && nRet != 6)
            {
                throw new SalienceException(getLastError());
            }

            string sTaggedText = MarshalPtrToUtf8(acTaggedText);
            lxaFreeString(acTaggedText);
            return sTaggedText;
         }


    #endregion // Marked Text methods

    #region Marshaling methods
        /// <summary>
        /// gets around .NET's lack of native utf8 support. given a pointer to a utf8 char* string, turns it into a byte[]
        /// </summary>
        /// <param name="pText">an IntPtr representing a char*</param>
        /// <returns>the bytes of the char*</returns>
        unsafe static byte[] Utf8byteArray(IntPtr pText)
        {
            if (pText == IntPtr.Zero) return new byte[0];

            //Get pText's pointer, cast it to a byte.
            byte* ptr = (byte*)pText.ToPointer();
            int length = 0;
            //read until we find a null character in the text.
            for (length = 0; ptr[length] != 0; length++) ;

            //now that we've got the size, copy from the pointer to an array.
            byte[] utf8 = new byte[length];
            for (int i = 0; i < utf8.Length; i++)
            {
                utf8[i] = ptr[i];
            }
            //converted! System.Text.Encoding.UTF8.GetString can now be used to get text
            //We don't return that string, because the byte array ends up being useful later in GetDocument.
            return utf8;
        }

        unsafe static string MarshalPtrToUtf8(IntPtr pText)
        {
            //We can marshal to ASCII and unicode, but not UTF8. For ease of use I've written this marshal function.
            return System.Text.Encoding.UTF8.GetString(Utf8byteArray(pText));
        }

        IntPtr GetUTF8String(string sText)
        {
            sText += "\0";
            var bytes = Encoding.UTF8.GetByteCount(sText);
            var buffer = new byte[bytes];

            Encoding.UTF8.GetBytes(sText, 0, sText.Length, buffer, 0);
            IntPtr pString = Marshal.AllocHGlobal(bytes + 1);
            Marshal.Copy(buffer, 0, pString, bytes);
            return pString;
        }

    #endregion 

#endregion // UNMANAGED .NET TO C LAYER (INTERNAL IMPLEMENTATION)

#region UNMANAGED C LAYER (CONNECTION TO UNDERLYING SALIENCE C API)
        private const string dllName = "Salience6.dll";

    #region Global methods
        [DllImport(dllName)]
        private unsafe static extern int lxaLoadLicense(string acPath, ref IntPtr pLicense);

        [DllImport(dllName)]
        private unsafe static extern int lxaFreeLicense(ref IntPtr pLicense);

        [DllImport(dllName)]
        private unsafe static extern int lxaFreeString(IntPtr acBuffer);

    #endregion // Global methods

    #region Session methods
        [DllImport(dllName)]
        private unsafe static extern int lxaOpenSalienceSession(IntPtr pLicense, ref umSalienceStartup pStartup, ref IntPtr ppSession);

        [DllImport(dllName)]
        private unsafe static extern int lxaCloseSalienceSession(IntPtr pSession);

        [DllImport(dllName)]
        private unsafe static extern int lxaAddSalienceConfiguration(IntPtr pSession, IntPtr acUserDir, IntPtr acConfigurationID);
        
        [DllImport(dllName)]
        private unsafe static extern int lxaRemoveSalienceConfiguration(IntPtr pSession, IntPtr acConfigurationID);

    #endregion // Session methods

    #region Prepare functions
        [DllImport(dllName)]
        private unsafe static extern int lxaPrepareText(IntPtr pSession, IntPtr acText);

        [DllImport(dllName)]
        private unsafe static extern int lxaPrepareTextFromFile(IntPtr pSession, string acFile);

        [DllImport(dllName)]
        private unsafe static extern int lxaAddSection(IntPtr pSession, IntPtr acHeader, IntPtr acText, int nProcess);

        [DllImport(dllName)]
        private unsafe static extern int lxaAddSectionFromFile(IntPtr pSession, IntPtr acHeader, string acFile, int nProcess);

        // TODO: SE5 Documentation, new API method
        [DllImport(dllName)]
        private unsafe static extern int lxaPrepareCollection(IntPtr pSession, umSalienceCollection* pCollection);

        // TODO: SE5 Documentation, new API method
        [DllImport(dllName)]
        private unsafe static extern int lxaPrepareCollectionFromFile(IntPtr pSession, string acName, string acFile);

    #endregion // Prepare functions

    #region Details functions
        [DllImport(dllName)]
        private unsafe static extern int lxaGetDocumentDetails(IntPtr pSession, umSalienceDocumentDetails* pDetails, IntPtr acId);

        // TODO: SE5 Documentation, new API method
        [DllImport(dllName)]
        private unsafe static extern int lxaGetCollectionDetails(IntPtr pSession, umSalienceCollectionDetails* pDetails, IntPtr acId);

    #endregion // Details functions

    #region Document level functions
        [DllImport(dllName)]
        private unsafe static extern int lxaGetSummary(IntPtr pSession, int nLength, umSalienceSummaryResult* pResult, IntPtr acId);

        [DllImport(dllName)]
        private unsafe static extern int lxaGetSentiment(IntPtr pSession, int nUseChains, umSalienceSentimentResult* pResult, IntPtr acId);

        [DllImport(dllName)]
        private unsafe static extern int lxaGetThemes(IntPtr pSession, umSalienceThemeList* pResults, IntPtr acId);

        [DllImport(dllName)]
        private unsafe static extern int lxaGetNamedEntities(IntPtr pSession, umSalienceEntityList* pResults, IntPtr acId);

        [DllImport(dllName)]
        private unsafe static extern int lxaGetUserDefinedEntities(IntPtr pSession, umSalienceEntityList* pResults, IntPtr acId);

        [DllImport(dllName)]
        private unsafe static extern int lxaGetQueryDefinedTopics(IntPtr pSession, umSalienceTopicList* pTopicList, IntPtr acId);

        [DllImport(dllName)]
        private unsafe static extern int lxaGetConceptDefinedTopics(IntPtr pSession, umSalienceTopicList* pTopicList, IntPtr acId);

        [DllImport(dllName)]
        private unsafe static extern int lxaGetDocumentClasses(IntPtr pSession, umSalienceTopicList *pTopicList, IntPtr acId);

        [DllImport(dllName)]
        private unsafe static extern int lxaGetDocumentCategories(IntPtr pSession, umSalienceTopicList* pTopicList, IntPtr acId);

        [DllImport(dllName)]
        private unsafe static extern int lxaExplainConceptMatches(IntPtr pSession, out IntPtr ppBuffer, IntPtr acId);

        [DllImport(dllName)]
        private unsafe static extern int lxaGetNamedEntityRelationships(IntPtr pSession, umSalienceRelationList* pResults, IntPtr acId);

        [DllImport(dllName)]
        private unsafe static extern int lxaGetNamedEntityOpinions(IntPtr pSession, umSalienceOpinionList* pResults, IntPtr acId);

        [DllImport(dllName)]
        private unsafe static extern int lxaGetUserEntityRelationships(IntPtr pSession, umSalienceRelationList* pResults, IntPtr acId);

        [DllImport(dllName)]
        private unsafe static extern int lxaGetUserEntityOpinions(IntPtr pSession, umSalienceOpinionList* pResults, IntPtr acId);

        [DllImport(dllName)]
        private unsafe static extern int lxaGetIntentions(IntPtr pSession, umSalienceIntentionList* pResults, IntPtr acId);

    #endregion // Document level functions

    #region Collection level functions
        [DllImport(dllName)]
        private unsafe static extern int lxaGetCollectionThemes(IntPtr pSession, ref umSalienceThemeList pResults, IntPtr acId);

        [DllImport(dllName)]
        private unsafe static extern int lxaGetCollectionFacets(IntPtr pSession, umSalienceFacetList* pResults, IntPtr acId);

        [DllImport(dllName)]
        private unsafe static extern int lxaGetCollectionQueryDefinedTopics(IntPtr pSession, umSalienceTopicList* pResults, IntPtr acId);

        [DllImport(dllName)]
        private unsafe static extern int lxaGetCollectionConceptDefinedTopics(IntPtr pSession, umSalienceTopicList* pResults, IntPtr acId);

        [DllImport(dllName)]
        private unsafe static extern int lxaGetCollectionEntities(IntPtr pSession, umSalienceCollectionEntityList* pResults, IntPtr acId);

        [DllImport(dllName)]
        private unsafe static extern int lxaGetCollectionUserEntities(IntPtr pSession, umSalienceCollectionEntityList* pResults, IntPtr acId);

    #endregion // Collection level functions

    #region Markup functions
        [DllImport(dllName)]
        private unsafe static extern int lxaGetNamedEntityMarkup(IntPtr pSession, umSalienceDocument* pDocument, IntPtr acId);

        [DllImport(dllName)]
        private unsafe static extern int lxaGetUserEntityMarkup(IntPtr pSession, umSalienceDocument* pDocument, IntPtr acId);

        [DllImport(dllName)]
        private unsafe static extern int lxaGetPOSMarkup(IntPtr pSession, umSalienceDocument* pDocument, IntPtr acId);

        [DllImport(dllName)]
        private unsafe static extern int lxaGetSentimentMarkup(IntPtr pSession, umSalienceDocument* pDocument, IntPtr acId);

        [DllImport(dllName)]
        private unsafe static extern int lxaGetNamedOpinionTaggedText(IntPtr pSession, out IntPtr acTaggedText, IntPtr acId);

        [DllImport(dllName)]
        private unsafe static extern int lxaGetUserOpinionTaggedText(IntPtr pSession, out IntPtr acTaggedText, IntPtr acId);

    #endregion // Markup functions

    #region Free functions
        [DllImport(dllName)]
        private unsafe static extern int lxaFreeEntityList(umSalienceEntityList* pResults);

        [DllImport(dllName)]
        private unsafe static extern int lxaFreeCollectionEntityList(umSalienceCollectionEntityList* pResults);

        [DllImport(dllName)]
        private unsafe static extern int lxaFreeThemeList(umSalienceThemeList* pThemesList);

        [DllImport(dllName)]
        private unsafe static extern int lxaFreeFacetList(umSalienceFacetList* pFacetList);

        [DllImport(dllName)]
        private unsafe static extern int lxaFreeRelationList(umSalienceRelationList* pRelationList);

        [DllImport(dllName)]
        private unsafe static extern int lxaFreeOpinionList(umSalienceOpinionList* pOpinionList);

        [DllImport(dllName)]
        private unsafe static extern int lxaFreeSentimentResult(umSalienceSentimentResult* pPhraseList);

        [DllImport(dllName)]
        private unsafe static extern int lxaFreeDocumentDetails(umSalienceDocumentDetails* pDetails);

        [DllImport(dllName)]
        private unsafe static extern int lxaFreeTopicList(umSalienceTopicList* pTopicList);

        [DllImport(dllName)]
        private unsafe static extern int lxaFreeDocument(umSalienceDocument* pDocument);

        [DllImport(dllName)]
        private unsafe static extern int lxaFreePhraseList(umSaliencePhraseList* pPhraseList);

        [DllImport(dllName)]
        private unsafe static extern int lxaFreeSummaryResult(umSalienceSummaryResult* pResult);

        [DllImport(dllName)]
        private unsafe static extern int lxaFreeIntentionList(umSalienceIntentionList* pIntentions);

    #endregion // Free methods

    #region Utility functions
        [DllImport(dllName)]
        private unsafe static extern int lxaGetDefaultSalienceLocation(out IntPtr ppBuffer);

        [DllImport(dllName)]
        private unsafe static extern int lxaGetSalienceVersion(out IntPtr ppBuffer);

        [DllImport(dllName)]
        private unsafe static extern int lxaGetSalienceErrorString(IntPtr pSession, out IntPtr ppErrorString);

        [DllImport(dllName)]
        private unsafe static extern int lxaGetLastWarnings(IntPtr pSession, out int nWarnings);

        [DllImport(dllName)]
        private unsafe static extern int lxaSetSalienceOption(IntPtr pSession, umSalienceOption* pOption, IntPtr acId);

        [DllImport(dllName)]
        private unsafe static extern int lxaSetSalienceCallback(IntPtr pSession, IntPtr pCallback, void* pParam, IntPtr acId);

        [DllImport(dllName)]
        private unsafe static extern int lxaDumpEnvironment(IntPtr pSession, out IntPtr acEnvironment);

    #endregion // Utility functions

#endregion // UNMANAGED C LAYER (CONNECTION TO UNDERLYING SALIENCE C API)


    }
}
