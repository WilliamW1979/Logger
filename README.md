# Logger

A lightweight, production-ready C# logging library for .NET 10+ applications. Supports synchronous and asynchronous logging, multiple severity levels, optional encryption, and clean resource disposal — with zero external dependencies.

---

## Features

- **Five log levels:** `Info`, `Warn`, `Error`, `Fatal`, `Debug`
- **Sync and async variants** for every log method — `Log(...)` and `LogAsync(...)`
- **Optional encryption** via a pluggable `IEncryptor` interface — bring your own implementation
- **Debug-only filtering** — `Debug` calls are silently no-ops in Release builds
- **Immediate flush** with `Flush()` or `FlushAsync()` for crash-safe logging
- **Proper disposal** — implements `IDisposable`; safe to use in `using` blocks
- **No external dependencies** — pure .NET 10, no NuGet packages required

---

## Requirements

- .NET 10 or later

---

## Installation

Clone or download the repository and add the `Logger` project as a reference in your solution, or build it as a DLL and reference the output directly.

```bash
git clone https://github.com/WilliamW1979/Logger.git
```

---

## Quick Start

```csharp
using var logger = new Logger("app.log");

logger.Info("Application started");
logger.Warn("Configuration value missing, using default");
logger.Error("Failed to connect to database");
logger.Fatal("Unrecoverable error — shutting down");
logger.Debug("Entering method ProcessQueue()");  // No-op in Release builds
```

---

## Async Usage

All methods have async counterparts for use in async/await workflows:

```csharp
await logger.InfoAsync("Server listening on port 8080");
await logger.WarnAsync("Retry attempt 2 of 3");
await logger.ErrorAsync("Timeout on external API call");
await logger.FatalAsync("Out of memory — terminating");
await logger.FlushAsync();
```

---

## Optional Encryption

Pass any `IEncryptor` implementation to encrypt log output at write time:

```csharp
IEncryptor encryptor = new MyAesEncryptor(key, iv);
using var logger = new Logger("secure.log", encryptor);

logger.Info("This line will be encrypted on disk");
```

Implement `IEncryptor` with any encryption strategy — AES, RSA, or a custom scheme.

---

## Flush and Disposal

```csharp
// Flush buffered writes immediately (useful before shutdown or crash reporting)
logger.Flush();
await logger.FlushAsync();

// Dispose when done — releases file handles cleanly
logger.Dispose();

// Or use 'using' for automatic disposal
using (var logger = new Logger("app.log"))
{
    logger.Info("This block disposes the logger automatically on exit");
}
```

---

## Design Notes

- The `IEncryptor` interface decouples encryption from logging — the logger has no opinion on your encryption algorithm.
- `Debug` log calls compile cleanly in Release mode and produce no output, with no runtime overhead.
- Async methods allow high-throughput applications to log without blocking the calling thread.

---

## License

Free to use for personal or commercial projects. Please credit **William Ward** if used. No warranty is provided. See [LICENSE.txt](LICENSE.txt) for full terms.

---

## Author

**William Ward** — [github.com/WilliamW1979](https://github.com/WilliamW1979)
