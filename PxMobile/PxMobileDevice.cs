namespace px_mobile.PxMobile
{
    public class PxMobileDevice
    {
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public double Temperature { get; set; }
        public double Voltage { get; set; }
        public int BatteryPercentage { get; set; }
        public int AndroidSdkVersion { get; set; }
        public string BatteryType => "Li-ion";
    }
}
