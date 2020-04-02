using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF2OR.Entities.EdFiOdsApi.Resourses.Parent
{
    public class Parents : BaseEdFiOdsData
    {
        public Class1[] Property1 { get; set; }
    }

    public class Class1
    {
        public string studentUniqueId;
        public string middleName;

        public string id { get; set; }
        public string ParentUniqueId { get; set; }
        public Address[] address { get; set; }
        public Address[] internationalAddress { get; set; }
        public Electronicmail[] electronicMails { get; set; }
        public string name { get; set; }
        public string loginId { get; set; }
        public string otherName { get; set; }
        public string sexType { get; set; }
        public Telephone[] telephones { get; set; }
    }

    public class Telephone
    {
        public string telephoneNumber;
        public string telephoneNumberType;

        public string orderOfPriority { get; set; }
    }

    public class Electronicmail
    {
        public string electronicMailType { get; set; }
        public string electronicMailAddress { get; set; }
    }

    public class Identificationcode
    {
        public string staffIdentificationSystemDescriptor { get; set; }
        public string identificationCode { get; set; }
    }

    public class Classification
    {
        public string classification { get; set; }
    }

    public class Address
    {
        public string addressType { get; set; }
        public string streetNumberName { get; set; }
        public string city { get; set; }
        public string stateAbbreviationType { get; set; }
        public string postalCode { get; set; }
        public string nameOfCounty { get; set; }
    }
}
