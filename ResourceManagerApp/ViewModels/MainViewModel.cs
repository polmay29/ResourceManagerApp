using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace ResourceManagerApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private System.Threading.Timer _resourceUpdateTimer;  // Используем Timer из System.Threading

        private double _cpuUsage;
        private double _memoryUsage;
        private string _currentDirectory;

        public ObservableCollection<FileSystemEntry> FileSystemEntries { get; private set; } = new ObservableCollection<FileSystemEntry>();

        public double CPUUsage
        {
            get => _cpuUsage;
            set => SetProperty(ref _cpuUsage, value);
        }

        public double MemoryUsage
        {
            get => _memoryUsage;
            set => SetProperty(ref _memoryUsage, value);
        }

        public string CurrentDirectory
        {
            get => _currentDirectory;
            set
            {
                if (SetProperty(ref _currentDirectory, value))
                {
                    LoadFileSystemEntries(_currentDirectory);
                }
            }
        }

        public MainViewModel()
        {
            // Создаем новый таймер, который будет обновлять ресурсы каждую секунду
            _resourceUpdateTimer = new System.Threading.Timer(UpdateResourceUsage, null, 0, 1000);

            // Установим начальный каталог
            CurrentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        // Этот метод вызывается каждую секунду таймером
        private void UpdateResourceUsage(object state)
        {
            try
            {
                // Рассчитываем использование ЦП (приблизительно)
                var cpuUsage = Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds / Environment.ProcessorCount;

                // Получаем использование памяти в мегабайтах
                var memoryUsage = Environment.WorkingSet / 1024.0 / 1024.0;

                // Обновляем данные о CPU и памяти
                CPUUsage = Math.Round(cpuUsage, 2);
                MemoryUsage = Math.Round(memoryUsage, 2);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating resource usage: {ex.Message}");
            }
        }

        public void LoadFileSystemEntries(string path)
        {
            FileSystemEntries.Clear();

            try
            {
                // Получаем список директорий
                var directories = Directory.GetDirectories(path)
                    .Select(d => new FileSystemEntry
                    {
                        Name = Path.GetFileName(d),
                        Size = 0,
                        LastModified = Directory.GetLastWriteTime(d),
                        IsDirectory = true
                    });

                // Получаем список файлов
                var files = Directory.GetFiles(path)
                    .Select(f => new FileSystemEntry
                    {
                        Name = Path.GetFileName(f),
                        Size = new FileInfo(f).Length,
                        LastModified = File.GetLastWriteTime(f),
                        IsDirectory = false
                    });

                // Объединяем файлы и папки и добавляем в коллекцию
                foreach (var entry in directories.Concat(files))
                    FileSystemEntries.Add(entry);
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.WriteLine($"Access denied to directory: {ex.Message}");
                FileSystemEntries.Add(new FileSystemEntry
                {
                    Name = "[Access Denied]",
                    Size = 0,
                    LastModified = DateTime.Now,
                    IsDirectory = true
                });
            }
            catch (DirectoryNotFoundException ex)
            {
                Debug.WriteLine($"Directory not found: {ex.Message}");
                FileSystemEntries.Add(new FileSystemEntry
                {
                    Name = "[Directory Not Found]",
                    Size = 0,
                    LastModified = DateTime.Now,
                    IsDirectory = true
                });
            }
            catch (IOException ex)
            {
                Debug.WriteLine($"I/O error: {ex.Message}");
                FileSystemEntries.Add(new FileSystemEntry
                {
                    Name = "[I/O Error]",
                    Size = 0,
                    LastModified = DateTime.Now,
                    IsDirectory = true
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error: {ex.Message}");
                FileSystemEntries.Add(new FileSystemEntry
                {
                    Name = "[Unexpected Error]",
                    Size = 0,
                    LastModified = DateTime.Now,
                    IsDirectory = true
                });
            }
        }

        public void CreateFile(string fileName)
        {
            try
            {
                var filePath = Path.Combine(CurrentDirectory, fileName);
                File.Create(filePath).Dispose();
                LoadFileSystemEntries(CurrentDirectory);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating file: {ex.Message}");
            }
        }

        public void DeleteFile(string fileName)
        {
            try
            {
                var filePath = Path.Combine(CurrentDirectory, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    LoadFileSystemEntries(CurrentDirectory);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting file: {ex.Message}");
            }
        }

        public void RenameFile(string oldFileName, string newFileName)
        {
            try
            {
                var oldFilePath = Path.Combine(CurrentDirectory, oldFileName);
                var newFilePath = Path.Combine(CurrentDirectory, newFileName);
                if (File.Exists(oldFilePath))
                {
                    File.Move(oldFilePath, newFilePath);
                    LoadFileSystemEntries(CurrentDirectory);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error renaming file: {ex.Message}");
            }
        }
    }

    public class FileSystemEntry
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public bool IsDirectory { get; set; }
    }
}
