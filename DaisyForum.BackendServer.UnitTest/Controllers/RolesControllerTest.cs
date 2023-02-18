using DaisyForum.BackendServer.Controllers;
using DaisyForum.ViewModels;
using DaisyForum.ViewModels.Systems;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MockQueryable.Moq;
using Moq;

namespace DaisyForum.BackendServer.UnitTest.Controllers
{
    public class RolesControllerTest
    {
        private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;

        private List<IdentityRole> _roleSources = new List<IdentityRole>(){
                             new IdentityRole("test1"),
                             new IdentityRole("test2"),
                             new IdentityRole("test3"),
                             new IdentityRole("test4")
                        };

        public RolesControllerTest()
        {
            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);
        }

        [Fact]
        public void ShouldCreateInstance_NotNull_Success()
        {
            var rolesController = new RolesController(_mockRoleManager.Object);
            Assert.NotNull(rolesController);
        }

        [Fact]
        public async Task PostRole_ValidInput_Success()
        {
            _mockRoleManager.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Success);

            var rolesController = new RolesController(_mockRoleManager.Object);
            var result = await rolesController.CreateRole(new RoleViewModel()
            {
                RoleId = "test",
                RoleName = "test"
            });

            Assert.NotNull(result);
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task PostRole_ValidInput_Failed()
        {
            _mockRoleManager.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError[] { }));

            var rolesController = new RolesController(_mockRoleManager.Object);
            var result = await rolesController.CreateRole(new RoleViewModel()
            {
                RoleId = "test",
                RoleName = "test"
            });

            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetRoles_HasData_ReturnSuccess()
        {
            _mockRoleManager.Setup(x => x.Roles)
                .Returns(_roleSources.AsQueryable()
                                     .BuildMock());
            var rolesController = new RolesController(_mockRoleManager.Object);
            var result = await rolesController.GetRoles();
            var okResult = result as OkObjectResult;
            var roleVms = okResult != null ? okResult.Value as IEnumerable<RoleViewModel> : null;
            Assert.True((roleVms != null ? roleVms.Count() : 0) > 0);
        }

        [Fact]
        public async Task GetRoles_ThrowException_Failed()
        {
            _mockRoleManager.Setup(x => x.Roles).Throws<Exception>();

            var rolesController = new RolesController(_mockRoleManager.Object);

            await Assert.ThrowsAnyAsync<Exception>(async () => await rolesController.GetRoles());
        }

        [Fact]
        public async Task GetRolesPaging_NoFilter_ReturnSuccess()
        {
            _mockRoleManager.Setup(x => x.Roles)
                .Returns(_roleSources.AsQueryable()
                                     .BuildMock());

            var rolesController = new RolesController(_mockRoleManager.Object);
            var result = await rolesController.GetRoles(null, 1, 2);
            var okResult = result as OkObjectResult;
            var roleVms = okResult != null ? okResult.Value as Pagination<RoleViewModel> : null;
            Assert.Equal(4, (roleVms != null ? roleVms.TotalRecords : 0));
            Assert.Equal(2, (roleVms != null ? (roleVms.Items != null ? roleVms.Items.Count() : 0) : 0));
        }

        [Fact]
        public async Task GetRolesPaging_HasFilter_ReturnSuccess()
        {
            _mockRoleManager.Setup(x => x.Roles)
                .Returns(_roleSources.AsQueryable()
                                     .BuildMock());

            var rolesController = new RolesController(_mockRoleManager.Object);
            var result = await rolesController.GetRoles("test3", 1, 2);
            var okResult = result as OkObjectResult;
            var roleVms = okResult != null ? okResult.Value as Pagination<RoleViewModel> : null;
            Assert.Equal(1, (roleVms != null ? roleVms.TotalRecords : 0));
            if (roleVms != null)
                if (roleVms.Items != null)
                    Assert.Single(roleVms.Items);
        }

        [Fact]
        public async Task GetRolesPaging_ThrowException_Failed()
        {
            _mockRoleManager.Setup(x => x.Roles).Throws<Exception>();

            var rolesController = new RolesController(_mockRoleManager.Object);

            await Assert.ThrowsAnyAsync<Exception>(async () => await rolesController.GetRoles(null, 1, 1));
        }

        [Fact]
        public async Task GetById_HasData_ReturnSuccess()
        {
            _mockRoleManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new IdentityRole()
                {
                    Id = "test1",
                    Name = "test1"
                });
            var rolesController = new RolesController(_mockRoleManager.Object);
            var result = await rolesController.GetRole("test1");
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var roleVm = okResult.Value as RoleViewModel;

            Assert.Equal("test1", roleVm != null ? roleVm.RoleName : null);
        }

        [Fact]
        public async Task GetById_ThrowException_Failed()
        {
            _mockRoleManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).Throws<Exception>();

            var rolesController = new RolesController(_mockRoleManager.Object);

            await Assert.ThrowsAnyAsync<Exception>(async () => await rolesController.GetRole("test1"));
        }

        [Fact]
        public async Task PutRole_ValidInput_Success()
        {
            _mockRoleManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
               .ReturnsAsync(new IdentityRole()
               {
                   Id = "test",
                   Name = "test"
               });

            _mockRoleManager.Setup(x => x.UpdateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Success);
            var rolesController = new RolesController(_mockRoleManager.Object);
            var result = await rolesController.UpdateRole("test", new RoleViewModel()
            {
                RoleId = "test",
                RoleName = "test"
            });

            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutRole_ValidInput_Failed()
        {
            _mockRoleManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
             .ReturnsAsync(new IdentityRole()
             {
                 Id = "test",
                 Name = "test"
             });

            _mockRoleManager.Setup(x => x.UpdateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError[] { }));

            var rolesController = new RolesController(_mockRoleManager.Object);
            var result = await rolesController.UpdateRole("test", new RoleViewModel()
            {
                RoleId = "test",
                RoleName = "test"
            });

            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteRole_ValidInput_Success()
        {
            _mockRoleManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
               .ReturnsAsync(new IdentityRole()
               {
                   Id = "test",
                   Name = "test"
               });

            _mockRoleManager.Setup(x => x.DeleteAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Success);
            var rolesController = new RolesController(_mockRoleManager.Object);
            var result = await rolesController.DeleteRole("test");
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteRole_ValidInput_Failed()
        {
            _mockRoleManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
             .ReturnsAsync(new IdentityRole()
             {
                 Id = "test",
                 Name = "test"
             });

            _mockRoleManager.Setup(x => x.DeleteAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError[] { }));

            var rolesController = new RolesController(_mockRoleManager.Object);
            var result = await rolesController.DeleteRole("test");
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}