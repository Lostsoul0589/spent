using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using AppServiceHelpers;
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
				() => AddExpense(), () => !IsBusy);

			GetExpensesAsync();
		}

		Expense selectedExpenseItem;
		public Expense SelectedExpenseItem
		{
			get { return selectedExpenseItem; }
			set
			{
				selectedExpenseItem = value;
				OnPropertyChanged();

				if (selectedExpenseItem != null)
				{
					MessagingCenter.Send(this, "NavigateToDetail", SelectedExpenseItem);
					SelectedExpenseItem = null;
				}
			}
		}

		async Task GetExpensesAsync()
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try
			{
				Expenses.Clear();

				var expenses = await EasyMobileServiceClient.Current.Table<Expense>().GetItemsAsync();
				foreach (var expense in expenses)
				{
					Expenses.Add(expense);
				}
			}
			catch (Exception ex)
			{
				MessagingCenter.Send(this, "Error", ex.Message);
			}
			finally
			{
				IsBusy = false;
			}
		}

		void AddExpense()
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try
			{
				MessagingCenter.Send(this, "Navigate", "NewExpensePage");
			}
			catch (Exception ex)
			{
				MessagingCenter.Send(this, "Error", ex.Message);
			}
			finally
			{
				IsBusy = false;
			}
		}
	}
}