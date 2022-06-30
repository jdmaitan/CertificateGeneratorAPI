namespace CertificateGeneratorAPI.Models.InputModels
{
    public class HolderInputFilter
    {
        public string BusinessName { get; set; }
        public string RIF { get; set; }
        public int Limit { get; set; }
    }
}
