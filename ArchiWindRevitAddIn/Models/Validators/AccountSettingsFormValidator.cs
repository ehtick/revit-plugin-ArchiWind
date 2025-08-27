using ArchiWindRevitAddIn.Models.Forms;
using FluentValidation;

namespace ArchiWindRevitAddIn.Models.Validators
{
    public class AccountSettingsFormValidator : AbstractValidator<AccountSettingsForm>
    {
        public AccountSettingsFormValidator()
        {
            RuleFor(s => s.Pat)
                .Must(pat => pat.Length > 0)
                .WithMessage("Cannot be empty.");
        }
    }
}
