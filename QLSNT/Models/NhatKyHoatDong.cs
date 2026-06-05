namespace QLSNT.Models
{
    public class NhatKyHoatDong
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string HanhDong { get; set; }

        public string MoTa { get; set; }

        public DateTime ThoiGian { get; set; }

        public string? DiaChiIP { get; set; }
    }
}