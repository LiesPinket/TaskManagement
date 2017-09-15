using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Data.SqlClient;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;

namespace TaskAzureFct
{
    public static class ReadTaskDB
    {
        [FunctionName("ReadTask")]
        public static async Task<HttpResponseMessage> ReadTask([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "tasks/{id}")]HttpRequestMessage req, int id, TraceWriter log)
        {//hello world
            log.Info("C# HTTP trigger function processed a request.");

            string constr = System.Environment.GetEnvironmentVariable("SQLConnectionString", System.EnvironmentVariableTarget.Process);

            SqlConnection conn = new SqlConnection(constr);
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Person", conn);
            SqlDataReader reader = cmd.ExecuteReader();

            List<Person> results = new List<Person>();

            while(reader.Read())
            {
                results.Add(new Person() { Id = int.Parse( reader["Id"].ToString()), Name = reader["Name"].ToString() });
                Debug.WriteLine(reader["Name"].ToString());
            }



            return req.CreateResponse(HttpStatusCode.OK, results);

            //string name = req.GetQueryNameValuePairs()
            //    .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
            //    .Value;

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            //// Set name to query string or body data
            //name = name ?? data?.name;

            //return name == null
            //    ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
            //    : req.CreateResponse(HttpStatusCode.OK, "Hello " + name);
        }

        [FunctionName("PostTask")]
        public static async Task<HttpResponseMessage> PostTask([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "tasks")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            string json =  await req.Content.ReadAsStringAsync();
            Person p = JsonConvert.DeserializeObject<Person>(json);

            string constr = System.Environment.GetEnvironmentVariable("SQLConnectionString", System.EnvironmentVariableTarget.Process);
  
            SqlConnection conn = new SqlConnection(constr);
            conn.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO Person VALUES (@id, @name)", conn);
            cmd.Parameters.Add(new SqlParameter("@id", p.Id));
            cmd.Parameters.Add(new SqlParameter("@name", p.Name));
            int result = cmd.ExecuteNonQuery();

            if (result > 0)
                return req.CreateResponse(HttpStatusCode.OK, "OK");
            else
                return req.CreateErrorResponse(HttpStatusCode.InternalServerError, "not ok");
        }

    }
}
