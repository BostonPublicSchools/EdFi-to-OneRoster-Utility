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
        public string id { get; set; }
        public string ParentUniqueId { get; set; }
        public string firstName { get; set; }
        public string generationCodeSuffix { get; set; }
        public string lastSurname { get; set; }
        public string middleName { get; set; }
        public string maidenName { get; set; }
        public string personalTitlePrefix { get; set; }
        public string sexType { get; set; }
        public Address[] address { get; set; }
        public Electronicmail[] electronicMails { get; set; }
        public IdentificationDocuments[] identificationDocuments { get; set; }
        public Address[] internationalAddresses { get; set; }
        public OtherNames[] otherNames { get; set; }
        public Telephone[] telephones { get; set; }
        public string _etag { get; set; }
    }

    public class Telephone
    {
        public string telephoneNumber { get; set; }
        public string telephoneNumberType { get; set; }
        public string orderOfPriority { get; set; }
        public string textMessageCapabilityIndicator { get; set; }
    }

    public class Electronicmail
    {
        public string electronicMailType { get; set; }
        public string electronicMailAddress { get; set; }
    }

    public class IdentificationDocuments
    {
        public string staffIdentificationSystemDescriptor { get; set; }
        public string identificationCode { get; set; }
        public string identificationDocumentUseType { get; set; }
        public string personalInformationVerificationType { get; set; }
        public string issuerCountryDescriptor { get; set; }
        public string documentTitle { get; set; }
        public string documentExpirationDate { get; set; }
        public string issuerDocumentIdentificationCode { get; set; }
        public string issuerName { get; set; }
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
        public string countryDescriptor { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string addressLine3 { get; set; }
        public string addressLine4 { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string beginDate { get; set; }
        public string endDate { get; set; }
    }

    public class OtherNames
    {
        public string otherNameType { get; set; }
        public string personalTitlePrefix { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastSurname { get; set; }
        public string generationCodeSuffix { get; set; }
    }
}
