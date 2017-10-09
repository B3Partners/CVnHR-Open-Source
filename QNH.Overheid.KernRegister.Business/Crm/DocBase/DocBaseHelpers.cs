using System;
using System.Data;

namespace QNH.Overheid.KernRegister.Business.Crm.DocBase
{
    public static class DocBaseHelpers
    {
        /// <summary>
        /// Sets a field to the specified value in the DataTable
        /// </summary>
        public static void SetValue(this DataTable table, string fieldName, string value)
        {
            foreach (DataRow rw in table.Rows)
            {
                if (rw["Name"].ToString() != fieldName) continue;

                if (rw["Type"].ToString() == "DATE")
                {
                    var dt = new DateTime(1900, 1, 1);
                    if (value != "")
                    {
                        dt = ParseStringToDate(value);
                        rw["Value"] = dt.ToString("yyyy-MM-dd", null);
                    }
                }
                else if (!string.IsNullOrEmpty(value))
                    rw["Value"] = value;

                break;
            }
        }

        /// <summary>
        /// Gets the value for the specified fieldName
        /// </summary>
        public static string GetValue(this DataTable table, string fieldName)
        {
            foreach (DataRow rw in table.Rows)
            {
                if (rw["Name"].ToString() == fieldName)
                    return (rw["Value"] ?? "").ToString();
            }
            return null;
        }

        public static DateTime ParseStringToDate(string DateString)
        {
            DateTime dtReturn = new DateTime();
            bool bSeperators = false;
            int iTemp, iMonth, iDay, iYear;

            //First check that it is at least 6 characters or more.
            if (!(DateString.Length > 5))
                throw new Exception("Date string not in correct format.");

            // Next, see if the framework can parse it.
            try
            {
                dtReturn = DateTime.Parse(DateString);
                return dtReturn;
            }
            catch
            { }

            //Check to see if it has any seperators.  If not, it should parse to a int.
            try
            {
                int.Parse(DateString);
                bSeperators = false;
            }
            catch
            {
                bSeperators = true;
            }

            if (!bSeperators)
            {
                if (DateString.Length == 6)
                {
                    iMonth = int.Parse(DateString.Substring(2, 2));
                    iDay = int.Parse(DateString.Substring(4, 2));
                    iYear = int.Parse(DateString.Substring(0, 2));
                    iTemp = DateTime.Now.Year;
                    iTemp = iTemp / 100;
                    iTemp = iTemp * 100;
                    iYear += iTemp;
                    try
                    {
                        return new DateTime(iYear, iMonth, iDay);
                    }
                    catch
                    {
                        throw new Exception("Date string not in correct format.");
                    }
                }
                if (DateString.Length == 8)
                {
                    iMonth = int.Parse(DateString.Substring(4, 2));
                    iDay = int.Parse(DateString.Substring(6, 2));
                    iYear = int.Parse(DateString.Substring(0, 4));
                    try
                    {
                        return new DateTime(iYear, iMonth, iDay);
                    }
                    catch
                    {
                        throw new Exception("Date string not in correct format.");
                    }
                }
                // Not a 6 or 8 digit number that was passed in.  Any other
                // combination would have ambiguity, therefor it is an error.
                throw new Exception("Date string not in correct format.");
            }
            // Looks like it is seperated by characters the framework doesn't support.
            // Next version will take this and parse it, but for now it is an error.
            throw new Exception("Date string not in correct format.");
        }
    }
}
