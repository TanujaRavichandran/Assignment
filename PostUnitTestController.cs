using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShopBridgeTestProject.Tests
{
    public class PostUnitTestController
    {
        private PostRepository repository;
        public static DbContextOptions<ShopBridgeDBContext> dbContextOptions { get; }
        public static string connectionString = "Server=HIBACL154628;Database=ShopBridgeDB;UID=sa;PWD=login1-2;";

        static PostUnitTestController()
        {
            dbContextOptions = new DbContextOptionsBuilder<ShopBridgeDBContext>()
                .UseSqlServer(connectionString)
                .Options;
        }

        public PostUnitTestController()
        {
            var context = new ShopBridgeDBContext(dbContextOptions);
            DummyDataDBInitializer db = new DummyDataDBInitializer();
            db.Seed(context);

            repository = new PostRepository(context);

        }

        #region Get By Id  

        [TestMethod]
        public async void Task_GetPostById_Return_OkResult()
        {
            //Arrange  
            var controller = new PostController(repository);
            var postId = 2;

            //Act  
            var data = await controller.GetPost(postId);

            //Assert  
            Assert.IsType<OkObjectResult>(data);
        }

        [TestMethod]
        public async void Task_GetPostById_Return_NotFoundResult()
        {
            //Arrange  
            var controller = new PostController(repository);
            var postId = 3;

            //Act  
            var data = await controller.GetPost(postId);

            //Assert  
            Assert.IsType<NotFoundResult>(data);
        }

        [TestMethod]
        public async void Task_GetPostById_Return_BadRequestResult()
        {
            //Arrange  
            var controller = new PostController(repository);
            int? postId = null;

            //Act  
            var data = await controller.GetPost(postId);

            //Assert  
            Assert.IsType<BadRequestResult>(data);
        }

        [TestMethod]
        public async void Task_GetPostById_MatchResult()
        {
            //Arrange  
            var controller = new PostController(repository);
            int? postId = 1;

            //Act  
            var data = await controller.GetPost(postId);

            //Assert  
            Assert.IsType<OkObjectResult>(data);

            var okResult = data.Should().BeOfType<OkObjectResult>().Subject;
            var post = okResult.Value.Should().BeAssignableTo<PostViewModel>().Subject;

            Assert.Equal("Test Title 1", post.Title);
            Assert.Equal("Test Description 1", post.Description);
        }

        #endregion

        #region Get All  

        [TestMethod]
        public async void Task_GetPosts_Return_OkResult()
        {
            //Arrange  
            var controller = new PostController(repository);

            //Act  
            var data = await controller.GetPosts();

            //Assert  
            Assert.IsType<OkObjectResult>(data);
        }

        [TestMethod]
        public void Task_GetPosts_Return_BadRequestResult()
        {
            //Arrange  
            var controller = new PostController(repository);

            //Act  
            var data = controller.GetPosts();
            data = null;

            if (data != null)
                //Assert  
                Assert.IsType<BadRequestResult>(data);
        }

        [TestMethod]
        public async void Task_GetPosts_MatchResult()
        {
            //Arrange  
            var controller = new PostController(repository);

            //Act  
            var data = await controller.GetPosts();

            //Assert  
            Assert.IsType<OkObjectResult>(data);

            var okResult = data.Should().BeOfType<OkObjectResult>().Subject;
            var post = okResult.Value.Should().BeAssignableTo<List<PostViewModel>>().Subject;

            Assert.Equal("Test Title 1", post[0].Title);
            Assert.Equal("Test Description 1", post[0].Description);

            Assert.Equal("Test Title 2", post[1].Title);
            Assert.Equal("Test Description 2", post[1].Description);
        }

        #endregion

        #region Add New ShopBridge  

        [TestMethod]
        public async void Task_Add_ValidData_Return_OkResult()
        {
            //Arrange  
            var controller = new PostController(repository);
            var post = new Post() { Title = "Test Title 3", Description = "Test Description 3", CategoryId = 2, CreatedDate = DateTime.Now };

            //Act  
            var data = await controller.AddPost(post);

            //Assert  
            Assert.IsType<OkObjectResult>(data);
        }

        [TestMethod]
        public async void Task_Add_InvalidData_Return_BadRequest()
        {
            //Arrange  
            var controller = new PostController(repository);
            Post post = new Post() { Title = "Test Title More Than 20 Characteres", Description = "Test Description 3", CategoryId = 3, CreatedDate = DateTime.Now };

            //Act              
            var data = await controller.AddPost(post);

            //Assert  
            Assert.IsType<BadRequestResult>(data);
        }

        [TestMethod]
        public async void Task_Add_ValidData_MatchResult()
        {
            //Arrange  
            var controller = new PostController(repository);
            var post = new Post() { Title = "Test Title 4", Description = "Test Description 4", CategoryId = 2, CreatedDate = DateTime.Now };

            //Act  
            var data = await controller.AddPost(post);

            //Assert  
            Assert.IsType<OkObjectResult>(data);

            var okResult = data.Should().BeOfType<OkObjectResult>().Subject;
            // var result = okResult.Value.Should().BeAssignableTo<PostViewModel>().Subject;  

            Assert.Equal(3, okResult.Value);
        }

        #endregion

        #region Update Existing ShopBridge 

        [TestMethod]
        public async void Task_Update_ValidData_Return_OkResult()
        {
            //Arrange  
            var controller = new PostController(repository);
            var postId = 2;

            //Act  
            var existingPost = await controller.GetPost(postId);
            var okResult = existingPost.Should().BeOfType<OkObjectResult>().Subject;
            var result = okResult.Value.Should().BeAssignableTo<PostViewModel>().Subject;

            var post = new Post();
            post.Title = "Test Title 2 Updated";
            post.Description = result.Description;
            post.CategoryId = result.CategoryId;
            post.CreatedDate = result.CreatedDate;

            var updatedData = await controller.UpdatePost(post);

            //Assert  
            Assert.IsType<OkResult>(updatedData);
        }

        [TestMethod]
        public async void Task_Update_InvalidData_Return_BadRequest()
        {
            //Arrange  
            var controller = new PostController(repository);
            var postId = 2;

            //Act  
            var existingPost = await controller.GetPost(postId);
            var okResult = existingPost.Should().BeOfType<OkObjectResult>().Subject;
            var result = okResult.Value.Should().BeAssignableTo<PostViewModel>().Subject;

            var post = new Post();
            post.Title = "Test Title More Than 20 Characteres";
            post.Description = result.Description;
            post.CategoryId = result.CategoryId;
            post.CreatedDate = result.CreatedDate;

            var data = await controller.UpdatePost(post);

            //Assert  
            Assert.IsType<BadRequestResult>(data);
        }

        [TestMethod]
        public async void Task_Update_InvalidData_Return_NotFound()
        {
            //Arrange  
            var controller = new PostController(repository);
            var postId = 2;

            //Act  
            var existingPost = await controller.GetPost(postId);
            var okResult = existingPost.Should().BeOfType<OkObjectResult>().Subject;
            var result = okResult.Value.Should().BeAssignableTo<PostViewModel>().Subject;

            var post = new Post();
            post.PostId = 5;
            post.Title = "Test Title More Than 20 Characteres";
            post.Description = result.Description;
            post.CategoryId = result.CategoryId;
            post.CreatedDate = result.CreatedDate;

            var data = await controller.UpdatePost(post);

            //Assert  
            Assert.IsType<NotFoundResult>(data);
        }

        #endregion
        #region Delete Post  

        [TestMethod]
        public async void Task_Delete_Post_Return_OkResult()
        {
            //Arrange  
            var controller = new PostController(repository);
            var postId = 2;

            //Act  
            var data = await controller.DeletePost(postId);

            //Assert  
            Assert.IsType<OkResult>(data);
        }

        [TestMethod]
        public async void Task_Delete_Post_Return_NotFoundResult()
        {
            //Arrange  
            var controller = new PostController(repository);
            var postId = 5;

            //Act  
            var data = await controller.DeletePost(postId);

            //Assert  
            Assert.IsType<NotFoundResult>(data);
        }

        [TestMethod]
        public async void Task_Delete_Return_BadRequestResult()
        {
            //Arrange  
            var controller = new PostController(repository);
            int? postId = null;

            //Act  
            var data = await controller.DeletePost(postId);

            //Assert  
            Assert.IsType<BadRequestResult>(data);
        }

        #endregion
    }
