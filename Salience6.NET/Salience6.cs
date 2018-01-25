using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Lexalytics
{
    public class Salience : Salience6Engine, IDisposable
    {

#region MANAGED .NET LAYER
    #region Document-specific options
        private bool m_UseSemantics;
        /// <summary>
        /// Determines whether or not to utilize the concept matrix during document operations
        /// By default, this option is set to false.
        /// </summary>
        public bool UseSemantics
        {
            get { return m_UseSemantics; }
            set { if (SetSalienceOption(1006, value) == 0) m_UseSemantics = value; }
        }
    #endregion // Document-specific options

    #region Constructors

        /// <summary>
        /// Standard constructor, requires path to license file and path to data directory
        /// </summary>
        /// <param name="sLicensePath">Path to a valid Lexalytics license file</param>
        /// <param name="sDataPath">Path to data directory to use for this session</param>
        /// <exception cref="SalienceException">
        /// Thrown if license file cannot be loaded or error in loading data directory (AccessViolationException), or unable to load underlying Salience Engine (DllNotFoundException)
        /// </exception>
        public Salience(string sLicensePath, string sDataPath) : base(sLicensePath, sDataPath) {}

        /// <summary>
        /// Standard constructor, requires path to license file and path to data directory
        /// </summary>
        /// <param name="sLicensePath">Path to a valid Lexalytics license file</param>
        /// <param name="sDataPath">Path to data directory to use for this session</param>
        /// <param name="sUserDir">Path to a user directory of customizations to use for this session</param>
        /// <exception cref="SalienceException">
        /// Thrown if license file cannot be loaded or error in loading data directory (AccessViolationException), or unable to load underlying Salience Engine (DllNotFoundException)
        /// </exception>
        public Salience(string sLicensePath, string sDataPath, string sUserDir) : base(sLicensePath, sDataPath, sUserDir) { }

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
        public Salience(string sLicensePath, string sDataPath, string sUserDir, string sLogPath) : base(sLicensePath, sDataPath, sUserDir, sLogPath) { }

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
        public Salience(string sLicensePath, string sDataPath, string sUserDir, string sLogPath, SALIENCE_MODE nSalienceMode) : base(sLicensePath, sDataPath, sUserDir, sLogPath, nSalienceMode) { }

        /// <summary>
        /// Standard constructor, requires path to license file and path to data directory
        /// </summary>
        /// <param name="sLicensePath">Path to a valid Lexalytics license file</param>
        /// <param name="oStartup">Startup object giving other session startup parameters</param>
        /// <exception cref="SalienceException">
        /// Thrown if license file cannot be loaded or error in loading data directory (AccessViolationException), or unable to load underlying Salience Engine (DllNotFoundException)
        /// </exception>
        public Salience(string sLicensePath, SalienceStartup oStartup) : base(sLicensePath, oStartup) {  }

    #endregion // Constructors

    #region Object disposal
        new public void Dispose()
        {
            base.Dispose();
        } 
    #endregion 

    #region Utility methods
        /// <summary>
        /// Wrapper method to return the Salience Engine version
        /// </summary>
        /// <returns>String with Salience Engine version information</returns>
        /// <remarks>Wrapper around C API method lxaGetSalienceVersion</remarks>
        /// <exception cref="SalienceException">Thrown if version information is not available</exception>
        new public string GetSalienceVersion()
        {
            return base.GetSalienceVersion();
        }

        /// <summary>
        /// Wrapper method to add an additional configuration to the session. This configuration can now be accessed anywhere by the given id
        /// </summary>
        /// <param name="userDir">User Directory to initialize configuration with</param>
        /// <param name="configurationID">Name you'll use to access this configuration</param>
        public new void AddSalienceConfiguration(String userDir, String configurationID)
        {
            base.AddSalienceConfiguration(userDir, configurationID);
        }

        /// <summary>
        /// Wrapper method to remove a configuration previously added to this session. The configuration id will no longer be valid.
        /// </summary>
        /// <param name="configurationID">Name of configuration you wish to remove from memory</param>
        public new void RemoveSalienceConfiguration(String configurationID)
        {
            base.RemoveSalienceConfiguration(configurationID);
        }


        /// <summary>
        /// Static method for retrieving the default Salience location without needing to instantiate an object
        /// </summary>
        /// <returns>String path to Salience installation</returns>
        new public static string GetDefaultSalienceLocation()
        {
            return Salience6Engine.GetDefaultSalienceLocation();
        }

        /// <summary>
        /// Wrapper method to return the output of Salience Engine operating environment
        /// </summary>
        /// <returns>String with Salience Engine environment information</returns>
        /// <remarks>Wrapper around C API method lxaDumpEnvironment</remarks>
        /// <exception cref="SalienceException">Thrown if version information is not available</exception>
        new public string DumpEnvironment()
        {
            return base.DumpEnvironment();
        }

        /// <summary>
        /// Returns an integer error code indicating last warning encountered by Salience Engine, generally in the process of text preparation
        /// </summary>
        /// <returns>Integer return code</returns>
        /// <remarks>Wrapper around C API method lxaGetLastWarnings</remarks>
        new public int GetLastWarnings()
        {
            return base.GetLastWarnings();
        }

        /// <summary>
        /// Wrapper method that sets the underlying SentimentDictionary option with an additional reset flag parameter. 
        /// </summary>
        /// <param name="sDictionary">Path to a sentiment hand-scored dictionary (HSD).</param>
        /// <param name="bReset">Flag to clear all previously loaded HSD files</param>
        /// <exception cref="SalienceException">Thrown if setting the option fails.</exception>
        new public int AddSentimentDictionary(string sDictionary, bool bReset, string sConfig = "")
        {
            return base.AddSentimentDictionary(sDictionary, bReset, sConfig);
        }


    #endregion // Utility methods

    #region Text Preparation
        /// <summary>
        /// Wrapper method to submit text to Salience Engine for processing
        /// </summary>
        /// <param name="sText">The text to be processed</param>
        /// <returns>Integer error return code</returns>
        /// <remarks>Wrapper around C API method lxaPrepareText</remarks>
        /// <exception cref="SalienceException">Thrown if text cannot be parsed</exception>
        new public int PrepareText(string sText)
        {
            return base.PrepareText(sText);
        }

        /// <summary>
        /// Wrapper method to submit contents of a file to Salience Engine for processing
        /// </summary>
        /// <param name="sFile">Full path to a readable text file</param>
        /// <returns>Integer error return code</returns>
        /// <remarks>Wrapper around C API method lxaPrepareTextFromFile</remarks>
        /// <exception cref="SalienceException">Thrown if file cannot be found or text cannot be parsed</exception>
        new public int PrepareTextFromFile(string sFile)
        {
            return base.PrepareTextFromFile(sFile);
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
        new public int AddSection(string sHeader, string sText, bool bProcess)
        {
            return base.AddSection(sHeader, sText, bProcess);
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
        new public int AddSectionFromFile(string sHeader, string sFile, bool bProcess)
        {
            return base.AddSectionFromFile(sHeader, sFile, bProcess);
        }

    #endregion // Text Preparation

    #region Details functions
        /// <summary>
        /// Wrapper method to return details about current document
        /// </summary>
        /// <returns>SalienceDocumentDetails object</returns>
        /// <remarks>Wrapper around C API method lxaGetDocumentDetails</remarks>
        new public SalienceDocumentDetails GetDocumentDetails(String id="")
        {
            return base.GetDocumentDetails(id);
        }

    #endregion // Details functions

    #region Document methods

        /// <summary>
        /// Returns a summary for the document
        /// </summary>
        /// <param name="nLength">The length in sentences that the summary should be.</param>
        /// <returns>A text string containing a summary of the document up to nLength sentences.</returns>
        /// <remarks>Wrapper around C API method lxaGetSummary</remarks>
        new public SalienceSummary GetSummary(int nLength, String id = "")
        {
            return base.GetSummary(nLength, id);
        }   

        /// <summary>
        /// Returns the sentiment analysis for the document
        /// </summary>
        /// <param name="bUseChains">A boolean indicating whether or not to use lexical chains for sentiment phrase analysis</param>
        /// <returns>A SalienceSentiment object containing members with sentiment results</returns>
        /// <remarks>Wrapper around C API method lxaGetSentiment</remarks>
        new public SalienceSentiment GetDocumentSentiment(bool bUseChains, String id = "")
        {
            return base.GetDocumentSentiment(bUseChains, id);
        }

        /// <summary>
        /// Returns a list of themes extracted from the document
        /// </summary>
        /// <returns>A List of SalienceTheme objects, each containing information about a theme extracted from the document</returns>
        /// <remarks>Wrapper around C API method lxaGetThemes</remarks>
        new public List<SalienceTheme> GetDocumentThemes(String id = "")
        {
            return base.GetDocumentThemes(id);
        }

        /// <summary>
        /// Returns a list of intentions expressed in the document
        /// </summary>
        /// <returns>A List of SalienceIntention objects, each containing informatio about an intention expressed in the document</returns>
        /// <remarks>Wrapper around C API method lxaGetDocumentIntentions</remarks>
        new public List<SalienceIntention> GetDocumentIntentions(String id = "")
        {
            return base.GetDocumentIntentions(id);
        }

        /// <summary>
        /// Returns a list of topics extracted from the document via tag query definitions.
        /// This is equivalent to the SE4.4 method GetDocumentTags.
        /// </summary>
        /// <returns>A List of SalienceTopic objects, each containing information about a topic identified for the document</returns>
        /// <remarks>Wrapper around C API method lxaGetQueryDefinedTopics</remarks>
        new public List<SalienceTopic> GetQueryDefinedTopics(String id = "")
        {
            return base.GetQueryDefinedTopics(id);
        }

        /// <summary>
        /// Returns a list of topics extracted from the document via the concept matrix.
        /// </summary>
        /// <returns>A List of SalienceTopic objects, each containing information about a topic identified for the document</returns>
        /// <remarks>Wrapper around C API method lxaGetConceptDefinedTopics</remarks>
        new public List<SalienceTopic> GetConceptDefinedTopics(String id = "")
        {
            return base.GetConceptDefinedTopics(id);
        }

        /// <summary>
        /// Returns a list of classifications of the current document.
        /// </summary>
        /// <returns>A List of SalienceTopic objects, each containing information about a class extracted from the document through the classification model</returns>
        /// <remarks>Wrapper around C API method lxaGetDocumentClasses</remarks>
        new public List<SalienceTopic> GetDocumentClasses(String id = "")
        {
            return base.GetDocumentClasses(id);
        }

        /// <summary>
        /// Returns a list of category topics extracted from the document via a topic model.
        /// </summary>
        /// <returns>A List of SalienceTopic objects, each containing information about a category identified for the document</returns>
        /// <remarks>Wrapper around C API method lxaGetDocumentCategories</remarks>
        new public List<SalienceTopic> GetDocumentCategories(String id = "")
        {
            return base.GetDocumentCategories(id);
        }


        /// <summary>
        /// Returns a string listing the top contributing words and phrases from the current document for each Concept Topic Match.
        /// </summary>
        /// <returns>A text string with concept topic results, and contributing words/phrases.</returns>
        /// <remarks>Wrapper around C API method lxaExplainConceptMatches</remarks>
        new public string ExplainConceptMatches(String id = "")
        {
            return base.ExplainConceptMatches(id);
        }

    #endregion // Document methods

    #region Entity methods

        /// <summary>
        /// Returns a list of named entities extracted from the document.
        /// This is equivalent to the SE4.4 method GetEntities.
        /// </summary>
        /// <returns>A List of SalienceEntity objects, each containing information about a named entity extracted from the document.</returns>
        /// <remarks>Wrapper around C API method lxaGetNamedEntities</remarks>
        new public List<SalienceEntity> GetNamedEntities(String id = "")
        {
            return base.GetNamedEntities(id);
        }

        /// <summary>
        /// Returns a list of named entities extracted from the document.
        /// This is equivalent to the SE4.4 method GetEntities.
        /// </summary>
        /// <param name="Params">A parameter structure of options for entity extraction</param>
        /// <returns>A List of SalienceEntity objects, each containing information about a named entity extracted from the document.</returns>
        /// <remarks>Wrapper around C API method lxaGetNamedEntities</remarks>
        new public List<SalienceEntity> GetNamedEntities(SalienceEntityParams Params, String id = "")
        {
            return base.GetNamedEntities(Params, id);
        }

        /// <summary>
        /// Returns a list of named entities extracted from the document.
        /// This is equivalent to the SE4.4 method GetSimpleEntities and GetQueryDefinedEntities.
        /// </summary>
        /// <returns>A List of SalienceEntity objects, each containing information about a user-defined entity extracted from the document.</returns>
        /// <remarks>Wrapper around C API method lxaGetUserDefinedEntities</remarks>
        new public List<SalienceEntity> GetUserDefinedEntities(String id = "")
        {
            return base.GetUserDefinedEntities(id);
        }

        /// <summary>
        /// Returns a list of relationships for named entities extracted from the document.
        /// This is equivalent to the SE4.4 method GetEntityRelationships.
        /// </summary>
        /// <returns>A List of SalienceRelationship objects, each containing information about a relationship between multiple named entities</returns>
        /// <remarks>Wrapper around C API method lxaGetNamedRelationships</remarks>
        new public List<SalienceRelationship> GetNamedEntityRelationships(int nThreshold, String id = "")
        {
            return base.GetNamedEntityRelationships(nThreshold, id);
        }

        /// <summary>
        /// Returns a list of opinions for named entities extracted from the document.
        /// This is equivalent to the SE4.4 method GetEntityOpinions.
        /// </summary>
        /// <returns>A List of SalienceOpinion objects, each containing information about an opinion related to a named entity</returns>
        /// <remarks>Wrapper around C API method lxaGetNamedEntityOpinions</remarks>
        new public List<SalienceOpinion> GetNamedEntityOpinions(int nThreshold, String id = "")
        {
            return base.GetNamedEntityOpinions(nThreshold, id);
        }

        /// <summary>
        /// Returns a list of relationships for user-defined entities extracted from the document.
        /// This is equivalent to the SE4.4 method GetEntityRelationships for named entities, but for simple and query-defined entities.
        /// </summary>
        /// <returns>A List of SalienceRelationship objects, each containing information about a relationship between multiple user-defined entities</returns>
        /// <remarks>Wrapper around C API method lxaGetUserEntityRelationships</remarks>
        new public List<SalienceRelationship> GetUserEntityRelationships(String id = "")
        {
            return base.GetUserEntityRelationships(id);
        }

        /// <summary>
        /// Returns a list of opinions for user-defined entities extracted from the document.
        /// This is equivalent to the SE4.4 method GetEntityOpinions for named entities, but for simple and query-defined entities.
        /// </summary>
        /// <returns>A List of SalienceOpinion objects, each containing information about an opinion related to a user-defined entity</returns>
        /// <remarks>Wrapper around C API method lxaGetUserEntityOpinions</remarks>
        new public List<SalienceOpinion> GetUserEntityOpinions(String id = "")
        {
            return base.GetUserEntityOpinions(id);
        }

    #endregion // Entity methods

    #region Markup functions
        new public string GetEntityTaggedText(String id = "")
        {
            return base.GetEntityTaggedText(id);
        }

        new public string GetUserEntityTaggedText(String id = "")
        {
            return base.GetUserEntityTaggedText(id);
        }

        new public string GetPOSTaggedText(String id = "")
        {
            return base.GetPOSTaggedText(id);
        }

        new public string GetSentimentTaggedText(String id = "")
        {
            return base.GetSentimentTaggedText(id);
        }

        new public string GetNamedOpinionTaggedText(String id = "")
        {
            return base.GetNamedOpinionTaggedText(id);
        }

        new public string GetUserOpinionTaggedText(String id = "")
        {
            return base.GetUserOpinionTaggedText(id);
        }

    #endregion // Markup functions

        #region Deprecated methods
        /// <summary>
        /// DEPRECATED: This method has been replaced with the property SalienceSix.EntityList. 
        /// The method has been retained for backward compatibility, it sets the appropriate property. It may be removed in later releases.
        /// </summary>
        /// <remarks>DEPRECATED METHOD</remarks>
        /// <param name="sFile">Path to a CDL file containing user-defined entities to extract from content.</param>
        /// <exception cref="SalienceException">Thrown if setting the option fails.</exception>
        public void SetSimpleEntitiesFile(string sFile)
        {
            base.EntityList = sFile;
        }

        /// <summary>
        /// DEPRECATED: This method has been replaced with the property SalienceSix.UserDirectory. 
        /// The method has been retained for backward compatibility, it sets the appropriate property. It may be removed in later releases.
        /// </summary>
        /// <remarks>DEPRECATED METHOD</remarks>
        /// <param name="sFile">Path to a user data directory containing customized datafiles.</param>
        /// <exception cref="SalienceException">Thrown if setting the option fails.</exception>
        public void SetUserDirectory(string sDirectory)
        {
            base.UserDirectory = sDirectory;
        }

        /// <summary>
        /// DEPRECATED: This method has been replaced with the property SalienceSix.TopicList. 
        /// The method has been retained for backward compatibility, it sets the appropriate property. It may be removed in later releases.
        /// </summary>
        /// <remarks>DEPRECATED METHOD</remarks>
        /// <param name="sFile">Path to a file of topic queries.</param>
        /// <exception cref="SalienceException">Thrown if setting the option fails.</exception>
        public void SetTagDefinitions(string sDefinitions)
        {
            base.TopicList = sDefinitions;
        }

    #endregion // Deprecated methods

#endregion // MANAGED .NET LAYER
    }
}
