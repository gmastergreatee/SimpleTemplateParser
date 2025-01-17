using ConsoleApp_HtmlRazor_TokenParser.RazorTemplates.RazorTemplateClasses;
using SimpleTokenParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_HtmlRazor_TokenParser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var filePath = "./RazorTemplates/RazorTokenTemplates/Student.cshtml";

            var student = new Student()
            {
                FirstName = "Rajarshi",
                LastName = "Vaidya",
                Street = "Wayward Street",
                City = "Hawana",
                District = "Unknown District",
                State = "CA",
                Pin = "100100",
                Class = new Class()
                {
                    Name = "6 1"
                },
                MobileNo = "1234567890",
                HasPassed = true,
            };

            var start = DateTime.Now;
            var studentTemplateParser = FileTokenParser.ParseTemplate<Student>(filePath);
            Console.WriteLine("Parsing template (only once) : " + (DateTime.Now - start).TotalMilliseconds + "ms");

            start = DateTime.Now;
            var content = studentTemplateParser.ApplyModel(student);
            Console.WriteLine("Generating template : " + (DateTime.Now - start).TotalMilliseconds + "ms");
            Console.WriteLine();
            Console.WriteLine(content);
            Console.WriteLine();

            student = new Student()
            {
                FirstName = "Binod",
                LastName = "Pandit",
                Street = "Wayward Street 2",
                City = "Hawana 2",
                District = "Unknown District 2",
                State = "CA 2",
                Pin = "100100 2",
                Class = new Class()
                {
                    Name = "6 2"
                },
                MobileNo = "1234567890 2",
                HasPassed = false,
            };

            start = DateTime.Now;
            content = studentTemplateParser.ApplyModel(student);
            Console.WriteLine("Generating template 2 : " + (DateTime.Now - start).TotalMilliseconds + "ms");
            Console.WriteLine();
            Console.WriteLine(content);
            Console.WriteLine();

            var studentTemplateParser_NoModel = FileTokenParser.ParseTemplate<object>(filePath, true);
            Console.WriteLine(studentTemplateParser_NoModel.ApplyModel(null));

            Console.ReadLine();
        }
    }
}
