namespace TechTeaStudio.Config.Sample
{
	public class MyConfig
	{
		public string DatabaseConnectionString { get; set; } = string.Empty;
		public string DatabaseName { get; set; } = "default_db";
		public string ApiToken { get; set; } = string.Empty;
		public string ApiBaseUrl { get; set; } = "https://api.example.com";
		public bool EnableLogging { get; set; } = true;
	}
}
