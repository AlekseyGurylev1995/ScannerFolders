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
        var scanDirectory = targetSection
            .GetSection("Scan:Directory")
            .Get<string>();

        var excludeScanFolders = targetSection
            .GetSection("Scan:ExcludeFolders")
            .Get<string[]>();

        var excludeScanFileExtensions = targetSection
            .GetSection("Scan:ExcludeFileExtensions")
            .Get<string[]>();

        var outputDirectory = targetSection
            .GetSection("Output:Directory")
            .Get<string>();

        var useSpaceBeetwenProjects = targetSection
            .GetSection("Output:UseSpaceBeetwenProjects")
            .Get<bool>();

        var useDomainDrivenDesignOrder = targetSection
            .GetSection("Output:UseDomainDrivenDesignOrder")
            .Get<bool>();

        var domainDrivenDesignOrder = targetSection
            .GetSection("Output:DomainDrivenDesignOrder")
            .Get<string[]>();

        var scanConfiguration = new ScanConfiguration(
            scanDirectory,
            excludeScanFolders, 
            excludeScanFileExtensions,
            outputDirectory,
            useSpaceBeetwenProjects,
            useDomainDrivenDesignOrder,
            domainDrivenDesignOrder);

        return scanConfiguration;
    }
}

