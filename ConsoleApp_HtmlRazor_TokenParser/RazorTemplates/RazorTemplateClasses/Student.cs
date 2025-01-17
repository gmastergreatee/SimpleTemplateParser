namespace ConsoleApp_HtmlRazor_TokenParser.RazorTemplates.RazorTemplateClasses
{
    public class Student
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Class Class { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string State { get; set; }
        public string Pin { get; set; }
        public string MobileNo { get; set; }
        public bool HasPassed { get; set; }

        public string PassedStatus
        {
            get
            {
                return HasPassed ? "Passed" : "Failed";
            }
        }

        public string Student_Description
        {
            get
            {
                return HasPassed ?
                    "The student has passed with flying colors" :
                    "Despite working hard the student has failed";
            }
        }

        public string Student_Name = "";

        public string Name
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }
}
