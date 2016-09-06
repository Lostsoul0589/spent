using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace Spent
{
	public class ExpensesViewModel : BaseViewModel
	{
		public ObservableCollection<Expense> Expenses { get; set; }

		public Command GetExpensesCommand { get; set; }
		public Command AddExpenseCommand { get; set; }

		public ExpensesViewModel()
		{
			Expenses = new ObservableCollection<Expense>();
			GetExpensesCommand = new Command(
				async () => await GetExpensesAsync(), () => !IsBusy);
			AddExpenseCommand = new Command(
				async () => await AddExpenseAsync(), () => !IsBusy);

			GetExpensesAsync();
		}

		async Task GetExpensesAsync()
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try
			{
				Expenses.Clear();
				Expenses.Add(new Expense
				{
					Company = "Target",
					Amount = 25.00
				});

				Expenses.Add(new Expense
				{
					Company = "Walmart",
					Amount = 99.00
				});
			}
			catch (Exception ex)
			{
				MessagingCenter.Send<ExpensesViewModel, string>(this, "Error", ex.Message);
			}
			finally
			{
				IsBusy = false;
			}
		}

		async Task AddExpenseAsync()
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try
			{
				await CrossMedia.Current.Initialize();

				MediaFile photo;
				if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
				{
					photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
					{
						Directory = "Expenses",
						Name = "expense.jpg"
					});
				}
				else
				{
					photo = await CrossMedia.Current.PickPhotoAsync();
				}

				Expenses.Add(new Expense
				{
					Company = "Test",
					Description = "Test description",
					Receipt = photo.Path
				});
			}
			catch (Exception ex)
			{
				MessagingCenter.Send<ExpensesViewModel, string>(this, "Error", ex.Message);
			}
			finally
			{
				IsBusy = false;
			}
		}
	}
}