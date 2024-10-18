using DepartmentEmployeeAPI.Database;
using DepartmentEmployeeAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly MyDbContext _context;

    public EmployeesController(MyDbContext context)
    {
        _context = context;
    }

    // GET: api/employees
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
    {
        return await _context.Employees.Include(e => e.Department).ToListAsync();
    }

    // GET: api/employees/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Employee>> GetEmployee(int id)
    {
        var employee = await _context.Employees.Include(e => e.Department).FirstOrDefaultAsync(e => e.EmployeeID == id);

        if (employee == null)
        {
            return NotFound();
        }

        return employee;
    }

    // POST: api/employees
    [HttpPost]
    public async Task<ActionResult<Employee>> CreateEmployee(Employee employee)
    {
        var department = await _context.Departments.FindAsync(employee.DepartmentID);
        if (department == null)
        {
            return BadRequest("Invalid Department ID.");
        }

        employee.Department = department;

        if (employee.DateOfBirth != default)
        {
            employee.Age = DateTime.Now.Year - employee.DateOfBirth.Year;

            if (DateTime.Now < employee.DateOfBirth.AddYears(employee.Age))
            {
                employee.Age--;
            }
        }

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmployeeID }, employee);
    }



    // PUT: api/employees/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, Employee employee)
    {
        if (id != employee.EmployeeID)
        {
            return BadRequest();
        }

        var department = await _context.Departments.FindAsync(employee.DepartmentID);
        if (department == null)
        {
            return BadRequest("Invalid Department ID.");
        }

        employee.Department = department;

        if (employee.DateOfBirth != default)
        {
            employee.Age = DateTime.Now.Year - employee.DateOfBirth.Year;

            if (DateTime.Now < employee.DateOfBirth.AddYears(employee.Age))
            {
                employee.Age--;
            }
        }

        _context.Entry(employee).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EmployeeExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/employees/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool EmployeeExists(int id)
    {
        return _context.Employees.Any(e => e.EmployeeID == id);
    }
}
