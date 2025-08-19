using ArchiWindRevitAddIn.Models.Forms;
using FluentValidation;

namespace ArchiWindRevitAddIn.Models.Validators
{
    public class CreateSimulationFormValidator : AbstractValidator<CreateSimulationForm>
    {
        public CreateSimulationFormValidator()
        {
            RuleFor(sim => sim.ProjectId).NotEmpty();
            RuleFor(sim => sim.Name).NotEmpty();
            RuleFor(sim => sim.Quality).IsInEnum();
            RuleFor(sim => sim.Latitude).GreaterThan(-90.0).LessThan(90.0);
            RuleFor(sim => sim.Longitude).GreaterThan(-180.0).LessThan(180.0);
            RuleFor(sim => sim.RefSystem)
                .Must(refSystem => refSystem == null || Epsg.Values.Contains(refSystem!.Value))
                .WithMessage("EPSG must be included in list of values");
        }
    }
}
