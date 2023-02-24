using DaisyForum.BackendServer.Controllers;
using DaisyForum.BackendServer.Data.Entities;
using DaisyForum.ViewModels;
using DaisyForum.ViewModels.Systems;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MockQueryable.Moq;
using Moq;

namespace DaisyForum.BackendServer.UnitTest.Controllers
{
    public class UsersControllerTest
    {
        private readonly Mock<UserManager<User>> _mockUserManager;

        private List<User> _userSources = new List<User>(){
                             new User("1","test1","Test 1","LastTest 1","test1@gmail.com","001111",DateTime.Now),
                             new User("2","test2","Test 2","LastTest 2","test2@gmail.com","001111",DateTime.Now),
                             new User("3","test3","Test 3","LastTest 3","test3@gmail.com","001111",DateTime.Now),
                             new User("4","test4","Test 4","LastTest 4","test4@gmail.com","001111",DateTime.Now),
                        };

        public UsersControllerTest()
        {
            var userStore = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(userStore.Object,
                null, null, null, null, null, null, null, null);
        }

        [Fact]
        public void ShouldCreateInstance_NotNull_Success()
        {
            var usersController = new UsersController(_mockUserManager.Object);
            Assert.NotNull(usersController);
        }

        [Fact]
        public async Task PostUser_ValidInput_Success()
        {
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new User()
                {
                    UserName = "test",
                });

            var usersController = new UsersController(_mockUserManager.Object);
            var result = await usersController.PostUser(new UserCreateRequest()
            {
                UserName = "test",
                Password = "Aa@12345"
            });

            Assert.NotNull(result);
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task PostUser_ValidInput_Failed()
        {
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError[] { }));

            var usersController = new UsersController(_mockUserManager.Object);
            var result = await usersController.PostUser(new UserCreateRequest()
            {
                UserName = "test",
                Password = "Aa@12345"
            });

            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetUsers_HasData_ReturnSuccess()
        {
            _mockUserManager.Setup(x => x.Users)
                .Returns(_userSources.AsQueryable().BuildMock());
            var usersController = new UsersController(_mockUserManager.Object);
            var result = await usersController.GetUsers();
            var okResult = result as OkObjectResult;
            var userViewModels = okResult != null ? okResult.Value as IEnumerable<UserViewModel> : null;
            Assert.True(userViewModels != null ? userViewModels.Count() > 0 : false);
        }

        [Fact]
        public async Task GetUsers_ThrowException_Failed()
        {
            _mockUserManager.Setup(x => x.Users).Throws<Exception>();

            var UsersController = new UsersController(_mockUserManager.Object);

            await Assert.ThrowsAnyAsync<Exception>(async () => await UsersController.GetUsers());
        }

        [Fact]
        public async Task GetUsersPaging_NoFilter_ReturnSuccess()
        {
            _mockUserManager.Setup(x => x.Users)
                .Returns(_userSources.AsQueryable().BuildMock());

            var usersController = new UsersController(_mockUserManager.Object);
            var result = await usersController.GetUsersPaging(null, 1, 2);
            var okResult = result as OkObjectResult;
            var userViewModels = okResult != null ? okResult.Value as Pagination<UserViewModel> : null;
            Assert.Equal(4, userViewModels != null ? userViewModels.TotalRecords : 0);
            Assert.Equal(2, (userViewModels != null ? (userViewModels.Items != null ? userViewModels.Items.Count() : 0) : 0));
        }

        [Fact]
        public async Task GetUsersPaging_HasFilter_ReturnSuccess()
        {
            _mockUserManager.Setup(x => x.Users)
                .Returns(_userSources.AsQueryable().BuildMock());

            var usersController = new UsersController(_mockUserManager.Object);
            var result = await usersController.GetUsersPaging("test3", 1, 2);
            var okResult = result as OkObjectResult;
            var userViewModels = okResult != null ? okResult.Value as Pagination<UserViewModel> : null;
            Assert.Equal(1, userViewModels != null ? userViewModels.TotalRecords : 0);
            if (userViewModels != null)
                if (userViewModels.Items != null)
                    Assert.Single(userViewModels.Items);
        }

        [Fact]
        public async Task GetUsersPaging_ThrowException_Failed()
        {
            _mockUserManager.Setup(x => x.Users).Throws<Exception>();

            var usersController = new UsersController(_mockUserManager.Object);

            await Assert.ThrowsAnyAsync<Exception>(async () => await usersController.GetUsersPaging(null, 1, 1));
        }

        [Fact]
        public async Task GetById_HasData_ReturnSuccess()
        {
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new User()
                {
                    UserName = "test1"
                });
            var usersController = new UsersController(_mockUserManager.Object);
            var result = await usersController.GetUserById("test1");
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var userVieUserViewModel = okResult.Value as UserViewModel;

            Assert.Equal("test1", userVieUserViewModel != null ? userVieUserViewModel.UserName : null);
        }

        [Fact]
        public async Task GetById_ThrowException_Failed()
        {
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).Throws<Exception>();

            var usersController = new UsersController(_mockUserManager.Object);

            await Assert.ThrowsAnyAsync<Exception>(async () => await usersController.GetUserById("test1"));
        }

        [Fact]
        public async Task PutUser_ValidInput_Success()
        {
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
               .ReturnsAsync(new User()
               {
                   UserName = "test1"
               });

            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);
            var usersController = new UsersController(_mockUserManager.Object);
            var result = await usersController.PutUser("test", new UserCreateRequest()
            {
                FirstName = "test2"
            });

            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutUser_ValidInput_Failed()
        {
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
             .ReturnsAsync(new User()
             {
                 UserName = "test1"
             });

            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError[] { }));

            var usersController = new UsersController(_mockUserManager.Object);
            var result = await usersController.PutUser("test", new UserCreateRequest()
            {
                UserName = "test1"
            });

            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteUser_ValidInput_Success()
        {
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
               .ReturnsAsync(new User()
               {
                   UserName = "test1"
               });

            _mockUserManager.Setup(x => x.DeleteAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);
            var usersController = new UsersController(_mockUserManager.Object);
            var result = await usersController.DeleteUser("test");
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteUser_ValidInput_Failed()
        {
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
             .ReturnsAsync(new User()
             {
                 UserName = "test1"
             });

            _mockUserManager.Setup(x => x.DeleteAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError[] { }));

            var usersController = new UsersController(_mockUserManager.Object);
            var result = await usersController.DeleteUser("test");
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}