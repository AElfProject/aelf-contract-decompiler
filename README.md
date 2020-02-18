# aelf-contract-decompiler
## Introduction

This tool is a C-Sharp decompiler for [Smart Contracts of AElf](https://docs.aelf.io/v/dev/main-2/architecture). It is based on the open-source [ILSpy](https://github.com/icsharpcode/ILSpy) and only works properly on windows platform when compiling contracts in AElf.

### Getting Started

The project uses Asp.Net Core Mvc template to serve. Default endpoint has been set to 5566 and you can run the AElfContractDecompiler listening on the specified endpoint.

``` c#
private static IHostBuilder CreateHostBuilder(string[] args) =>
Host.CreateDefaultBuilder(args)
.ConfigureLogging(builder =>
	{
		builder.ClearProviders();
		builder.AddConsole();
		builder.SetMinimumLevel(LogLevel.Trace);
	})
	.ConfigureWebHostDefaults(webBuilder =>
	{
		webBuilder.UseStartup<Startup>();
		webBuilder.UseUrls("http://*:5566");
  })
	.UseAutofac();
```

Then you can send a post request like http://*:5566/getfiles with necessary data in body. The value of Base64String is the base64String converted by a contract dll.

``` json
{
 "Base64String":"TVqQAAMAAAAEAAAA//8AALgAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAA4fug4AtAnNIbgBTM0hVGhpcyBwcm9ncmFtIGNhbm5vdCBiZSBydW4gaW4gRE9TIG1vZGUuDQ0KJAAAAA...."
}
```

### Interface

Main interfaces of this decompiler are shown below. 

#### IContractDecompileService

It is used to call decompling service of ILSpy.

```c#
Task ExecuteDecompileAsync(string[] args);
```

#### IFileParserService

It is used to show the hierarchical structure of the decompiled contract.

```c#
Task<ResponseTemplate> GetResponseTemplateByPath(string dictPath);
```