using EmployeeMgmt.Data;
using EmployeeMgmt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeMgmt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : Controller
    {
        private readonly EmployeeStore _employeeStore;
        public ManagerController(EmployeeStore employeeStore)
        {
            _employeeStore = employeeStore;
        }

        [HttpGet]
        [Authorize(Roles ="Admin")]
        public IActionResult GetAllManagers()
        {
            var managers = _employeeStore.GetAllEmployees();
            if(managers == null)
            {
                return NotFound();
            }
            return Ok(managers);
        }

        [HttpGet("employees")]
        [Authorize(Roles = "Manager")]
        public IActionResult GetAllEmployees()
        {
            var userId = GetCurrentUserId();
            var reportees = _employeeStore.GetReportees(userId);
            return Ok(reportees);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AddManager([FromBody] ManagerRequest managerRequest)
        {
            var newManager = new Employee
            {
                Username = managerRequest.Username,
                Password = managerRequest.Password,
                Role = UserRole.Manager,
                ReportingManagerId = 1001,
            };
            _employeeStore.AddEmployee(newManager);
            return Ok("Manager Added Successfully");
        }

        [HttpPost("employees")]
        [Authorize(Roles = "Manager")]
        public IActionResult AddEmployee([FromBody] AddEmployeeDTO employeeDTO)
        {
            var newEmployee = new Employee
            {
                Username = employeeDTO.Username,
                Password = employeeDTO.Password,
                Role = UserRole.Employee,
                ReportingManagerId = GetCurrentUserId()
            };
            _employeeStore.AddEmployee(newEmployee);
            return Ok("Employee Added Successfully");
        }

        [HttpPost("{employeeId}/leaves/{leaveId}")]
        [Authorize(Roles = "Manager")]
        public IActionResult ApproveLeave(int employeeId, int leaveId)
        {
            var managerId = GetCurrentUserId();
            var success = _employeeStore.ApproveLeave(managerId, employeeId, leaveId);
            if (!success)
            {
                return BadRequest("Leave Approval Failed");
            }
            return Ok("Leave Approved Successfully");
        }




        private int GetCurrentUserId()
        {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0;
        }
    }
}
