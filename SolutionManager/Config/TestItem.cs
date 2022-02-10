using Newtonsoft.Json;

namespace SolutionManager.Config;

public class TestItem
{
    [JsonProperty("name", Required = Required.Always)]
    public string Name { get; set; }

    [JsonProperty("type", Required = Required.Always)]
    public string Type { get; set; }

    [JsonProperty("baseUrl", Required = Required.Always)]
    public string BaseUrl { get; set; }

    [JsonProperty("basicAuthUsername")]
    public string BasicAuthUsername { get; set; }

    [JsonProperty("basicAuthPassword")]
    public string BasicAuthPassword { get; set; }

    [JsonProperty("formsAuthCookieName")]
    public string FormsAuthCookieName { get; set; }

    [JsonProperty("formsAuthCookieValue")]
    public string FormsAuthCookieValue { get; set; }

    [JsonProperty("formsAuthCookieDomain")]
    public string FormsAuthCookieDomain { get; set; }
}