using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DaisyForum.BackendServer.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize("Bearer")]
public class BaseController : Controller
{

}