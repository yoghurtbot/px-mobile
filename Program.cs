using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using px_mobile.HTTP;
using px_mobile.PxMobile;

namespace px_mobile
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            PxMobileGenerator pxMobileGen = new PxMobileGenerator();
            var device = PxMobileGenerator.RandomDevice();

            PxMobileSensor sensor = pxMobileGen.BuildPxMobileSensor(device, "1.13.2", "com.asda.android", "23.01.03", "PX1UGLZTko", "Asda");
            Console.WriteLine(sensor.Payload);

            //Post first sensor
            JObject firstResponse = await SubmitFirstSensor(sensor, "PX1UGLZTko", "1.13.2");

            //Parse response and post challenge
            string sid = firstResponse["do"].FirstOrDefault(s => s.ToString().Contains("sid")).ToString().Split('|')[1];
            string vid = firstResponse["do"].FirstOrDefault(s => s.ToString().Contains("vid")).ToString().Split('|')[1];
            string appc2 = firstResponse["do"].FirstOrDefault(s => s.ToString().Contains("appc|2")).ToString();
            string appcFixed = "2|" + appc2.Split(new string[] {"appc|2|"}, StringSplitOptions.RemoveEmptyEntries)[1];
            List<string> challengeParts = appcFixed.Replace("appc|2|", "").Split('|').ToList();

            var challengePayload = pxMobileGen.BuildPxMobileChallengePayload(device, "2", "23.01.03", "Asda", "1.13.2", challengeParts);

            //Submit Challenge
            JObject px3Payload = await SubmitSecondSensor(challengePayload.Payload, "PX1UGLZTko", vid, sid, sensor.Uuid, "1.13.2");

            //Return baked cookie
            string[] bakeParts = px3Payload["do"][0].ToString().Split(new string[] {"|"}, StringSplitOptions.RemoveEmptyEntries);
            PxBakedCookie bakedCookie = new PxBakedCookie()
            {
                DeviceManufacturer = device.Manufacturer,
                DeviceModel = device.Model,
                Px3Cookie = $"3:{bakeParts[3]}"
            };
        }

        private static async Task<JObject> SubmitFirstSensor(PxMobileSensor pxSensor, string appId, string version)
        {
            Headers headers = new Headers()
            {
                {"User-Agent",$"PerimeterX Android SDK/{version}"},
                {"Content-Type","application/x-www-form-urlencoded"},
                {"Host",$"collector-{appId}.perimeterx.net"},
                {"Connection","Close"},
                {"Accept-Encoding","gzip"},
            };

            List<KeyValuePair<string, string>> payload = new List<KeyValuePair<string, string>>()
            {
                new("ftag", "22"),
                new("payload", pxSensor.Payload),
                new("appId", appId),
                new("tag", "mobile"),
                new("uuid", pxSensor.Uuid.ToString())
            };

            //TODO: Removed my TLS Client. Replace with your own implementation
            Request request = new Request(new Uri($"https://collector-{appId.ToLower()}.perimeterx.net"), "/api/v1/collector/mobile", Method.POST, headers, null);
            request.AddFormData(payload, false);

            IResponse response = await _httpClient.DoRequest(request, false);
            JObject responseBody = JObject.Parse(response.Body.ToString());
            return responseBody;
        }

        private static async Task<JObject> SubmitSecondSensor(string challengePayload, string appId, string vid, string sid, string uuid, string version)
        {
            Headers headers = new Headers()
            {
                {"User-Agent",$"PerimeterX Android SDK/{version}"},
                {"Content-Type","application/x-www-form-urlencoded"},
                {"Host",$"collector-{appId}.perimeterx.net"},
                {"Connection","Close"},
                {"Accept-Encoding","gzip"},
            };

            List<KeyValuePair<string, string>> payload = new List<KeyValuePair<string, string>>()
            {
                new("vid", vid),
                new("ftag", "22"),
                new("payload", challengePayload),
                new("appId", appId),
                new("tag", "mobile"),
                new("uuid", uuid),
                new("sid", sid)
            };

            //TODO: Removed my TLS Client. Replace with your own implementation
            Request request = new Request(new Uri($"https://collector-{appId.ToLower()}.perimeterx.net"), "/api/v1/collector/mobile", Method.POST, headers, null);
            request.AddFormData(payload, false);

            IResponse response = await _httpClient.DoRequest(request, false);
            JObject responseBody = JObject.Parse(response.Body.ToString());
            return responseBody;
        }
    }
}
