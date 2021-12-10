using System.Net;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace PatientsService.Models
{
    public class FhirResponse
    {
        public HttpStatusCode StatusCode;
        public IKey Key;
        public Resource Resource;

        public FhirResponse(HttpStatusCode code, IKey key, Resource resource)
        {
            StatusCode = code;
            Key = key;
            Resource = resource;
        }

        public FhirResponse(HttpStatusCode code, Resource resource)
        {
            StatusCode = code;
            Key = null;
            Resource = resource;
        }

        public FhirResponse(HttpStatusCode code)
        {
            StatusCode = code;
            Key = null;
            Resource = null;
        }

        public bool IsValid
        {
            get
            {
                var code = (int)StatusCode;
                return code <= 300;
            }
        }

        public bool HasBody => Resource != null;

        public override string ToString()
        {
            var details = (Resource != null) ? $"({Resource.TypeName})" : null;
            var location = Key?.ToString();
            return $"{(int) StatusCode}: {StatusCode} {details} ({location})";
        }
    }
}
