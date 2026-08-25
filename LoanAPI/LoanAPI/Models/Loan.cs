namespace LoanAPI.Models
{
    public enum LoanType
    {
        QuickLoan,
        AutoLoan,
        Installment
    }

    public enum LoanStatus
    {
        InProcess,
        Approved,
        Rejected
    }

    public enum LoanCurrency
    {
        GEL,
        USD,
        EUR
    }
    public class Loan
    {
        public int Id { get; set; } //primary key
        public LoanType Type { get; set; }
        public int Amount { get; set; }
        public LoanCurrency Currency { get; set; }
        public int PeriodMonths { get; set; }
        public LoanStatus Status { get; set; } = LoanStatus.InProcess;
        public int UserId { get; set; } //foreign key
    }
}
