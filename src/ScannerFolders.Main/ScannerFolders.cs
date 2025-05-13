namespace ScannerFolders.Main;
internal class ScannerFolders
{
	/// <summary>
	///		Текущий путь (где происходит сканирование)
	/// </summary>
	private string _currentPath;

	/// <summary>
	///		Текущее наименование каталога
	/// </summary>
	private string _currentDirectoryName;

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
		_currentPath = _scanConfiguration.ScanDirectory;
		_currentNestingLevel = 0;

		var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd (HH-mm-ss.ffff)");
		var outputFilePath = Path.Combine(
			_scanConfiguration.OutputDirectory,
			$"scanFolders_{timestamp}.txt");

		using (_writer = new StreamWriter(outputFilePath, true)) 
		{
            var directories = _scanConfiguration.UseDomainDrivenDesignOrder
                ? GetDirectoriesOrderedByDomainDrivenDesign()
                : Directory.GetDirectories(_currentPath);

            foreach (var directory in directories) 
            {
                _currentPath = directory;
                RecursiveScanDirectory();
            }
        }
	}

	/// <summary>
	///		Получить каталоги в порядке, соответствующе стилю DDD
	/// </summary>
	private IEnumerable<string> GetDirectoriesOrderedByDomainDrivenDesign() 
	{
        var subDirectories = Directory.GetDirectories(_currentPath);
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
		_currentDirectoryName = Path.GetFileName(_currentPath);
		if (_scanConfiguration.ExcludeScanFolders.Contains(_currentDirectoryName)) 
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
		
		// запись наименования каталога в файл
        _writer.WriteLine($"{indentation}.{_currentDirectoryName}");
		
        var subDirectories = Directory.GetDirectories(_currentPath);
		if (subDirectories.Any()) 
		{
			var parentPath = _currentPath;
			_currentNestingLevel++;
			for (int i = 0; i < subDirectories.Length; i++) 
			{
				_currentPath = subDirectories[i];
				RecursiveScanDirectory();
			}
			_currentPath = parentPath;
			_currentNestingLevel--;
		}

		// файлы каталога
		var files = Directory.GetFiles(_currentPath);
		foreach (var file in files) 
		{
			var fileName = Path.GetFileName(file);
			var fileExtension = Path.GetExtension(file);

			if (_scanConfiguration.ExcludeScanFileExtensions.Contains(fileExtension)) 
			{
				continue;
			}

			// запись наименования файла в файл
			_writer.WriteLine($"{indentation + "\t"}{fileName}");
		}
	}

	/// <summary>
	///		Записать отступ между проектами
	/// </summary>
	private void WriteSpacingBeetwenProjects() 
	{
        var files = Directory.GetFiles(_currentPath, "*.csproj");
        if (files.Any()) 
        {
            _writer.WriteLine();
        }
    }
}

