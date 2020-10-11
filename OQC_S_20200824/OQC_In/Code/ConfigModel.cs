using System.Collections.Generic;

namespace OQC_IN
{
    public class ConfigModel
    {
        /// <summary>
        /// 
        /// </summary>
        public bool CheckOut { get; set; }
        public int StopDelay { get; set; }
        public string LineOutIp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public VisionConfig Vision { get; set; }
        public POSTData POSTData { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<TriggerConfig> Trigger { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IOCardConfig IOCard { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<DataMappingItem> DataMapping { get; set; }
    }
    public class VisionConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Port { get; set; }
    }
    /// <summary>
    /// 上抛参数
    /// </summary>
    public class POSTData
    {
        public string ProcessName { get; set; }
        /// <summary>
        /// 启用 Trace Process Control
        /// </summary>
        public bool ProcessControl { get; set; }
        /// <summary>
        /// Trace Process Control 地址
        /// </summary>
        public string ProcessControlUrl { get; set; }
        /// <summary>
        /// 上抛SN类型 band-band码，fg-fg码
        /// </summary>
        public string SnType { get; set; }
        /// <summary>
        /// 启用trace上抛
        /// </summary>
        public bool ToTrace { get; set; }
        /// <summary>
        /// trace上抛地址
        /// </summary>
        public string TeaceUrl { get; set; }
        /// <summary>
        /// Band码获取类型
        /// </summary>
        public string GetBandType { get; set; } = "jgp";
        /// <summary>
        /// Band码获取地址
        /// </summary>
        public string GetBandUrl { get; set; }
    }

    public class TriggerConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Command { get; set; }
    }

    public class IOCardConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string Enable { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int CardNo { get; set; }
        public bool MachineStopEnable { get; set; }
        public int MachineStopNo { get; set; }
        public int MachineStopDelay { get; set; }
        public List<LineConfig> Line { get; set; }
    }

    public class LineConfig
    {
        public int TriggerNo { get; set; }
        public int MachineWaringNo { get; set; }
        public int MachineWaringTime { get; set; }
        public int MachineWaringDelay { get; set; }
        public int OnDelay { get; set; }
        public int OffDelay { get; set; }
    }

    public class MappingItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Index { get; set; }
    }

    public class DataMappingItem
    {
        /// <summary>
        /// 
        /// </summary>
        public List<int> CAMNO { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<MappingItem> Mapping { get; set; }
    }
}
