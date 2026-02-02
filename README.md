# Logger Wrapper

This is a Logger wrapper that is free to use but I do want credit when it is used.

The logger does have an interface for adding Encryption. I have an Encryption wrapper that I like to use with it in case I am logging sensative material on a project (user names or information) and the information must be logged. It is not required to work though.
It can handle both Async and Sync logging.
Debug will only write if your project is in DEBUG mode. Release mode will skip over Debug messages so that you are not forced to go back and delete all of them.
It has a Flush feature so that if your program crashes for some reason, the flush forces the messages to finish writting before disposing and cleaning up the logger.

The DLL that compiles is usable in DEBUG and RELEASE modes.

## Disclaimor
DLLs are VERY DANGEROUS to download. I will always recommend that you download the source code and compile it yourself so that you know for sure you are getting a clean DLL that someone didn't tamper with. A lot of web sites that will allow you to download free games to play that usually cost money, they will sometimes try to add code to the DLLs to hide their true intentions on giving you the game for free. Methods like these are very common and Microsoft Defender / Virus Protections generally won't catch them until they are well known. Injecting bad code into DLLs is actually very easy to do and something you should be aware that people do. That is why I will always recommend that you NEVER use a DLL straight from the internet unless it comes from a source you trust 100%. With that said, I did post the DLL that compiled with the very code you see here in this github. If you follow my instructions and compile your own, you will see that your DLL will be identical to mine. That is how you will know mine is clean. Any added code will add to the size and change the CHECKSUM of the file. Use these methods in the future to tell if people are being honest with you. It is always better to be safe then sorry!

# Example Code
```
Logger Log = new Logger("log.txt");

// Sync logging
Log.Info("Starting program");
Log.Debug("Debug info");

// Async logging
Log.InfoAsync("Starting program");
Log.DebugAsync("Debug info");

// Before exiting
Log.Dispose();
```
