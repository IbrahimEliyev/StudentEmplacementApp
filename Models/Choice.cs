using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace StudentEmplacementApp.Models
{
    public class Choice
    {
        public float? EnterenceScore { get; set; }


        [ForeignKey("Major")]
        public int MajorId { get; set; }
        public Major? Major { get; set; }


        [ForeignKey("University")]
        public int UniId { get; set; }
        public University? University { get; set; }

        public int NumOfPlaces { get; private set; }
        public int Code { get; private set; }


        public ICollection<StudentChoice> StudentChoices { get; set; } = new List<StudentChoice>();

        public int AvailablePlaces { get; set; }

        public Choice()
        {
            AvailablePlaces = NumOfPlaces;
        }

        //  ResultCode ResultCode { get; set; } // One-to-one relationship with ResultCode 

    } 
}
