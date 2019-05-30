using System;
using System.Collections.Generic;
using System.Text;

namespace TFSCollector.Model
{   

    public interface IARConfig
    {
        string UserToken { get; set; }
        string BaseUri { get; set; }
        string ProjectName { get; set; }
        string DefaultQuery { get; set; }
        DateTime LastUpdate { get; set; }
    }

    public class ARConfig : IARConfig
    {
        #region Declarations

        #endregion

        #region Constructors

        #endregion

        #region Methods

        #endregion

        #region Properties
        public string UserToken { get; set; }
        public string BaseUri { get; set; }
        public string ProjectName { get; set; }
        public string DefaultQuery { get; set; }
        public DateTime LastUpdate { get; set; }
        #endregion
    }
}
