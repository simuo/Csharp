using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OQC_OUT
{
    public class UsersBase
    {
        [SugarColumn(IsPrimaryKey = true)]
        /// <summary>
        /// 工号
        /// </summary>
        public string UserNumber { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 姓名拼音
        /// </summary>
        public string UserNamePY { get; set; }
        /// <summary>
        /// 等级
        /// </summary>
        public string Grade { get; set; }
        /// <summary>
        /// 入职日期
        /// </summary>
        public DateTime? OnboardDate { get; set; }
        /// <summary>
        /// 上岗日期
        /// </summary>
        public DateTime? QualificationDate { get; set; }
    }
}
