namespace ScannerFolders.Main;
internal interface IScanConfiguration
{
    /// <summary>
    ///     Корневой каталог сканирования
    /// </summary>
    string RootDirectory { get; }

    /// <summary>
    ///     Выходной каталог для сохранения результата сканирования
    /// </summary>
    string OutputDirectory { get; }

    /// <summary>
    ///     Набор папок исключаемых из сканирования
    /// </summary>
    HashSet<string> ExcludeFolders { get; }

    /// <summary>
    ///     Набор расширений файлов исключаемых из сканирования
    /// </summary>
    HashSet<string> ExcludeFileExtensions { get; }

    /// <summary>
    ///     Признак необходимости сортировки в порядке соответствующем стилю DDD
    /// </summary>
    bool UseDomainDrivenDesignOrder { get; }

    /// <summary>
    ///     Порядок слоёв для сортировки структуры соответствующему стилю DDD
    /// </summary>
    string[] DomainDrivenDesignOrder { get; }
}

