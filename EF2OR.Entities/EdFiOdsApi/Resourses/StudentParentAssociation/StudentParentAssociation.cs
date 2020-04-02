using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF2OR.Entities.EdFiOdsApi.Resourses.StudentParentAssociation
{
    public class StudentParentAssociation : BaseEdFiOdsData
    {
        public Class1[] Property1 { get; set; }
    }
    public class Class1
    {
        public string id { get; set; }
        public StudentReference studentReference { get; set; }
        public ParentReference parentReference { get; set; }
        public string schoolChoiceTransfer { get; set; }
        public string educationPlans { get; set; }
        public string _etag { get; set; }
    }
        public class StudentReference
    {
        public string studentUniqueId { get; set; }
        public Link link { get; set; }
    }
    public class ParentReference
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
