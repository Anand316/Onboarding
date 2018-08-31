using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OnboardingService.Controllers;
using OnboardingService.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnboardServiceTest
{
    public class OnboardControllerTest
    {
        private readonly UserContext userContext;
        private OnboardController controller;

        public OnboardControllerTest()
        {
            var optionBuilder = new DbContextOptionsBuilder<UserContext>();
            optionBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            userContext = new UserContext(optionBuilder.Options);
            controller = new OnboardController(userContext);
            CreateData();
        }

        private void CreateData()
        {
            List<User> users = new List<User>()
            {
                new User()
                {
                    FirstName = "Sudarshan",
                    LastName = "S",
                    Username = "ssk@gmail.com",
                    Password = "ssk123",
                    Designation = "Developer",
                    WorkspaceName = "StackRoute"
                },

                new User()
                {
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


        [Fact]
        public async Task TestLogin()
        {
            LoginViewModel model = new LoginViewModel()
            {
                Username = "ssk@gmail.com",
                Password = "ssk123",
                WorkspaceName = "StackRoute"
            };

            var result = await controller.Login(model);

            var resultAsOkObjectResult = result as OkObjectResult;
            var response = (resultAsOkObjectResult.Value as object);
            var jwtstring = JObject.Parse(JsonConvert.SerializeObject(response));
            var token = new JwtSecurityToken(jwtEncodedString: jwtstring["token"].ToString());
            // Console.WriteLine("email => " + token.Claims.First(c => c.Type == "email").Value);
            //Console.WriteLine(jwtstring["token"]);

            model.Username.Equals(token.Claims.First(c => c.Type == "email").Value);
        }


        [Fact]
        public async Task SignUpTest()
        {
            var user = new User()
            {
                FirstName = "Srikant",
                LastName = "Jadhav",
                Username = "srikant@gmail.com",
                Password = "sjk123",
                Designation = "Full Stack Developer",
                WorkspaceName = "StackRoute"
            };

            var result = await controller.SignUp(user);
            var resultAsCreatedAction = result as CreatedAtActionResult;
            var userReturn = resultAsCreatedAction.Value as User;

            Assert.Equal(user.Username, userReturn.Username);
        }
    }
}
