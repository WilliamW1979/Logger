using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading;

public interface IEncryptor
{
    byte[] EncryptString(string Data, string Key);
}

public class Logger : IDisposable
{
    private readonly string FilePath;
    private readonly string EncryptionKey;
    private readonly IEncryptor? Encryptor;
    private static readonly bool IsDebug = CheckIfMainProgramDebug();
    private readonly ConcurrentQueue<string> LogQueue = new();
    private readonly Task WorkerTask;
    private readonly CancellationTokenSource Cts = new();
    private readonly SemaphoreSlim FileSemaphore = new(1, 1);

    public Logger(string Path, string Key = "", IEncryptor? Encryptor = null)
    {
        FilePath = Path;
        EncryptionKey = Key;
        this.Encryptor = Encryptor;
        string DirectoryPath = System.IO.Path.GetDirectoryName(FilePath) ?? "";
        if (!string.IsNullOrEmpty(DirectoryPath) && !Directory.Exists(DirectoryPath))
            Directory.CreateDirectory(DirectoryPath);
        WorkerTask = Task.Run(ProcessQueueAsync);
    }

    private static bool CheckIfMainProgramDebug()
    {
        Assembly? main = Assembly.GetEntryAssembly();
        if (main == null) return false;
        DebuggableAttribute? da = main.GetCustomAttribute<DebuggableAttribute>();
        return da != null && da.IsJITTrackingEnabled && da.IsJITOptimizerDisabled;
    }

    public Task InfoAsync(string Message) => EnqueueAsync("[INFO] " + Message);
    public Task InformationAsync(string Message) => InfoAsync(Message);
    public Task WarnAsync(string Message) => EnqueueAsync("[WARN] " + Message);
    public Task WarningAsync(string Message) => WarnAsync(Message);
    public Task ErrorAsync(string Message) => EnqueueAsync("[ERROR] " + Message);
    public Task FatalAsync(string Message) => EnqueueAsync("[FATAL] " + Message);
    public Task DebugAsync(string Message) => IsDebug ? EnqueueAsync("[DEBUG] " + Message) : Task.CompletedTask;
    public void Info(string Message) => Enqueue("[INFO] " + Message);
    public void Information(string Message) => Info(Message);
    public void Warn(string Message) => Enqueue("[WARN] " + Message);
    public void Warning(string Message) => Warn(Message);
    public void Error(string Message) => Enqueue("[ERROR] " + Message);
    public void Fatal(string Message) => Enqueue("[FATAL] " + Message);
    public void Debug(string Message) { if (IsDebug) Enqueue("[DEBUG] " + Message); }

    private Task EnqueueAsync(string Message)
    {
        LogQueue.Enqueue(Message);
        return Task.CompletedTask;
    }

    private void Enqueue(string Message) => LogQueue.Enqueue(Message);

    private async Task ProcessQueueAsync()
    {
        while (!Cts.IsCancellationRequested)
        {
            while (LogQueue.TryDequeue(out string? message))
            {
                string Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                string FullMessage = $"[{Timestamp}] {message}{Environment.NewLine}";
                try
                {
                    await FileSemaphore.WaitAsync(Cts.Token);
                    if (Encryptor != null)
                    {
                        byte[] EncryptedData = Encryptor.EncryptString(FullMessage, EncryptionKey);
                        await using FileStream fs = new(FilePath, FileMode.Append, FileAccess.Write, FileShare.Read, 4096, true);
                        await fs.WriteAsync(EncryptedData, 0, EncryptedData.Length, Cts.Token);
                    }
                    else
                        await File.AppendAllTextAsync(FilePath, FullMessage, Encoding.UTF8, Cts.Token);
                }
                catch { }
                finally { FileSemaphore.Release(); }
            }
            await Task.Delay(10, Cts.Token).ContinueWith(_ => { });
        }
    }

    public async Task FlushAsync()
    {
        while (!LogQueue.IsEmpty)
            await Task.Delay(5);
        await FileSemaphore.WaitAsync();
        FileSemaphore.Release();
    }

    public void Flush()
    {
        while (!LogQueue.IsEmpty)
            Thread.Sleep(5);
        FileSemaphore.Wait();
        FileSemaphore.Release();
    }

    public void Dispose()
    {
        Cts.Cancel();
        Flush();
        WorkerTask.Wait();
        Cts.Dispose();
        FileSemaphore.Dispose();
    }
}