using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnboardingService.Controllers;
using OnboardingService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OnboardServiceTest
{
    public class UsersControllerTest
    {

        public UsersController GetController()
        {
            var optionsBuilder = new DbContextOptionsBuilder<UserContext>();
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            var appcontext = new UserContext(optionsBuilder.Options);
            CreateData(optionsBuilder.Options);
            return new UsersController(appcontext);
        }



        public void CreateData(DbContextOptions<UserContext> options)
        {
            using (var userContext = new UserContext(options))
            {
                List<User> users = new List<User>()
            {
                new User()
                {
                    UserId=11,
                    FirstName = "Sudarshan",
                    LastName = "S",
                    Username = "ssk@gmail.com",
                    Password = "ssk123",
                    Designation = "Developer",
                    WorkspaceName = "StackRoute"
                },

                new User()
                {
                    UserId=12,
                    FirstName = "Rahul",
                    LastName = "Kumar",
                    Username = "Rahul@gmail.com",
                    Password = "rbk123",
                    Designation = "Full Stack Developer",
                    WorkspaceName = "StackRoute"
                }
            };

                userContext.User.AddRange(users);
                userContext.SaveChanges();
            }
        }

    [Fact]
    public void TestGetUsers()
    {
            var controller = GetController();
            var users = controller.GetUser().ToList();
        Console.WriteLine(users.Count);
        foreach (User user in users)
        {
            Console.WriteLine(user.UserId);
        }
        Assert.Equal(2, users.Count);
    }


    [Fact]
    public async Task TestPost()
    {
            var controller = GetController();
            var user = new User()
        {
            FirstName = "Kiran",
            LastName = "Jain",
            Username = "kiran@gmail.com",
            Password = "kj123",
            Designation = "Full Stack Developer",
            WorkspaceName = "StackRoute"
        };

        var result = await controller.PostUser(user);
        var resultAsOkObjectResult = result as CreatedAtActionResult;
        var userReturn = resultAsOkObjectResult.Value as User;

        Assert.Equal(user.Username, userReturn.Username);

    }

    [Fact]
    public async Task TestPut()
    {
            var controller = GetController();
            var user = new User()
        {
            UserId = 11,
            FirstName = "Prashant",
            LastName = "Kumar",
        };

        var result = await controller.PutUser(11, user);
        var resultAsOkObjectResult = result as NoContentResult;
        Assert.Equal(204, resultAsOkObjectResult.StatusCode);
    }

    [Fact]
    public async Task TestDelete()
    {
            var controller = GetController();
            var result = await controller.DeleteUser(11);
        var resultAsOkObjectResult = result as OkObjectResult;
        var userReturn = resultAsOkObjectResult.Value as User;

        Console.WriteLine(userReturn.FirstName);
        Assert.Equal(11, userReturn.UserId);
    }
}
}
