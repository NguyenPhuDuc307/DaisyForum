using System.Data;
using DaisyForum.BackendServer.Authorization;
using DaisyForum.BackendServer.Constants;
using DaisyForum.ViewModels.Systems;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DaisyForum.BackendServer.Controllers;

public class PermissionsController : BaseController
{
    private readonly IConfiguration _configuration;

    public PermissionsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    [ClaimRequirement(FunctionCode.SYSTEM_PERMISSION, CommandCode.VIEW)]
    public async Task<IActionResult> GetCommandViews()
    {
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            if (conn.State == ConnectionState.Closed)
            {
                await conn.OpenAsync();
            }

            var sql = @"SELECT f.Id,
	                       f.Name,
	                       f.ParentId,
	                       sum(case when sa.Id = 'CREATE' then 1 else 0 end) as HasCreate,
	                       sum(case when sa.Id = 'UPDATE' then 1 else 0 end) as HasUpdate,
	                       sum(case when sa.Id = 'DELETE' then 1 else 0 end) as HasDelete,
	                       sum(case when sa.Id = 'VIEW' then 1 else 0 end) as HasView,
	                       sum(case when sa.Id = 'APPROVE' then 1 else 0 end) as HasApprove
                        from Functions f join CommandInFunctions cif on f.Id = cif.FunctionId
		                    left join Commands sa on cif.CommandId = sa.Id
                        GROUP BY f.Id,f.Name, f.ParentId
                        order BY f.ParentId";

            var result = await conn.QueryAsync<PermissionScreenViewModel>(sql, null, null, 120, CommandType.Text);
            return Ok(result.ToList());
        }
    }
}