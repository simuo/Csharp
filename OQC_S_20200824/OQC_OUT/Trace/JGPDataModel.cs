using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OQC_OUT
{
    public class JGPDataModel
    {
        /// <summary>
        /// 
        /// </summary>
        public MainModel Main { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<InspectorItem> inspector { get; set; }
    }

    public class MainModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string serialnumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string project { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string color { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string region { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string line_location { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string pahse { get; set; }
    }

    public class InspectorItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string station_name { get; set; }
    }
}
