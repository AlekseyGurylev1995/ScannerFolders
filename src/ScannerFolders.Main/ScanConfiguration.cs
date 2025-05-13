namespace ScannerFolders.Main;
internal class ScanConfiguration : IScanConfiguration
{
    public string ScanDirectory { get; }
    public HashSet<string> ExcludeScanFolders { get; }
    public HashSet<string> ExcludeScanFileExtensions { get; }
    public string OutputDirectory { get; }
    public bool UseSpaceBeetwenProjects { get; }
    public bool UseDomainDrivenDesignOrder { get; }
    public string[] DomainDrivenDesignOrder { get; }

    public ScanConfiguration(
        string scanDirectory,
        IEnumerable<string> excludeScanFolders,
        IEnumerable<string> excludeScanFileExtensions,
        string outputDirectory,
        bool useSpaceBeetwenProjects,
        bool useDomainDrivenDesignOrder,
        IEnumerable<string> domainDrivenDesignOrder)
    {
        excludeScanFolders = excludeScanFolders
            ?? Enumerable.Empty<string>();

        excludeScanFileExtensions = excludeScanFileExtensions
            ?? Enumerable.Empty<string>();

        domainDrivenDesignOrder = domainDrivenDesignOrder
            ?? Enumerable.Empty<string>();

        ScanDirectory = scanDirectory;
        ExcludeScanFolders = excludeScanFolders.ToHashSet();
        ExcludeScanFileExtensions = excludeScanFileExtensions.ToHashSet();

        OutputDirectory = outputDirectory;
        UseSpaceBeetwenProjects = useSpaceBeetwenProjects;
        UseDomainDrivenDesignOrder = useDomainDrivenDesignOrder;
        DomainDrivenDesignOrder = domainDrivenDesignOrder.ToArray();
    }
}

