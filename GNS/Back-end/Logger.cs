using System;
using System.IO;
using System.Windows.Forms;

public static class Logger
{
    // Logi można znaleźć w folderze \\AppData\Local\Stacja naziemna
    private static readonly string LogFolder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Stacja naziemna"
    );

    private static readonly string LogPath = Path.Combine(
        LogFolder,
        "startup_log.txt"
    );

    static Logger()
    {
        Directory.CreateDirectory(LogFolder);
    }

    public static void Log(string message)
    {
        try
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string logMessage = $"[{timestamp}] {message}\n";
            File.AppendAllText(LogPath, logMessage);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Critical logging error: {ex.Message}");
        }
    }

    public static void LogException(string context, Exception ex)
    {
        Log($"ERROR in {context}: {ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
    }
}