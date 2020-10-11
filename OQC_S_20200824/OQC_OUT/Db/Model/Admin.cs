using System;

namespace OQC_OUT
{
    public class Admin
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string UserType { get; set; }

        [SqlSugar.SugarColumn(IsIgnore = true)]
        public static Admin LoginAdmin { get; set; }
    }
}
