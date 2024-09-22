# ConfigBase

Welcome to **ConfigBase**! 🎉

A cool, old-school library for handling configuration files with style. Whether you're dealing with JSON, XML, YAML, or INI formats, ConfigBase has got you covered! 🛠️

## Features

- **Versatile Configuration**: Easily manage settings in your preferred format—be it JSON, XML, YAML, or INI. 🌈
- **User-Friendly**: Designed with simplicity in mind, making configuration management a breeze! 🌬️
- **Robust Performance**: Built to handle your config needs efficiently and reliably. ⚡

## Getting Started

To get started, simply install the package using your favourite package manager. Here’s an example for NuGet:
```bash
    dotnet add package TechTeaStudio.Config
```
## Usage

To use ConfigBase, you'll need to create a class that represents your configuration.
Use the ConfigFileHandler class to read and write your configuration:
```cs
    var configHandler = new ConfigFileHandler<MyConfig>(
        directoryPath: @"", // Path to the directory
        fileName: "config", // File name without extension
        fileExtension: "json", // File extension
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
```
## Contribution

Feel free to contribute! Open issues, submit pull requests, or share your feedback. We love to hear from you! ❤️

## License

This project is licensed under the MIT License. For more details, please check the LICENSE file.

---

Join us in making configuration management a delightful experience! Happy coding! 🎊
