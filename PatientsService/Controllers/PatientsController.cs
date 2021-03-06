using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using PatientsService.Models;
using Task = System.Threading.Tasks.Task;

namespace PatientsService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly ILogger _logger;

        public PatientsController(ILogger<PatientsController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{id}")]
        public Task<FhirResponse> Read(string id)
        {
            return Task.FromResult(new FhirResponse(HttpStatusCode.BadRequest));
        }

        [HttpPost]
        public Task<FhirResponse> Create(Patient resource)
        {
            return Task.FromResult(new FhirResponse(HttpStatusCode.BadRequest));
        }
    }
}
