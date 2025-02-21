// <auto-generated>
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Azure.Management.ServiceFabricMesh.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Describes a rule for http route matching.
    /// </summary>
    public partial class HttpRouteMatchRule
    {
        /// <summary>
        /// Initializes a new instance of the HttpRouteMatchRule class.
        /// </summary>
        public HttpRouteMatchRule()
        {
            Path = new HttpRouteMatchPath();
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the HttpRouteMatchRule class.
        /// </summary>
        /// <param name="path">Path to match for routing.</param>
        /// <param name="headers">headers and their values to match in
        /// request.</param>
        public HttpRouteMatchRule(HttpRouteMatchPath path, IList<HttpRouteMatchHeader> headers = default(IList<HttpRouteMatchHeader>))
        {
            Path = path;
            Headers = headers;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets path to match for routing.
        /// </summary>
        [JsonProperty(PropertyName = "path")]
        public HttpRouteMatchPath Path { get; set; }

        /// <summary>
        /// Gets or sets headers and their values to match in request.
        /// </summary>
        [JsonProperty(PropertyName = "headers")]
        public IList<HttpRouteMatchHeader> Headers { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Path == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Path");
            }
            if (Path != null)
            {
                Path.Validate();
            }
            if (Headers != null)
            {
                foreach (var element in Headers)
                {
                    if (element != null)
                    {
                        element.Validate();
                    }
                }
            }
        }
    }
}
