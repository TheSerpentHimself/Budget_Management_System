using Budget_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Budget_Management_System.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;


        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> Index()
        {
            DateTime startDate = DateTime.Today.AddDays(-6);
            DateTime endDate = DateTime.Today;

            List<Transaction> selectedTransactions = await _context.Transactions
                .Include(x => x.Category)
                .Where(y => y.Date >= startDate && y.Date <= endDate)
                .ToListAsync();

            int totalIncome = selectedTransactions
                .Where(i => i.Category.Type == "Income")
                .Sum(j => j.Amount);
            ViewBag.TotalIncome = totalIncome.ToString("C0");

            int totalExpense = selectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .Sum(j => j.Amount);
            ViewBag.totalExpense = totalExpense.ToString("C0");

            int Balance = (totalIncome - totalExpense);
            CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture("en-US");
            cultureInfo.NumberFormat.CurrencyNegativePattern = 1;
            ViewBag.Balance = string.Format(cultureInfo, "{0:C0}", Balance);

            ViewBag.DoughnutChartData = selectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Category.CategoryId)
                .Select(k => new
                {
                    CategoryTitleWithIcon = k.First().Category.Icon + " " + k.First().Category.Title,
                    amount = k.Sum(j => j.Amount),
                    formattedAmount = k.Sum(j => j.Amount).ToString("C0"),
                })
                .OrderByDescending(l => l.amount)
                .ToList();

            List<SplineChartData> incomeSummary = selectedTransactions
                .Where(i => i.Category.Type == "Income")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    income = k.Sum(l => l.Amount)
                })
                .ToList();

            List<SplineChartData> expenseSummary = selectedTransactions
             .Where(i => i.Category.Type == "Expense")
             .GroupBy(j => j.Date)
             .Select(k => new SplineChartData
              {
                 day = k.First().Date.ToString("dd-MMM"),
                 expense = k.Sum(l => l.Amount)
              })
              .ToList();

            string[] lastSevenDays = Enumerable.Range(0,7)
                .Select(i => startDate.AddDays(i).ToString("dd-MMM"))
                .ToArray();

            ViewBag.SplineChartData = from day in lastSevenDays
                                      join income in incomeSummary on day equals income.day
                                      into dayIncomeJoined
                                      from income in dayIncomeJoined.DefaultIfEmpty()
                                      join expense in expenseSummary on day equals expense.day
                                      into dayExpenseJoined
                                      from expense in dayExpenseJoined.DefaultIfEmpty()
                                      select new
                                      {
                                          day = day,
                                          income = income == null 
                                            ? 0 
                                            : income.income,
                                          expense = expense == null ? 0 : expense.expense,
                                      };

            ViewBag.RecentTransaction = await _context.Transactions
                .Include(i => i.Category)
                .OrderByDescending(j => j.Date)
                .Take(5)
                .ToListAsync();


            return View();
        }
    }

    public class SplineChartData
    {
        public string day;
        public int income;
        public int expense;
    }
}
