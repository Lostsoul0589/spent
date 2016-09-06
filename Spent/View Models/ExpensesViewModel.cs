using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Spent
{
	public class ExpensesViewModel : BaseViewModel
	{
		public ObservableCollection<Expense> Expenses { get; set; }
		public Command GetExpensesCommand { get; set; }

		public ExpensesViewModel()
		{
			Expenses = new ObservableCollection<Expense>();
			GetExpensesCommand = new Command(
				async ()=> await GetExpensesAsync(), () => !IsBusy);

			GetExpensesAsync();
		}

		private async Task GetExpensesAsync()
		{
			if (IsBusy)
				return;

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
	}
}
