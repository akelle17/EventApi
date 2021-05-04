using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventsAPI.Services
{
    public class LinkMaker
    {
        private readonly IHttpContextAccessor _accessor;

        public LinkMaker(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }
    }
}
