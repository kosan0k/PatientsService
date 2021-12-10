using Hl7.Fhir.Rest;
using Hl7.Fhir.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using PatientsService.Models;
using System;
using System.Linq;
using System.Net.Http;

namespace PatientsService.Extensions
{
    public static class HttpRequestExtensions
    {
        internal static void AcquireHeaders(this HttpResponse response, FhirResponse fhirResponse)
        {
            if (fhirResponse.Key != null)
            {
                response.Headers.Add("ETag", ETag.Create(fhirResponse.Key.VersionId)?.ToString());

                Uri location = fhirResponse.Key.ToUri();
                response.Headers.Add("Location", location.OriginalString);

                if (response.Body != null)
                {
                    response.Headers.Add("Content-Location", location.OriginalString);
                    if (fhirResponse.Resource != null && fhirResponse.Resource.Meta != null)
                    {
                        response.Headers.Add("Last-Modified", fhirResponse.Resource.Meta.LastUpdated.Value.ToString("R"));
                    }
                }
            }
        }

        internal static SummaryType RequestSummary(this HttpRequest request)
        {
            request.Query.TryGetValue("_summary", out StringValues stringValues);
            return GetSummary(stringValues.FirstOrDefault());
        }

        private static SummaryType GetSummary(string summary)
        {
            SummaryType? summaryType;
            if (string.IsNullOrWhiteSpace(summary))
                summaryType = SummaryType.False;
            else
                summaryType = EnumUtility.ParseLiteral<SummaryType>(summary, true);

            return summaryType ?? SummaryType.False;
        }
    }
}
