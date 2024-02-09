using EmployeeMgmt.Models;

namespace EmployeeMgmt.Data
{
    public class EmployeeStore
    {
        private readonly Dictionary<int, Employee> _employees;
        private int _employeeId = 1001;
        private int _leaveRequestId = 1;
        public EmployeeStore()
        {
            _employees = new Dictionary<int, Employee>();
            _employees.Add(1001, new Employee { Id = 1001, Username = "admin", Password = "admin", Role = UserRole.Admin, ReportingManagerId = 0 ,LeaveRequests = new List<LeaveRequest>()});
            _employees.Add(1002, new Employee { Id = 1002, Username = "manager", Password = "manager", Role = UserRole.Manager, ReportingManagerId = 1001 , LeaveRequests = new List<LeaveRequest>() });
            _employees.Add(1003, new Employee { Id = 1003, Username = "employee", Password = "employee", Role = UserRole.Employee, ReportingManagerId = 1002 , LeaveRequests = new List<LeaveRequest>() });
        }
        public IEnumerable<Employee> GetReportees(int managerId)
        {
            return _employees.Values.Where(e => e.ReportingManagerId == managerId);
        }
        public Employee? GetEmployeeById(int id)
        {
            _employees.TryGetValue(id, out Employee? emp);
            return emp;
        }
        public void AddEmployee(Employee emp)
        {
            emp.Id = ++_employeeId;
            emp.LeaveRequests = new List<LeaveRequest>();
            _employees.Add(emp.Id, emp);
        }
        public Employee? FindEmployee(string username ,string password)
        {
            return _employees.Values.FirstOrDefault(e => e.Username == username && e.Password == password);
        }

        public bool ApplyLeave(int employeeId ,DateTime startDate , DateTime endDate)
        {
            var employee = _employees[employeeId];
            var leaveRequest = new LeaveRequest
            {
                LeaveRequestId = ++_leaveRequestId,
                EmployeeId = employeeId,
                StartDate = startDate,
                EndDate = endDate,
                Status = LeaveStatus.Pending,
            };
            employee.LeaveRequests.Add(leaveRequest);
            return true;
        }

        public bool ApproveLeave(int managerId , int employeeId , int leaveRequestId)
        {
            var employee = _employees[employeeId];
            var leaveRequest = employee.LeaveRequests.Find(lr => lr.LeaveRequestId == leaveRequestId);
            if(leaveRequest == null)
            {
                return false;
            }
            if(!(employee.ReportingManagerId == managerId))
            {
                return false;
            }
            leaveRequest.Status = LeaveStatus.Approved;
            return true;
        }
    }
}
