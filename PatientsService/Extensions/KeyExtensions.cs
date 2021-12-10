﻿using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using PatientsService.Models;
using System;
using System.Collections.Generic;

namespace PatientsService.Extensions
{
    public static class KeyExtensions
    {
        public static Key ExtractKey(this Resource resource)
        {
            string _base = resource.ResourceBase?.ToString();
            Key key = new Key(_base, resource.TypeName, resource.Id, resource.VersionId);
            return key;
        }

        public static Key ExtractKey(Uri uri)
        {
            var identity = new ResourceIdentity(uri);

            string _base = (identity.HasBaseUri) ? identity.BaseUri.ToString() : null;
            Key key = new Key(_base, identity.ResourceType, identity.Id, identity.VersionId);
            return key;
        }

        public static void ApplyTo(this IKey key, Resource resource)
        {
            resource.ResourceBase = key.HasBase() ? new Uri(key.Base) : null;
            resource.Id = key.ResourceId;
            resource.VersionId = key.VersionId;
        }

        public static Key Clone(this IKey self)
        {
            Key key = new Key(self.Base, self.TypeName, self.ResourceId, self.VersionId);
            return key;
        }

        public static bool HasBase(this IKey key)
        {
            return !string.IsNullOrEmpty(key.Base);
        }

        public static Key WithBase(this IKey self, string _base)
        {
            Key key = self.Clone();
            key.Base = _base;
            return key;
        }

        public static Key WithoutBase(this IKey self)
        {
            Key key = self.Clone();
            key.Base = null;
            return key;
        }

        public static Key WithoutVersion(this IKey self)
        {
            Key key = self.Clone();
            key.VersionId = null;
            return key;
        }

        public static bool HasVersionId(this IKey self)
        {
            return !string.IsNullOrEmpty(self.VersionId);
        }

        public static bool HasResourceId(this IKey self)
        {
            return !string.IsNullOrEmpty(self.ResourceId);
        }

        public static IKey WithoutResourceId(this IKey self)
        {
            var key = self.Clone();
            key.ResourceId = null;
            return key;
        }

        /// <summary>
        /// If an id is provided, the server SHALL ignore it.
        /// If the request body includes a meta, the server SHALL ignore
        /// the existing versionId and lastUpdated values.
        /// http://hl7.org/fhir/STU3/http.html#create
        /// http://hl7.org/fhir/R4/http.html#create
        /// </summary>
        public static IKey CleanupForCreate(this IKey key)
        {
            if (key.HasResourceId())
            {
                key = key.WithoutResourceId();
            }

            if (key.HasVersionId())
            {
                key = key.WithoutVersion();
            }

            return key;
        }

        public static IEnumerable<string> GetSegments(this IKey key)
        {
            if (key.Base != null)
                yield return key.Base;

            if (key.TypeName != null)
                yield return key.TypeName;

            if (key.ResourceId != null) 
                yield return key.ResourceId;

            if (key.VersionId != null)
            {
                yield return "_history";
                yield return key.VersionId;
            }
        }

        public static string ToUriString(this IKey key)
        {
            var segments = key.GetSegments();
            return string.Join("/", segments);
        }

        public static string ToOperationPath(this IKey self)
        {
            Key key = self.WithoutBase();
            return key.ToUriString();
        }

        /// <summary>
        /// A storage key is a resource reference string that is ensured to be server wide unique.
        /// This way resource can refer to eachother at a database level.
        /// These references are also used in SearchResult lists.
        /// The format is "resource/id/_history/vid"
        /// </summary>
        /// <returns>a string</returns>
        public static string ToStorageKey(this IKey key)
        {
            return key.WithoutBase().ToUriString();
        }

        public static Key CreateFromLocalReference(string reference)
        {
            string[] parts = reference.Split('/');
            if (parts.Length == 2)
            {
                return Key.Create(parts[0], parts[1], parts[3]);
            }
            else if (parts.Length == 4)
            {
                return Key.Create(parts[0], parts[1], parts[3]);
            }
            else throw new ArgumentException("Could not create key from local-reference: " + reference);
        }

        public static Uri ToRelativeUri(this IKey key)
        {
            string path = key.ToOperationPath();
            return new Uri(path, UriKind.Relative);
        }

        public static Uri ToUri(this IKey self)
        {
            return new Uri(self.ToUriString(), UriKind.RelativeOrAbsolute);
        }

        public static Uri ToUri(this IKey key, Uri endpoint)
        {
            string _base = endpoint.ToString().TrimEnd('/');
            string s = string.Format("{0}/{1}", _base, key);
            return new Uri(s);
        }

        /// <summary>
        /// Value equality for two IKey's
        /// </summary>
        /// <returns>true if all parts of of the keys are the same</returns>
        public static bool EqualTo(this IKey key, IKey other)
        {
            return (key.Base == other.Base)
                && (key.TypeName == other.TypeName)
                && (key.ResourceId == other.ResourceId)
                && (key.VersionId == other.VersionId);
        }
    }
}
