using Microsoft.Extensions.Configuration;

namespace ScannerFolders.Main;
internal class Program
{
    private static void Main(string[] args)
    {
        new Program().StartUp();
    }

    private void StartUp() 
    {
        var scanConfiguration = CreateScanConfiguration();
        var scannerFolders = new ScannerFolders(scanConfiguration);
        scannerFolders.Run();
    }

    private ScanConfiguration CreateScanConfiguration() 
    {
        // директория работы приложения
        //      например в рамках дебага: bin/Debug/net6.0
        var appDirectory = AppContext.BaseDirectory;

        // поднимаемся по директории на уровень проекта
        var projectDirectory = Path.GetFullPath(Path.Combine(appDirectory, @"..\..\.."));

        // формируем путь к файлу конфигурации
        var configPath = Path.Combine(projectDirectory, "appsettings.json");

        // считываем файл конфигурации
        var config = new ConfigurationBuilder()
            .AddJsonFile(configPath, optional: false, reloadOnChange: true)
            .Build();

        var targetSection = config.GetSection("AppSettings");
        var rootDirectory = targetSection
            .GetSection("Scan:RootDirectory")
            .Get<string>();

        var outputDirectory = targetSection
            .GetSection("Scan:OutputDirectory")
            .Get<string>();

        var excludeFolders = targetSection
            .GetSection("Scan:ExcludeFolders")
            .Get<string[]>();

        var excludeFileExtensions = targetSection
            .GetSection("Scan:ExcludeFileExtensions")
            .Get<string[]>();

        var useDomainDrivenDesignOrder = targetSection
            .GetSection("FolderOrdering:UseDomainDrivenDesignOrder")
            .Get<bool>();

        var domainDrivenDesignOrder = targetSection
            .GetSection("FolderOrdering:DomainDrivenDesignOrder")
            .Get<string[]>();

        var scanConfiguration = new ScanConfiguration(
            rootDirectory,
            outputDirectory,
            excludeFolders, 
            excludeFileExtensions,
            useDomainDrivenDesignOrder,
            domainDrivenDesignOrder);

        return scanConfiguration;
    }
}

