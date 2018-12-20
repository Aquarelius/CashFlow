using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CashFlow.Domain.Enums;

namespace CashFlow.Storage
{
    public interface ISettingsStorage
    {
        string ReadSetting(SettingsNames name);
        void WriteSettings(SettingsNames name, string value);
    }
}
