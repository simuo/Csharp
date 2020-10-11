using System;
using System.Collections.Generic;
using xxw.utilities;

namespace OQC_IN
{
    public class StopModel
    {
        /// <summary>
        /// 最后一片产品时间
        /// </summary>
        public DateTime? LastDateTime { get; set; }
        /// <summary>
        /// 停机原因
        /// </summary>
        public int? StopType { get; set; }
        /// <summary>
        /// 停机原因列表
        /// </summary>
        public List<StopCode> Codes { get; set; }
    }
    public  class StopCode
    {
        public string Text { get; set; }
        public string Code { get; set; }
    }
}
