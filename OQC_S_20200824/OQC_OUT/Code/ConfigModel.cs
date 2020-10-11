using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace OQC_OUT
{
    public class ConfigModel
    {
        /// <summary>
        /// 验证线头入料
        /// </summary>
        public bool CheckIn { get; set; }
        /// <summary>
        /// 读码方向（left、right），注意Group行号调整
        /// </summary>
        public string Direction { get; set; }
        /// <summary>
        /// 工站名
        /// </summary>
        public List<string> Station { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public POSTData POSTData { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public TrayConfig Tray { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Vision Vision { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public TriggerConfig Trigger { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IOCardConfig IOCard { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<GroupItem> Group { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<DataMappingItem> DataMapping
        {
            get; set;
        }
        /// <summary>
        /// Trace上抛数据格式
        /// </summary>
        public JObject Trace { get; set; }
    }
    /// <summary>
    /// 上抛参数
    /// </summary>
    public class POSTData
    {
        public string ProcessName { get; set; }
        /// <summary>
        /// 启用OktoStart
        /// </summary>
        public bool ToOktoStart { get; set; }
        /// <summary>
        /// OktoStart上抛地址
        /// </summary>
        public string OkToStartUrl { get; set; }
        /// <summary>
        /// OktoStart上抛数据
        /// </summary>
        public string OktoStartParam { get; set; }
        /// <summary>
        /// 启用IFactory上抛
        /// </summary>
        public bool ToIFactory { get; set; }
        /// <summary>
        /// IFactory上抛地址
        /// </summary>
        public string IFactoryUrl { get; set; }
        /// <summary>
        /// IFactory上抛数据
        /// </summary>
        public string IFactoryParam { get; set; }
        /// <summary>
        /// 启用JGP上抛
        /// </summary>
        public bool ToJGP { get; set; }
        /// <summary>
        /// JGP上抛地址
        /// </summary>
        public string JGPUrl { get; set; }
        public string GetUserUrl { get; set; }
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
        /// 启用颜色国别检测
        /// </summary>
        public bool CheckFG { get; set; }
        /// <summary>
        /// Band码获取类型
        /// </summary>
        public string GetBandType { get; set; } = "jgp";
        /// <summary>
        /// Band码获取地址
        /// </summary>
        public string GetBandUrl { get; set; }
        /// <summary>
        /// 启用重复上抛检测
        /// </summary>
        public bool CheckRepeat { get; set; }
        /// <summary>
        /// 启用机台参数验证
        /// </summary>
        public bool CheckParam { get; set; }
        /// <summary>
        /// 机台参数验证URL
        /// </summary>
        public string CheckParamUrl { get; set; }
    }
    /// <summary>
    /// 料盘参数
    /// </summary>
    public class TrayConfig
    {
        /// <summary>
        /// 料盘长度（mm）
        /// </summary>
        public double Length { get; set; }
        /// <summary>
        /// 第一片产品间隔距离（mm）
        /// </summary>
        public double FirstInterval { get; set; }
        /// <summary>
        /// 其他产品间隔距离（mm）
        /// </summary>
        public double OtherInterval { get; set; }
        /// <summary>
        /// NG时停机距离(mm)
        /// </summary>
        public double StopInterval { get; set; }
    }
    /// <summary>
    /// 视觉配置
    /// </summary>
    public class Vision
    {
        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 视野（mm）
        /// </summary>
        public double View { get; set; }
        /// <summary>
        /// 像数（px）
        /// </summary>
        public double Pixel { get; set; }
    }
    /// <summary>
    /// 触发配置
    /// </summary>
    public class TriggerConfig
    {
        /// <summary>
        /// 触发命令发送次数
        /// </summary>
        public int CommandSendTimes { get; set; }
        /// <summary>
        /// 多次发送命令延时时间(ms)
        /// </summary>
        public int CommandSendDelay { get; set; }
        /// <summary>
        /// 接收数据超时时间(ms)
        /// </summary>
        public int ReceiveTimeOut { get; set; }
    }
    /// <summary>
    /// IO卡参数
    /// </summary>
    public class IOCardConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// IO 板号
        /// </summary>
        public int CardNo { get; set; }
        /// <summary>
        /// 料盘信号IO口
        /// </summary>
        public int TrayNo { get; set; }
        /// <summary>
        /// 上升沿滤波时长（ms）
        /// </summary>
        public int TrayOnDelay { get; set; }
        /// <summary>
        /// 下降沿滤波时长（ms）
        /// </summary>
        public int TrayOffDelay { get; set; }
        /// <summary>
        /// 控制机台启停IO接口号
        /// </summary>
        public int MachineControlNo { get; set; }
        /// <summary>
        /// 机台状态IO接口号
        /// </summary>
        public int MachineStateNo { get; set; }
        /// <summary>
        /// 机台状态滤波时长（ms）
        /// </summary>
        public int MachineStateDelay { get; set; }
        /// <summary>
        /// 停机补偿时长（ms）
        /// </summary>
        public int MachineStopCompensate { get; set; }
        /// <summary>
        /// 停机报警IO接口号
        /// </summary>
        public int MachineWaringNo { get; set; }
        /// <summary>
        /// 停机报警持续时长（ms）
        /// </summary>
        public int MachineWaringTime { get; set; }
    }

    public class GroupItem
    {
        /// <summary>
        /// 行号
        /// </summary>
        public int Row { get; set; }
        /// <summary>
        /// 相机号
        /// </summary>
        public List<int> CAMNO { get; set; }
        /// <summary>
        /// 命令
        /// </summary>
        public string Command { get; set; }
    }

    public class MappingItem
    {
        /// <summary>
        /// 映射名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int Index { get; set; }
    }

    public class DataMappingItem
    {
        /// <summary>
        /// 相机号
        /// </summary>
        public List<int> CAMNO { get; set; }
        /// <summary>
        /// 映射关系
        /// </summary>
        public List<MappingItem> Mapping { get; set; }
    }
}
