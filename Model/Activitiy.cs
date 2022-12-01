using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp3.Model
{
    public class Activitiy
    {
        public string ActivityId { get; set; }
        public string TypeOf { get; set; }
        public float Km { get; set; }
        public int Kcal { get; set; }
        public string Date { get; set; }
        public string UserId { get; set; }

        public Activitiy(string userId, string activityId, string typeOf, int km, int kcal, string date)
        {
            ActivityId = activityId;
            TypeOf = typeOf;
            Km = km;
            Kcal = kcal;
            Date = date;
            UserId = userId;
        }
        public Activitiy()
        {

        }
    }
}
