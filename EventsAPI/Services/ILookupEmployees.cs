using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventsAPI.Services
{
    public interface ILookupEmployees
    {
        Task<bool> CheckEmployeeIsActive(int id);
    }
}
