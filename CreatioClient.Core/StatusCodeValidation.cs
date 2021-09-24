using CreatioClient.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace CreatioClient.Core
{
    internal static class SuccessStatusCodes
    {

        private static readonly Dictionary<Type, HttpStatusCode[]> _rules = new Dictionary<Type, HttpStatusCode[]>()
        {
            {typeof(ILogin), new HttpStatusCode[]{ HttpStatusCode.OK, HttpStatusCode.Unauthorized}},
            {typeof(IRestRequest), new HttpStatusCode[]{ HttpStatusCode.OK}},
        };

        internal static bool IsStatusSuccess(Type type, HttpStatusCode code)
        {
            if (_rules.ContainsKey(type))
            {
                return _rules[type].Contains(code);
            }
            return false;
        }

        private static bool IsContentText(HttpContent content)
        {
            if (
                content.Headers.ContentType.MediaType == "application/json" ||
                content.Headers.ContentType.MediaType == "application/javascript"||
                content.Headers.ContentType.MediaType == "text/html"||
                content.Headers.ContentType.MediaType == "text/css"||
                content.Headers.ContentType.MediaType == "image/svg+xml"
            )
            {
                return true;
            }
            return false;
        }
    }
}
