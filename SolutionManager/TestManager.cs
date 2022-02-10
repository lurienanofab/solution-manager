using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using SolutionManager.Config;
using SolutionManager.Options;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SolutionManager;

public class TestManager : ManagerBase
{
    public TestManager(SolutionsConfig config) : base(config) { }

    public async Task TestAsync(TestOptions opts)
    {
        var solutions = FilterSolutions(opts);

        foreach (var sol in solutions)
        {
            if (sol.Test.Enabled)
            {
                try
                {
                    var test = Config.Tests.FirstOrDefault(x => x.Name == sol.Test.Name);

                    if (test == null)
                        throw new Exception($"Cannot find test: {sol.Test.Name}");

                    WebResponseData data;

                    if (test.Type == "api")
                    {
                        data = await RunApiTest(test, sol);
                        var content = JsonConvert.DeserializeObject(data.Content).ToString();
                        WriteOutput($"[{test.Name}:{sol.Test.Path}] {(int)data.StatusCode} ({data.StatusCode}) {content}", false);
                    }
                    else if (test.Type == "web")
                    {
                        data = await RunWebTest(test, sol);
                        WriteOutput($"[{test.Name}:{sol.Test.Path}] {(int)data.StatusCode} ({data.StatusCode}) {data.Bytes.Length} bytes", false);
                    }
                    else
                    {
                        throw new Exception($"Unknown test type: {test.Type}");
                    }
                }
                catch (Exception ex)
                {
                    WriteOutput(ex.Message, true);
                }
            }
        }
    }

    private async Task<WebResponseData> RunApiTest(TestItem test, SolutionItem sol)
    {
        var client = new RestClient(test.BaseUrl)
        {
            Authenticator = new HttpBasicAuthenticator(test.BasicAuthUsername, test.BasicAuthPassword)
        };

        var req = new RestRequest(sol.Test.Path);
        var resp = await client.GetAsync(req);

        return new WebResponseData { StatusCode = resp.StatusCode, Content = resp.Content, ContentType = resp.ContentType };
    }

    private async Task<WebResponseData> RunWebTest(TestItem test, SolutionItem sol)
    {
        var p = sol.Test.Path.StartsWith("/") ? sol.Test.Path : "/" + sol.Test.Path;
        
        var baseAddress = new Uri(test.BaseUrl);
        var cookieContainer = new CookieContainer();
        using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
        using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
        {
            cookieContainer.Add(baseAddress, new Cookie(test.FormsAuthCookieName, test.FormsAuthCookieValue, null, test.FormsAuthCookieDomain));
            
            //var result = await client.PostAsync("/test", content);
            var result = await client.GetAsync(p);
            
            string content = null;

            if (result.IsSuccessStatusCode)
                content = await result.Content.ReadAsStringAsync();

            return new WebResponseData { StatusCode = result.StatusCode, Content = content, ContentType = result.Content.Headers.ContentType.MediaType };
        }
    }
}

public class WebResponseData
{
    public HttpStatusCode StatusCode { get; set; }
    public string Content { get; set; }
    public byte[] Bytes => Encoding.UTF8.GetBytes(Content);
    public string ContentType { get; set; }
}
