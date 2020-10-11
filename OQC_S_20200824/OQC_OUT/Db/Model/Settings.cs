using System;
using System.Linq;

namespace OQC_OUT
{
    public class Settings
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Type { get; set; }
        public string Value { get; set; }
        public bool IsSelected { get; set; }
    }

    public class SettingsModel
    {
        public string StationId { get; set; }
        public string Color { get; set; }
        public string Region { get; set; }
        public string Project { get; set; }
        public string Location { get; set; }
        public string Pahse { get; set; }
        public string Speed { get; set; }
        public string EngineeringCode { get; set; }
        public string ShiftCode { get; set; }

        public void Load()
        {
            DbContext db = new DbContext();
            var list = db.SettingsDb.GetList(p => p.IsSelected == true);
            var PropertyInfos
             = GetType().GetProperties().Where(p =>
             p.CanWrite == true
             && p.PropertyType == typeof(string));
            if (list.Count > 0)
            {
                foreach (var one in PropertyInfos)
                {
                    one.SetValue(this, list.FirstOrDefault(p => p.Type == one.Name)?.Value, null);
                }
            }
        }
        public static void Set(string type, object val)
        {
            DbContext db = new DbContext();
            var model = db.Db.Queryable<Settings>().Where(p => p.Type == type && p.IsSelected == true).First();
            if (model == null)
            {
                model = new Settings { Type = type, IsSelected = true, Value = val.ToString() };
                db.Db.Insertable(model).ExecuteCommand();
            }
            else
            {
                model.Value = val.ToString();
                db.Db.Updateable(model).ExecuteCommand();
            }
            App.Settings.Load();
        }
    }
}
