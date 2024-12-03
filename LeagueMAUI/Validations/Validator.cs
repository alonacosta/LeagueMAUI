using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LeagueMAUI.Validations
{
    public class Validator : IValidator
    {
        public string FirstNameError { get; set; } = "";
        public string LastNameError { get; set; } = "";
        public string EmailError { get; set; } = "";
        public string PhoneError { get; set; } = "";
        public string PasswordError { get; set; } = "";
        public string ConfirmError { get; set; } = "";

        private const string FirstNameEmptyErroMsg = "Please enter your first name.";
        private const string FirstNameInvalidErroMsg = "Please enter a valid first name.";
        private const string LastNameEmptyErroMsg = "Please enter your last name.";
        private const string LastNameInvalidErroMsg = "Please enter a valid last name.";
        private const string EmailEmptyErroMsg = "Please enter an email address.";
        private const string EmailInvalidErroMsg = "Please enter a valid email address.";
        private const string PhoneEmptyErroMsg = "Please provide a telephone number.";
        private const string PhoneInvalidErroMsg = "Please enter a valid telephone number.";
        private const string PasswordEmptyErroMsg = "Please enter the password.";
        private const string PasswordInvalidErroMsg = "The password must contain at least 6 characters, including letters and numbers.";
        private const string ConfirmEmptyErroMsg = "Please confirm the password.";
        private const string ConfirmInvalidErroMsg = "The password not mutch.";

        public Task<bool> Validate(string firstName, string lastName, string email,
                           string phoneNumber, string password, string confirm)
        {
            var isFirstNameValid = ValidateFirstName(firstName);
            var isLastNameValid = ValidateLastName(lastName);
            var isEmailValid = ValidateEmail(email);
            var isPhoneValid = ValidatePhone(phoneNumber);
            var isPasswordValid = ValidatePassword(password);
            var isConfirmValid = ValidateConfirmPassword(password, confirm);

            return Task.FromResult(isFirstNameValid && isLastNameValid && isEmailValid && isPhoneValid && isPasswordValid && isConfirmValid);
        }

        private bool ValidateFirstName(string firstName)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                FirstNameError = FirstNameEmptyErroMsg;
                return false;
            }

            if (firstName.Length < 3)
            {
                FirstNameError = FirstNameInvalidErroMsg;
                return false;
            }

            FirstNameError = "";
            return true;
        }

        private bool ValidateLastName(string lastName)
        {
            if (string.IsNullOrEmpty(lastName))
            {
                LastNameError = LastNameEmptyErroMsg;
                return false;
            }

            if (lastName.Length < 3)
            {
                LastNameError = LastNameInvalidErroMsg;
                return false;
            }

            LastNameError = "";
            return true;
        }

        private bool ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                EmailError = EmailEmptyErroMsg;
                return false;
            }

            if (!Regex.IsMatch(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                EmailError = EmailInvalidErroMsg;
                return false;
            }

            EmailError = "";
            return true;
        }

        private bool ValidatePhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                PhoneError = PhoneEmptyErroMsg;
                return false;
            }

            if (phone.Length < 9)
            {
                PhoneError = PhoneInvalidErroMsg;
                return false;
            }

            PhoneError = "";
            return true;
        }

        private bool ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                PasswordError = PasswordEmptyErroMsg;
                return false;
            }

            if (password.Length < 6 || !Regex.IsMatch(password, @"[a-zA-Z]") || !Regex.IsMatch(password, @"\d"))
            {
                PasswordError = PasswordInvalidErroMsg;
                return false;
            }

            PasswordError = "";
            return true;
        }

        private bool ValidateConfirmPassword(string password, string confirm)
        {
            if (string.IsNullOrEmpty(confirm))
            {
                ConfirmError = ConfirmEmptyErroMsg;
                return false;
            }
            if (confirm != password)
            {
                ConfirmError = ConfirmInvalidErroMsg;
                return false;
            }

            ConfirmError = "";
            return true;
        }

    }
}
