namespace LoanAPI.Models
{
    public enum ActionType
    {
        UserRegistered,
        UserLoggedIn,
        UserUpdated,
        PasswordUpdated,
        UserDeleted,
        UserBlockedStatusChanged,
        LoanCreated,
        LoanUpdated,
        LoanDeleted
    }
    public class ActionLog
    {
        public int Id { get; set; }
        public ActionType ActionName { get; set; }
        public int? UserId { get; set; }
        public int? LoanId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
