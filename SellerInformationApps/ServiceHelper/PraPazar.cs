﻿using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;

namespace PraPazar.ServiceHelper
{
	public static class HttpClientFactory
	{
		private static ConcurrentDictionary<string, HttpClient> cache = new ConcurrentDictionary<string, HttpClient>();

		public static HttpClient Create(string endpoint)
		{

			var handler = ClientHandler();

			HttpClient client = cache.GetOrAdd(endpoint,
				new HttpClient(handler, false)
				{
					BaseAddress = new Uri(endpoint),
					Timeout = TimeSpan.FromSeconds(10),
				}
			);
			return client;
		}
		public static HttpClientHandler ClientHandler()
		{
			HttpClientHandler handler = new HttpClientHandler() { UseCookies = false };
			handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
			{
				if (cert.Issuer.Equals("CN=localhost"))
					return true;
				return errors == System.Net.Security.SslPolicyErrors.None;
			};
			return handler;
		}
	}
}
