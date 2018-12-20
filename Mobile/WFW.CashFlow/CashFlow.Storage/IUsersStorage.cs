using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CashFlow.Domain.Models;

namespace CashFlow.Storage
{
    public interface IUsersStorage
    {
        CfUser GetUser(Guid id);
        List<CfUser> ListUsers(bool onlyVisible = true);
    }
}
