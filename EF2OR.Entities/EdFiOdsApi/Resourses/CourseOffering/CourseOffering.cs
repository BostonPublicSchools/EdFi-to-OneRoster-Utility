using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF2OR.Entities.EdFiOdsApi.Resourses.CourseOffering
{
    public class CourseOffering : BaseEdFiOdsData
    {
        public Class1[] Property1 { get; set; }
    }

    public class Class1
    {
        public string id { get; set; }
        public CourseReference courseReference { get; set; }
        public SchoolReference schoolReference { get; set; }
        public Sessionreference sessionreference { get; set; }
        public string instructionalTimePlanned { get; set; }
        public string localCourseCode { get; set; }
        public string localCourseTitle { get; set; }
        public curriculumUsed[] curriculumUseds { get; set; }
        public string _etag { get; set; }
    }

    public class curriculumUsed
    {
        public string curriculumUsedType { get; set; }
    }

    public class CourseReference
    {
        public string educationOrganizationId { get; set; }
        public string Code { get; set; }
        public Link link { get; set; }
    }
    public class SchoolReference
    {
        public string schoolId { get; set; }
        public Link link { get; set; }
    }
    public class Sessionreference
    {
        public string id { get; set; }
        public string schoolId { get; set; }
        public string schoolYear { get; set; }
        public string termDescriptor { get; set; }
        public string sessionName { get; set; }
        public string beginDate { get; set; }
        public string endDate { get; set; }
        public string totalInstructionalDays { get; set; }
    }
    public class Link
    {
        public string rel { get; set; }
        public string href { get; set; }
    }
}
