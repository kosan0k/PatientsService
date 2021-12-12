using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Utility;
using Microsoft.AspNetCore.Mvc.Formatters;
using PatientsService.Extensions;
using PatientsService.Models;
using System;
using System.Text;
using Task = System.Threading.Tasks.Task;

namespace PatientsService.Serialization.Formatters
{
    public class PatientJsonOutputFormatter : TextOutputFormatter
    {
        private readonly FhirJsonSerializer _patientSerializer;

        public PatientJsonOutputFormatter(FhirJsonSerializer serializer)
        {
            _patientSerializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

            SupportedEncodings.Clear();
            SupportedEncodings.Add(Encoding.UTF8);

            foreach (var mediaType in ContentType.JSON_CONTENT_HEADERS)
            {
                SupportedMediaTypes.Add(mediaType);
            }
        }

        protected override bool CanWriteType(Type type)
        {
            return typeof(Patient).IsAssignableFrom(type)
                || typeof(FhirResponse).IsAssignableFrom(type);
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (selectedEncoding == null)
                throw new ArgumentNullException(nameof(selectedEncoding));

            if (selectedEncoding != Encoding.UTF8)
                throw Error.InvalidOperation($"FHIR supports UTF-8 encoding exclusively, not {selectedEncoding.WebName}");

            var responseBody = context.HttpContext.Response.Body;
            var writeBodyString = string.Empty;
            var summaryType = context.HttpContext.Request.RequestSummary();

            if (typeof(FhirResponse).IsAssignableFrom(context.ObjectType))
            {
                FhirResponse response = context.Object as FhirResponse;

                context.HttpContext.Response.AcquireHeaders(response);
                context.HttpContext.Response.StatusCode = (int)response.StatusCode;

                if (response.Resource != null)
                {
                    writeBodyString = _patientSerializer.SerializeToString(response.Resource, summaryType);
                }
            }
            else if (typeof(Patient).IsAssignableFrom(context.ObjectType))
            {
                if (context.Object != null)
                {
                    writeBodyString = _patientSerializer.SerializeToString(context.Object as Patient, summaryType);
                }
            }

            if (!string.IsNullOrWhiteSpace(writeBodyString))
            {
                var writeBuffer = selectedEncoding.GetBytes(writeBodyString);
                await responseBody.WriteAsync(writeBuffer, 0, writeBuffer.Length);
                await responseBody.FlushAsync();
            }
        }
    }
}
