﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.

using System.Text.Json;

namespace Azure.Security.KeyVault.Certificates
{
    internal class CertificateOperationUpdateParameters : IJsonSerializable
    {
        private const string CancellationRequestedPropertyName = "cancellation_requested";
        private static readonly JsonEncodedText CancellationRequestedPropertyNameBytes = JsonEncodedText.Encode(CancellationRequestedPropertyName);

        private bool _cancellationRequested;

        public CertificateOperationUpdateParameters(bool cancellationRequested)
        {
            _cancellationRequested = cancellationRequested;
        }

        void IJsonSerializable.WriteProperties(Utf8JsonWriter json)
        {
            json.WriteBoolean(CancellationRequestedPropertyNameBytes, _cancellationRequested);
        }
    }
}
