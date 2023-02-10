using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CurrencyConverter
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            string fromUnit;
            string toUnit;
            float amount;
            
            Console.WriteLine("                 === Currency Converter in C# (Written by yigitgumus) ===                 ");
            Console.Write("                 From: ");
            fromUnit = Console.ReadLine();
            Console.Write("                 To: ");
            toUnit = Console.ReadLine();
            Console.Write("                 Amount: ");
            while (true) {
                try { amount = (float)Convert.ToDouble(Console.ReadLine()); break;  }
                catch(Exception ex) { Console.Write("                 Amount: "); }

            }

            CurrencyConv requestData = new CurrencyConv(fromUnit, toUnit, amount);
            CurrencyResponse response = await MakeRequest(requestData);
            Console.WriteLine($@"
                 = Sonuc =
                 from: {response.query.from}
                 to: {response.query.to}
                 amount: {response.query.amount}
                 date: {response.date}
                 SONUC: {response.info.rate * response.query.amount}
             ");
            if(response.info.rate == null)
            {
                Console.WriteLine("                 Error: yanlış para birimi kısaltması girdiniz!");
            }
        }

        static async Task<CurrencyResponse?> MakeRequest(CurrencyConv request)
        {
            string baseUrl = @"https://api.exchangerate.host/";
            var client = new HttpClient();

            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Add("User-Agent", "C# console program");
            var url = $"/convert?from={request.baseUnit}&to={request.targetUnit}&amount={request.amount}";
            client.DefaultRequestHeaders.Accept.Add(
                 new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var resp = await response.Content.ReadAsStringAsync();
            
            CurrencyResponse? responseJson = JsonConvert.DeserializeObject<CurrencyResponse>(resp);
            return responseJson;
        }
    }

    [Serializable]
    public class CurrencyResponse
    {
        [JsonProperty("query")]
        public Query query { get; set; }
        [JsonProperty("date")]
        public string date { get; set; }
        [JsonProperty("info")]
        public Info? info { get; set; }

        public string errMsg;
    }
    [Serializable]

    public class Info {
        [JsonProperty("rate")] public float? rate { get; set; }
    }

    [Serializable]
    public class Query
    {
        [JsonProperty("from")] public string from { get; set; }
        [JsonProperty("to")] public string to { get; set; }
        [JsonProperty("amount")] public float amount { get; set; }
    }

    public class CurrencyConv
    {
        public string baseUnit;
        public string targetUnit;
        public float amount;

        public CurrencyConv(string fromUnit, string toUnit, float amount)
        {
            this.baseUnit = fromUnit;
            this.targetUnit = toUnit;
            this.amount = amount;
        }
    }


}