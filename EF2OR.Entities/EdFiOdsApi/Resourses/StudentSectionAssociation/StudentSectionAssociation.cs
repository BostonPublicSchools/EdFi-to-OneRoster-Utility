using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF2OR.Entities.EdFiOdsApi.Resourses.StudentSectionAssociation
{
    public class StudentSectionAssociation: BaseEdFiOdsData
    {
        public Class1[] Property1 { get; set; }
    }
    public class Class1
    {
        public string id { get; set; }
        public SectionReference sectionReference { get; set; }
        public StudentReference studentReference { get; set; }
        public string beginDate { get; set; }
        public string endDate { get; set; }
        public string homeroomIndicator { get; set; }
        //public string teacherStudentDataLinkExclusion { get; set; }
        public string _etag { get; set; }
    }
    public class SectionReference
    {
        public string classPeriodName { get; set; }
        public string classroomIdentificationCode { get; set; }
        public string localCourseCode { get; set; }
        public string schoolId { get; set; }
        public string schoolYear { get; set; }
        public string sequenceOfCourse { get; set; }
        public string termDescriptor { get; set; }
        public string uniqueSectionCode { get; set; }
        public Link link { get; set; }
    }
    public class StudentReference
    {
        public string studentUniqueId { get; set; }
        public Link link { get; set; }
    }
    public class Link
    {
        public string rel { get; set; }
        public string href { get; set; }
    }
}
