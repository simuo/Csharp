namespace OQC_OUT
{
    public class Versions
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string Ver { get; set; }
    }
}
