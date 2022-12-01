using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Collections.Generic;
using FunctionApp3.Model;

namespace FunctionApp3.Functions
{
    public static class GetUsersInChallenge
    {
        [FunctionName("GetUsersInChallenge")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {

            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                // Get the connection string from app settings and use it to create a connection.
                var str = Environment.GetEnvironmentVariable("sqldb_connection");
                string challId = req.Query["challId"];
                //var user = new AppUser();
                string usersJsonString = "";
                string activitiesJsonString = "";
                string detailsJsonString = "";
                var jsonString = "";
                ChallengeDetails cd = new ChallengeDetails();

                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;

                    cmd.CommandText = $"SELECT dbo.GetUsersInChallenge('{challId}')";

                    cmd.Connection = conn;

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            usersJsonString = reader.GetString(0);
                        }
                    }
                    conn.Close();
                    conn.Open();
                    cmd.CommandText = $"SELECT dbo.GetChallengeDetails('{challId}')";

                    cmd.Connection = conn;

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            cd.Details = reader.GetString(0);
                        }
                    }

                    conn.Close();
                    conn.Open();
                    cmd.CommandText = $"SELECT dbo.GetActivities('{challId}')";

                    cmd.Connection = conn;
                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            activitiesJsonString = reader.GetString(0);
                        }
                    }

                    var actList = JsonConvert.DeserializeObject<List<Activitiy>>(activitiesJsonString);
                    var userList = JsonConvert.DeserializeObject<List<AppUser>>(usersJsonString);

                    foreach (var u in userList)
                    {
                        u.Activities = actList.FindAll(act => act.UserId.Equals(u.UserId)).ToArray();
                    }

                    cd.Users = JsonConvert.SerializeObject(new
                    {
                        users = userList
                    }, Formatting.Indented);

                    if (cd.Details != null && cd.Users != null)
                    {
                        log.LogInformation("Successfull");
                    }
                    else
                    {
                        log.LogInformation("Failed");
                    }
                }

                return new OkObjectResult(cd);

            }
            catch (Exception e)
            {

                log.LogInformation("Catched Error", e);
                log.LogDebug(e.Message);
                return new OkObjectResult(e);
            }
        }
    }
}
