using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace TEST_MetaService.TestSamples
{
    public class TestMonitor
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        public TestMonitor(ITestOutputHelper output)
        {
            var hostBuilder = new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                // Add TestServer
                webHost.UseTestServer();
                webHost.UseStartup<MonitorService.Startup>();
            });

           
            // Create and start up the host
            var host = hostBuilder.Start();

            // Create an HttpClient which is setup for the test host
            _client = host.GetTestClient(); 

            _output = output;
        }

        [Trait ("Category", "Monitor")]
        [Fact]
        public async Task Test_ErrorCmdDetectAndPush()
        {
           

            HttpResponseMessage response = await _client.GetAsync($"/Monitor/ErrorCmdDetectAndPush");

            //檢查是否成功
            response.EnsureSuccessStatusCode();

            //讀取內容
            string responseString = await response.Content.ReadAsStringAsync();
            _output.WriteLine(responseString);

            JObject result = JObject.Parse(responseString);            
            

            Assert.Equal("-1", result["Result"]);
        }

        [Trait ("Category", "Monitor")]
        [Theory]
        [InlineData("5")]        
        [InlineData("10")]
        public async Task Test_EachErrorCmdDetectAndPush(string minute)
        {
           

            HttpResponseMessage response = await _client.GetAsync($"/Monitor/ErrorCmdDetectAndPush?minute="+minute);

            //檢查是否成功
            response.EnsureSuccessStatusCode();

            //讀取內容
            string responseString = await response.Content.ReadAsStringAsync();
            _output.WriteLine(responseString);

            JObject result = JObject.Parse(responseString);            
            

             Assert.Equal("1", result["Result"]);
        }


    }
}