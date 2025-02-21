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
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Describes parameters for creating application-scoped volumes.
    /// </summary>
    public partial class ApplicationScopedVolumeCreationParameters
    {
        /// <summary>
        /// Initializes a new instance of the
        /// ApplicationScopedVolumeCreationParameters class.
        /// </summary>
        public ApplicationScopedVolumeCreationParameters()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// ApplicationScopedVolumeCreationParameters class.
        /// </summary>
        /// <param name="description">User readable description of the
        /// volume.</param>
        public ApplicationScopedVolumeCreationParameters(string description = default(string))
        {
            Description = description;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets user readable description of the volume.
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

    }
}
