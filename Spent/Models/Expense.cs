using System;

namespace Spent
{
	public class Expense
	{
		public string Id { get; set; }
		public DateTime Date { get; set; }
		public string Company { get; set; }
		public double Amount { get; set; }
		public string Description { get; set; }
		public string Receipt { get; set; }
	}
}