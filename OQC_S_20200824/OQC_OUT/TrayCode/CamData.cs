using System.Collections.Generic;

namespace OQC_OUT
{
    public class CamData
    {
        /// <summary>
        /// 相机号
        /// </summary>
        public int CamNo { get; set; }
        /// <summary>
        /// 读码次数
        /// </summary>
        public int ReadTimes { get; set; }
        /// <summary>
        /// 读码结果
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 产品状态（0、无产品，1、有产品，2、产品颠倒，3、产品翻盖，-1、信号异常，-2、读码返回数据异常）
        /// </summary>
        public double State { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public List<string> Data { get; set; }
    }
}
