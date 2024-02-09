using EmployeeMgmt.Data;
using EmployeeMgmt.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeMgmt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : Controller
    {
        private EmployeeStore _employeeStore;

        public EmployeeController(EmployeeStore employeeStore)
        {
            _employeeStore = employeeStore;
        }

        [HttpGet]
        [Authorize(Roles = "Manager")]
        public IActionResult GetAllEmployees()
        {
            var userId = GetCurrentUserId();
            var reportees = _employeeStore.GetReportees(userId);
            return Ok(reportees);
        }


        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetEmployeeById(int id)
        { 
            if(!User.IsInRole(UserRole.Admin.ToString()) && !User.IsInRole(UserRole.Manager.ToString()) && GetCurrentUserId() != id)
            { 
                return Forbid("You are not authorized to view this employee's details");
            }
            var employee = _employeeStore.GetEmployeeById(id);
            return Ok(employee);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            var employee = _employeeStore.FindEmployee(loginRequest.UserName, loginRequest.Password);
            if(employee == null)
            {
                return Unauthorized("Invaid Username or Password");
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()),
                new Claim(ClaimTypes.Name , employee.Username),
                new Claim(ClaimTypes.Role , employee.Role.ToString())
            };
            var identity = new ClaimsIdentity(claims , CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return Ok("Logged In Successfully");
        }

        [HttpPost("managers")]
        [Authorize(Roles = "Admin")]
        public IActionResult AddManager([FromBody] ManagerRequest managerRequest)
        {
            var newManager = new Employee{
                Username = managerRequest.Username,
                Password = managerRequest.Password,
                Role = UserRole.Manager,
                ReportingManagerId = 1001,
            };
            _employeeStore.AddEmployee(newManager);
            return Ok("Manager Added Successfully");
        }

        [HttpPost]
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

        [HttpPost("leaves")]
        [Authorize(Roles = "Employee")]
        public IActionResult ApplyLeave([FromBody] LeaveApplicationDTO leaveApplicationDTO)
        {
            var employeeId = GetCurrentUserId();
            var success = _employeeStore.ApplyLeave(employeeId, leaveApplicationDTO.StartDate, leaveApplicationDTO.EndDate);
            if(!success)
            {
                return BadRequest("Failed to Apply Leave");
            }
            return Ok("Leave Applied Successfully");
        }


        [HttpPost("{employeeId}/leaves/{leaveId}")]
        [Authorize(Roles = "Manager")]
        public IActionResult ApproveLeave(int employeeId ,int leaveId)
        {
            var managerId = GetCurrentUserId();
            var success = _employeeStore.ApproveLeave(managerId, employeeId, leaveId);
            if(!success)
            {
                return BadRequest("Leave Approval Failed");
            }
            return Ok("Leave Approved Successfully");
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return Ok("Logged out successfully");
        }

        private int GetCurrentUserId()
        {
            
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if(userIdClaim != null && int.TryParse(userIdClaim.Value , out int userId))
            {
                return userId;
            }
            return 0;
        }
    }
}
