using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using px_mobile.Cryptography;
using px_mobile.Extensions;
using px_mobile.Properties;

namespace px_mobile.PxMobile
{
    public class PxMobileGenerator
    {
        private DateTime _startTime;

        public PxMobileGenerator()
        {
        }

        public static PxMobileDevice RandomDevice()
        {
            Random _random = new Random();
            JObject devices = JObject.Parse(Resources.device_data);

            int randomIndex = _random.Next(0, ((JArray)devices["fingerprints"]).Count);
            JToken device = ((JArray)devices["fingerprints"])[randomIndex];

            PxMobileDevice pxDevice = new PxMobileDevice()
            {
                BatteryPercentage = RandomNumberGenerator.RandomNumberBetween(30, 90),
                Model = device[4].ToString(),
                Manufacturer = device[5].ToString(),
                ScreenHeight = Convert.ToInt32(device[1]),
                ScreenWidth = Convert.ToInt32(device[0]),
                AndroidSdkVersion = Convert.ToInt32(device[2]),
                Temperature = RandomNumberGenerator.RandomNumberWithDecimals(28, 35, 2),
                Voltage = RandomNumberGenerator.RandomNumberWithDecimals(4, 5, 3)
            };
            return pxDevice;
        }

        public PxMobileSensor BuildPxMobileSensor(PxMobileDevice device, string version, string appName, string appVersion, string pxVersion, string appNameClean)
        {
            switch (version)
            {
                case "1.13.2":
                    _startTime = DateTime.Now;
                    var guid = PxUuidGenerator.GeneratePxUuid().ToString().ToLower();
                    string guidPart = guid.ToString().Split('-')[0].ToUpper();

                    JArray final = new JArray
                    {
                        new JObject
                        {
                            ["t"] = "PX315",
                            ["d"] = new JObject
                            {
                                //["PX350"] = RandomNumberGenerator.GenerateRandomNumber(20, 90), //SDK Ready Time
                                ["PX91"] = device.ScreenWidth, //width
                                ["PX92"] = device.ScreenHeight, //height
                                ["PX316"] = false,
                                ["PX345"] = 1, //context.getSystemService("phone")).getNetworkOperatorName
                                ["PX351"] = 0, //APP_IS_ACTIVE_TIME_INTERVAL
                                ["PX317"] = "wifi", //wifi, mobile, disconnected, permission_denied
                                ["PX318"] = device.AndroidSdkVersion.ToString(), //new Integer(Build.VERSION.SDK_INT).toString();
                                ["PX319"] = "4.4.153-perf+", //Kernal
                                ["PX320"] = device.Model, //Model
                                ["PX323"] = DateExtensions.UnixTimeNow(),
                                ["PX326"] = guid.ToString(),
                                ["PX327"] = guidPart,
                                ["PX328"] = SHA1.Hash($"{device.Model}{guid}{guidPart}"),
                                ["PX337"] = true, //packageManager.hasSystemFeature("android.hardware.location.gps");
                                ["PX336"] = true, //packageManager.hasSystemFeature("android.hardware.sensor.gyroscope");
                                ["PX335"] = true, //packageManager.hasSystemFeature("android.hardware.sensor.accelerometer");
                                ["PX334"] = false, //packageManager.hasSystemFeature("android.hardware.ethernet");
                                ["PX333"] = true, // packageManager.hasSystemFeature("android.hardware.touchscreen");
                                ["PX331"] = true, // packageManager.hasSystemFeature("android.hardware.nfc");
                                ["PX332"] = true, //packageManager.hasSystemFeature("android.hardware.wifi");
                                ["PX330"] = "new_session", //new_session, bg_to_fg, wakeup
                                ["PX421"] = "false", //root check, should always be false
                                ["PX442"] = "false", //Build.TAGS.contains("test-keys")
                                ["PX339"] = device.Manufacturer, //manufacturer
                                ["PX322"] = "Android",
                                ["PX340"] = $"v{pxVersion}", //px version
                                ["PX341"] = appNameClean, //clean name i.e. Asda
                                ["PX342"] = appVersion, //App Version
                                ["PX348"] = appName, //App Name
                                ["PX343"] = "Unknown",
                                ["PX344"] = "missing_value",
                                ["PX347"] = new JArray(new[] { "en_US" }),
                                ["PX413"] = "good", //battery health
                                ["PX414"] = "charging", //unknown, charging, discharging, not charging
                                ["PX415"] = device.BatteryPercentage,
                                ["PX146"] = "USB", //none, ac, usb
                                ["PX419"] = "Li-ion", //battery type
                                ["PX418"] = device.Temperature, //((float) registerReceiver.getIntExtra("temperature", 0)) / 10.0f; // battery temp
                                ["PX420"] = device.Voltage //((float) registerReceiver.getIntExtra("voltage", 0)) / 1000.0f; // battery voltage
                            }
                        }
                    };
                    return new PxMobileSensor()
                    {
                        Payload = final.ToString(Formatting.None).Base64Encode(),
                        Uuid = guid.ToString()
                    };
                default:
                    throw new ArgumentException("Unsupported Version");
            }
        }


        public PxMobileSensor BuildPxMobileChallengePayload(PxMobileDevice device, string appVersion, string appName, string appCleanName, string version, List<string> challengeParts)
        {
            int challengeResult =
                ChallengeSolver(Convert.ToInt32(challengeParts[5]), Convert.ToInt32(challengeParts[6]),
                    Convert.ToInt32(challengeParts[7]), Convert.ToInt32(challengeParts[3]),
                    Convert.ToInt32(challengeParts[4]), Convert.ToInt32(challengeParts[8]), device.Model);

            JArray finalArray = new JArray();
            JObject final = new JObject
            {
                ["t"] = "PX329",
                ["d"] = new JObject()
                {
                    ["PX349"] = (DateTime.Now - _startTime).Milliseconds, //time taken to complete challenge
                    ["PX320"] = device.Model, //model
                    ["PX259"] = Convert.ToInt64(challengeParts[1]), //challenge timestamp
                    ["PX256"] = challengeParts[2], //challenge signature
                    ["PX257"] = challengeResult.ToString(), //challenge result
                    ["PX339"] = device.Manufacturer, //manufacturer
                    ["PX322"] = "Android",
                    ["PX340"] = $"v{version}",
                    ["PX341"] = appCleanName,
                    ["PX342"] = appVersion,
                    ["PX348"] = appName,
                    ["PX343"] = "Unknown",
                    ["PX344"] = "missing_value",
                    ["PX347"] = new JArray(new[] { "en_US" }),
                    ["PX413"] = "good", //battery health
                    ["PX414"] = "charging", //battery state
                    ["PX415"] = device.BatteryPercentage, //battery percentage
                    ["PX416"] = "usb", 
                    ["PX419"] = "Li-ion", //batery type
                    ["PX418"] = device.Temperature, //battery temp
                    ["PX420"] = device.Voltage //battery voltage
                }
            };
            finalArray.Add(final);
            return new PxMobileSensor()
            {
                Payload = finalArray.ToString(Formatting.None).Base64Encode(),
            };
        }

        #region Challenge Stuff
        public static int ChallengeSolver(int i, int i2, int i3, int i4, int i5, int i6, string model)
        {
            return InternalChallengeSolver(
                InternalChallengeSolver(i, i2, i4, i6), i3, i5, i6) ^ StringToInt2(model);
        }
        public static int InternalChallengeSolver(int i, int i2, int i3, int i4)
        {
            int i5 = i4 % 10;
            int i6 = i5 != 0 ? i3 % i5 : i3 % 10;
            int i7 = i * i;
            int i8 = i2 * i2;
            switch (i6)
            {
                case 0:
                    return i7 + i2;
                case 1:
                    return i + i8;
                case 2:
                    return i7 * i2;
                case 3:
                    return i ^ i2;
                case 4:
                    return i - i8;
                case 5:
                    int i9 = i + 783;
                    return (i9 * i9) + i8;
                case 6:
                    return (i ^ i2) + i2;
                case 7:
                    return i7 - i8;
                case 8:
                    return i * i2;
                case 9:
                    return (i2 * i) - i;
                default:
                    return -1;
            }
        }
        public static int StringToInt2(string str)
        {
            byte[] bytes = new byte[str.Length];

            for (int i = 0; i < str.Length; i++)
                bytes[i] = (byte)str[i];
            return SwapEndian(BitConverterToInt(bytes));
        }
        public static int BitConverterToInt(byte[] array)
        {
            return (array[0] | array[1] << 8 | array[2] << 16 | array[3] << 24);
        }
        public static int SwapEndian(int val)
        {
            return ((val & 0xFF) << 24)
                   | ((val & 0xFF00) << 8)
                   | ((val >> 8) & 0xFF00)
                   | ((val >> 24) & 0xFF);
        }
        #endregion
    }
}
