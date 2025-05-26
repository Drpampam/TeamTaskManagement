using ConfigurationService.Domain.Common;

namespace Domain.Entities.FamilyAndFriends
{
    public class PersonInfo : BaseEntity
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? DateOfBirth { get; set; }
        public string? PhoneNo { get; set; }
        public string? AccountNumber { get; set; }
        public string? RelationShip { get; set; }
        public string? Bvn { get; set; }
        public string? ApplicationStatus { get; set; }
        public string? FnFType { get; set; }
        public string? Otp { get; set; }
        public string? NameOfApplicant { get; set; }
        public string? PhoneNumberOfApplicant { get; set; }
        public int? ApplicationScore { get; set; }
    }
}