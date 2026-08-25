namespace LoanAPI.Models
{
    public enum ErrorType
    {
        Register,
        LogIn,
        UpdateUser,
        UpdatePassword,
        DeleteUser,
        BlockUser,
        GetUserById,
        CreateLoan,
        UpdateLoan,
        DeleteLoan,
        GetLoan
    }
    public class ErrorLog
    {
        public int Id { get; set; }
        public ErrorType Type { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }  
    }
}
