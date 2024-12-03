using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueMAUI.Validations
{
    public interface IValidator
    {
        string FirstNameError { get; set; }
        string LastNameError { get; set; }
        string EmailError { get; set; }
        string PhoneError { get; set; }
        string PasswordError { get; set; }
        string ConfirmError { get; set; }
        Task<bool> Validate(string firstName, string lastName, string email,
                           string phoneNumber, string password, string confirm);
    }
}
