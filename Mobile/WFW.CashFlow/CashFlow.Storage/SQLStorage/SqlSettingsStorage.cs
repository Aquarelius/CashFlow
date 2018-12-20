using System;
using System.Linq;
using CashFlow.Domain.Enums;
using CashFlow.Domain.Models;

namespace CashFlow.Storage.SQLStorage
{
   public class SqlSettingsStorage:ISettingsStorage
   {
       private readonly StorageContext _context;

       public SqlSettingsStorage(StorageContext context)
       {
           _context = context;
       }

       public string ReadSetting(SettingsNames name)
       {
           var val = _context.SettingItems.SingleOrDefault(z => z.Name == name.ToString());
           if (val == null) return string.Empty;
           return val.Value;
       }

        public void WriteSettings(SettingsNames name, string value)
        {
            var val = _context.SettingItems.SingleOrDefault(z => z.Name == name.ToString());
            if (val == null)
            {
               val = new SettingItem
               {
                   Name = name.ToString(),
                   Value = value,
                   LastChanged = DateTime.UtcNow
               };
                _context.SettingItems.Add(val);
            }
            else
            {
                val.Value = value;
                val.LastChanged = DateTime.UtcNow;
            }
            _context.SaveChanges();
        }
    }
}
