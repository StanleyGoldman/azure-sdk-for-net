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
    /// Describes common properties of a provisioned resource.
    /// </summary>
    public partial class ProvisionedResourceProperties
    {
        /// <summary>
        /// Initializes a new instance of the ProvisionedResourceProperties
        /// class.
        /// </summary>
        public ProvisionedResourceProperties()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ProvisionedResourceProperties
        /// class.
        /// </summary>
        /// <param name="provisioningState">State of the resource.</param>
        public ProvisionedResourceProperties(string provisioningState = default(string))
        {
            ProvisioningState = provisioningState;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets state of the resource.
        /// </summary>
        [JsonProperty(PropertyName = "provisioningState")]
        public string ProvisioningState { get; private set; }

    }
}
