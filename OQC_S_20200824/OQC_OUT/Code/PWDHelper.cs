using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xxw.Licence;
using xxw.utilities.Security;

namespace OQC_OUT
{
    public static class PWDHelper
    {
        public static string ToPwd(this string pwd) => pwd.ToPassword(LicenceHelper.SoftName.ToMd5());
    }
}
