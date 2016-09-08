using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using AppServiceHelpers;
using AppServiceHelpers.Abstractions;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Spent
{
	public partial class App : Application
	{
		public static IEasyMobileServiceClient AzureClient { get; set; }

		public App()
		{
			InitializeComponent();

			// Initialize App Service Helpers
			AzureClient = EasyMobileServiceClient.Create();
			AzureClient.Initialize("http://spendapplab.azurewebsites.net/");
			AzureClient.RegisterTable<Expense>();
			AzureClient.FinalizeSchema();

			MainPage = new NavigationPage(new ExpensesPage())
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