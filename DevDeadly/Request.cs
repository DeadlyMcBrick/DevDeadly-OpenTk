//using System.Net.Http;
//using System.Text.Json;

//namespace DevTesting { }

//class Program
//{
//    static readonly HttpClient Client = new HttpClient();
//    static async Task Main()
//    {
//        try
//        {
//            Random random = new Random();
//            string[] DataCollector = { "1", "2", "3", "4" };
//            int TokenCollector = random.Next(0,DataCollector.Length);
//            using HttpResponseMessage response = await Client.GetAsync("");
//            response.EnsureSuccessStatusCode();
//            var RequestBody = await response.Content.ReadAsStringAsync();
//            Console.WriteLine($"This is the response OK 200{RequestBody}");
//        }
//        catch (HttpIOException e)
//        {
//            Console.WriteLine("\n Message Changed");
//            Console.WriteLine("Message is been sent", e.Message);
//        }    
//    }

//    protected void GetRequest()
//    {
//        var Structure = new
//        {
//            Cilinderinhoud = "2.457 cm³\r\n",
//            Topsnelheid = "255 km/u\r\n",
//            Aantal = 4,
//            igntion = true && false,
//            GPSOn = true,
//            GPSOff = false,
//            PlaatsVN = "Unknown"
//        };

//        var JsonPath = JsonSerializer.Serialize(Structure);
//        Console.WriteLine($"This is the path from the structure" + Structure);
//    }
//}