namespace OQC_OUT
{
    public class TraceModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Serials serials { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Data data { get; set; }
    }
    public class Serials
    {
        /// <summary>
        /// 
        /// </summary>
        public string fg { get; set; }
    }

    public class Test_attributes
    {
        /// <summary>
        /// 
        /// </summary>
        public string test_result { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string unit_serial_number { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string uut_start { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string uut_stop { get; set; }
    }

    public class Test_station_attributes
    {
        /// <summary>
        /// 
        /// </summary>
        public string line_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string station_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fixture_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string software_name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string software_version { get; set; }
    }

    public class Uut_attributes
    {
        /// <summary>
        /// 
        /// </summary>
        public string STATION_STRING { get; set; }
    }

    public class Insight
    {
        /// <summary>
        /// 
        /// </summary>
        public Test_attributes test_attributes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Test_station_attributes test_station_attributes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Uut_attributes uut_attributes { get; set; }
    }

    public class Data
    {
        /// <summary>
        /// 
        /// </summary>
        public Insight insight { get; set; }
    }
}
