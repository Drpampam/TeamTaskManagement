using Domain.Entities.FamilyAndFriends;

namespace Application.DTOs
{
    public class ApprovalRequestDTO
    {
        public string? Otp { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Bvn { get; set; }
        public string? AccountNumber { get; set; }
        public string? DateOfBirth { get; set; }
        public string? NameOfApplicant { get; set; }
        public string? PhoneNumberOfApplicant { get; set; }
        public string? FnFType { get; set; }
        public string? ApprovalStatus { get; set; }

        public void ConvertToDTO(PersonInfo request)
        {
            if (request == null)
            {
                new PersonInfo();
            }

            FirstName = request.FirstName;
            LastName = request.LastName;
            DateOfBirth = request.DateOfBirth;
            Otp = request.Otp;
            AccountNumber = request.AccountNumber;
            NameOfApplicant = request.NameOfApplicant;
            PhoneNumberOfApplicant = request.PhoneNumberOfApplicant;
            Bvn = request.Bvn;
            FnFType = request.FnFType;
            ApprovalStatus = request.ApplicationStatus;
        }

        public void ConvertFromDTO(PersonInfo request)
        {
            if (request == null)
            {
                new PersonInfo();
            }

            request.FirstName = FirstName;
            request.LastName = LastName;
            request.DateOfBirth = DateOfBirth;
            request.Otp = Otp;
            request.Bvn = Bvn;
            request.FnFType = FnFType;
            request.AccountNumber = AccountNumber;
            request.NameOfApplicant = NameOfApplicant;
            request.PhoneNumberOfApplicant = PhoneNumberOfApplicant;
            request.ApplicationStatus = ApprovalStatus;
        }
    }
}
