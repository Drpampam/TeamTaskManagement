using Domain.Entities.FamilyAndFriends;

namespace Application.DTOs
{
    public class AddNextOfKinDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string? PhoneNo { get; set; }
        public string? Bvn { get; set; }
        public string? RelationShip { get; set; }


        public void ConvertToDTO(PersonInfo request)
        {
            if (request == null)
            {
                new PersonInfo();
            }

            FirstName = request.FirstName;
            LastName = request.LastName;
            DateOfBirth = request.DateOfBirth;
            PhoneNo = request.PhoneNo;
            Bvn = request.Bvn;
            RelationShip = request.RelationShip;
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
            request.PhoneNo = PhoneNo;
            request.Bvn = Bvn;
            request.RelationShip = RelationShip;
        }
    }
}