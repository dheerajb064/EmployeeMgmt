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
