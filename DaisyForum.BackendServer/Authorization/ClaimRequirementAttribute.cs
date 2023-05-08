using DaisyForum.BackendServer.Constants;
using Microsoft.AspNetCore.Mvc;

namespace DaisyForum.BackendServer.Authorization;

public class ClaimRequirementAttribute : TypeFilterAttribute
{
    public ClaimRequirementAttribute(FunctionCode functionId, CommandCode commandId)
        : base(typeof(ClaimRequirementFilter))
    {
        Arguments = new object[] { functionId, commandId };
    }
}