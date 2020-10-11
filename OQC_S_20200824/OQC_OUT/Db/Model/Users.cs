using SqlSugar;
using System;
using xxw.utilities;

namespace OQC_OUT
{
    public class Users: BaseModel
    {
        [SugarColumn(IsPrimaryKey = true)]
        /// <summary>
        /// 工号
        /// </summary>
        public string UserNumber { get; set; }
        /// <summary>
        /// 工站码
        /// </summary>
        public string UserCode { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 工站
        /// </summary>
        public string UserType { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string UserPhoto { get; set; }
        /// <summary>
        /// 用户姓名（拼音）
        /// </summary>
        public string UserNamePY { get; set; }
        /// <summary>
        /// 坐位号
        /// </summary>
        public string SeatingCode { get; set; }
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
        /// <summary>
        /// 班别
        /// </summary>
        public string UserClasses { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string StationName
        {
            get
            {
                if (string.IsNullOrEmpty(UserType)) return "";
                int index = int.Parse(UserType);
                if (App.Config.Station.Count < index)
                    return "";
                return App.Config.Station[index - 1];
                //return App.Config.Station.Count < int.Parse(UserType) ? "" : string.IsNullOrEmpty(UserType) ? "" : App.Config.Station[int.Parse(UserType) - 1];
            }
        }
    }
}
