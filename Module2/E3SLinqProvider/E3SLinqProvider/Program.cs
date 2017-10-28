using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E3SLinqProvider.E3SClient;
using E3SLinqProvider.E3SClient.Entities;

namespace E3SLinqProvider
{
    class Program
    {
        static void Main(string[] args)
        {
            var user = ConfigurationManager.AppSettings["user"];
            var password = ConfigurationManager.AppSettings["password"];

            var client = new E3SQueryClient(user, password);
            var res = client.SearchFTS<EmployeeEntity>("workstation:(EPRUIZHW0249)", 0, 1);

            Console.OutputEncoding = Encoding.UTF8;

            foreach (var emp in res)
            {
                Console.WriteLine("{0} {1}", emp.nativename, emp.shortstartworkdate);
            }

            var res1 = client.SearchFTS(typeof(EmployeeEntity), "workstation:(EPRUIZHW0249)", 0, 10);

            foreach (var emp in res1.OfType<EmployeeEntity>())
            {
                Console.WriteLine("{0} {1}", emp.nativename, emp.shortstartworkdate);
            }

            var employees = new E3SEntitySet<EmployeeEntity>(user, password);

            foreach (var emp in employees.Where(e => e.workstation == "EPRUIZHW0249"))
            {
                Console.WriteLine("{0} {1}", emp.nativename, emp.shortstartworkdate);
            }

            Console.ReadKey();
        }
    }
}
