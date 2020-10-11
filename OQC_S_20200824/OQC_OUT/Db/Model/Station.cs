using SqlSugar;

namespace OQC_OUT
{
    public class Station
    {
        [SugarColumn(IsPrimaryKey = true)]
        /// <summary>
        /// 工站码
        /// </summary>
        public string StationCode { get; set; }
        /// <summary>
        /// 工站号
        /// </summary>
        public string StationNum { get; set; }
        /// <summary>
        /// 工站名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string StationName { get; set; }
    }
}
