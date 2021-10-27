﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Globalization;

namespace Preceda.HealthCheck.STP.Export
{
    static class ISeriesDbTypeConversion
    {

        public static DateTime GetISeriesDate(this OleDbDataReader reader, int ordinal)
        {
            var dateValue = reader.GetString(ordinal);

            if (DateTime.TryParseExact(dateValue, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime date))
                return date;
            else
                return new DateTime(0001, 01, 01);
        }

        public static TimeSpan GetISeriesTime(this OleDbDataReader reader, int ordinal)
        {
            var timeSpanValue = reader.GetString(ordinal);

            if (TimeSpan.TryParseExact(timeSpanValue, "hh.mm.ss", CultureInfo.CurrentCulture, out TimeSpan timeSpan))
                return timeSpan;
            else
                return new TimeSpan(00, 00, 00);
        }

        public static DateTime GetISeriesTimeStamp(this OleDbDataReader reader, int ordinal)
        {
            var timeStampValue = reader.GetString(ordinal);

            if (DateTime.TryParseExact(timeStampValue, "yyyy-MM-dd-hh.mm.ss.000000", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime date))
                return date;
            else
                return new DateTime(0001, 01, 01);
        }

    }
}
