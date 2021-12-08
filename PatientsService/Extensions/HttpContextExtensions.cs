using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;

namespace PatientsService.Extensions
{
    public static class HttpContextExtensions
    {
        private const string RESOURCE_TYPE_KEY = "resourceType";

        public static void AllowSynchronousIO(this HttpContext context)
        {
            var bodyControlFeature = context.Features.Get<IHttpBodyControlFeature>();
            if (bodyControlFeature != null)
            {
                bodyControlFeature.AllowSynchronousIO = true;
            }
        }

        public static void AddResourceType(this HttpContext context, Type resourceType)
        {
            if (context.Items.ContainsKey(RESOURCE_TYPE_KEY)) return;
            context.Items.Add(RESOURCE_TYPE_KEY, resourceType);
        }

        public static Type GetResourceType(this HttpContext context)
        {
            return context.Items.TryGetValue(RESOURCE_TYPE_KEY, out object resourceType) ? resourceType as Type : null;
        }
    }
}
