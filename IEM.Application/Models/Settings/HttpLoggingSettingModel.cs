namespace IEM.Application.Models.Settings
{
    public class HttpLoggingSettingModel
    {
        public bool Enabled { get; set; }
        public bool LogRequest { get; set; }
        public bool LogResponse { get; set; }
        public string LogLevel { get; set; }
    }
}
