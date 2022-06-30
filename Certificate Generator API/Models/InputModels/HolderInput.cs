using CertificateGeneratorAPI.Data;
using System.ComponentModel.DataAnnotations;

namespace CertificateGeneratorAPI.Models.InputModels
{
    public class HolderInput
    {
        [Required]
        [StringLength(100)]
        public string BusinessName { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "RIF must begin with a letter followed by 9 numbers")]
        [RIF]
        public string RIF { get; set; }

        public Holder ToHolder()
        {
            return new Holder
            {
                BusinessName = this.BusinessName,
                RIF = this.RIF.ToUpper().Replace("-","").Replace(" ",""),
            };
        }
    }

    public class RIFAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string rif = value as string;
            rif = rif.ToUpper().Replace("-", "").Replace(" ", "");

            if (RIFHasAValidCheckDigit(rif))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Not a valid RIF");
            }
        }

        protected bool RIFHasAValidCheckDigit(string RIF)
        {
            try
            {
                char rifLetter = RIF[0];

                int firstMultiplier = 0;

                switch (rifLetter)
                {
                    case 'V':
                        firstMultiplier = 1;
                        break;

                    case 'E':
                        firstMultiplier = 2;
                        break;

                    case 'J':
                        firstMultiplier = 3;
                        break;

                    case 'G':
                        firstMultiplier = 5;
                        break;

                    case 'C':
                        firstMultiplier = 3;
                        break;

                    default:
                        return false;
                }

                int rifValueWithoutDV = int.Parse(RIF.Substring(1, RIF.Length - 2));

                int[] multiplicands = new int[] { 4, 3, 2, 7, 6, 5, 4, 3, 2 };

                int[] multipliers = new int[]
                {
                firstMultiplier,
                rifValueWithoutDV/10000000,
                (rifValueWithoutDV%10000000)/1000000,
                (rifValueWithoutDV%1000000)/100000,
                (rifValueWithoutDV%100000)/10000,
                (rifValueWithoutDV%10000)/1000,
                (rifValueWithoutDV%1000)/100,
                (rifValueWithoutDV%100)/10,
                (rifValueWithoutDV%10),
                };

                int Summatory = 0;

                for (int i = 0; i < multiplicands.Length; i++)
                {
                    Summatory += multiplicands[i] * multipliers[i];
                }

                int calculatedCheckDigit = 11 - (Summatory % 11);

                if (calculatedCheckDigit > 9)
                {
                    calculatedCheckDigit = 0;
                }

                int givenRIFCheckDigit = int.Parse(RIF.Substring(RIF.Length - 1));

                if (givenRIFCheckDigit == calculatedCheckDigit)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}