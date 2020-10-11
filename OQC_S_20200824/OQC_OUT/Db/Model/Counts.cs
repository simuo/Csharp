using System;

namespace OQC_OUT
{
    public class Counts
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; } = 1;
        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// OK数
        /// </summary>
        public int OkCount { get; set; }
        /// <summary>
        /// Ng数
        /// </summary>
        public int NgCount { get; set; }
        /// <summary>
        /// 重复数
        /// </summary>
        public int ReCount { get; set; }
        /// <summary>
        /// 空盘数
        /// </summary>
        public int NullCount { get; set; }
        /// <summary>
        /// 颠倒数
        /// </summary>
        public int DianDaoCount { get; set; }
        /// <summary>
        /// 翻盖数
        /// </summary>
        public int FanGaiCount { get; set; }
        /// <summary>
        /// 需上抛总数
        /// </summary>
        public int PostTotalCount { get; set; }
        /// <summary>
        /// 上抛JGP数
        /// </summary>
        public int JGPCount { get; set; }
        /// <summary>
        /// 上抛Trace数
        /// </summary>
        public int TraceCount { get; set; }
        /// <summary>
        /// 上抛IFactory数
        /// </summary>
        public int IFactoryCount { get; set; }
        /// <summary>
        /// 最后读取时间
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public DateTime? LastDate { get; set; }
    }
}
