using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF2OR.Entities.EdFiOdsApi.Resourses.StudentSchoolAssociation
{
    public class StudentSchoolAssociation : BaseEdFiOdsData
    {
        public Class1[] Property1 { get; set; }
    }
    public class Class1
    {
        public string id { get; set; }
        public ClassOfSchoolYearTypeReference classOfSchoolYearTypeReference { get; set; }
        public GraduationPlanReference graduationPlanReference { get; set; }        
        public SchoolReference schoolReference { get; set; }
        public SchoolYearTypeReference schoolYearTypeReference { get; set; }
        public StudentReference studentReference { get; set; }
        public string entryDate { get; set; }
        public string entryGradeLevelDescriptor { get; set; }
        public string entryTypeDescriptor { get; set; }
        public string exitWithdrawDate { get; set; }
        public string exitWithdrawTypeDescriptor { get; set; }
        public string primarySchool { get; set; }
        public string repeatGradeIndicator { get; set; }
        public string residencyStatusDescriptor { get; set; }
        public string schoolChoiceTransfer { get; set; }
        public EducationPlan[] educationPlans { get; set; }
        public string _etag { get; set; }
    }
    public class ClassOfSchoolYearTypeReference
    {
        public string schoolYear { get; set; }
        public Link link { get; set; }
    }
    public class GraduationPlanReference
    {
        public string educationOrganizationId { get; set; }
        public string typeDescriptor { get; set; }
        public string graduationSchoolYear { get; set; }
        public Link link { get; set; }
    }
    public class SchoolReference
    {
        public string schoolId { get; set; }
        public Link link { get; set; }
    }
    public class SchoolYearTypeReference
    {
        public string schoolYear { get; set; }
        public Link link { get; set; }
    }
    public class StudentReference
    {
        public string studentUniqueId { get; set; }
        public Link link { get; set; }
    }
    public class EducationPlan
    {
        public string educationPlanType { get; set; }
    }
    public class Link
    {
        public string rel { get; set; }
        public string href { get; set; }
    }
}
