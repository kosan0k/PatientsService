using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Utility;
using Microsoft.AspNetCore.Mvc.Formatters;
using PatientsService.Extensions;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PatientsService.Serialization.Formatters
{
    public class PatientJsonInputFormatter : TextInputFormatter
    {
        private readonly FhirJsonParser _parser;

        public PatientJsonInputFormatter(FhirJsonParser parser)
        {
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));

            SupportedEncodings.Clear();
            SupportedEncodings.Add(Encoding.UTF8);

            foreach (var mediaType in ContentType.JSON_CONTENT_HEADERS)
            {
                SupportedMediaTypes.Add(mediaType);
            }
        }

        protected override bool CanReadType(Type type)
        {
            return typeof(Resource).IsAssignableFrom(type);
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (encoding is null)
                throw new ArgumentNullException(nameof(encoding));

            if (encoding != Encoding.UTF8)
                throw Error.NotSupported($"FHIR supports UTF-8 encoding exclusively, not {encoding.WebName}");

            try
            {
                using var reader = new StreamReader(context.HttpContext.Request.Body, Encoding.UTF8);
                var body = await reader.ReadToEndAsync();
                var resource = _parser.Parse<Patient>(body);
                context.HttpContext.AddResourceType(resource.GetType());

                return await InputFormatterResult.SuccessAsync(resource);
            }
            catch (FormatException exception)
            {
                throw Error.Argument($"Body parsing failed: {exception.Message}");
            }
        }
    }
}
