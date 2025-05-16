namespace ScannerFolders.Main;
internal class ScannerFolders
{
	/// <summary>
	///		Текущий путь сканируемого каталога
	/// </summary>
	private string _currentScanDirectoryPath;

	/// <summary>
	///		Текущее наименование сканируемого каталога
	/// </summary>
	private string _currentScanDirectoryName;

	/// <summary>
	///		Текущий уровень вложенности (относительно корня)
	/// </summary>
	private int _currentNestingLevel;

    /// <summary>
    ///		Файл открывается единоразово до закрытия ->
	///		файл держится в потоке -> n циклов записей нужной инфы -> файл закрыватся единоразово по окончанию работы
    ///			* в отличие от использования File.WriteAllLines()
	///				который на каждый вызов будет открывать поток -> писать -> закрывать 
    /// </summary>
    private StreamWriter _writer;

    private readonly IScanConfiguration _scanConfiguration;

    public ScannerFolders(IScanConfiguration scanConfiguration)
	{
		_scanConfiguration = scanConfiguration;
	}

	/// <summary>
	///		Запустить сканирование
	/// </summary>
	public void Run()
	{
		_currentScanDirectoryPath = _scanConfiguration.ScanDirectory;
		_currentNestingLevel = 0;

		var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd (HH-mm-ss.ffff)");
		var outputFilePath = Path.Combine(
			_scanConfiguration.OutputDirectory,
			$"scanFolders_{timestamp}.txt");

		using (_writer = new StreamWriter(outputFilePath, true)) 
		{
            var directories = _scanConfiguration.UseDomainDrivenDesignOrder
                ? GetDirectoriesOrderedByDomainDrivenDesign()
                : Directory.GetDirectories(_currentScanDirectoryPath);

            foreach (var directory in directories) 
            {
                _currentScanDirectoryPath = directory;
                RecursiveScanDirectory();
            }
        }
	}

	/// <summary>
	///		Получить каталоги в порядке, соответствующе стилю DDD
	/// </summary>
	private IEnumerable<string> GetDirectoriesOrderedByDomainDrivenDesign() 
	{
        var subDirectories = Directory.GetDirectories(_currentScanDirectoryPath);
        var orderedDirectories = new List<string>();

		foreach (var findDirectory in _scanConfiguration.DomainDrivenDesignOrder) 
		{
            var foundDirectoryPath = subDirectories
				.FirstOrDefault(dir => Path.GetFileName(dir).EndsWith('.' + findDirectory));

            if (foundDirectoryPath == default)
            {
				continue;
            }

			orderedDirectories.Add(foundDirectoryPath);
        }

        if (orderedDirectories.Any() == false) 
        {
            throw new Exception(
                "Ни одна из дочерних папок (относительно корня) не соответствует стилю DDD");
        }

		return orderedDirectories;
    }
	
	/// <summary>
	///		Сканировать каталог (рекурсивно)		
	/// </summary>
	private void RecursiveScanDirectory()
	{
		_currentScanDirectoryName = Path.GetFileName(_currentScanDirectoryPath);
		if (_scanConfiguration.ExcludeScanFolders.Contains(_currentScanDirectoryName)) 
		{
            return;
		}

        if (_scanConfiguration.UseSpaceBeetwenProjects) 
        {
            WriteSpacingBeetwenProjects();
        }
       
        // генерируем n количество физических отступов табуляции
        //		в зависимости от уровня вложенности
        var indentation = new string('\t', _currentNestingLevel);
		
		// записать наименования каталога
        _writer.WriteLine($"{indentation}.{_currentScanDirectoryName}");
		
        var subDirectories = Directory.GetDirectories(_currentScanDirectoryPath);
		if (subDirectories.Any()) 
		{
			var parentPath = _currentScanDirectoryPath;
			_currentNestingLevel++;
			for (int i = 0; i < subDirectories.Length; i++) 
			{
				_currentScanDirectoryPath = subDirectories[i];
				RecursiveScanDirectory();
			}
			_currentScanDirectoryPath = parentPath;
			_currentNestingLevel--;
		}

		// файлы каталога
		var files = Directory.GetFiles(_currentScanDirectoryPath);
		foreach (var file in files) 
		{
			var fileName = Path.GetFileName(file);
			var fileExtension = Path.GetExtension(file);

			if (_scanConfiguration.ExcludeScanFileExtensions.Contains(fileExtension)) 
			{
				continue;
			}

			// записать наименования файла
			_writer.WriteLine($"{indentation + "\t"}{fileName}");
		}
	}

	/// <summary>
	///		Записать отступ между проектами
	/// </summary>
	private void WriteSpacingBeetwenProjects() 
	{
        var files = Directory.GetFiles(_currentScanDirectoryPath, "*.csproj");
        if (files.Any()) 
        {
            _writer.WriteLine();
        }
    }
}

