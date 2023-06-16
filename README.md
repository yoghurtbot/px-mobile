# PX Mobile Cookie Baker

Built for version `1.13.2` but probably not hard to update for newer versions.

> I have removed my custom TLS/HTTP library from the code, you will need to implement your own solution



# Usage

```csharp
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
```
