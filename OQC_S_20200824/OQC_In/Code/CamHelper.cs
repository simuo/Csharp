using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using xxw.Logs;

namespace OQC_IN
{
    public class CamHelper
    {
        /// <summary>
        /// 解析Cam data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="action"></param>
        public static void ParsingCamData(string data, Action<int, int, string, List<string>> action)
        {
            try
            {
                //处理caijiwancheng 数据
                Regex reg = new Regex("caijiwancheng\\s\\d?");
                data = reg.Replace(data, "");
                string[] d = data.Replace("CAM ", ";CAM ").Split(';');
                foreach (var one in d.Where(p => !string.IsNullOrEmpty(p)))
                {
                    var oneData = one.TrimEnd();
                    if (oneData.IndexOf("CAM") != 0) continue;
                    List<string> Data = oneData.Split(' ').ToList();
                    if (Data.Count < 2) continue;
                    bool isInt = int.TryParse(Data[1], out int CamNo);
                    if (!isInt) continue;
                    int StageId = int.Parse(Data[2]);
                    action?.Invoke(CamNo, StageId, oneData, Data);
                }
            }
            catch (Exception e)
            {
                LogInfo.Log.Error($"解码数据失败：{e.Message} Data:{data}");
            }
        }
    }
}
