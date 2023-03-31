using DaisyForum.BackendServer.Controllers;
using DaisyForum.BackendServer.Data;
using DaisyForum.BackendServer.Data.Entities;
using DaisyForum.ViewModels;
using DaisyForum.ViewModels.Systems;
using Microsoft.AspNetCore.Mvc;

namespace DaisyForum.BackendServer.UnitTest.Controllers
{
    public class FunctionsControllerTest
    {
        private ApplicationDbContext _context;

        public FunctionsControllerTest()
        {
            _context = new InMemoryDbContextFactory().GetApplicationDbContext();
        }

        [Fact]
        public void Should_Create_Instance_Not_Null_Success()
        {
            var controller = new FunctionsController(_context);
            Assert.NotNull(controller);
        }

        [Fact]
        public async Task PostFunction_ValidInput_Success()
        {
            var functionsController = new FunctionsController(_context);
            var result = await functionsController.PostFunction(new FunctionCreateRequest()
            {
                Id = "PostFunction_ValidInput_Success",
                ParentId = null,
                Name = "PostFunction_ValidInput_Success",
                SortOrder = 5,
                Url = "/PostFunction_ValidInput_Success"
            });

            Assert.IsType<CreatedAtActionResult>(result);
        }

        // [Fact]
        // public async Task PostCommandToFunction_ValidInput_Success()
        // {
        //     _context.Database.EnsureDeleted();
        //     _context.Functions.AddRange(new List<Function>()
        //     {
        //         new Function(){
        //             Id = "GetCommandsInFunction_ReturnSuccess",
        //             ParentId = null,
        //             Name = "GetCommandsInFunction_ReturnSuccess",
        //             SortOrder = 3,
        //             Url ="/GetCommandsInFunction_ReturnSuccess"
        //         }
        //     });

        //     _context.Commands.AddRange(new List<Command>()
        //     {
        //         new Command(){
        //             Id = "CREATE",
        //             Name = "Thêm"
        //         }
        //     });

        //     await _context.SaveChangesAsync();
        //     var functionsController = new FunctionsController(_context);
        //     var result = await functionsController.PostCommandToFunction("GetCommandsInFunction_ReturnSuccess", new PostCommandToFunction()
        //     {
        //         CommandId = "CREATE",
        //         FunctionId = "GetCommandsInFunction_ReturnSuccess",
        //     });

        //     Assert.IsType<CreatedAtActionResult>(result);
        // }

        [Fact]
        public async Task PostFunction_ValidInput_Failed()
        {
            _context.Functions.AddRange(new List<Function>()
            {
                new Function(){
                    Id = "PostFunction_ValidInput_Failed",
                    ParentId = null,
                    Name = "PostFunction_ValidInput_Failed",
                    SortOrder =1,
                    Url ="/PostFunction_ValidInput_Failed"
                }
            });
            await _context.SaveChangesAsync();
            var functionsController = new FunctionsController(_context);

            var result = await functionsController.PostFunction(new FunctionCreateRequest()
            {
                Id = "PostFunction_ValidInput_Failed",
                ParentId = null,
                Name = "PostFunction_ValidInput_Failed",
                SortOrder = 5,
                Url = "/PostFunction_ValidInput_Failed"
            });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // [Fact]
        // public async Task PostCommandToFunction_ValidInput_Failed()
        // {
        //     _context.Database.EnsureDeleted();
        //     _context.Functions.AddRange(new List<Function>()
        //     {
        //         new Function(){
        //             Id = "GetCommandsInFunction_ReturnSuccess",
        //             ParentId = null,
        //             Name = "GetCommandsInFunction_ReturnSuccess",
        //             SortOrder = 3,
        //             Url ="/GetCommandsInFunction_ReturnSuccess"
        //         }
        //     });

        //     _context.Commands.AddRange(new List<Command>()
        //     {
        //         new Command(){
        //             Id = "CREATE",
        //             Name = "Thêm"
        //         }
        //     });

        //     _context.CommandInFunctions.AddRange(new List<CommandInFunction>()
        //     {
        //         new CommandInFunction(){
        //             CommandId = "CREATE",
        //             FunctionId = "GetCommandsInFunction_ReturnSuccess"
        //         }
        //     });

        //     await _context.SaveChangesAsync();
        //     var functionsController = new FunctionsController(_context);

        //     var result = await functionsController.PostCommandToFunction("GetCommandsInFunction_ReturnSuccess", new AddCommandToFunctionRequest()
        //     {
        //         CommandId = "CREATE",
        //         FunctionId = "GetCommandsInFunction_ReturnSuccess",
        //     });

        //     Assert.IsType<BadRequestObjectResult>(result);
        // }

        [Fact]
        public async Task GetFunctionsPaging_NoFilter_ReturnSuccess()
        {
            _context.Database.EnsureDeleted();
            _context.Functions.AddRange(new List<Function>()
            {
                new Function(){
                    Id = "GetFunctionsPaging_NoFilter_ReturnSuccess1",
                    ParentId = null,
                    Name = "GetFunctionsPaging_NoFilter_ReturnSuccess1",
                    SortOrder =1,
                    Url ="/test1"
                },
                 new Function(){
                    Id = "GetFunctionsPaging_NoFilter_ReturnSuccess2",
                    ParentId = null,
                    Name = "GetFunctionsPaging_NoFilter_ReturnSuccess2",
                    SortOrder =2,
                    Url ="/test2"
                },
                  new Function(){
                    Id = "GetFunctionsPaging_NoFilter_ReturnSuccess3",
                    ParentId = null,
                    Name = "GetFunctionsPaging_NoFilter_ReturnSuccess3",
                    SortOrder = 3,
                    Url ="/test3"
                },
                   new Function(){
                    Id = "GetFunctionsPaging_NoFilter_ReturnSuccess4",
                    ParentId = null,
                    Name = "GetFunctionsPaging_NoFilter_ReturnSuccess4",
                    SortOrder =4,
                    Url ="/test4"
                }
            });

            await _context.SaveChangesAsync();
            var functionsController = new FunctionsController(_context);
            var result = await functionsController.GetFunctionsPaging(null, 1, 2);
            var okResult = result as OkObjectResult;
            var functionViewModels = okResult != null ? okResult.Value as Pagination<FunctionViewModel> : null;
            Assert.Equal(4, functionViewModels != null ? functionViewModels.TotalRecords : 0);
            Assert.Equal(2, functionViewModels != null ? (functionViewModels.Items != null ? functionViewModels.Items.Count() : 0) : 0);
        }

        [Fact]
        public async Task GetFunction_HasData_ReturnSuccess()
        {
            _context.Functions.AddRange(new List<Function>()
            {
                new Function(){
                    Id = "GetFunction_HasData_ReturnSuccess",
                    ParentId = null,
                    Name = "GetFunction_HasData_ReturnSuccess",
                    SortOrder =1,
                    Url ="/GetFunction_HasData_ReturnSuccess"
                }
            });
            await _context.SaveChangesAsync();
            var functionsController = new FunctionsController(_context);
            var result = await functionsController.GetFunctions();
            var okResult = result as OkObjectResult;
            var functionViewModels = okResult != null ? okResult.Value as IEnumerable<FunctionViewModel> : null;
            Assert.True(functionViewModels != null ? functionViewModels.Count() > 0 : false);
        }

        [Fact]
        public async Task GetFunctionsPaging_HasFilter_ReturnSuccess()
        {
            _context.Functions.AddRange(new List<Function>()
            {
                new Function(){
                    Id = "GetFunctionsPaging_HasFilter_ReturnSuccess",
                    ParentId = null,
                    Name = "GetFunctionsPaging_HasFilter_ReturnSuccess",
                    SortOrder = 3,
                    Url ="/GetFunctionsPaging_HasFilter_ReturnSuccess"
                }
            });
            await _context.SaveChangesAsync();

            var functionsController = new FunctionsController(_context);
            var result = await functionsController.GetFunctionsPaging("GetFunctionsPaging_HasFilter_ReturnSuccess", 1, 2);
            var okResult = result as OkObjectResult;
            var functionViewModels = okResult != null ? okResult.Value as Pagination<FunctionViewModel> : null;
            Assert.Equal(1, functionViewModels != null ? functionViewModels.TotalRecords : 0);
            if (functionViewModels != null)
                if (functionViewModels.Items != null)
                    Assert.Single(functionViewModels.Items);
        }

        [Fact]
        public async Task GetCommandsInFunction_ReturnSuccess()
        {
            _context.Database.EnsureDeleted();
            _context.Functions.AddRange(new List<Function>()
            {
                new Function(){
                    Id = "GetCommandsInFunction_ReturnSuccess",
                    ParentId = null,
                    Name = "GetCommandsInFunction_ReturnSuccess",
                    SortOrder = 3,
                    Url ="/GetCommandsInFunction_ReturnSuccess"
                }
            });

            _context.Commands.AddRange(new List<Command>()
            {
                new Command(){
                    Id = "CREATE",
                    Name = "Thêm"
                },
                new Command(){
                    Id = "UPDATE",
                    Name = "Cập nhật"
                },
                new Command(){
                    Id = "DELETE",
                    Name = "Xoá"
                }
            });

            _context.CommandInFunctions.AddRange(new List<CommandInFunction>()
            {
                new CommandInFunction(){
                    CommandId = "CREATE",
                    FunctionId = "GetCommandsInFunction_ReturnSuccess"
                },
                new CommandInFunction(){
                    CommandId = "UPDATE",
                    FunctionId = "GetCommandsInFunction_ReturnSuccess"
                },
                new CommandInFunction(){
                    CommandId = "DELETE",
                    FunctionId = "GetCommandsInFunction_ReturnSuccess"
                }
            });
            await _context.SaveChangesAsync();

            var functionsController = new FunctionsController(_context);
            var result = await functionsController.GetCommandsInFunction("GetCommandsInFunction_ReturnSuccess");

            var okResult = result as OkObjectResult;
            var commandViewModels = okResult != null ? okResult.Value as IEnumerable<CommandViewModel> : null;
            Assert.Equal(3, commandViewModels != null ? commandViewModels.Count() : 0);
        }

        // [Fact]
        // public async Task GetCommandsNotInFunction_ReturnSuccess()
        // {
        //     _context.Functions.AddRange(new List<Function>()
        //     {
        //         new Function(){
        //             Id = "GetCommandsNotInFunction_ReturnSuccess",
        //             ParentId = null,
        //             Name = "GetCommandsNotInFunction_ReturnSuccess",
        //             SortOrder = 3,
        //             Url ="/GetCommandsNotInFunction_ReturnSuccess"
        //         }
        //     });

        //     _context.Commands.AddRange(new List<Command>()
        //     {
        //         new Command(){
        //             Id = "CREATE",
        //             Name = "Thêm"
        //         },
        //         new Command(){
        //             Id = "UPDATE",
        //             Name = "Cập nhật"
        //         },
        //         new Command(){
        //             Id = "DELETE",
        //             Name = "Xoá"
        //         }
        //     });

        //     _context.CommandInFunctions.AddRange(new List<CommandInFunction>()
        //     {
        //         new CommandInFunction(){
        //             CommandId = "CREATE",
        //             FunctionId = "GetCommandsNotInFunction_ReturnSuccess"
        //         }
        //     });
        //     await _context.SaveChangesAsync();

        //     var functionsController = new FunctionsController(_context);
        //     var result = await functionsController.GetCommandsNotInFunction("GetCommandsNotInFunction_ReturnSuccess");

        //     var okResult = result as OkObjectResult;
        //     var commandViewModels = okResult != null ? okResult.Value as IEnumerable<CommandViewModel> : null;
        //     Assert.Equal(2, commandViewModels != null ? commandViewModels.Count() : 0);
        // }

        [Fact]
        public async Task GetById_HasData_ReturnSuccess()
        {
            _context.Functions.AddRange(new List<Function>()
            {
                new Function(){
                    Id = "GetById_HasData_ReturnSuccess",
                    ParentId = null,
                    Name = "GetById_HasData_ReturnSuccess",
                    SortOrder =1,
                    Url ="/GetById_HasData_ReturnSuccess"
                }
            });
            await _context.SaveChangesAsync();
            var functionsController = new FunctionsController(_context);
            var result = await functionsController.GetById("GetById_HasData_ReturnSuccess");
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var functionViewModel = okResult.Value as FunctionViewModel;
            if (functionViewModel != null)
                Assert.Equal("GetById_HasData_ReturnSuccess", functionViewModel.Id);
        }

        [Fact]
        public async Task PutFunction_ValidInput_Success()
        {
            _context.Functions.AddRange(new List<Function>()
            {
                new Function(){
                    Id = "PutFunction_ValidInput_Success",
                    ParentId = null,
                    Name = "PutFunction_ValidInput_Success",
                    SortOrder =1,
                    Url ="/PutFunction_ValidInput_Success"
                }
            });
            await _context.SaveChangesAsync();
            var functionsController = new FunctionsController(_context);
            var result = await functionsController.PutFunction("PutFunction_ValidInput_Success", new FunctionCreateRequest()
            {
                ParentId = null,
                Name = "PutFunction_ValidInput_Success updated",
                SortOrder = 6,
                Url = "/PutFunction_ValidInput_Success"
            });
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutFunction_ValidInput_Failed()
        {
            var functionsController = new FunctionsController(_context);

            var result = await functionsController.PutFunction("PutFunction_ValidInput_Failed", new FunctionCreateRequest()
            {
                ParentId = null,
                Name = "PutFunction_ValidInput_Failed update",
                SortOrder = 6,
                Url = "/PutFunction_ValidInput_Failed"
            });
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteFunction_ValidInput_Success()
        {
            _context.Functions.AddRange(new List<Function>()
            {
                new Function(){
                    Id = "DeleteFunction_ValidInput_Success",
                    ParentId = null,
                    Name = "DeleteFunction_ValidInput_Success",
                    SortOrder =1,
                    Url ="/DeleteFunction_ValidInput_Success"
                }
            });
            await _context.SaveChangesAsync();
            var functionsController = new FunctionsController(_context);
            var result = await functionsController.DeleteFunction("DeleteFunction_ValidInput_Success");
            Assert.IsType<OkObjectResult>(result);
        }

        // [Fact]
        // public async Task DeleteCommandInFunction_ValidInput_Success()
        // {
        //     _context.Database.EnsureDeleted();
        //     _context.Functions.AddRange(new List<Function>()
        //     {
        //         new Function(){
        //             Id = "DeleteCommandToFunction_ValidInput_Success",
        //             ParentId = null,
        //             Name = "DeleteCommandToFunction_ValidInput_Success",
        //             SortOrder = 3,
        //             Url ="/DeleteCommandToFunction_ValidInput_Success"
        //         }
        //     });

        //     _context.Commands.AddRange(new List<Command>()
        //     {
        //         new Command(){
        //             Id = "CREATE",
        //             Name = "Thêm"
        //         }
        //     });

        //     _context.CommandInFunctions.AddRange(new List<CommandInFunction>()
        //     {
        //         new CommandInFunction(){
        //             CommandId = "CREATE",
        //             FunctionId = "DeleteCommandToFunction_ValidInput_Success"
        //         }
        //     });

        //     await _context.SaveChangesAsync();
        //     var functionsController = new FunctionsController(_context);
        //     var result = await functionsController.DeleteCommandInFunction("DeleteCommandToFunction_ValidInput_Success", "CREATE");
        //     Assert.IsType<OkObjectResult>(result);
        // }

        [Fact]
        public async Task DeleteFunction_ValidInput_Failed()
        {
            var functionsController = new FunctionsController(_context);
            var result = await functionsController.DeleteFunction("DeleteFunction_ValidInput_Failed");
            Assert.IsType<NotFoundObjectResult>(result);
        }

        // [Fact]
        // public async Task DeleteCommandInFunction_ValidInput_Failed()
        // {
        //     _context.Database.EnsureDeleted();
        //     _context.Functions.AddRange(new List<Function>()
        //     {
        //         new Function(){
        //             Id = "DeleteCommandToFunction_ValidInput_Success",
        //             ParentId = null,
        //             Name = "DeleteCommandToFunction_ValidInput_Success",
        //             SortOrder = 3,
        //             Url ="/DeleteCommandToFunction_ValidInput_Success"
        //         }
        //     });

        //     _context.Commands.AddRange(new List<Command>()
        //     {
        //         new Command(){
        //             Id = "CREATE",
        //             Name = "Thêm"
        //         }
        //     });

        //     await _context.SaveChangesAsync();
        //     var functionsController = new FunctionsController(_context);
        //     var result = await functionsController.DeleteCommandInFunction("DeleteCommandToFunction_ValidInput_Success", "CREATE");
        //     Assert.IsType<NotFoundObjectResult>(result);
        // }
    }
}