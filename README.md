# Logger DLL

Logger is a C# DLL for asynchronous and synchronous logging to files, with optional encryption. Works in both Debug and Release modes. Requires .NET 10+.

Features:
- Log information with `Info("message")` or `InfoAsync("message")`
- Log warnings with `Warn("message")` or `WarnAsync("message")`
- Log errors with `Error("message")` or `ErrorAsync("message")`
- Log fatal messages with `Fatal("message")` or `FatalAsync("message")`
- Log debug messages (only if main program is Debug) with `Debug("message")` or `DebugAsync("message")`
- Optional encryption using an `IEncryptor` implementation
- Flush logs immediately with `Flush()` or `FlushAsync()`
- Proper disposal with `Dispose()`

License / Credit: Free to use for personal or commercial projects. Please credit **William Ward** if used. No warranty is provided.
