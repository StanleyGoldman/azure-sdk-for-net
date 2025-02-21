﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core.Http;
using Azure.Core.Testing;
using Azure.Storage.Common;
using Azure.Storage.Files.Models;
using Azure.Storage.Files.Tests;
using Azure.Storage.Test;
using Azure.Storage.Test.Shared;
using NUnit.Framework;
using TestConstants = Azure.Storage.Test.Constants;

namespace Azure.Storage.Files.Test
{
    public class FileClientTests : FileTestBase
    {
        public FileClientTests(bool async)
            : base(async, null /* RecordedTestMode.Record /* to re-record */)
        {
        }

        [Test]
        public void Ctor_ConnectionString()
        {
            var accountName = "accountName";
            var accountKey = Convert.ToBase64String(new byte[] { 0, 1, 2, 3, 4, 5 });

            var credentials = new StorageSharedKeyCredential(accountName, accountKey);
            var fileEndpoint = new Uri("http://127.0.0.1/" + accountName);
            var fileSecondaryEndpoint = new Uri("http://127.0.0.1/" + accountName + "-secondary");

            var connectionString = new StorageConnectionString(credentials, (default, default), (default, default), (default, default), (fileEndpoint, fileSecondaryEndpoint));

            var shareName = this.GetNewShareName();
            var filePath = this.GetNewFileName();

            var file = this.InstrumentClient(new FileClient(connectionString.ToString(true), shareName, filePath, this.GetOptions()));

            var builder = new FileUriBuilder(file.Uri);

            Assert.AreEqual(shareName, builder.ShareName);
            Assert.AreEqual(filePath, builder.DirectoryOrFilePath);
            //Assert.AreEqual("accountName", builder.AccountName);
        }

        [Test]
        public async Task CreateAsync()
        {
            using (this.GetNewDirectory(out var directory))
            {
                // Arrange
                var file = this.InstrumentClient(directory.GetFileClient(this.GetNewFileName()));

                // Act
                var response = await file.CreateAsync(maxSize: Constants.MB);

                // Assert
                AssertValidStorageFileInfo(response);
            }
        }

        [Test]
        public async Task CreateAsync_FilePermission()
        {
            using (this.GetNewDirectory(out var directory))
            {
                // Arrange
                var file = this.InstrumentClient(directory.GetFileClient(this.GetNewFileName()));
                var filePermission = "O:S-1-5-21-2127521184-1604012920-1887927527-21560751G:S-1-5-21-2127521184-1604012920-1887927527-513D:AI(A;;FA;;;SY)(A;;FA;;;BA)(A;;0x1200a9;;;S-1-5-21-397955417-626881126-188441444-3053964)";

                // Act
                var response = await file.CreateAsync(
                    maxSize: Constants.MB,
                    filePermission: filePermission);

                // Assert
                AssertValidStorageFileInfo(response);
            }
        }

        [Test]
        public async Task CreateAsync_FilePermissionAndFilePermissionKeySet()
        {
            using (this.GetNewDirectory(out var directory))
            {
                // Arrange
                var file = this.InstrumentClient(directory.GetFileClient(this.GetNewFileName()));
                var filePermission = "O:S-1-5-21-2127521184-1604012920-1887927527-21560751G:S-1-5-21-2127521184-1604012920-1887927527-513D:AI(A;;FA;;;SY)(A;;FA;;;BA)(A;;0x1200a9;;;S-1-5-21-397955417-626881126-188441444-3053964)";
                var fileSmbProperties = new FileSmbProperties()
                {
                    FilePermissionKey = "filePermissionKey"
                };

                // Act
                await TestHelper.AssertExpectedExceptionAsync<ArgumentException>(
                    file.CreateAsync(
                        maxSize: Constants.MB,
                        smbProperties: fileSmbProperties,
                        filePermission: filePermission),
                    e => Assert.AreEqual("filePermission and filePermissionKey cannot both be set", e.Message));
            }
        }

        [Test]
        public async Task CreateAsync_FilePermissionTooLarge()
        {
            using (this.GetNewDirectory(out var directory))
            {
                // Arrange
                var file = this.InstrumentClient(directory.GetFileClient(this.GetNewFileName()));
                var filePermission = new string('*', 9 * Constants.KB);

                // Act
                await TestHelper.AssertExpectedExceptionAsync<ArgumentOutOfRangeException>(
                    file.CreateAsync(
                        maxSize: Constants.MB,
                        filePermission: filePermission),
                    e => Assert.AreEqual(
                        "Value must be less than or equal to 8192" + Environment.NewLine 
                        + "Parameter name: filePermission", e.Message));
            }
        }

        [Test]
        public async Task CreateAsync_SmbProperties()
        {
            using (this.GetNewShare(out var share))
            {
                // Arrange
                var permission = "O:S-1-5-21-2127521184-1604012920-1887927527-21560751G:S-1-5-21-2127521184-1604012920-1887927527-513D:AI(A;;FA;;;SY)(A;;FA;;;BA)(A;;0x1200a9;;;S-1-5-21-397955417-626881126-188441444-3053964)";
                var createPermissionResponse = await share.CreatePermissionAsync(permission);

                var directory = this.InstrumentClient(share.GetDirectoryClient(this.GetNewDirectoryName()));
                await directory.CreateAsync();

                var file = this.InstrumentClient(directory.GetFileClient(this.GetNewFileName()));
                var smbProperties = new FileSmbProperties
                {
                    FilePermissionKey = createPermissionResponse.Value.FilePermissionKey,
                    FileAttributes = NtfsFileAttributes.Parse("Archive|ReadOnly"),
                    FileCreationTime = new DateTimeOffset(2019, 8, 15, 5, 15, 25, 60, TimeSpan.Zero),
                    FileLastWriteTime = new DateTimeOffset(2019, 8, 26, 5, 15, 25, 60, TimeSpan.Zero),
                };

                // Act
                var response = await file.CreateAsync(
                    maxSize: Constants.KB,
                    smbProperties: smbProperties);

                // Assert
                AssertValidStorageFileInfo(response);
                Assert.AreEqual(smbProperties.FileAttributes, response.Value.SmbProperties.Value.FileAttributes);
                Assert.AreEqual(smbProperties.FileCreationTime, response.Value.SmbProperties.Value.FileCreationTime);
                Assert.AreEqual(smbProperties.FileLastWriteTime, response.Value.SmbProperties.Value.FileLastWriteTime);
            }
        }

        [Test]
        public async Task CreateAsync_Metadata()
        {
            using (this.GetNewDirectory(out var directory))
            {
                // Arrange
                var file = this.InstrumentClient(directory.GetFileClient(this.GetNewFileName()));
                var metadata = this.BuildMetadata();

                // Act
                await file.CreateAsync(
                    maxSize: Constants.MB,
                    metadata: metadata);

                // Assert
                var response = await file.GetPropertiesAsync();
                this.AssertMetadataEquality(metadata, response.Value.Metadata);
            }
        }

        [Test]
        public async Task CreateAsync_Headers()
        {
            var constants = new TestConstants(this);
            using (this.GetNewDirectory(out var directory))
            {
                // Arrange
                var file = this.InstrumentClient(directory.GetFileClient(this.GetNewFileName()));

                // Act
                await file.CreateAsync(
                    maxSize: Constants.MB,
                    httpHeaders: new FileHttpHeaders
                    {
                        CacheControl = constants.CacheControl,
                        ContentDisposition = constants.ContentDisposition,
                        ContentEncoding = new string[] { constants.ContentEncoding },
                        ContentLanguage = new string[] { constants.ContentLanguage },
                        ContentHash = constants.ContentMD5,
                        ContentType = constants.ContentType
                    });

                // Assert
                var response = await file.GetPropertiesAsync();
                Assert.AreEqual(constants.ContentType, response.Value.ContentType);
                TestHelper.AssertSequenceEqual(constants.ContentMD5.ToList(), response.Value.ContentHash.ToList());
                Assert.AreEqual(1, response.Value.ContentEncoding.Count());
                Assert.AreEqual(constants.ContentEncoding, response.Value.ContentEncoding.First());
                Assert.AreEqual(1, response.Value.ContentLanguage.Count());
                Assert.AreEqual(constants.ContentLanguage, response.Value.ContentLanguage.First());
                Assert.AreEqual(constants.ContentDisposition, response.Value.ContentDisposition);
                Assert.AreEqual(constants.CacheControl, response.Value.CacheControl);
            }
        }

        [Test]
        public async Task CreateAsync_Error()
        {
            using (this.GetNewShare(out var share))
            {
                // Arrange
                var directory = this.InstrumentClient(share.GetDirectoryClient(this.GetNewDirectoryName()));
                var file = this.InstrumentClient(directory.GetFileClient(this.GetNewFileName()));

                // Act
                await TestHelper.AssertExpectedExceptionAsync<StorageRequestFailedException>(
                    file.CreateAsync(maxSize: Constants.KB),
                    e => Assert.AreEqual("ParentNotFound", e.ErrorCode.Split('\n')[0]));
            }
        }

        [Test]
        public async Task SetMetadataAsync()
        {
            using (this.GetNewFile(out var file))
            {
                // Arrange
                var metadata = this.BuildMetadata();

                // Act
                await file.SetMetadataAsync(metadata);

                // Assert
                var response = await file.GetPropertiesAsync();
                this.AssertMetadataEquality(metadata, response.Value.Metadata);
            }
        }

        [Test]
        public async Task SetMetadataAsync_Error()
        {
            using (this.GetNewDirectory(out var directory))
            {
                // Arrange
                var file = this.InstrumentClient(directory.GetFileClient(this.GetNewFileName()));
                var metadata = this.BuildMetadata();

                // Act
                await TestHelper.AssertExpectedExceptionAsync<StorageRequestFailedException>(
                    file.SetMetadataAsync(metadata),
                    e => Assert.AreEqual("ResourceNotFound", e.ErrorCode.Split('\n')[0]));
            }
        }

        [Test]
        public async Task GetPropertiesAsync()
        {
            using (this.GetNewDirectory(out var directory))
            {
                // Arrange
                var file = this.InstrumentClient(directory.GetFileClient(this.GetNewFileName()));

                // Act
                var createResponse = await file.CreateAsync(maxSize: Constants.KB);
                var getPropertiesResponse = await file.GetPropertiesAsync();

                // Assert
                Assert.AreEqual(createResponse.Value.ETag, getPropertiesResponse.Value.ETag);
                Assert.AreEqual(createResponse.Value.LastModified, getPropertiesResponse.Value.LastModified);
                Assert.AreEqual(createResponse.Value.IsServerEncrypted, getPropertiesResponse.Value.IsServerEncrypted);
                Assert.AreEqual(createResponse.Value.SmbProperties, getPropertiesResponse.Value.SmbProperties);
            }
        }

        [Test]
        public async Task GetPropertiesAsync_ShareSAS()
        {
            var shareName = this.GetNewShareName();
            var directoryName = this.GetNewDirectoryName();
            var fileName = this.GetNewFileName();
            using (this.GetNewFile(out _, shareName: shareName, directoryName: directoryName, fileName: fileName))
            {
                // Arrange
                var sasFile = this.InstrumentClient(
                    this.GetServiceClient_FileServiceSasShare(shareName)
                    .GetShareClient(shareName)
                    .GetDirectoryClient(directoryName)
                    .GetFileClient(fileName));

                // Act
                var response = await sasFile.GetPropertiesAsync();

                // Assert
                Assert.IsNotNull(response.GetRawResponse().Headers.RequestId);
            }
        }

        [Test]
        public async Task GetPropertiesAsync_FileSAS()
        {
            var shareName = this.GetNewShareName();
            var directoryName = this.GetNewDirectoryName();
            var fileName = this.GetNewFileName();
            using (this.GetNewFile(out _, shareName: shareName, directoryName: directoryName, fileName: fileName))
            {
                // Arrange
                var sasFile = this.InstrumentClient(
                    this.GetServiceClient_FileServiceSasFile(shareName, directoryName + "/" + fileName)
                    .GetShareClient(shareName)
                    .GetDirectoryClient(directoryName)
                    .GetFileClient(fileName));

                // Act
                var response = await sasFile.GetPropertiesAsync();

                // Assert
                Assert.IsNotNull(response.GetRawResponse().Headers.RequestId);
            }
        }

        [Test]
        public async Task GetPropertiesAsync_Error()
        {
            using (this.GetNewDirectory(out var directory))
            {
                // Arrange
                var file = this.InstrumentClient(directory.GetFileClient(this.GetNewFileName()));

                // Act
                await TestHelper.AssertExpectedExceptionAsync<StorageRequestFailedException>(
                    file.GetPropertiesAsync(),
                    e =>
                    {
                        Assert.AreEqual("ResourceNotFound", e.ErrorCode.Split('\n')[0]);
                        if (this.Mode != RecordedTestMode.Playback)
                        {
                            // The MockResponse type doesn't supply the ReasonPhrase we're
                            // checking for with this test
                            Assert.AreEqual("The specified resource does not exist.", e.Message.Split('(')[1].Split(')')[0].Trim());
                        }
                    });
            }
        }

        [Test]
        public async Task SetHttpHeadersAsync()
        {
            var constants = new TestConstants(this);
            using (this.GetNewFile(out var file))
            {
                // Act
                await file.SetHttpHeadersAsync(
                    httpHeaders: new FileHttpHeaders
                    {
                        CacheControl = constants.CacheControl,
                        ContentDisposition = constants.ContentDisposition,
                        ContentEncoding = new string[] { constants.ContentEncoding },
                        ContentLanguage = new string[] { constants.ContentLanguage },
                        ContentHash = constants.ContentMD5,
                        ContentType = constants.ContentType
                    });

                // Assert
                var response = await file.GetPropertiesAsync();
                Assert.AreEqual(constants.ContentType, response.Value.ContentType);
                TestHelper.AssertSequenceEqual(constants.ContentMD5.ToList(), response.Value.ContentHash.ToList());
                Assert.AreEqual(1, response.Value.ContentEncoding.Count());
                Assert.AreEqual(constants.ContentEncoding, response.Value.ContentEncoding.First());
                Assert.AreEqual(1, response.Value.ContentLanguage.Count());
                Assert.AreEqual(constants.ContentLanguage, response.Value.ContentLanguage.First());
                Assert.AreEqual(constants.ContentDisposition, response.Value.ContentDisposition);
                Assert.AreEqual(constants.CacheControl, response.Value.CacheControl);
            }
        }

        [Test]
        public async Task SetPropertiesAsync_FilePermission()
        {
            using (this.GetNewDirectory(out var directory))
            {
                // Arrange
                var file = this.InstrumentClient(directory.GetFileClient(this.GetNewFileName()));
                var filePermission = "O:S-1-5-21-2127521184-1604012920-1887927527-21560751G:S-1-5-21-2127521184-1604012920-1887927527-513D:AI(A;;FA;;;SY)(A;;FA;;;BA)(A;;0x1200a9;;;S-1-5-21-397955417-626881126-188441444-3053964)";
                await file.CreateAsync(maxSize: Constants.KB);

                // Act
                var response = await file.SetHttpHeadersAsync(filePermission: filePermission);

                // Assert
                AssertValidStorageFileInfo(response);
            }
        }

        [Test]
        public async Task SetPropertiesAsync_SmbProperties()
        {
            using (this.GetNewShare(out var share))
            {
                // Arrange
                var permission = "O:S-1-5-21-2127521184-1604012920-1887927527-21560751G:S-1-5-21-2127521184-1604012920-1887927527-513D:AI(A;;FA;;;SY)(A;;FA;;;BA)(A;;0x1200a9;;;S-1-5-21-397955417-626881126-188441444-3053964)";
                var createPermissionResponse = await share.CreatePermissionAsync(permission);

                var directory = this.InstrumentClient(share.GetDirectoryClient(this.GetNewDirectoryName()));
                await directory.CreateAsync();

                var file = this.InstrumentClient(directory.GetFileClient(this.GetNewFileName()));
                var smbProperties = new FileSmbProperties
                {
                    FilePermissionKey = createPermissionResponse.Value.FilePermissionKey,
                    FileAttributes = NtfsFileAttributes.Parse("Archive|ReadOnly"),
                    FileCreationTime = new DateTimeOffset(2019, 8, 15, 5, 15, 25, 60, TimeSpan.Zero),
                    FileLastWriteTime = new DateTimeOffset(2019, 8, 26, 5, 15, 25, 60, TimeSpan.Zero),
                };


                await file.CreateAsync(maxSize: Constants.KB);

                // Act
                var response = await file.SetHttpHeadersAsync(smbProperties: smbProperties);

                // Assert
                AssertValidStorageFileInfo(response);
                Assert.AreEqual(smbProperties.FileAttributes, response.Value.SmbProperties.Value.FileAttributes);
                Assert.AreEqual(smbProperties.FileCreationTime, response.Value.SmbProperties.Value.FileCreationTime);
                Assert.AreEqual(smbProperties.FileLastWriteTime, response.Value.SmbProperties.Value.FileLastWriteTime);
            }
        }

        [Test]
        public async Task SetPropertiesAsync_FilePermissionTooLong()
        {
            var constants = new TestConstants(this);
            using (this.GetNewDirectory(out var directory))
            {
                // Arrange
                var file = this.InstrumentClient(directory.GetFileClient(this.GetNewFileName()));
                var filePermission = new string('*', 9 * Constants.KB);
                await file.CreateAsync(maxSize: Constants.KB);

                // Act
                await TestHelper.AssertExpectedExceptionAsync<ArgumentOutOfRangeException>(
                    file.SetHttpHeadersAsync(
                        filePermission: filePermission),
                    e => Assert.AreEqual(
                        "Value must be less than or equal to 8192" + Environment.NewLine
                        + "Parameter name: filePermission", e.Message));
            }
        }

        [Test]
        public async Task SetPropertiesAsync_FilePermissionAndFilePermissionKeySet()
        {
            using (this.GetNewDirectory(out var directory))
            {
                // Arrange
                var file = this.InstrumentClient(directory.GetFileClient(this.GetNewFileName()));
                await file.CreateAsync(maxSize: Constants.KB);

                var filePermission = "O:S-1-5-21-2127521184-1604012920-1887927527-21560751G:S-1-5-21-2127521184-1604012920-1887927527-513D:AI(A;;FA;;;SY)(A;;FA;;;BA)(A;;0x1200a9;;;S-1-5-21-397955417-626881126-188441444-3053964)";
                var fileSmbProperties = new FileSmbProperties()
                {
                    FilePermissionKey = "filePermissionKey"
                };

                // Act
                await TestHelper.AssertExpectedExceptionAsync<ArgumentException>(
                    file.SetHttpHeadersAsync(
                        smbProperties: fileSmbProperties,
                        filePermission: filePermission),
                    e => Assert.AreEqual("filePermission and filePermissionKey cannot both be set", e.Message));
            }
        }

        [Test]
        public async Task SetPropertiesAsync_Error()
        {
            var constants = new TestConstants(this);
            using (this.GetNewDirectory(out var directory))
            {
                // Arrange
                var file = this.InstrumentClient(directory.GetFileClient(this.GetNewFileName()));

                // Act
                await TestHelper.AssertExpectedExceptionAsync<StorageRequestFailedException>(
                    file.SetHttpHeadersAsync(
                        httpHeaders: new FileHttpHeaders
                        {
                            CacheControl = constants.CacheControl,
                            ContentDisposition = constants.ContentDisposition,
                            ContentEncoding = new string[] { constants.ContentEncoding },
                            ContentLanguage = new string[] { constants.ContentLanguage },
                            ContentHash = constants.ContentMD5,
                            ContentType = constants.ContentType
                        }),
                    e => Assert.AreEqual("ResourceNotFound", e.ErrorCode.Split('\n')[0]));
            }
        }

        [Test]
        public async Task DeleteAsync()
        {
            using (this.GetNewFile(out var file))
            {
                // Act
                var response = await file.DeleteAsync();

                // Assert
                Assert.IsNotNull(response.Headers.RequestId);
            }
        }

        [Test]
        public async Task DeleteAsync_Error()
        {
            using (this.GetNewDirectory(out var directory))
            {
                // Arrange
                var file = this.InstrumentClient(directory.GetFileClient(this.GetNewFileName()));

                // Act
                await TestHelper.AssertExpectedExceptionAsync<StorageRequestFailedException>(
                    file.DeleteAsync(),
                    e => Assert.AreEqual("ResourceNotFound", e.ErrorCode.Split('\n')[0]));
            }
        }

        [Test]
        public async Task StartCopyAsync()
        {
            using (this.GetNewFile(out var source))
            using (this.GetNewFile(out var dest))
            {
                // Arrange
                var data = this.GetRandomBuffer(Constants.KB);

                using (var stream = new MemoryStream(data))
                {
                    await source.UploadRangeAsync(
                        writeType: FileRangeWriteType.Update,
                        range: new HttpRange(0, Constants.KB),
                        content: stream);
                }

                // Act
                var response = await dest.StartCopyAsync(source.Uri);

                // Assert
                Assert.IsNotNull(response.GetRawResponse().Headers.RequestId);
            }
        }

        [Test]
        public async Task StartCopyAsync_Metata()
        {
            using (this.GetNewFile(out var source))
            using (this.GetNewFile(out var dest))
            {
                // Arrange
                await source.CreateAsync(maxSize: Constants.MB);
                var data = this.GetRandomBuffer(Constants.KB);

                using (var stream = new MemoryStream(data))
                {
                    await source.UploadRangeAsync(
                        writeType: FileRangeWriteType.Update,
                        range: new HttpRange(0, Constants.KB),
                        content: stream);
                }

                var metadata = this.BuildMetadata();

                // Act
                var copyResponse = await dest.StartCopyAsync(
                    sourceUri: source.Uri,
                    metadata: metadata);

                await this.WaitForCopy(dest);

                // Assert
                var response = await dest.GetPropertiesAsync();
                this.AssertMetadataEquality(metadata, response.Value.Metadata);
            }
        }

        [Test]
        public async Task StartCopyAsync_Error()
        {
            using (this.GetNewFile(out var file))
            {
                // Act
                await TestHelper.AssertExpectedExceptionAsync<StorageRequestFailedException>(
                    file.StartCopyAsync(sourceUri: InvalidUri),
                    e => Assert.AreEqual("CannotVerifyCopySource", e.ErrorCode.Split('\n')[0]));
            }
        }

        [Test]
        public async Task AbortCopyAsync()
        {
            using (this.GetNewDirectory(out var directory))
            {
                // Arrange
                var source = this.InstrumentClient(directory.GetFileClient(this.GetNewFileName()));
                await source.CreateAsync(maxSize: Constants.MB);
                var data = this.GetRandomBuffer(Constants.MB);

                using (var stream = new MemoryStream(data))
                {
                    await source.UploadRangeAsync(
                        writeType: FileRangeWriteType.Update,
                        range: new HttpRange(0, Constants.MB),
                        content: stream);
                }

                var dest = this.InstrumentClient(directory.GetFileClient(this.GetNewFileName()));
                await dest.CreateAsync(maxSize: Constants.MB);
                var copyResponse = await dest.StartCopyAsync(source.Uri);

                // Act
                try
                {
                    var response = await dest.AbortCopyAsync(copyResponse.Value.CopyId);

                    // Assert
                    Assert.IsNotNull(response.Headers.RequestId);
                }
                catch (StorageRequestFailedException e) when (e.ErrorCode == "NoPendingCopyOperation")
                {
                    // This exception is intentionally.  It is difficult to test AbortCopyAsync() in a deterministic way.
                    // this.WarnCopyCompletedTooQuickly();
                }
            }
        }

        [Test]
        public async Task AbortCopyAsync_Error()
        {
            using (this.GetNewDirectory(out var directory))
            {
                // Arrange
                var file = this.InstrumentClient(directory.GetFileClient(this.GetNewFileName()));
                await file.CreateAsync(maxSize: Constants.MB);

                // Act
                await TestHelper.AssertExpectedExceptionAsync<StorageRequestFailedException>(
                    file.AbortCopyAsync("id"),
                    e => Assert.AreEqual("InvalidQueryParameterValue", e.ErrorCode.Split('\n')[0]));
            }
        }

        [Test]
        public void WithSnapshot()
        {
            var shareName = this.GetNewShareName();
            var directoryName = this.GetNewDirectoryName();
            var fileName = this.GetNewFileName();

            var service = this.GetServiceClient_SharedKey();

            var share = this.InstrumentClient(service.GetShareClient(shareName));

            var directory = this.InstrumentClient(share.GetDirectoryClient(directoryName));

            var file = this.InstrumentClient(directory.GetFileClient(fileName));

            var builder = new FileUriBuilder(file.Uri);

            Assert.AreEqual("", builder.Snapshot);

            file = this.InstrumentClient(file.WithSnapshot("foo"));

            builder = new FileUriBuilder(file.Uri);

            Assert.AreEqual("foo", builder.Snapshot);

            file = this.InstrumentClient(file.WithSnapshot(null));

            builder = new FileUriBuilder(file.Uri);

            Assert.AreEqual("", builder.Snapshot);
        }

        [Test]
        public async Task DownloadAsync()
        {
            // Arrange
            var data = this.GetRandomBuffer(Constants.KB);
            using (this.GetNewFile(out var file))
            using (var stream = new MemoryStream(data))
            {
                await file.UploadRangeAsync(
                    writeType: FileRangeWriteType.Update,
                    range: new HttpRange(Constants.KB, data.LongLength),
                    content: stream);

                // Act
                var getPropertiesResponse = await file.GetPropertiesAsync();
                var downloadResponse = await file.DownloadAsync(range: new HttpRange(Constants.KB, data.LongLength));

                // Assert

                // Content is equal
                Assert.AreEqual(data.Length, downloadResponse.Value.ContentLength);
                var actual = new MemoryStream();
                await downloadResponse.Value.Content.CopyToAsync(actual);
                TestHelper.AssertSequenceEqual(data, actual.ToArray());

                // Properties are equal
                Assert.AreEqual(getPropertiesResponse.Value.LastModified, downloadResponse.Value.Properties.LastModified);
                this.AssertMetadataEquality(getPropertiesResponse.Value.Metadata, downloadResponse.Value.Properties.Metadata);
                Assert.AreEqual(getPropertiesResponse.Value.ContentType, downloadResponse.Value.Properties.ContentType);
                Assert.AreEqual(getPropertiesResponse.Value.ETag, downloadResponse.Value.Properties.ETag);
                Assert.AreEqual(getPropertiesResponse.Value.ContentEncoding, downloadResponse.Value.Properties.ContentEncoding);
                Assert.AreEqual(getPropertiesResponse.Value.CacheControl, downloadResponse.Value.Properties.CacheControl);
                Assert.AreEqual(getPropertiesResponse.Value.ContentDisposition, downloadResponse.Value.Properties.ContentDisposition);
                Assert.AreEqual(getPropertiesResponse.Value.ContentLanguage, downloadResponse.Value.Properties.ContentLanguage);
                Assert.AreEqual(getPropertiesResponse.Value.CopyCompletionTime, downloadResponse.Value.Properties.CopyCompletionTime);
                Assert.AreEqual(getPropertiesResponse.Value.CopyStatusDescription, downloadResponse.Value.Properties.CopyStatusDescription);
                Assert.AreEqual(getPropertiesResponse.Value.CopyId, downloadResponse.Value.Properties.CopyId);
                Assert.AreEqual(getPropertiesResponse.Value.CopyProgress, downloadResponse.Value.Properties.CopyProgress);
                Assert.AreEqual(getPropertiesResponse.Value.CopySource, downloadResponse.Value.Properties.CopySource);
                Assert.AreEqual(getPropertiesResponse.Value.CopyStatus, downloadResponse.Value.Properties.CopyStatus);
                Assert.AreEqual(getPropertiesResponse.Value.IsServerEncrypted, downloadResponse.Value.Properties.IsServerEncrypted);
                Assert.AreEqual(getPropertiesResponse.Value.SmbProperties, downloadResponse.Value.Properties.SmbProperties);
            }
        }

        [Test]
        public async Task DownloadAsync_WithUnreliableConnection()
        {
            var fileSize = 2 * Constants.MB;
            var dataSize = 1 * Constants.MB;
            var offset = 512 * Constants.KB;

            using (this.GetNewShare(out var share))
            {
                var directory = this.InstrumentClient(share.GetDirectoryClient(this.GetNewDirectoryName()));
                var directoryFaulty = this.InstrumentClient(
                    new DirectoryClient(
                        directory.Uri,
                        new StorageSharedKeyCredential(this.TestConfigDefault.AccountName, this.TestConfigDefault.AccountKey),
                        this.GetFaultyFileConnectionOptions(raiseAt: 256 * Constants.KB)));

                await directory.CreateAsync();

                // Arrange
                var fileName = this.GetNewFileName();
                var fileFaulty = this.InstrumentClient(directoryFaulty.GetFileClient(fileName));
                var file = this.InstrumentClient(directory.GetFileClient(fileName));
                await file.CreateAsync(maxSize: fileSize);

                var data = this.GetRandomBuffer(dataSize);

                // Act
                using (var stream = new MemoryStream(data))
                {
                    await fileFaulty.UploadRangeAsync(
                        writeType: FileRangeWriteType.Update,
                        range: new HttpRange(offset, dataSize),
                        content: stream);
                }

                // Assert
                var downloadResponse = await fileFaulty.DownloadAsync(range: new HttpRange(offset, data.LongLength));
                var actual = new MemoryStream();
                await downloadResponse.Value.Content.CopyToAsync(actual, 128 * Constants.KB);
                TestHelper.AssertSequenceEqual(data, actual.ToArray());
            }
        }

        [Test]
        public async Task GetRangeListAsync()
        {
            using (this.GetNewFile(out var file))
            {
                var response = await file.GetRangeListAsync(range: new HttpRange(0, Constants.MB));

                Assert.IsNotNull(response);
            }
        }

        [Test]
        public async Task GetRangeListAsync_Error()
        {
            using (this.GetNewDirectory(out var directory))
            {
                // Arrange
                var file = this.InstrumentClient(directory.GetFileClient(this.GetNewFileName()));

                // Act
                await TestHelper.AssertExpectedExceptionAsync<StorageRequestFailedException>(
                    file.GetRangeListAsync(range: new HttpRange(0, Constants.MB)),
                    e => Assert.AreEqual("ResourceNotFound", e.ErrorCode.Split('\n')[0]));
            }
        }

        [Test]
        public async Task UploadRangeAsync()
        {
            var data = this.GetRandomBuffer(Constants.KB);

            using (this.GetNewFile(out var file))
            using (var stream = new MemoryStream(data))
            {
                var response = await file.UploadRangeAsync(
                    writeType: FileRangeWriteType.Update,
                    range: new HttpRange(Constants.KB, Constants.KB),
                    content: stream);

                Assert.IsNotNull(response.GetRawResponse().Headers.RequestId);
            }
        }

        [Test]
        public async Task UploadRangeAsync_Error()
        {
            using (this.GetNewDirectory(out var directory))
            {
                // Arrange
                var file = this.InstrumentClient(directory.GetFileClient(this.GetNewFileName()));
                var data = this.GetRandomBuffer(Constants.KB);

                using (var stream = new MemoryStream(data))
                {
                    // Act
                    await TestHelper.AssertExpectedExceptionAsync<StorageRequestFailedException>(
                    file.UploadRangeAsync(
                        writeType: FileRangeWriteType.Update,
                        range: new HttpRange(Constants.KB, Constants.KB),
                        content: stream),
                    e => Assert.AreEqual("ResourceNotFound", e.ErrorCode.Split('\n')[0]));
                }
            }
        }

        [Test]
        [TestCase(512)]
        [TestCase(1 * Constants.KB)]
        [TestCase(2 * Constants.KB)]
        [TestCase(4 * Constants.KB)]
        [TestCase(10 * Constants.KB)]
        [TestCase(20 * Constants.KB)]
        [TestCase(30 * Constants.KB)]
        [TestCase(50 * Constants.KB)]
        [TestCase(501 * Constants.KB)]
        public async Task UploadAsync_SmallBlobs(int size) =>
            // Use a 1KB threshold so we get a lot of individual blocks
            await this.UploadAndVerify(size, Constants.KB);

        [Test]
        [LiveOnly]
        [TestCase(33 * Constants.MB)]
        [TestCase(257 * Constants.MB)]
        [TestCase(1 * Constants.GB)]
        public async Task UploadAsync_LargeBlobs(int size) =>
            // TODO: #6781 We don't want to add 1GB of random data in the recordings
            await this.UploadAndVerify(size, Constants.MB);

        private async Task UploadAndVerify(long size, int singleRangeThreshold)
        {
            var data = this.GetRandomBuffer(size);
            using (this.GetNewShare(out var share))
            {
                var name = this.GetNewFileName();
                var file = this.InstrumentClient(share.GetRootDirectoryClient().GetFileClient(name));
                await file.CreateAsync(size);
                using (var stream = new MemoryStream(data))
                {
                    await file.UploadInternal(
                        content: stream,
                        progressHandler: default,
                        singleRangeThreshold: singleRangeThreshold,
                        async: true,
                        cancellationToken: CancellationToken.None);
                }

                using var bufferedContent = new MemoryStream();
                var download = await file.DownloadAsync();
                await download.Value.Content.CopyToAsync(bufferedContent);
                TestHelper.AssertSequenceEqual(data, bufferedContent.ToArray());
            }
        }

        [Test]
        public async Task UploadRangeAsync_WithUnreliableConnection()
        {
            var fileSize = 2 * Constants.MB;
            var dataSize = 1 * Constants.MB;
            var offset = 512 * Constants.KB;

            using (this.GetNewShare(out var share))
            {
                var directory = this.InstrumentClient(share.GetDirectoryClient(this.GetNewDirectoryName()));
                var directoryFaulty = this.InstrumentClient(
                    new DirectoryClient(
                        directory.Uri,
                        new StorageSharedKeyCredential(
                            this.TestConfigDefault.AccountName,
                            this.TestConfigDefault.AccountKey),
                        this.GetFaultyFileConnectionOptions()));

                await directory.CreateAsync();

                // Arrange
                var fileName = this.GetNewFileName();
                var fileFaulty = this.InstrumentClient(directoryFaulty.GetFileClient(fileName));
                var file = this.InstrumentClient(directory.GetFileClient(fileName));
                await file.CreateAsync(maxSize: fileSize);

                var data = this.GetRandomBuffer(dataSize);
                var progressList = new List<StorageProgress>();
                var progressHandler = new Progress<StorageProgress>(progress => { progressList.Add(progress); /*logger.LogTrace("Progress: {progress}", progress.BytesTransferred);*/ });

                // Act
                using (var stream = new FaultyStream(new MemoryStream(data), 256 * Constants.KB, 1, new Exception("Simulated stream fault")))
                {
                    var result = await fileFaulty.UploadRangeAsync(
                        writeType: FileRangeWriteType.Update,
                        range: new HttpRange(offset, dataSize),
                        content: stream,
                        progressHandler: progressHandler);

                    Assert.IsNotNull(result);
                    Assert.IsNotNull(result.GetRawResponse().Headers.Date);
                    Assert.IsNotNull(result.GetRawResponse().Headers.RequestId);
                    result.GetRawResponse().Headers.TryGetValue("x-ms-version", out var version);
                    Assert.IsNotNull(version);

                    await this.WaitForProgressAsync(progressList, data.LongLength);
                    Assert.IsTrue(progressList.Count > 1, "Too few progress received");
                    Assert.GreaterOrEqual(data.LongLength, progressList.Last().BytesTransferred, "Final progress has unexpected value");
                }

                // Assert
                var downloadResponse = await file.DownloadAsync(range: new HttpRange(offset, data.LongLength));
                var actual = new MemoryStream();
                await downloadResponse.Value.Content.CopyToAsync(actual);
                TestHelper.AssertSequenceEqual(data, actual.ToArray());
            }
        }

        [Test]
        [LiveOnly]
        // TODO: #7645
        public async Task UploadRangeFromUriAsync()
        {
            var shareName = this.GetNewShareName();
            using (this.GetNewShare(out var share, shareName))
            {
                // Arrange
                var directoryName = this.GetNewDirectoryName();
                var directory = this.InstrumentClient(share.GetDirectoryClient(directoryName));
                await directory.CreateAsync();

                var fileName = this.GetNewFileName();
                var data = this.GetRandomBuffer(Constants.KB);
                var sourceFile = this.InstrumentClient(directory.GetFileClient(fileName));
                await sourceFile.CreateAsync(maxSize: 1024);
                using (var stream = new MemoryStream(data))
                {
                    await sourceFile.UploadRangeAsync(FileRangeWriteType.Update, new HttpRange(0, 1024), stream);
                }

                var destFile = directory.GetFileClient("destFile");
                await destFile.CreateAsync(maxSize: 1024);
                var destRange = new HttpRange(256, 256);
                var sourceRange = new HttpRange(512, 256);

                var sasFile = this.InstrumentClient(
                    this.GetServiceClient_FileServiceSasShare(shareName)
                    .GetShareClient(shareName)
                    .GetDirectoryClient(directoryName)
                    .GetFileClient(fileName));

                // Act
                await destFile.UploadRangeFromUriAsync(
                    sourceUri: sasFile.Uri,
                    range: destRange,
                    sourceRange: sourceRange);

                // Assert
                var sourceDownloadResponse = await sourceFile.DownloadAsync(range: sourceRange);
                var destDownloadResponse = await destFile.DownloadAsync(range: destRange);

                var sourceStream = new MemoryStream();
                await sourceDownloadResponse.Value.Content.CopyToAsync(sourceStream);

                var destStream = new MemoryStream();
                await destDownloadResponse.Value.Content.CopyToAsync(destStream);

                TestHelper.AssertSequenceEqual(sourceStream.ToArray(), destStream.ToArray());
            }
        }

        [Test]
        public async Task UploadRangeFromUriAsync_Error()
        {
            var shareName = this.GetNewShareName();
            using (this.GetNewShare(out var share, shareName))
            {
                // Arrange
                var directoryName = this.GetNewDirectoryName();
                var directory = this.InstrumentClient(share.GetDirectoryClient(directoryName));
                await directory.CreateAsync();

                var fileName = this.GetNewFileName();
                var sourceFile = this.InstrumentClient(directory.GetFileClient(fileName));

                var destFile = directory.GetFileClient("destFile");
                await destFile.CreateAsync(maxSize: 1024);
                var destRange = new HttpRange(256, 256);
                var sourceRange = new HttpRange(512, 256);


                // Act
                await TestHelper.AssertExpectedExceptionAsync<StorageRequestFailedException>(
                    destFile.UploadRangeFromUriAsync(
                    sourceUri: destFile.Uri,
                    range: destRange,
                    sourceRange: sourceRange),
                    e => Assert.AreEqual("CannotVerifyCopySource", e.ErrorCode));
            }
        }

        [Test]
        public async Task ListHandles()
        {
            // Arrange
            using (this.GetNewFile(out var file))
            {
                // Act
                var handles = await file.GetHandlesAsync().ToListAsync();

                // Assert
                Assert.AreEqual(0, handles.Count);
            }
        }

        [Test]
        public async Task ListHandles_Min()
        {
            // Arrange
            using (this.GetNewFile(out var file))
            {
                // Act
                var handles = await file.GetHandlesAsync().ToListAsync();

                // Assert
                Assert.AreEqual(0, handles.Count);
            }
        }

        [Test]
        public async Task ListHandles_Error()
        {
            // Arrange
            using (this.GetNewDirectory(out var directory))
            {
                var file = this.InstrumentClient(directory.GetFileClient(this.GetNewDirectoryName()));

                // Act
                await TestHelper.AssertExpectedExceptionAsync<StorageRequestFailedException>(
                    file.GetHandlesAsync().ToListAsync(),
                    actualException => Assert.AreEqual("ResourceNotFound", actualException.ErrorCode));

            }
        }

        [Test]
        public async Task ForceCloseHandles_Min()
        {
            // Arrange
            using (this.GetNewFile(out var file))
            {
                // Act
                var response = await file.ForceCloseHandlesAsync();

                // Assert
                Assert.AreEqual(0, response.Value.NumberOfHandlesClosed);
            }
        }

        [Test]
        public async Task ForceCloseHandles_Error()
        {
            // Arrange
            using (this.GetNewDirectory(out var directory))
            {
                var file = this.InstrumentClient(directory.GetFileClient(this.GetNewDirectoryName()));

                // Act
                await TestHelper.AssertExpectedExceptionAsync<StorageRequestFailedException>(
                    file.ForceCloseHandlesAsync(),
                    actualException => Assert.AreEqual("ResourceNotFound", actualException.ErrorCode));

            }
        }

        private async Task WaitForCopy(FileClient file, int milliWait = 200)
        {
            var status = CopyStatus.Pending;
            var start = this.Recording.Now;
            while (status != CopyStatus.Success)
            {
                status = (await file.GetPropertiesAsync()).Value.CopyStatus;
                var currentTime = this.Recording.Now;
                if (status == CopyStatus.Failed || currentTime.AddMinutes(-1) > start)
                {
                    throw new Exception("Copy failed or took too long");
                }
                await this.Delay(milliWait);
            }
        }
    }
}
