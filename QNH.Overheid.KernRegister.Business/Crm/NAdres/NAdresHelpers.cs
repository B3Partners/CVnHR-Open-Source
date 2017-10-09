using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace QNH.Overheid.KernRegister.Business.Crm.NAdres
{
    public static class NAdresHelpers
    {
        public static string GetValue(this DataTable table, string fieldName)
        {
            return table.Rows[0][fieldName].ToString();
        }
    }
}
