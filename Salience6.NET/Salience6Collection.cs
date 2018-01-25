using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Runtime.Serialization;

namespace Lexalytics
{
    public class SalienceCollection : Salience6Engine, IDisposable
    {
#region MANAGED .NET LAYER

    #region Collection-specific options
        private float m_NeutralSentimentUpperBound;
        /// <summary>
        /// Float defining the upper bound of a sentiment value to be considered neutral.
        /// Min/Max range   MAX FLOAT
        /// Default value   -0.5
        /// Adjusting this value will impact the counts returned for collection level sentiment as more or less documents will be considered positive or neutral.
        /// </summary>
        public float NeutralSentimentUpperBound
        {
            get { return m_NeutralSentimentUpperBound; }
            set { if (SetSalienceOption(4002, value) == 0) m_NeutralSentimentUpperBound = value; }
        }

        private float m_NeutralSentimentLowerBound;
        /// <summary>
        /// Float defining the lower bound of a sentiment value to be considered neutral
        /// Min/Max range   MAX FLOAT
        /// Default value   -0.45
        /// Adjusting this value will impact the counts returned for collection level sentiment as more or less documents will be considered negative or neutral.
        /// </summary>
        public float NeutralSentimentLowerBound
        {
            get { return m_NeutralSentimentLowerBound; }
            set { if (SetSalienceOption(4003, value) == 0) m_NeutralSentimentLowerBound = value; }
        }

        private int m_CollectionResultSize;
        /// <summary>
        /// Defines maximum number of results to return for collection functions such as facets or entities
        /// Min/max range   0-MAX INT
        /// Default value   20
        /// </summary>
        public int CollectionResultSize
        {
            get { return m_CollectionResultSize; }
            set { if (SetSalienceOption(6000, value) == 0) m_CollectionResultSize = value; }
        }

        private bool m_UseSemantics;
        /// <summary>
        /// Determines whether or not to utilize the concept matrix during collection operations.
		/// Concept matrix functionality is used for concept topics as well as the rollup of themes and facets at the collection level.
		/// Setting this option to false reduces memory footprint at the expense of combining similar results across the collection documents.
        /// By default, this option is set to true.
        /// </summary>
        public bool UseSemantics
        {
            get { return m_UseSemantics; }
            set { if (SetSalienceOption(6001, value) == 0) m_UseSemantics = value; }
        }

        private int m_MaxCollectionSize;
        /// <summary>
        /// Maximum number of documents that can be included in a single collection.
        /// Min/Max range   0-MAX INT
        /// Default value   50000
        /// </summary>
        /// <remarks>Added in Salience 5.1</remarks>
        public int MaxCollectionSize
        {
            get { return m_MaxCollectionSize; }
            set { if (SetSalienceOption(6003, value) == 0) m_MaxCollectionSize = value; }
        }

        private bool m_ReturnAllThemeMentions;
        /// <summary>
        /// Determines whether or not to return all theme mentions for collection themes.
        /// Default value is true, returning all theme mentions in the result structure for collection themes.
        /// Otherwise, it returns one occurrence for every unique theme text.
        /// </summary>
        /// <remarks>Added in Salience 5.1</remarks>
        public bool ReturnAllThemeMentions
        {
            get { return m_ReturnAllThemeMentions; }
            set { if (SetSalienceOption(6004, value) == 0) m_ReturnAllThemeMentions = value; }
        }

        private bool m_StemFacets;
        /// <summary>
        /// Controls stemming for facets and their respective attributes.
        /// Default value is true, returning the stemmed string for facets and their attributes.
        /// Otherwise, it will not stem the phrase and will return it as-is.
        /// </summary>
        /// <remarks>Added in Salience 5.1.1</remarks>
        public bool StemFacets
        {
            get { return m_StemFacets; }
            set { if (SetSalienceOption(6008, value) == 0) m_StemFacets = value; }
        }

    #endregion // Collection-specific options

    #region Constructors

        /// <summary>
        /// Standard constructor, requires path to license file and path to data directory
        /// </summary>
        /// <param name="sLicensePath">Path to a valid Lexalytics license file</param>
        /// <param name="sDataPath">Path to data directory to use for this session</param>
        /// <exception cref="SalienceException">
        /// Thrown if license file cannot be loaded or error in loading data directory (AccessViolationException), or unable to load underlying Salience Engine (DllNotFoundException)
        /// </exception>
        public SalienceCollection(string sLicensePath, string sDataPath) : base(sLicensePath, sDataPath) {}

        /// <summary>
        /// Standard constructor, requires path to license file and path to data directory
        /// </summary>
        /// <param name="sLicensePath">Path to a valid Lexalytics license file</param>
        /// <param name="sDataPath">Path to data directory to use for this session</param>
        /// <param name="sUserDir">Path to a user directory of customizations to use for this session</param>
        /// <exception cref="SalienceException">
        /// Thrown if license file cannot be loaded or error in loading data directory (AccessViolationException), or unable to load underlying Salience Engine (DllNotFoundException)
        /// </exception>
        public SalienceCollection(string sLicensePath, string sDataPath, string sUserDir) : base(sLicensePath, sDataPath, sUserDir) { }

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
        public SalienceCollection(string sLicensePath, string sDataPath, string sUserDir, string sLogPath) : base(sLicensePath, sDataPath, sUserDir, sLogPath) { }

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
        public SalienceCollection(string sLicensePath, string sDataPath, string sUserDir, string sLogPath, SALIENCE_MODE nSalienceMode) : base(sLicensePath, sDataPath, sUserDir, sLogPath, nSalienceMode) { }

        /// <summary>
        /// Standard constructor, requires path to license file and path to data directory
        /// </summary>
        /// <param name="sLicensePath">Path to a valid Lexalytics license file</param>
        /// <param name="oStartup">Startup object giving other session startup parameters</param>
        /// <exception cref="SalienceException">
        /// Thrown if license file cannot be loaded or error in loading data directory (AccessViolationException), or unable to load underlying Salience Engine (DllNotFoundException)
        /// </exception>
        public SalienceCollection(string sLicensePath, SalienceStartup oStartup) : base(sLicensePath, oStartup) { }

    #endregion

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

    #endregion // Utility methods

    #region Text preparation methods
        /// <summary>
        /// Prepares a collection from a list collection of strings
        /// </summary>
        /// <returns>Integer error return code</returns>
        /// <remarks>Wrapper around C API method lxaPrepareCollection</remarks>
        /// <exception cref="SalienceException">Thrown if text cannot be parsed</exception>
        new public int PrepareCollectionFromList(String sName,List<String> lstText)
        {
            return base.PrepareCollectionFromList(sName, lstText);
        }

        /// <summary>
        /// Prepares a collection from a file containing individual pieces of content, one per line
        /// </summary>
        /// <param name="sFile">Full path to a readable text file</param>
        /// <param name="sName">Name for the collection</param>
        /// <returns>Integer error return code</returns>
        /// <remarks>Wrapper around C API method lxaPrepareCollectionFromFile</remarks>
        /// <exception cref="SalienceException">Thrown if text cannot be parsed</exception>
        new public int PrepareCollectionFromFile(string sName, string sFile)
        {
            return base.PrepareCollectionFromFile(sName, sFile);
        }

    #endregion 

    #region Details methods
        /// <summary>
        /// Wrapper method to return details about the current collection
        /// </summary>
        /// <returns>SalienceCollectionDetails object</returns>
        /// <remarks>Wrapper around C API method lxaGetCollectionDetails</remarks>
        new public SalienceCollectionDetails GetCollectionDetails(String id = "")
        {
            return base.GetCollectionDetails(id);
        }

    #endregion // Details methods

    #region Collection level functions

        /// <summary>
        /// Returns a list of themes extracted from the collection
        /// </summary>
        /// <returns>A List of SalienceTheme objects, each containing information about a theme extracted from the collection</returns>
        /// <remarks>Wrapper around C API method lxaGetCollectionThemes</remarks>
        public List<SalienceTheme> GetThemes(String id = "")
        {
            return base.GetCollectionThemes(id);
        }

        /// <summary>
        /// Returns a list of entities extracted from the collection
        /// </summary>
        /// <returns>A List of SalienceCollectionEntity objects, each containing information about an entity extracted from the collection</returns>
        /// <remarks>Wrapper around C API method lxaGetCollectionEntities</remarks>
        public List<SalienceCollectionEntity> GetEntities(String id = "")
        {
            return base.GetCollectionEntities(id);
        }

        /// <summary>
        /// Returns a list of user entities extracted from the collection
        /// </summary>
        /// <returns>A List of SalienceCollectionEntity objects, each containing information about a user entity extracted from the collection</returns>
        /// <remarks>Wrapper around C API method lxaGetCollectionUserEntities</remarks>
        public List<SalienceCollectionEntity> GetUserEntities(String id = "")
        {
            return base.GetCollectionUserEntities(id);
        }

        /// <summary>
        /// Returns a list of facets extracted from the collection
        /// </summary>
        /// <returns>A List of SalienceFacet objects, each containing information about a facet and its attributed extracted from the collection</returns>
        /// <remarks>Wrapper around C API method lxaGetCollectionFacets</remarks>
        public List<SalienceFacet> GetFacets(String id = "")
        {
            return base.GetCollectionFacets(id);
        }

        /// <summary>
        /// Returns a list of topics extracted from the collection via tag query definitions.
        /// </summary>
        /// <returns>A List of SalienceTopic objects, each containing information about a topic identified for at least one document in the collection</returns>
        /// <remarks>Wrapper around C API method lxaGetCollectionQueryDefinedTopics</remarks>
        new public List<SalienceTopic> GetQueryDefinedTopics(String id = "")
        {
            return base.GetCollectionQueryDefinedTopics(id);
        }

        /// <summary>
        /// Returns a list of topics extracted from the collection via the concept matrix.
        /// </summary>
        /// <returns>A List of SalienceTopic objects, each containing information about a topic identified for at least one document in the collection</returns>
        /// <remarks>Wrapper around C API method lxaGetCollectionConceptDefinedTopics</remarks>
        public List<SalienceTopic> GetConceptTopics(String id = "")
        {
            return base.GetCollectionConceptTopics(id);
        }

    #endregion // Collection level functions
#endregion // MANAGED .NET LAYER

    }
}
