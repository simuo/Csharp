using SqlSugar;
using System;

namespace OQC_OUT
{
    public class Datas
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
        public string FGCode { get; set; }
        /// <summary>
        /// 线体号
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string StationId { get; set; }
        /// <summary>
        /// 检验员1号编码
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Ins1Code { get; set; }
        /// <summary>
        /// 检验员2号编码
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Ins2Code { get; set; }
        /// <summary>
        /// 检验员3号编码
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Ins3Code { get; set; }
        /// <summary>
        /// 检验员4号编码
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Ins4Code { get; set; }
        /// <summary>
        /// 检验员5号编码
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Ins5Code { get; set; }
        /// <summary>
        /// 检验员6号编码
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Ins6Code { get; set; }
        /// <summary>
        /// 检验员7号编码
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Ins7Code { get; set; }
        /// <summary>
        /// 检验员8号编码
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Ins8Code { get; set; }
        /// <summary>
        /// 检验员9号编码
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Ins9Code { get; set; }
        /// <summary>
        /// 检验员10号编码
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Ins10Code { get; set; }
        /// <summary>
        /// 检验员1号名称
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Ins1Name { get; set; }
        /// <summary>
        /// 检验员2号名称
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Ins2Name { get; set; }
        /// <summary>
        /// 检验员3号名称
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Ins3Name { get; set; }
        /// <summary>
        /// 检验员4号名称
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Ins4Name { get; set; }
        /// <summary>
        /// 检验员5号名称
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Ins5Name { get; set; }
        /// <summary>
        /// 检验员6号名称
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Ins6Name { get; set; }
        /// <summary>
        /// 检验员7号名称
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Ins7Name { get; set; }
        /// <summary>
        /// 检验员8号名称
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Ins8Name { get; set; }
        /// <summary>
        /// 检验员9号名称
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Ins9Name { get; set; }
        /// <summary>
        /// 检验员10号名称
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Ins10Name { get; set; }
        /// <summary>
        /// 产品颜色
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Color { get; set; }
        /// <summary>
        /// 国别
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Region { get; set; }
        /// <summary>
        /// 专案
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Project { get; set; }
        /// <summary>
        /// 专案
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Location { get; set; }
        /// <summary>
        /// 生产阶段
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Pahse { get; set; }
        /// <summary>
        /// 是否上抛到JGP web service
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public bool? JgpPost { get; set; } = false;
        /// <summary>
        /// JGP 上抛返回信息
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string JgpPostInformation { get; set; }
        /// <summary>
        /// 是否上抛 Trace
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public bool? TracePost { get; set; } = false;
        /// <summary>
        /// Trace 上抛返回信息
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string TracePostInformation { get; set; }
        /// <summary>
        /// 是否上抛 IFactory
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public bool? IFactoryPost { get; set; } = false;
       
        /// <summary>
        /// IFactory 上抛返回信息
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string IFactoryPostInformation { get; set; }

        /// <summary>
        /// 是否上抛 OktoStart
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public bool? OktoStartPost { get; set; } = false;
        /// <summary>
        /// IFactory 上抛返回信息
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string OktoStartPostInformation { get; set; }
        /// <summary>
        /// 信息创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
