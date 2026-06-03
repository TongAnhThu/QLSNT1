namespace QLSNT.Areas.User.ViewModel
{
    public class DiaChiMoiViewModel
    {
        public string DiaChiCu { get; set; }

        public List<KetQuaXaMoiViewModel> DsXaMoi
        { get; set; } = new();
    }
}
