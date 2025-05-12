namespace ScannerFolders.Main;
internal class ScanConfiguration : IScanConfiguration
{
    public string RootDirectory { get; }
    public string OutputDirectory { get; }
    public HashSet<string> ExcludeFolders { get; }
    public HashSet<string> ExcludeFileExtensions { get; }
    public bool UseDomainDrivenDesignOrder { get; }
    public string[] DomainDrivenDesignOrder { get; }

    public ScanConfiguration(
        string rootDirectory,
        string outputDirectory,
        IEnumerable<string> excludeFolders,
        IEnumerable<string> excludeFileExtensions,
        bool useDomainDrivenDesignOrder,
        IEnumerable<string> domainDrivenDesignOrder)
    {
        excludeFolders = excludeFolders
            ?? Enumerable.Empty<string>();

        excludeFileExtensions = excludeFileExtensions
            ?? Enumerable.Empty<string>();

        domainDrivenDesignOrder = domainDrivenDesignOrder
            ?? Enumerable.Empty<string>();

        RootDirectory = rootDirectory;
        OutputDirectory = outputDirectory;
        ExcludeFolders = excludeFolders.ToHashSet();
        ExcludeFileExtensions = excludeFileExtensions.ToHashSet();
        UseDomainDrivenDesignOrder = useDomainDrivenDesignOrder;
        DomainDrivenDesignOrder = domainDrivenDesignOrder.ToArray();
    }
}

