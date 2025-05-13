namespace ScannerFolders.Main;
internal interface IScanConfiguration
{
    /// <summary>
    ///     Каталог сканирования
    /// </summary>
    string ScanDirectory { get; }

    /// <summary>
    ///     Набор папок исключаемых из сканирования
    /// </summary>
    HashSet<string> ExcludeScanFolders { get; }

    /// <summary>
    ///     Набор расширений файлов исключаемых из сканирования
    /// </summary>
    HashSet<string> ExcludeScanFileExtensions { get; }

    /// <summary>
    ///     Выходной каталог для сохранения результата сканирования
    /// </summary>
    string OutputDirectory { get; }

    /// <summary>
    ///     Признак необходимости использовать отступ между проектами
    /// </summary>
    bool UseSpaceBeetwenProjects { get; }

    /// <summary>
    ///     Признак необходимости сортировки в порядке соответствующем стилю DDD
    /// </summary>
    bool UseDomainDrivenDesignOrder { get; }

    /// <summary>
    ///     Порядок слоёв для сортировки структуры соответствующему стилю DDD
    /// </summary>
    string[] DomainDrivenDesignOrder { get; }
}

