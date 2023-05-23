using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using SQLAuthITokenAcquisition.Models;
using System.Diagnostics;

namespace SQLAuthITokenAcquisition.Controllers
{
    public class HomeController : Controller
    {
        // private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly HttpClient _httpClient;
        private readonly ILogger logger;

        public HomeController(ILogger<HomeController> _logger, IConfiguration configuration, ITokenAcquisition tokenAcquistion)
        {
            _configuration = configuration;
            _tokenAcquisition = tokenAcquistion;
            _httpClient = new HttpClient();
            logger = _logger;
        }

        public IActionResult Index()
        {
            //string downstreamAccessToken = string.Empty;
            string ConnectionString = @"Data Source=mydatabaseserver.8722ce7a94567.database.usgovcloudapi.net,3342; Initial Catalog=AdventureWorks;TrustServerCertificate=true;";

            // Get access token for the authorization API from the cache
            string accessToken = _tokenAcquisition.GetAccessTokenForUserAsync(new[] { @"https://sql.azuresynapse.usgovcloudapi.net/.default" }).Result;

            if (!string.IsNullOrEmpty(accessToken))
            {
                //_httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
               // HttpResponseMessage response = _httpClient.GetAsync($"{_configuration.GetValue<string>("API:APIBaseAddress")}Authorize").Result;

               /* if (response.IsSuccessStatusCode)
                {
                    downstreamAccessToken = response.ToString();//.ReadAsStringAsync().Result;
                }
                else
                {
                    string error = response.Content.ReadAsStringAsync().Result;
                    logger.LogError("Call to the Authorization API failed with the following error {0}", response.StatusCode);
                }*/
            }

            // Create connection to database
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.AccessToken = accessToken; // downstreamAccessToken;
                sqlConnection.Open();

            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}