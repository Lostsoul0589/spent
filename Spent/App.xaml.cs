using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using AppServiceHelpers;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Spent
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			// Initialize App Service Helpers
			EasyMobileServiceClient.Current.Initialize("http://spendapplab.azurewebsites.net/");
			EasyMobileServiceClient.Current.RegisterTable<Expense>();
			EasyMobileServiceClient.Current.FinalizeSchema();

			MainPage = new NavigationPage(new NewExpensePage())
			{
				BarBackgroundColor = (Color)Resources["Primary"],
				BarTextColor = Color.White
			};
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}