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

			MessagingCenter.Subscribe<ExpensesViewModel, string>(this, "Error", (obj, s) =>
			{
				DisplayAlert("Error", s, "OK");
			});
		}
	}
}
