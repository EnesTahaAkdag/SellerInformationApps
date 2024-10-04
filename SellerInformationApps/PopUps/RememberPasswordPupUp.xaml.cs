using CommunityToolkit.Maui.Views;

namespace SellerInformationApps.PopUps
{
	public partial class RememberPasswordPupUp : Popup
	{
		public RememberPasswordPupUp()
		{
			InitializeComponent();
		}
		private void SubmitButton(object sender,EventArgs e)
		{

		}

		private  void ClosePopUpButton(object sender, EventArgs e)
		{
			Close();
		}
	}
}
