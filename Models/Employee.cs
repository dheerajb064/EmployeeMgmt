namespace EmployeeMgmt.Models
{
    public enum UserRole
    {
        Admin ,
        Manager ,
        Employee
    }
    public class Employee
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public UserRole Role { get; set; }
        public int ReportingManagerId {  get; set; }
        public List<LeaveRequest>? LeaveRequests { get; set; }
    }
}
