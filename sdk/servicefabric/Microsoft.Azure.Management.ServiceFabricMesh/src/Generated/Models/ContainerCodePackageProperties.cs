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
    /// Describes a container and its runtime properties.
    /// </summary>
    public partial class ContainerCodePackageProperties
    {
        /// <summary>
        /// Initializes a new instance of the ContainerCodePackageProperties
        /// class.
        /// </summary>
        public ContainerCodePackageProperties()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ContainerCodePackageProperties
        /// class.
        /// </summary>
        /// <param name="name">The name of the code package.</param>
        /// <param name="image">The Container image to use.</param>
        /// <param name="resources">The resources required by this
        /// container.</param>
        /// <param name="imageRegistryCredential">Image registry
        /// credential.</param>
        /// <param name="entrypoint">Override for the default entry point in
        /// the container.</param>
        /// <param name="commands">Command array to execute within the
        /// container in exec form.</param>
        /// <param name="environmentVariables">The environment variables to set
        /// in this container</param>
        /// <param name="settings">The settings to set in this container. The
        /// setting file path can be fetched from environment variable
        /// "Fabric_SettingPath". The path for Windows container is
        /// "C:\\secrets". The path for Linux container is
        /// "/var/secrets".</param>
        /// <param name="labels">The labels to set in this container.</param>
        /// <param name="endpoints">The endpoints exposed by this
        /// container.</param>
        /// <param name="volumeRefs">Volumes to be attached to the container.
        /// The lifetime of these volumes is independent of the application's
        /// lifetime.</param>
        /// <param name="volumes">Volumes to be attached to the container. The
        /// lifetime of these volumes is scoped to the application's
        /// lifetime.</param>
        /// <param name="diagnostics">Reference to sinks in
        /// DiagnosticsDescription.</param>
        /// <param name="reliableCollectionsRefs">A list of ReliableCollection
        /// resources used by this particular code package. Please refer to
        /// ReliableCollectionsRef for more details.</param>
        /// <param name="instanceView">Runtime information of a container
        /// instance.</param>
        public ContainerCodePackageProperties(string name, string image, ResourceRequirements resources, ImageRegistryCredential imageRegistryCredential = default(ImageRegistryCredential), string entrypoint = default(string), IList<string> commands = default(IList<string>), IList<EnvironmentVariable> environmentVariables = default(IList<EnvironmentVariable>), IList<Setting> settings = default(IList<Setting>), IList<ContainerLabel> labels = default(IList<ContainerLabel>), IList<EndpointProperties> endpoints = default(IList<EndpointProperties>), IList<VolumeReference> volumeRefs = default(IList<VolumeReference>), IList<ApplicationScopedVolume> volumes = default(IList<ApplicationScopedVolume>), DiagnosticsRef diagnostics = default(DiagnosticsRef), IList<ReliableCollectionsRef> reliableCollectionsRefs = default(IList<ReliableCollectionsRef>), ContainerInstanceView instanceView = default(ContainerInstanceView))
        {
            Name = name;
            Image = image;
            ImageRegistryCredential = imageRegistryCredential;
            Entrypoint = entrypoint;
            Commands = commands;
            EnvironmentVariables = environmentVariables;
            Settings = settings;
            Labels = labels;
            Endpoints = endpoints;
            Resources = resources;
            VolumeRefs = volumeRefs;
            Volumes = volumes;
            Diagnostics = diagnostics;
            ReliableCollectionsRefs = reliableCollectionsRefs;
            InstanceView = instanceView;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the name of the code package.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Container image to use.
        /// </summary>
        [JsonProperty(PropertyName = "image")]
        public string Image { get; set; }

        /// <summary>
        /// Gets or sets image registry credential.
        /// </summary>
        [JsonProperty(PropertyName = "imageRegistryCredential")]
        public ImageRegistryCredential ImageRegistryCredential { get; set; }

        /// <summary>
        /// Gets or sets override for the default entry point in the container.
        /// </summary>
        [JsonProperty(PropertyName = "entrypoint")]
        public string Entrypoint { get; set; }

        /// <summary>
        /// Gets or sets command array to execute within the container in exec
        /// form.
        /// </summary>
        [JsonProperty(PropertyName = "commands")]
        public IList<string> Commands { get; set; }

        /// <summary>
        /// Gets or sets the environment variables to set in this container
        /// </summary>
        [JsonProperty(PropertyName = "environmentVariables")]
        public IList<EnvironmentVariable> EnvironmentVariables { get; set; }

        /// <summary>
        /// Gets or sets the settings to set in this container. The setting
        /// file path can be fetched from environment variable
        /// "Fabric_SettingPath". The path for Windows container is
        /// "C:\\secrets". The path for Linux container is "/var/secrets".
        /// </summary>
        [JsonProperty(PropertyName = "settings")]
        public IList<Setting> Settings { get; set; }

        /// <summary>
        /// Gets or sets the labels to set in this container.
        /// </summary>
        [JsonProperty(PropertyName = "labels")]
        public IList<ContainerLabel> Labels { get; set; }

        /// <summary>
        /// Gets or sets the endpoints exposed by this container.
        /// </summary>
        [JsonProperty(PropertyName = "endpoints")]
        public IList<EndpointProperties> Endpoints { get; set; }

        /// <summary>
        /// Gets or sets the resources required by this container.
        /// </summary>
        [JsonProperty(PropertyName = "resources")]
        public ResourceRequirements Resources { get; set; }

        /// <summary>
        /// Gets or sets volumes to be attached to the container. The lifetime
        /// of these volumes is independent of the application's lifetime.
        /// </summary>
        [JsonProperty(PropertyName = "volumeRefs")]
        public IList<VolumeReference> VolumeRefs { get; set; }

        /// <summary>
        /// Gets or sets volumes to be attached to the container. The lifetime
        /// of these volumes is scoped to the application's lifetime.
        /// </summary>
        [JsonProperty(PropertyName = "volumes")]
        public IList<ApplicationScopedVolume> Volumes { get; set; }

        /// <summary>
        /// Gets or sets reference to sinks in DiagnosticsDescription.
        /// </summary>
        [JsonProperty(PropertyName = "diagnostics")]
        public DiagnosticsRef Diagnostics { get; set; }

        /// <summary>
        /// Gets or sets a list of ReliableCollection resources used by this
        /// particular code package. Please refer to ReliableCollectionsRef for
        /// more details.
        /// </summary>
        [JsonProperty(PropertyName = "reliableCollectionsRefs")]
        public IList<ReliableCollectionsRef> ReliableCollectionsRefs { get; set; }

        /// <summary>
        /// Gets runtime information of a container instance.
        /// </summary>
        [JsonProperty(PropertyName = "instanceView")]
        public ContainerInstanceView InstanceView { get; private set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Name == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Name");
            }
            if (Image == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Image");
            }
            if (Resources == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Resources");
            }
            if (ImageRegistryCredential != null)
            {
                ImageRegistryCredential.Validate();
            }
            if (Labels != null)
            {
                foreach (var element in Labels)
                {
                    if (element != null)
                    {
                        element.Validate();
                    }
                }
            }
            if (Endpoints != null)
            {
                foreach (var element1 in Endpoints)
                {
                    if (element1 != null)
                    {
                        element1.Validate();
                    }
                }
            }
            if (Resources != null)
            {
                Resources.Validate();
            }
            if (VolumeRefs != null)
            {
                foreach (var element2 in VolumeRefs)
                {
                    if (element2 != null)
                    {
                        element2.Validate();
                    }
                }
            }
            if (Volumes != null)
            {
                foreach (var element3 in Volumes)
                {
                    if (element3 != null)
                    {
                        element3.Validate();
                    }
                }
            }
            if (ReliableCollectionsRefs != null)
            {
                foreach (var element4 in ReliableCollectionsRefs)
                {
                    if (element4 != null)
                    {
                        element4.Validate();
                    }
                }
            }
        }
    }
}
