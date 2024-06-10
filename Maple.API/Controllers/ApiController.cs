using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Maple.API.Controllers;

[ApiController]
public class ApiController : ControllerBase
{
    [Route("/error")]
    protected ActionResult Problem(List<Error> errors)
    {
        if (errors.Count is 0)
        {
            return Problem();
        }

        return errors.All(error => error.Type == ErrorType.Validation) ? ValidationProblem(errors) : Problem(errors[0]);
    }

    private ObjectResult Problem(Error error)
    {
        var code = error.Code switch
        {
            "NoExportNodesProvided" => 501,
            "MustHaveVoltageSource" => 502,
            "UnknownError" => 503,
            _ => 500,
        };

        return Problem(statusCode: code, title: error.Description);
    }

    private ActionResult ValidationProblem(List<Error> errors)
    {
        var modelStateDictionary = new ModelStateDictionary();

        errors.ForEach(error => modelStateDictionary.AddModelError(error.Code, error.Description));

        return ValidationProblem(modelStateDictionary);
    }
}