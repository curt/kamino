using FluentValidation;
using System.Text.Json.Nodes;

namespace Kamino.Validators;

public class InboundActorValidator : AbstractJsonNodeValidator<JsonObject>
{
    public InboundActorValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(obj => obj["id"])
            .NotNull()
            .Must(BeString)
            .WithMessage("'{PropertyName}' must be a string.")
            .WithName("id");

        RuleFor(obj => obj["publicKey"])
            .NotNull()
            .Must(BeObject)
            .WithMessage("'{PropertyName}' must be an object.")
            .SetValidator(new InboundPublicKeyValidator())
            .WithName("publicKey");
    }
}