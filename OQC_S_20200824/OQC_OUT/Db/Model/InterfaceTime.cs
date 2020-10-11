using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OQC_OUT
{
    public class InterfaceTime
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string InterfaceType { get; set; }
        public double RequestTime { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
