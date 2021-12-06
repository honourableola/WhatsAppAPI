using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Twilio.AspNet.Common;
using Twilio.AspNet.Core;
using Twilio.TwiML;
using WhatsAppAPI.Models;
using WhatsAppAPI.Services;

namespace WhatsAppAPI.Controllers
{
    public class HomeController : TwilioController
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ContextManager contextManager;

        private const string APPNAME = "LMS";

        public IDictionary<string, string> words = new Dictionary<string, string>()
        {
            {"hi", "hello" },
            {"what is your name?", "Test Bot" },
            {"what do you do?", "I am a LMS" },
            {"what is LMS?", "LMS is an acronym for Learning Management System" },
        };

        public List<User> users = new List<User>()
        {
            new User(){Id = 1, FirstName = "Ola", LastName = "Ade", PhoneNumber = "+2347062539241", IsRegistered=true, UserCourses = new List<Course>(){new Course(){Id = 1, CourseName = "C-Sharp", CourseCode = "CS101", Description = "A back end programming language developed by Microsoft" },
            new Course(){Id = 2, CourseName = "HTML", CourseCode = "HT101", Description = "A front end Mark-up language for designing user interface" },
            new Course(){Id = 3, CourseName = "CSS", CourseCode = "CS101", Description = "A front styling language for designing user interface" }} },
            new User(){Id = 2, FirstName = "Olu", LastName = "Tunde", PhoneNumber = "+2347062339241"},
            new User(){Id = 3, FirstName = "Lekan", LastName = "Bola", PhoneNumber = "+2347062539341"}
        };

        public List<Course> courses = new List<Course>()
        {
            new Course(){Id = 1, CourseName = "C-Sharp", CourseCode = "CS101", Description = "A back end programming language developed by Microsoft" },
            new Course(){Id = 2, CourseName = "HTML", CourseCode = "HT101", Description = "A front end Mark-up language for designing user interface" },
            new Course(){Id = 3, CourseName = "CSS", CourseCode = "CS101", Description = "A front styling language for designing user interface" }
        };

        public List<UserCourse> usercourses = new List<UserCourse>()
        {
            new UserCourse(){Id = 1, CourseId = 1, UserId = 1  },
            new UserCourse(){Id = 2, CourseId = 2, UserId = 1  },
            new UserCourse(){Id = 3, CourseId = 3, UserId = 1  },
            new UserCourse(){Id = 2, CourseId = 2, UserId = 2  }
        };

        public User GetUser(string phoneNumber)
        {
            return users.FirstOrDefault(u => u.PhoneNumber == phoneNumber);
        }

        public Course GetCourse(int id)
        {
            return courses.FirstOrDefault(u => u.Id == id);
        }

        public List<Course> GetUserRegisteredCourses (int id)
        {
            return users.Where(u => u.Id == id).SelectMany(e => e.UserCourses).ToList();
        }

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            contextManager = new ContextManager();
        }

        public IActionResult tt()
        {
            var response = new VoiceResponse();
            response.Say("hello world");
            return TwiML(response);
        }

        [HttpPost]
        public IActionResult Index(SmsRequest req)
        {

            var suffix = "whatsapp:";
            var userNumber = req.From.Substring(req.From.IndexOf(suffix) + suffix.Length);

            var user = GetUser(userNumber);

            var messagingResponse = new MessagingResponse();

            if (user == null || !user.IsRegistered)
            {
                messagingResponse.Message(GetUnRegisteredMessage());
                return TwiML(messagingResponse);
            }

            var userState = contextManager.GetUserContext(user.PhoneNumber);
            string text = "";

            if (req.Body.Trim().ToUpperInvariant().Equals("MENU"))
            {
                text = GetWelcomeMessage(user.FirstName);
                contextManager.SetUserContext(user.PhoneNumber, UserContext.Welcomed);
                messagingResponse.Message(text);
                return TwiML(messagingResponse);
            }
     
            var isValid = int.TryParse(req.Body.Trim(), out var option);
            if (!isValid && !userState.Equals(UserContext.Begin))
            {
                text = "Invalid Input";
                messagingResponse.Message(text);
                return TwiML(messagingResponse);
            }

            switch (userState)
            {
                case UserContext.Begin:
                    text = GetWelcomeMessage(user.FirstName);
                    contextManager.SetUserContext(user.PhoneNumber, UserContext.Welcomed);
                    break;

                case UserContext.Welcomed:
                    switch (option)
                    {
                        case 1:
                            text = GetUserCourses(user);
                            contextManager.SetUserContext(user.PhoneNumber,UserContext.ViewingCourses);
                            break;
                        case 2:
                            text = "Viewing assessments";
                            contextManager.SetUserContext(user.PhoneNumber, UserContext.ViewingAssessments);
                            break;
                        default:
                            text = "Invalid Input \n\n" + GetWelcomeMessage(user.FirstName);
                            break;
                    }

                    break;

                case UserContext.ViewingCourses:
                    switch (option)
                    {
                        case 1:
                            text = "Showing C#";
                            contextManager.SetUserContext(user.PhoneNumber, UserContext.ViewingCourseDetails);
                            break;
                        case 2:
                            text = "showing Css";
                            contextManager.SetUserContext(user.PhoneNumber, UserContext.ViewingCourseDetails);
                            break;
                        default:
                            text = "Invalid Input \n\n" + GetWelcomeMessage(user.FirstName);
                            break;
                    }
                    break;


            }

            messagingResponse.Message(text);

            return TwiML(messagingResponse);
        }

        private string GetUnRegisteredMessage()
        {

            var text = @"Hi 

This phone number is not registered. kindly contact your institution.
";

            return text;

        }


        private string GetWelcomeMessage(string userName)
        {
            var text = @$"Hi {userName} ✌

           Welcome to {APPNAME}

1. View Courses 
2. View Assessments
3. View Assignment

*_PRESS MENU ANYTIME TO COME BACK TO THIUS MENU_*
    ";

            return text;

        }

        private string GetUserCourses(User user)
        {

            var userCourses = GetUserRegisteredCourses(user.Id);
            var coursesString = "";
            for(int i =0; i < userCourses.Count(); i++)
            {
                var s = $"{i + 1}. {userCourses[i].CourseName}\n";
                //var s = $"{i + 1}. {userCourses[i].CourseId}\n";
                coursesString += s;
            }
            var text = @$" 😊 courses retrieved 

{user.FirstName} {user.LastName}, Your are offering the following courses: 

*_Press the course number to view course details and take further action_*

{coursesString}

0. Back
            ";

            return text;

        }


    }
}
