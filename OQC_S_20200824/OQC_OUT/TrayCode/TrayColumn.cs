using System;
using System.Collections.ObjectModel;
using System.Linq;
using xxw.utilities;

namespace OQC_OUT
{
    public class TrayColumn : BaseModel
    {
        public ObservableCollection<Product> Products { get; private set; }
        /// <summary>
        /// 列读码完成
        /// </summary>
        public bool Complate => Products.All(p => p.Complate);
        /// <summary>
        /// 创建列
        /// </summary>
        /// <param name="rowCount"></param>
        public void Create(int rowCount)
        {
            int readCount = App.Config.Trigger.CommandSendTimes;
            int camNumber = App.Config.Group.Count;
            Products = new ObservableCollection<Product>();
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                for (int r = 0; r < rowCount; r++)
                    Products.Add(new Product(readCount, camNumber));
                OnPropertyChanged("Products");
            }));

        }
    }
}
