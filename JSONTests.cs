using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using Newtonsoft.Json;

namespace com.businesscentral
{
    public class JSONTests
    {
        [FunctionName("JSONTests")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            var sb=new StringBuilder();
            log.LogInformation("C# HTTP trigger function processed a request.");

            var timeZone="yyyy-MM-dd HH:mm:ss \"GMT\"zzz";

            sb.AppendLine("SERVER TIME ZONE IS " + DateTime.Now.ToString("\"GMT\"zzz"));

            var contentA = @"{""MyTime"":""2020-06-04T22:00:33+00:00""}";
            var objA=JsonConvert.DeserializeObject<TestTime>(contentA);
            sb.AppendLine(contentA + "->" + objA.MyTime.ToString(timeZone));

            var contentC = @"{""MyTime"":""2020-06-04T22:02:33+00:00""}";
            var objC=JsonConvert.DeserializeObject<TestTime>(contentC);
            sb.AppendLine(contentC + "->" + objC.MyTime.ToString(timeZone));

            var contentB = @"{""MyTime"":""2020-06-04T22:00:33+02:00""}";
            var objB=JsonConvert.DeserializeObject<TestTime>(contentB);
            sb.AppendLine(contentB + "->" + objB.MyTime.ToString(timeZone));

            var content1 = @"{""MyTime"":""2020-06-04T22:00:33+00:00""}";
            var obj1=Convert1(content1);
            sb.AppendLine(obj1.MyTime.ToString(timeZone));
            var obj2=Convert1(content1);
            sb.AppendLine(obj2.MyTime.ToString(timeZone));

            var content3 = @"{""MyTime"":""2020-06-04T22:00:33+02:00""}";
            var obj5=Convert1(content3);
            sb.AppendLine(obj1.MyTime.ToString(timeZone));
            var obj6=Convert1(content3);
            sb.AppendLine(obj2.MyTime.ToString(timeZone));

            var content2 = @"{""MyTime"":""2020-06-04T00:00:44+02:00""}";
            var obj3=Convert1(content2);
            sb.AppendLine(obj1.MyTime.ToString(timeZone));
            var obj4=Convert1(content2);
            sb.AppendLine(obj2.MyTime.ToString(timeZone));


            log.LogInformation(sb.ToString());

            return new OkObjectResult(sb.ToString());
        }

        public TestTime Convert1(string content)
        {
            return JsonConvert.DeserializeObject<TestTime>(content);
        }
        public TestTime Convert2(string content)
        {
            var jss = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Local,
                DateParseHandling = DateParseHandling.DateTimeOffset
            };

            return JsonConvert.DeserializeObject<TestTime>(content, jss);
        }

        public class TestTime
        {
            public DateTime MyTime {get;set;}
        }

    }
}
