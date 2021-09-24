using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Threading.Tasks;

namespace CreatioClient.Console
{
    internal class Program
    {
        static async Task Main()
        {

            var app = new App();
            //await app.CallConfService();
            //await app.ExecuteGet();
            //await app.ExecutePost();
            //await app.UseJson();
            //await app.UseProtoBuf();
            //var app = new App("http://k_krylov:7040", "Supervisor", "Supervisor"); ;
            var r = BenchmarkRunner.Run<App>();
            System.Console.ReadLine();
        }
    }

    [MemoryDiagnoser]
    public class App
    {
        private readonly Core.Client _client;

        public App()
        {
            _client = new Core.Client(
                "http://k_krylov:7040", "Supervisor", "Supervisor");    
        }
        
        //public async Task CallConfService()
        //{
        //    var r = await _client.CallConfigurationServiceAsync("DemoService", "GetJson", "GET" );
        //    System.Console.WriteLine(r);
        //}

        //public async Task ExecuteGet()
        //{
        //    string r = _client.ExecuteGetRequest("http://k_krylov:7040/Login/NuiLogin.aspx?ReturnUrl=%2f");
        //    System.Console.WriteLine(r);
        //}

        //public async Task ExecutePost()
        //{
        //    var r = await _client.ExecutePostRequestAsync("http://k_krylov:7040/0/DataService/json/SyncReply/SelectQuery", "{\"rootSchemaName\":\"Survey\"");
        //    string str = await r.Content.ReadAsStringAsync();
        //    System.Console.WriteLine(str);
        //}

        [Benchmark]
        public async Task UseProtoBuf()
        {
            
            DTO.Proto photo = await _client.CallConfigurationServiceDeserializedAsync<DTO.Proto>(
              "DemoService", "GetStream", "GET",Core.SerializedWith.ProtobufNet);
        }


        [Benchmark]
        public async Task UseJson()
        {
            DTO.Json photo = await _client.CallConfigurationServiceDeserializedAsync<DTO.Json>(
                "DemoService", "GetJson", "GET", Core.SerializedWith.Microsoft);
        }
    }
}
