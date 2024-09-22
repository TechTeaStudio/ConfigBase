namespace TechTeaStudio.Config.Sample
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var configHandler = new ConfigFileHandler<MyConfig>(
				directoryPath: @"E:\\", //путь к директории
				fileName: "config", // Имя файла без расширения
				fileExtension: "json", // Расширение файла
				defaultConfig: new MyConfig
				{
					DatabaseConnectionString = "Server=myServer;Database=myDb;User=myUser;Password=myPass;",
					DatabaseName = "default_db",
					ApiToken = "your_api_token_here",
					ApiBaseUrl = "https://api.example.com",
					EnableLogging = true
				},
				new JsonConfigSerializer<MyConfig>()
			);

			var config = configHandler.ReadConfig();

			Console.WriteLine("Current Configuration:");
			Console.WriteLine($"Database Connection String: {config.DatabaseConnectionString}");
			Console.WriteLine($"Database Name: {config.DatabaseName}");
			Console.WriteLine($"API Token: {config.ApiToken}");
			Console.WriteLine($"API Base URL: {config.ApiBaseUrl}");
			Console.WriteLine($"Enable Logging: {config.EnableLogging}");

			config.ApiToken = $"new_api_token + {Guid.NewGuid()}";
			config.EnableLogging = false;

			configHandler.SaveConfig(config);

			Console.WriteLine("Configuration updated and saved.");
			var config2 = configHandler.ReadConfig();
			Console.WriteLine($"API Token: {config2.ApiToken}");

		}
	}
}
