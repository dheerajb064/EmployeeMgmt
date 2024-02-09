namespace EmployeeMgmt.Models
{

    public enum LeaveStatus
    {
        Pending ,
        Approved ,
        Rejected
    }
    public class LeaveRequest
    {
        public int LeaveRequestId {  get; set; }
        public int EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LeaveStatus Status { get; set; }
    }
}
