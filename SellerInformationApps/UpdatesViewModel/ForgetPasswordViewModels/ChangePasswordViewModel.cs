﻿using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using ServiceHelper.Authentication;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Core;
using ServiceHelper.Alerts;

namespace SellerInformationApps.UpdatesViewModel.ForgetPasswordViewModels
{
	public partial class ChangePasswordViewModel : Authentication
	{
		public AlertsHelper alertsHelper = new AlertsHelper();


		private readonly Popup _popup;

		[ObservableProperty]
		private string password;

		[ObservableProperty]
		private string verifyPassword;

		private readonly string _userName = Preferences.Get("UserName", string.Empty);

		private static readonly HttpClient httpClient = HttpClientFactory.Create("https://59b7-37-130-115-91.ngrok-free.app");

		public IRelayCommand UpdatePasswordCommand { get; }
		public IRelayCommand CancelCommand { get; }

		public ChangePasswordViewModel(Popup popup)
		{
			_popup = popup;
			UpdatePasswordCommand = new AsyncRelayCommand(SubmitCommandAsync);
			CancelCommand = new RelayCommand(ClosePopup);
		}

		private void ClosePopup()
		{
			_popup?.Close();
		}

		[RelayCommand]
		private async Task SubmitCommandAsync()
		{
			if (!IsFormValid())
			{
				await alertsHelper.ShowSnackBar("Lütfen tüm alanları doldurduğunuzdan emin olun.", true);
				return;
			}

			if (string.IsNullOrEmpty(_userName))
			{
				await alertsHelper.ShowSnackBar("Kullanıcı adı boş olamaz.", true);
				return;
			}

			try
			{
				var changePasswordModel = CreateNewPasswordModel();
				if (changePasswordModel == null)
				{
					await alertsHelper.ShowSnackBar("Kullanıcı adı boş olamaz.", true);
					return;
				}

				string url = "https://59b7-37-130-115-91.ngrok-free.app/RegisterAndLoginApi/ChangePassword";
				var content = new StringContent(JsonConvert.SerializeObject(changePasswordModel), Encoding.UTF8, "application/json");

				using (var response = await httpClient.PostAsync(url, content).ConfigureAwait(false))
				{
					string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
					var apiResponse = JsonConvert.DeserializeObject<ChancePasswordApiResponse>(json);

					if (response.IsSuccessStatusCode && apiResponse.Success)
					{
						await alertsHelper.ShowSnackBar("Şifreniz başarıyla değiştirildi. Lütfen giriş yapın.");
						Preferences.Remove("UserName");
						ClosePopup();
					}
					else
					{
						await alertsHelper.ShowSnackBar(apiResponse?.ErrorMessage ?? "Bilinmeyen bir hata oluştu.", true);
					}
				}
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"Bir hata oluştu: {ex.Message}", true);
			}
		}

		private bool IsFormValid()
		{
			return !string.IsNullOrEmpty(Password) && Password == VerifyPassword;
		}

		private ChancePasswordModel CreateNewPasswordModel()
		{
			return new ChancePasswordModel
			{
				UserName = _userName,
				Password = Password
			};
		}
	}
}
