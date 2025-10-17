using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using StudentRegisterationSystem.Client;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddScoped(sp =>
{
	var js = sp.GetRequiredService<IJSRuntime>();

	// Create a handler that attaches JWT token
	var handler = new HttpClientHandler(); // default handler

	var client = new HttpClient(new JwtHandler(js, handler))
	{
		BaseAddress = new Uri("https://localhost:7179/")
	};

	return client;
});

// Build and run
await builder.Build().RunAsync();
public class JwtHandler : DelegatingHandler
{
	private readonly IJSRuntime _js;

	public JwtHandler(IJSRuntime js, HttpMessageHandler innerHandler) : base(innerHandler)
	{
		_js = js;
	}

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		// Read token from local storage
		var token = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");
		if (!string.IsNullOrEmpty(token))
		{
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
		}

		return await base.SendAsync(request, cancellationToken);
	}
}
