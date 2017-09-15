using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WallyLookaLayout.Data;

namespace TaskAzureFct
{
    public static class ReadTaskDB
    {
        [FunctionName("GetCategoriesTask")]
        public static async Task<HttpResponseMessage> GetCategoriesTask([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "categories")]HttpRequestMessage req, TraceWriter log)
        {
            string constr = System.Environment.GetEnvironmentVariable("SQLConnectionString", System.EnvironmentVariableTarget.Process);

            SqlConnection conn = new SqlConnection(constr);
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT Id, Name, Icon, Color FROM Categories", conn);
            SqlDataReader reader = cmd.ExecuteReader();

            List<Category> results = new List<Category>();

            while (reader.Read())
            {
                results.Add(new Category() {
                    Id = int.Parse(reader["Id"].ToString()),
                    Name = reader["Name"].ToString(),
                    IconUrl = reader["Icon"].ToString(),
                    ColorHex = reader["Color"].ToString()
                });

                Debug.WriteLine(reader["Name"].ToString());
            }
            
            return req.CreateResponse(HttpStatusCode.OK, results);
        }

        [FunctionName("GetCategoryByIdTask")]
        public static async Task<HttpResponseMessage> GetCategoryByIdTask([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "categories/{id}")]HttpRequestMessage req, int id, TraceWriter log)
        {
            string constr = System.Environment.GetEnvironmentVariable("SQLConnectionString", System.EnvironmentVariableTarget.Process);

            SqlConnection conn = new SqlConnection(constr);
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT Name, Icon, Color FROM Categories WHERE Id = @Id", conn);
            cmd.Parameters.Add(new SqlParameter("@Id", id));
            SqlDataReader reader = cmd.ExecuteReader();

            Category result = null;

            if (reader.Read())
            {
                result = new Category()
                {
                    Id = int.Parse(reader["Id"].ToString()),
                    Name = reader["Name"].ToString(),
                    IconUrl = reader["Icon"].ToString(),
                    ColorHex = reader["Color"].ToString()
                };

                Debug.WriteLine(reader["Name"].ToString());
            }

            return req.CreateResponse(HttpStatusCode.OK, result);
        }

        [FunctionName("PostExpenseTask")]
        public static async Task<HttpResponseMessage> PostExpenseTask([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "expenses")]HttpRequestMessage req, TraceWriter log)
        {
            string constr = System.Environment.GetEnvironmentVariable("SQLConnectionString", System.EnvironmentVariableTarget.Process);
            string json = await req.Content.ReadAsStringAsync();
            Expense exp = JsonConvert.DeserializeObject<Expense>(json);

            SqlConnection conn = new SqlConnection(constr);
            conn.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO Expenses (CategoryId, Location, Amount, DateSpent) VALUES (@CategoryId, @Location, @Amount, @DateSpent)", conn);
            cmd.Parameters.Add(new SqlParameter("@CategoryId", exp.Category.Id));
            cmd.Parameters.Add(new SqlParameter("@Location", exp.Location));
            cmd.Parameters.Add(new SqlParameter("@Amount", exp.Amount));
            cmd.Parameters.Add(new SqlParameter("@DateSpent", exp.DateSpent));
            int numrows = cmd.ExecuteNonQuery();            

            return req.CreateResponse(HttpStatusCode.OK, numrows + " row(s) added");
        }


        //[FunctionName("ReadTask")]
        //public static async Task<HttpResponseMessage> ReadTask([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "tasks/{id}")]HttpRequestMessage req, int id, TraceWriter log)
        //{//hello world
        //    log.Info("C# HTTP trigger function processed a request.");

        //    string constr = System.Environment.GetEnvironmentVariable("SQLConnectionString", System.EnvironmentVariableTarget.Process);

        //    SqlConnection conn = new SqlConnection(constr);
        //    conn.Open();
        //    SqlCommand cmd = new SqlCommand("SELECT * FROM Person", conn);
        //    SqlDataReader reader = cmd.ExecuteReader();

        //    List<Person> results = new List<Person>();

        //    while(reader.Read())
        //    {
        //        results.Add(new Person() { Id = int.Parse( reader["Id"].ToString()), Name = reader["Name"].ToString() });
        //        Debug.WriteLine(reader["Name"].ToString());
        //    }



        //    return req.CreateResponse(HttpStatusCode.OK, results);

        //    //string name = req.GetQueryNameValuePairs()
        //    //    .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
        //    //    .Value;

        //    // Get request body
        //    dynamic data = await req.Content.ReadAsAsync<object>();

        //    //// Set name to query string or body data
        //    //name = name ?? data?.name;

        //    //return name == null
        //    //    ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
        //    //    : req.CreateResponse(HttpStatusCode.OK, "Hello " + name);
        //}

        //[FunctionName("PostTask")]
        //public static async Task<HttpResponseMessage> PostTask([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "tasks")]HttpRequestMessage req, TraceWriter log)
        //{
        //    log.Info("C# HTTP trigger function processed a request.");

        //    string json =  await req.Content.ReadAsStringAsync();
        //    Person p = JsonConvert.DeserializeObject<Person>(json);

        //    string constr = System.Environment.GetEnvironmentVariable("SQLConnectionString", System.EnvironmentVariableTarget.Process);

        //    SqlConnection conn = new SqlConnection(constr);
        //    conn.Open();
        //    SqlCommand cmd = new SqlCommand("INSERT INTO Person VALUES (@id, @name)", conn);
        //    cmd.Parameters.Add(new SqlParameter("@id", p.Id));
        //    cmd.Parameters.Add(new SqlParameter("@name", p.Name));
        //    int result = cmd.ExecuteNonQuery();

        //    if (result > 0)
        //        return req.CreateResponse(HttpStatusCode.OK, "OK");
        //    else
        //        return req.CreateErrorResponse(HttpStatusCode.InternalServerError, "not ok");
        //}

    }
}
