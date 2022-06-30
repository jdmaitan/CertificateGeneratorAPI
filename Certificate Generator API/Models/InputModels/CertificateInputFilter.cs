namespace CertificateGeneratorAPI.Models.InputModels
{
    public class CertificateInputFilter
    {
        public int HolderID { get; set; }
        public int TypeID { get; set; }
        public int Limit { get; set; }
    }
}