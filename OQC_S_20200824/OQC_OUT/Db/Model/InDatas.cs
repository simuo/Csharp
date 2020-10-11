using SqlSugar;
using System;

namespace OQC_OUT
{
    public class InDatas
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }
        /// <summary>
        /// Band码
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string SN { get; set; }
        /// <summary>
        /// FG码
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string FG { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
