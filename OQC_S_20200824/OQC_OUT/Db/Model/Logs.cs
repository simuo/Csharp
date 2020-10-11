using System;

namespace OQC_OUT
{
    public class Logs
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime CreateDate { get; set; }
        public string User { get; set; }
        public string LogInfo { get; set; }
    }

    public static class LogsHelper
    {
        public static void LogWrite(string msg)
            => new DbContext().LogsDb.Insert(new Logs
            {
                CreateDate = DateTime.Now,
                User = Admin.LoginAdmin?.UserName ?? "",
                LogInfo = msg
            });
    }
}
