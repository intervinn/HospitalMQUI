using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalCommon
{
    public class Patient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Age { get; set; }

        public int AttendingDoctorId { get; set; }

        [ForeignKey(nameof(AttendingDoctorId))]
        public required Doctor AttendingDoctor { get; set; }
    }
}
