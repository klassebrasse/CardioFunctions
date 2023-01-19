using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp3.Model
{
    public class AppUser
    {
        public string Email { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePic { get; set; }
        public Activitiy[] Activities { get; set; }
        public Boolean isPremiumUser { get; set; }

        public AppUser(string email, string userId, string firstName, string lastName, string profilePic, Activitiy[] activities)
        {
            Email = email;
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            ProfilePic = profilePic;
            this.Activities = activities;
        }
        public AppUser()
        {

        }
    }
}
