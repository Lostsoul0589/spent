using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Spent
{
	public partial class ExpensesPage : ContentPage
	{
		public ExpensesPage()
		{
			InitializeComponent();

			BindingContext = new ExpensesViewModel();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			SubscribeToMessages();
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			UnsubscribeFromMessages();
		}

		void SubscribeToMessages()
		{
			MessagingCenter.Subscribe<ExpensesViewModel, string>(this, "Error", (obj, s) =>
			{
				DisplayAlert("Error", s, "OK");
			});

			MessagingCenter.Subscribe<ExpensesViewModel, string>(this, "Navigate", async (obj, s) =>
			{
				if (s == "NewExpensePage")
				{
					await Navigation.PushAsync(new NewExpensePage());
				}
			});

			MessagingCenter.Subscribe<ExpensesViewModel, Expense>(this, "NavigateToDetail", async (obj, expense) =>
			{
				if (expense != null)
				{
					await Navigation.PushAsync(new ExpenseDetailPage(expense));
				}
			});
		}

		void UnsubscribeFromMessages()
		{
			MessagingCenter.Unsubscribe<ExpensesViewModel, string>(this, "Error");
			MessagingCenter.Unsubscribe<ExpensesViewModel, string>(this, "Navigate");
			MessagingCenter.Unsubscribe<ExpensesViewModel, Expense>(this, "NavigateToDetail");
		}
	}
}