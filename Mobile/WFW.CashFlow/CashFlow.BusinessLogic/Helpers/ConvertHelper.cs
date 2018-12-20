using System;
using CashFlow.Domain.Enums;
using CashFlow.Storage;

namespace CashFlow.BusinessLogic.Helpers
{
    public class ConvertHelper
    {
        private readonly ISettingsStorage _sttings;

        public ConvertHelper(ISettingsStorage sttings)
        {
            _sttings = sttings;
        }

        public DateTime GetActualDateTime(DateTime utc)
        {
            var tif = GetCurrenTimeZone();
           
            var res =TimeZoneInfo.ConvertTimeFromUtc(utc, tif);
            return res;
        }

        public DateTime GetUtcDateTime(DateTime local)
        {
            var tif = GetCurrenTimeZone();
           
            var res = TimeZoneInfo.ConvertTimeToUtc(local, tif);
            return res;
        }

        public TimeZoneInfo GetCurrenTimeZone()
        {
            TimeZoneInfo tif = null;
            try
            {
                tif = TimeZoneInfo.FromSerializedString(_sttings.ReadSetting(SettingsNames.TimeZone));
            }
            catch
            {
                // ignored
            }
            if (tif == null)
            {
                tif = TimeZoneInfo.Local;
                _sttings.WriteSettings(SettingsNames.TimeZone, tif.ToSerializedString());
            }
            return tif;
        }
    }
}
