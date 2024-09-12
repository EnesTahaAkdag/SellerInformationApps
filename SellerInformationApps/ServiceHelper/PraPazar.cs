using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;

namespace PraPazar.ServiceHelper
{
	public static class HttpClientFactory
	{
		private static readonly ConcurrentDictionary<string, HttpClient> cache = new ConcurrentDictionary<string, HttpClient>();

		private static readonly Lazy<HttpClientHandler> handler = new Lazy<HttpClientHandler>(CreateHandler);

		public static HttpClient Create(string endpoint)
		{
			return cache.GetOrAdd(endpoint, (key) =>
			{
				return new HttpClient(handler.Value, disposeHandler: false)
				{
					BaseAddress = new Uri(endpoint),
					Timeout = TimeSpan.FromSeconds(10),
				};
			});
		}

		private static HttpClientHandler CreateHandler()
		{
			var handler = new HttpClientHandler
			{
				UseCookies = false
			};

			handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
			{
				if (cert.Issuer.Equals("CN=localhost", StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}

				return errors == System.Net.Security.SslPolicyErrors.None;
			};

			return handler;
		}
	}
}
