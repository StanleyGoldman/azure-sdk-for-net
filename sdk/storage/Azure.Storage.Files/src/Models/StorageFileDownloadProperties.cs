﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Azure.Core.Http;

namespace Azure.Storage.Files.Models
{
    /// <summary>
    /// Properties returned when downloading a File
    /// </summary>
    public partial class StorageFileDownloadProperties
    {
        /// <summary>
        /// Internal flattened property representation
        /// </summary>
        internal FlattenedStorageFileProperties _flattened;

        /// <summary>
        /// Returns the date and time the file was last modified. Any operation that modifies the file or its properties updates the last modified time.
        /// </summary>
        public DateTimeOffset LastModified => this._flattened.LastModified;

        /// <summary>
        /// A set of name-value pairs associated with this file as user-defined metadata.
        /// </summary>
        public IDictionary<string, string> Metadata => this._flattened.Metadata;

        /// <summary>
        /// The content type specified for the file. The default content type is 'application/octet-stream'
        /// </summary>
        public string ContentType => this._flattened.ContentType;

        /// <summary>
        /// Indicates the range of bytes returned if the client requested a subset of the file by setting the Range request header.
        /// </summary>
        public string ContentRange => this._flattened.ContentRange;

        /// <summary>
        /// The ETag contains a value that you can use to perform operations conditionally, in quotes.
        /// </summary>
        public ETag ETag => this._flattened.ETag;

        /// <summary>
        /// Returns the value that was specified for the Content-Encoding request header.
        /// </summary>
        public IEnumerable<string> ContentEncoding => this._flattened.ContentEncoding;

        /// <summary>
        /// Returned if it was previously specified for the file.
        /// </summary>
        public string CacheControl => this._flattened.CacheControl;

        /// <summary>
        /// Returns the value that was specified for the 'x-ms-content-disposition' header and specifies how to process the response.
        /// </summary>
        public string ContentDisposition => this._flattened.ContentDisposition;

        /// <summary>
        /// Returns the value that was specified for the Content-Language request header.
        /// </summary>
        public IEnumerable<string> ContentLanguage => this._flattened.ContentLanguage;

        /// <summary>
        /// Indicates that the service supports requests for partial file content.
        /// </summary>
        public string AcceptRanges => this._flattened.AcceptRanges;

        /// <summary>
        /// Conclusion time of the last attempted Copy File operation where this file was the destination file. This value can specify the time of a completed, aborted, or failed copy attempt.
        /// </summary>
        public DateTimeOffset CopyCompletionTime => this._flattened.CopyCompletionTime;

        /// <summary>
        /// Only appears when x-ms-copy-status is failed or pending. Describes cause of fatal or non-fatal copy operation failure.
        /// </summary>
        public string CopyStatusDescription => this._flattened.CopyStatusDescription;

        /// <summary>
        /// String identifier for the last attempted Copy File operation where this file was the destination file.
        /// </summary>
        public string CopyId => this._flattened.CopyId;

        /// <summary>
        /// Contains the number of bytes copied and the total bytes in the source in the last attempted Copy File operation where this file was the destination file. Can show between 0 and Content-Length bytes copied.
        /// </summary>
        public string CopyProgress => this._flattened.CopyProgress;

        /// <summary>
        /// URL up to 2KB in length that specifies the source file used in the last attempted Copy File operation where this file was the destination file.
        /// </summary>
        public Uri CopySource => this._flattened.CopySource;

        /// <summary>
        /// State of the copy operation identified by 'x-ms-copy-id'.
        /// </summary>
        public CopyStatus CopyStatus => this._flattened.CopyStatus;

        /// <summary>
        /// If the file has a MD5 hash, and if request contains range header (Range or x-ms-range), this response header is returned with the value of the whole file's MD5 value. This value may or may not be equal to the value returned in Content-MD5 header, with the latter calculated from the requested range.
        /// </summary>
#pragma warning disable CA1819 // Properties should not return arrays
        public byte[] FileContentHash => this._flattened.FileContentHash;
#pragma warning restore CA1819 // Properties should not return arrays

        /// <summary>
        /// The value of this header is set to true if the file data and application metadata are completely encrypted using the specified algorithm. Otherwise, the value is set to false (when the file is unencrypted, or if only parts of the file/application metadata are encrypted).
        /// </summary>
        public bool IsServerEncrypted => this._flattened.IsServerEncrypted;

        /// <summary>
        /// The SMB properties for the file
        /// </summary>
        public FileSmbProperties SmbProperties { get; set; }

        internal StorageFileDownloadProperties(FlattenedStorageFileProperties flattened)
        {
            this._flattened = flattened;
            this.SmbProperties = new FileSmbProperties(flattened);
        }
    }

    /// <summary>
    /// FilesModelFactory provides utilities for mocking.
    /// </summary>
    public static partial class FilesModelFactory
    {
        /// <summary>
        /// Creates a new StorageFileDownloadProperties instance for mocking.
        /// </summary>
        public static StorageFileDownloadProperties StorageFileDownloadProperties(
            DateTimeOffset lastModified,
            IDictionary<string, string> metadata,
            string contentType,
            string contentRange,
            ETag eTag,
            IEnumerable<string> contentEncoding,
            string cacheControl,
            string contentDisposition,
            IEnumerable<string> contentLanguage,
            string acceptRanges,
            DateTimeOffset copyCompletionTime,
            string copyStatusDescription,
            string copyId,
            string copyProgress,
            Uri copySource,
            CopyStatus copyStatus,
            byte[] fileContentHash,
            bool isServiceEncrypted)
        {
            var flattened = new FlattenedStorageFileProperties()
            {
                LastModified = lastModified,
                Metadata = metadata,
                ContentType = contentType,
                ContentRange = contentRange,
                ETag = eTag,
                ContentEncoding = contentEncoding,
                CacheControl = cacheControl,
                ContentDisposition = contentDisposition,
                ContentLanguage = contentLanguage,
                AcceptRanges = acceptRanges,
                CopyCompletionTime = copyCompletionTime,
                CopyStatusDescription = copyStatusDescription,
                CopyId = copyId,
                CopyProgress = copyProgress,
                CopySource = copySource,
                CopyStatus = copyStatus,
                FileContentHash = fileContentHash,
                IsServerEncrypted = isServiceEncrypted
            };
            return new StorageFileDownloadProperties(flattened);
        }
    }
}
