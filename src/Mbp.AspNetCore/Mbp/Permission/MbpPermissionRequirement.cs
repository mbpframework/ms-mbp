using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.AspNetCore.Permission
{
    public class MbpPermissionRequirement : IAuthorizationRequirement
    {
        public MbpPermissionRequirement(params string[] codes)
        {
            ActionCodes = codes;
        }

        public string[] ActionCodes { get; private set; }
    }
}
