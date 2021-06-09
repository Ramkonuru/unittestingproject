using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCApplication.Models;
using MVCApplication.Controllers;
using MVCApplication.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web;
using System.Security.Principal;

namespace MVCApplication.Tests.Controllers
{
    [TestClass]
    public class EmployeeControllerTest
    {
         
        [TestMethod]
        public void IndexView()
        {
            var empcontroller = GetEmployeeController(new InMemoryEmployeeRepository());
            ViewResult result = empcontroller.Index();
            Assert.AreEqual("Index", result.ViewName);
        }
    
     
        private static EmployeeController GetEmployeeController(IEmployeeRepository emprepository)
        {
            EmployeeController empcontroller = new EmployeeController(emprepository);
            empcontroller.ControllerContext = new ControllerContext()
            {
                Controller = empcontroller,
                RequestContext = new RequestContext(new MockHttpContext(), new RouteData())
            };
            return empcontroller;
        }
       
        [TestMethod]
        public void GetAllEmployeeFromRepository()
        {
            // Arrange  
            tbl_Employee employee1 = GetEmployeeName(1, 101, "ram", ".net","btech");
            tbl_Employee employee2 = GetEmployeeName(1, 101, "uma", "devops", "btech");
            InMemoryEmployeeRepository emprepository = new InMemoryEmployeeRepository();
            emprepository.Add(employee1);
            emprepository.Add(employee2);
            var controller = GetEmployeeController(emprepository);
            var result = controller.Index();
            var datamodel = (IEnumerable<tbl_Employee>)result.ViewData.Model;
            CollectionAssert.Contains(datamodel.ToList(), employee1);
            CollectionAssert.Contains(datamodel.ToList(), employee2);
        }
       
       
        tbl_Employee GetEmployeeName(int id, int empcode, string name, string spec, string edu)
        {
            return new tbl_Employee
            {
                Id = id,
                EmpCode = empcode,
                Name = name,
                Specialization = spec,
                Education = edu,
               
            };
        }
        
        [TestMethod]
        public void Create_PostEmployeeInRepository()
        {
            InMemoryEmployeeRepository emprepository = new InMemoryEmployeeRepository();
            EmployeeController empcontroller = GetEmployeeController(emprepository);
            tbl_Employee employee = GetEmployeeID();
            empcontroller.Create(employee);
            IEnumerable<tbl_Employee> employees = emprepository.GetAllEmployee();
            Assert.IsTrue(employees.Contains(employee));
        }
        
        tbl_Employee GetEmployeeID()
        {
            return GetEmployeeName(1, 101, "ram", ".net", "btech");
        }
        
        [TestMethod]
        public void Create_PostRedirectOnSuccess()
        {
            EmployeeController controller = GetEmployeeController(new InMemoryEmployeeRepository());
            tbl_Employee model = GetEmployeeID();
            var result = (RedirectToRouteResult)controller.Create(model);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
       
        [TestMethod]
        public void ViewIsNotValid()
        {
            EmployeeController empcontroller = GetEmployeeController(new InMemoryEmployeeRepository());
            empcontroller.ModelState.AddModelError("", "mock error message");
            tbl_Employee model = GetEmployeeName(1, 101, "", "", "");
            var result = (ViewResult)empcontroller.Create(model);
            Assert.AreEqual("Create", result.ViewName);
        }
        
        [TestMethod]
        public void RepositoryThrowsException()
        {
            // Arrange  
            InMemoryEmployeeRepository emprepository = new InMemoryEmployeeRepository();
            Exception exception = new Exception();
            emprepository.ExceptionToThrow = exception;
            EmployeeController controller = GetEmployeeController(emprepository);
            tbl_Employee employee = GetEmployeeID();
            var result = (ViewResult)controller.Create(employee);
            Assert.AreEqual("Create", result.ViewName);
            ModelState modelState = result.ViewData.ModelState[""];
            Assert.IsNotNull(modelState);
            Assert.IsTrue(modelState.Errors.Any());
            Assert.AreEqual(exception, modelState.Errors[0].Exception);
        }
        private class MockHttpContext : HttpContextBase
        {
            private readonly IPrincipal _user = new GenericPrincipal(new GenericIdentity("someUser"), null);
            public override IPrincipal User
            {
                get
                {
                    return _user;
                }
                set
                {
                    base.User = value;
                }
            }
        }
    }
}
