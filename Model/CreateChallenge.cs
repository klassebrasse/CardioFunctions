using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp3.Model
{
    public class CreateChallenge
    {
        public CreateChallenge(string challengeId, DateOnly startDate, DateOnly endDate, string name, string userId, int returnValue)
        {
            ChallengeId = challengeId;
            StartDate = startDate;
            EndDate = endDate;
            Name = name;
            UserId = userId;
            ReturnValue = returnValue;
        }

        public string ChallengeId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public int ReturnValue { get; set; }


    }
}
