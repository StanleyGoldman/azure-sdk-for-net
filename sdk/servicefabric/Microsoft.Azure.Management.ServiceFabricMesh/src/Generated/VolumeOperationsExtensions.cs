// <auto-generated>
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Azure.Management.ServiceFabricMesh
{
    using Microsoft.Rest;
    using Microsoft.Rest.Azure;
    using Models;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for VolumeOperations.
    /// </summary>
    public static partial class VolumeOperationsExtensions
    {
            /// <summary>
            /// Creates or updates a volume resource.
            /// </summary>
            /// <remarks>
            /// Creates a volume resource with the specified name, description and
            /// properties. If a volume resource with the same name exists, then it is
            /// updated with the specified description and properties.
            /// </remarks>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='resourceGroupName'>
            /// Azure resource group name
            /// </param>
            /// <param name='volumeResourceName'>
            /// The identity of the volume.
            /// </param>
            /// <param name='volumeResourceDescription'>
            /// Description for creating a Volume resource.
            /// </param>
            public static VolumeResourceDescription Create(this IVolumeOperations operations, string resourceGroupName, string volumeResourceName, VolumeResourceDescription volumeResourceDescription)
            {
                return operations.CreateAsync(resourceGroupName, volumeResourceName, volumeResourceDescription).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Creates or updates a volume resource.
            /// </summary>
            /// <remarks>
            /// Creates a volume resource with the specified name, description and
            /// properties. If a volume resource with the same name exists, then it is
            /// updated with the specified description and properties.
            /// </remarks>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='resourceGroupName'>
            /// Azure resource group name
            /// </param>
            /// <param name='volumeResourceName'>
            /// The identity of the volume.
            /// </param>
            /// <param name='volumeResourceDescription'>
            /// Description for creating a Volume resource.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<VolumeResourceDescription> CreateAsync(this IVolumeOperations operations, string resourceGroupName, string volumeResourceName, VolumeResourceDescription volumeResourceDescription, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.CreateWithHttpMessagesAsync(resourceGroupName, volumeResourceName, volumeResourceDescription, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Gets the volume resource with the given name.
            /// </summary>
            /// <remarks>
            /// Gets the information about the volume resource with the given name. The
            /// information include the description and other properties of the volume.
            /// </remarks>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='resourceGroupName'>
            /// Azure resource group name
            /// </param>
            /// <param name='volumeResourceName'>
            /// The identity of the volume.
            /// </param>
            public static VolumeResourceDescription Get(this IVolumeOperations operations, string resourceGroupName, string volumeResourceName)
            {
                return operations.GetAsync(resourceGroupName, volumeResourceName).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Gets the volume resource with the given name.
            /// </summary>
            /// <remarks>
            /// Gets the information about the volume resource with the given name. The
            /// information include the description and other properties of the volume.
            /// </remarks>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='resourceGroupName'>
            /// Azure resource group name
            /// </param>
            /// <param name='volumeResourceName'>
            /// The identity of the volume.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<VolumeResourceDescription> GetAsync(this IVolumeOperations operations, string resourceGroupName, string volumeResourceName, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetWithHttpMessagesAsync(resourceGroupName, volumeResourceName, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Deletes the volume resource.
            /// </summary>
            /// <remarks>
            /// Deletes the volume resource identified by the name.
            /// </remarks>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='resourceGroupName'>
            /// Azure resource group name
            /// </param>
            /// <param name='volumeResourceName'>
            /// The identity of the volume.
            /// </param>
            public static void Delete(this IVolumeOperations operations, string resourceGroupName, string volumeResourceName)
            {
                operations.DeleteAsync(resourceGroupName, volumeResourceName).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Deletes the volume resource.
            /// </summary>
            /// <remarks>
            /// Deletes the volume resource identified by the name.
            /// </remarks>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='resourceGroupName'>
            /// Azure resource group name
            /// </param>
            /// <param name='volumeResourceName'>
            /// The identity of the volume.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task DeleteAsync(this IVolumeOperations operations, string resourceGroupName, string volumeResourceName, CancellationToken cancellationToken = default(CancellationToken))
            {
                (await operations.DeleteWithHttpMessagesAsync(resourceGroupName, volumeResourceName, null, cancellationToken).ConfigureAwait(false)).Dispose();
            }

            /// <summary>
            /// Gets all the volume resources in a given resource group.
            /// </summary>
            /// <remarks>
            /// Gets the information about all volume resources in a given resource group.
            /// The information include the description and other properties of the Volume.
            /// </remarks>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='resourceGroupName'>
            /// Azure resource group name
            /// </param>
            public static IPage<VolumeResourceDescription> ListByResourceGroup(this IVolumeOperations operations, string resourceGroupName)
            {
                return operations.ListByResourceGroupAsync(resourceGroupName).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Gets all the volume resources in a given resource group.
            /// </summary>
            /// <remarks>
            /// Gets the information about all volume resources in a given resource group.
            /// The information include the description and other properties of the Volume.
            /// </remarks>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='resourceGroupName'>
            /// Azure resource group name
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IPage<VolumeResourceDescription>> ListByResourceGroupAsync(this IVolumeOperations operations, string resourceGroupName, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ListByResourceGroupWithHttpMessagesAsync(resourceGroupName, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Gets all the volume resources in a given subscription.
            /// </summary>
            /// <remarks>
            /// Gets the information about all volume resources in a given resource group.
            /// The information include the description and other properties of the volume.
            /// </remarks>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static IPage<VolumeResourceDescription> ListBySubscription(this IVolumeOperations operations)
            {
                return operations.ListBySubscriptionAsync().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Gets all the volume resources in a given subscription.
            /// </summary>
            /// <remarks>
            /// Gets the information about all volume resources in a given resource group.
            /// The information include the description and other properties of the volume.
            /// </remarks>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IPage<VolumeResourceDescription>> ListBySubscriptionAsync(this IVolumeOperations operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ListBySubscriptionWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Gets all the volume resources in a given resource group.
            /// </summary>
            /// <remarks>
            /// Gets the information about all volume resources in a given resource group.
            /// The information include the description and other properties of the Volume.
            /// </remarks>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='nextPageLink'>
            /// The NextLink from the previous successful call to List operation.
            /// </param>
            public static IPage<VolumeResourceDescription> ListByResourceGroupNext(this IVolumeOperations operations, string nextPageLink)
            {
                return operations.ListByResourceGroupNextAsync(nextPageLink).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Gets all the volume resources in a given resource group.
            /// </summary>
            /// <remarks>
            /// Gets the information about all volume resources in a given resource group.
            /// The information include the description and other properties of the Volume.
            /// </remarks>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='nextPageLink'>
            /// The NextLink from the previous successful call to List operation.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IPage<VolumeResourceDescription>> ListByResourceGroupNextAsync(this IVolumeOperations operations, string nextPageLink, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ListByResourceGroupNextWithHttpMessagesAsync(nextPageLink, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Gets all the volume resources in a given subscription.
            /// </summary>
            /// <remarks>
            /// Gets the information about all volume resources in a given resource group.
            /// The information include the description and other properties of the volume.
            /// </remarks>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='nextPageLink'>
            /// The NextLink from the previous successful call to List operation.
            /// </param>
            public static IPage<VolumeResourceDescription> ListBySubscriptionNext(this IVolumeOperations operations, string nextPageLink)
            {
                return operations.ListBySubscriptionNextAsync(nextPageLink).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Gets all the volume resources in a given subscription.
            /// </summary>
            /// <remarks>
            /// Gets the information about all volume resources in a given resource group.
            /// The information include the description and other properties of the volume.
            /// </remarks>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='nextPageLink'>
            /// The NextLink from the previous successful call to List operation.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IPage<VolumeResourceDescription>> ListBySubscriptionNextAsync(this IVolumeOperations operations, string nextPageLink, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ListBySubscriptionNextWithHttpMessagesAsync(nextPageLink, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

    }
}
