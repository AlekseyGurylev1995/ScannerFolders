namespace ScannerFolders.Main;
internal class ScannerFolders
{
	private readonly IScanConfiguration _scanConfiguration;

	/// <summary>
	///		Текущий путь (где проходит сканирование)
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

	public ScannerFolders(IScanConfiguration scanConfiguration)
	{
		_scanConfiguration = scanConfiguration;
	}

	/// <summary>
	///		Запустить сканирование
	/// </summary>
	public void Run()
	{
		_currentPath = _scanConfiguration.RootDirectory;
		_currentNestingLevel = 0;

		var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd (HH-mm-ss.ffff)");
		var outputFilePath = Path.Combine(
			_scanConfiguration.OutputDirectory,
			$"scanFolders_{timestamp}.txt");

		using (_writer = new StreamWriter(outputFilePath, true)) 
		{
            if (_scanConfiguration.UseDomainDrivenDesignOrder)
            {
                var directoriesOrdered = GetDirectoriesOrderedByDomainDrivenDesign();
                foreach (var directory in directoriesOrdered)
                {
                    _currentPath = directory;
                    RecursiveScanFolder();
                }

                return;
            }

            RecursiveScanFolder();
        }
	}

	/// <summary>
	///		Получить каталоги в порядке, соответствующем DDD
	/// </summary>
	private IEnumerable<string> GetDirectoriesOrderedByDomainDrivenDesign() 
	{
        var subDirectories = Directory.GetDirectories(_currentPath);
        var orderedDirectories = new List<string>(subDirectories.Length);

		foreach (var findDirectory in _scanConfiguration.DomainDrivenDesignOrder) 
		{
            var foundDirectoryPath = subDirectories
				.FirstOrDefault(dir => Path.GetFileName(dir).EndsWith('.' + findDirectory));

            if (foundDirectoryPath == default)
            {
                throw new Exception("Дочерние папки не соответствует формату Domain Driven Design");
            }

			orderedDirectories.Add(foundDirectoryPath);
        }

		return orderedDirectories;
    }
	
	/// <summary>
	///		Сканировать каталог (рекурсивно)		
	/// </summary>
	private void RecursiveScanFolder()
	{
		// отступ между проектами
		WriteSpacingBeetwenProjects();

		_currentDirectoryName = Path.GetFileName(_currentPath);
		if (_scanConfiguration.ExcludeFolders.Contains(_currentDirectoryName)) 
		{
            return;
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
				RecursiveScanFolder();
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

			if (_scanConfiguration.ExcludeFileExtensions.Contains(fileExtension)) 
			{
				continue;
			}

			// запись наименования файла в файл
			_writer.WriteLine($"{indentation + "\t"}{fileName}");
		}
	}

	/// <summary>
	///		Отступ между проектами
	/// </summary>
	private void WriteSpacingBeetwenProjects() 
	{
        // в случае вкл. сортировки директорий в стиле DDD
		// проекты будут на уровне вложенности 0
        if (_scanConfiguration.UseDomainDrivenDesignOrder)
        {
            if (_currentNestingLevel == 0)
            {
                _writer.WriteLine();
            }
        }
        else if (_currentNestingLevel == 1)
        {
            _writer.WriteLine();
        }
    }
}

