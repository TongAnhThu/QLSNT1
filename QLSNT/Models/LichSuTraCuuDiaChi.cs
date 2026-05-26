using System.ComponentModel.DataAnnotations;

public class LichSuTraCuu
{
    [Key]
    public int MaLichSu { get; set; }

    public string DiaChiCuNhap { get; set; }

    public string DiaChiMoiKetQua { get; set; }

    public DateTime NgayTraCuu { get; set; }

    public string? IP { get; set; }
}