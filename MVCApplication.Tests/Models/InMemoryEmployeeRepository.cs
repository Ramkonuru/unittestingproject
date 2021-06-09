using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCApplication.Models;

namespace MVCApplication.Tests.Models
{
    class InMemoryEmployeeRepository : IEmployeeRepository
    {
        private List<tbl_Employee> _db = new List<tbl_Employee>();
        public Exception ExceptionToThrow { get; set; }
        public IEnumerable<tbl_Employee> GetAllEmployee()
        {
            return _db.ToList();
        }
        public tbl_Employee GetEmployeeByID(int id)
        {
            return _db.FirstOrDefault(d => d.Id == id);
        }
        public void CreateNewEmployee(tbl_Employee employeeToCreate)
        {
            if (ExceptionToThrow != null)
                throw ExceptionToThrow;
            _db.Add(employeeToCreate);
        }
        public void SaveChanges(tbl_Employee employeeToUpdate)
        {
            foreach (tbl_Employee employee in _db)
            {
                if (employee.Id == employeeToUpdate.Id)
                {
                    _db.Remove(employee);
                    _db.Add(employeeToUpdate);
                    break;
                }
            }
        }
        public void Add(tbl_Employee employeeToAdd)
        {
            _db.Add(employeeToAdd);
        }
        public int SaveChanges()
        {
            return 1;
        }
        public void DeleteEmployee(int id)
        {
            _db.Remove(GetEmployeeByID(id));
        }
    }
}
