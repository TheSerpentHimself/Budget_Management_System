using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Budget_Management_System.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please choose a category!")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Amount should be greater than ZERO!")]
        public int Amount { get; set; }

        [Column(TypeName = "nvarchar(80)")]
        public string? Note { get; set; } 

        public DateTime Date { get; set; } = DateTime.Now;

        [NotMapped]
        public string? CategoryTitleWithIcon { get 
            {
                return Category == null? "": Category.Icon + " " + Category.Title;
            } 
        }

        [NotMapped]
        public string? FormattedAmount
        {
            get
            {
                return ((Category == null || Category.Type == "Expense" ) ? "- " : "+ ") + Amount.ToString("C0");
            }
        }

    }
}
