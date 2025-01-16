using MarkdownProcessorWasm;
using MdPWASM;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
using MarkdownProcessor;
using MarkdownProcessor.Classes;
using MarkdownProcessor.Interfaces;
using MarkdownProcessor.Structs.Tags;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var host = builder.Build();

var jsRuntime = host.Services.GetRequiredService<IJSRuntime>();
await jsRuntime.InvokeVoidAsync("console.log", "Blazor module is loading...");

var jsInterop = new MarkdownProcessorInterop();

// Регистрация метода для вызова из JS
await jsRuntime.InvokeVoidAsync("blazorInterop.registerProcessor", DotNetObjectReference.Create(jsInterop));

// Вызов RunAsync
await host.RunAsync();