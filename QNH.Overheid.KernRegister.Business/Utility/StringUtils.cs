using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QNH.Overheid.KernRegister.Business
{
    public static class StringUtils
    {
        public static string ToMaxChars(this string input, int maxLength)
        {
            return 
                input == null 
                ? null
                : input.Length <= maxLength ? input : input.Substring(0, maxLength);
        }
    }
}
