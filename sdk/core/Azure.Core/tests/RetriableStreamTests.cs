﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core.Http;
using Azure.Core.Pipeline;
using Azure.Core.Testing;
using NUnit.Framework;

namespace Azure.Core.Tests
{
    [TestFixture(true)]
    [TestFixture(false)]
    public class RetriableStreamTests : SyncAsyncTestBase
    {
        private readonly byte[] _buffer = new byte[256];

        public RetriableStreamTests(bool isAsync) : base(isAsync)
        {
        }

        [Test]
        public async Task MaintainsGlobalLengthAndPosition()
        {
            var stream1 = new MockReadStream(100, throwAfter: 50);
            var stream2 = new MockReadStream(50, offset: 50, throwIOException: false);

            MockTransport mockTransport = CreateMockTransport(
                new MockResponse(200) { ContentStream = stream1 },
                new MockResponse(200) { ContentStream = stream2 }
            );
            var pipeline = new HttpPipeline(mockTransport);

            Stream reliableStream = await CreateAsync(
                offset => SendTestRequest(pipeline, offset),
                offset => SendTestRequestAsync(pipeline, offset),
                new ResponseClassifier(), maxRetries: 5);

            Assert.AreEqual(25, await ReadAsync(reliableStream, _buffer, 0, 25));
            Assert.AreEqual(100, reliableStream.Length);
            Assert.AreEqual(25, reliableStream.Position);

            Assert.AreEqual(25, await ReadAsync(reliableStream, _buffer, 25, 25));
            Assert.AreEqual(100, reliableStream.Length);
            Assert.AreEqual(50, reliableStream.Position);

            Assert.AreEqual(50, await ReadAsync(reliableStream, _buffer, 50, 50));
            Assert.AreEqual(100, reliableStream.Length);
            Assert.AreEqual(100, reliableStream.Position);

            Assert.AreEqual(0, await ReadAsync(reliableStream, _buffer, 0, 50));
            AssertReads(_buffer, 100);
        }

        [Test]
        public async Task DoesntRetryNonRetryableExceptions()
        {
            var stream1 = new MockReadStream(100, throwAfter: 50);
            var stream2 = new MockReadStream(50, offset: 50, throwAfter: 0, throwIOException: false);

            MockTransport mockTransport = CreateMockTransport(
                new MockResponse(200) { ContentStream = stream1 },
                new MockResponse(200) { ContentStream = stream2 }
            );
            var pipeline = new HttpPipeline(mockTransport);

            Stream reliableStream = await CreateAsync(
                offset => SendTestRequest(pipeline, offset),
                offset => SendTestRequestAsync(pipeline, offset),
                new ResponseClassifier(),
                maxRetries: 5);

            Assert.AreEqual(25, await ReadAsync(reliableStream, _buffer, 0, 25));
            Assert.AreEqual(100, reliableStream.Length);
            Assert.AreEqual(25, reliableStream.Position);

            Assert.AreEqual(25, await ReadAsync(reliableStream, _buffer, 25, 25));
            Assert.AreEqual(100, reliableStream.Length);
            Assert.AreEqual(50, reliableStream.Position);

            Assert.ThrowsAsync<InvalidOperationException>(() => reliableStream.ReadAsync(_buffer, 50, 50));

            AssertReads(_buffer, 50);
        }

        [Test]
        public async Task DoesntCallLengthPrematurely()
        {
            var stream1 = new NoLengthStream();
            var stream2 = new MockReadStream(50);

            MockTransport mockTransport = CreateMockTransport(
                new MockResponse(200) { ContentStream = stream1 },
                new MockResponse(200) { ContentStream = stream2 }
            );
            var pipeline = new HttpPipeline(mockTransport);

            Stream reliableStream = RetriableStream.Create(
                IsAsync ? await SendTestRequestAsync(pipeline, 0) : SendTestRequest(pipeline, 0),
                offset => SendTestRequest(pipeline, offset),
                offset => SendTestRequestAsync(pipeline, offset),
                new ResponseClassifier(),
                maxRetries: 5);

            Assert.AreEqual(50, await ReadAsync(reliableStream, _buffer, 0, 50));
            Assert.AreEqual(50, reliableStream.Position);

            Assert.AreEqual(0, await ReadAsync(reliableStream, _buffer, 0, 50));

            Assert.Throws<NotSupportedException>(() => _ = reliableStream.Length);

            AssertReads(_buffer, 50);
        }

        [Test]
        public async Task ThrowsIfSendingRetryRequestThrows()
        {
            var stream1 = new MockReadStream(100, throwAfter: 50);
            MockTransport mockTransport = CreateMockTransport(new MockResponse(200) { ContentStream = stream1 });

            var pipeline = new HttpPipeline(mockTransport);

            Stream reliableStream = await CreateAsync(
                offset =>
                {
                    if (offset == 0)
                    {
                        return SendTestRequest(pipeline, offset);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                },
                async offset =>
                {
                    if (offset == 0)
                    {
                        return await SendTestRequestAsync(pipeline, offset);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }, new ResponseClassifier(), maxRetries: 5);

            await ReadAsync(reliableStream, _buffer, 0, 25);
            await ReadAsync(reliableStream, _buffer, 25, 25);

            AssertReads(_buffer, 50);
            Assert.ThrowsAsync<InvalidOperationException>(() => ReadAsync(reliableStream, _buffer, 50, 50));
        }

        [Test]
        public async Task RetriesMaxCountAndThrowsAggregateException()
        {
            MockTransport mockTransport = CreateMockTransport(
                new MockResponse(200) { ContentStream = new MockReadStream(100, throwAfter: 1) },
                new MockResponse(200) { ContentStream = new MockReadStream(100, throwAfter: 1, offset: 1) },
                new MockResponse(200) { ContentStream = new MockReadStream(100, throwAfter: 1, offset: 2) },
                new MockResponse(200) { ContentStream = new MockReadStream(100, throwAfter: 1, offset: 3) }
                );

            var pipeline = new HttpPipeline(mockTransport);

            Stream reliableStream = await CreateAsync(offset =>
                {
                    if (offset == 0)
                    {
                        return SendTestRequest(pipeline, offset);
                    }

                    throw new InvalidOperationException();
                },
                async offset =>
                {
                    if (offset == 0)
                    {
                        return await SendTestRequestAsync(pipeline, offset);
                    }

                    throw new InvalidOperationException();
                }, new ResponseClassifier(), maxRetries: 3);

            AggregateException aggregateException = Assert.ThrowsAsync<AggregateException>(() => ReadAsync(reliableStream, _buffer, 0, 4));
            StringAssert.StartsWith("Retry failed after 4 tries", aggregateException.Message);
            Assert.AreEqual(4, aggregateException.InnerExceptions.Count);
            Assert.AreEqual("Failed at 0", aggregateException.InnerExceptions[0].Message);
            Assert.AreEqual("Failed at 1", aggregateException.InnerExceptions[1].Message);
            Assert.AreEqual("Failed at 2", aggregateException.InnerExceptions[2].Message);
            Assert.AreEqual("Failed at 3", aggregateException.InnerExceptions[3].Message);
            Assert.AreEqual(4, mockTransport.Requests.Count);
        }

        [Test]
        public void ThrowsIfInitialRequestThrow()
        {
            Assert.Throws<InvalidOperationException>(() => RetriableStream.Create(
                _ => throw new InvalidOperationException(),
                _ => null,
                new ResponseClassifier(),
                5));
        }

        [Test]
        public void ThrowsIfInitialRequestThrowAsync()
        {
            Assert.ThrowsAsync<InvalidOperationException>(() => RetriableStream.CreateAsync(
                _ => null,
                _ => throw new InvalidOperationException(),
                new ResponseClassifier(),
                5));
        }

        private void AssertReads(byte[] buffer, int length)
        {
            for (int i = 0; i < length; i++)
            {
                Assert.AreEqual((byte)i, buffer[i]);
            }
        }

        private Task<Stream> CreateAsync(
            Func<long, Response> responseFactory,
            Func<long, Task<Response>> asyncResponseFactory,
            ResponseClassifier responseClassifier,
            int maxRetries)
        {
            return IsAsync ?
                RetriableStream.CreateAsync(responseFactory, asyncResponseFactory, responseClassifier, maxRetries) :
                Task.FromResult(RetriableStream.Create(responseFactory, asyncResponseFactory, responseClassifier, maxRetries));
        }

        private Task<int> ReadAsync(Stream stream, byte[] buffer, int offset, int length)
        {
            return IsAsync ? stream.ReadAsync(buffer, offset, length) : Task.FromResult(stream.Read(buffer, offset, length));
        }

        private static Response SendTestRequest(HttpPipeline pipeline, long offset)
        {
            using Request request = CreateRequest(pipeline, offset);

            return pipeline.SendRequest(request, CancellationToken.None);
        }

        private static Task<Response> SendTestRequestAsync(HttpPipeline pipeline, long offset)
        {
            using Request request = CreateRequest(pipeline, offset);

            return pipeline.SendRequestAsync(request, CancellationToken.None);
        }

        private static Request CreateRequest(HttpPipeline pipeline, long offset)
        {
            Request request = pipeline.CreateRequest();
            request.SetRequestLine(RequestMethod.Get, new Uri("http://example.com"));
            request.Headers.Add("Range", "bytes=" + offset);
            return request;
        }

        private class NoLengthStream : ReadOnlyStream
        {
            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new IOException();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override bool CanRead { get; } = true;
            public override bool CanSeek { get; } = false;
            public override long Length => throw new NotSupportedException();
            public override long Position { get; set; }
        }

        private class MockReadStream : ReadOnlyStream
        {
            private readonly long _throwAfter;

            private byte _offset;

            private readonly bool _throwIOException;

            public MockReadStream(long length, long throwAfter = int.MaxValue, byte offset = 0, bool throwIOException = true)
            {
                _throwAfter = throwAfter;
                _offset = offset;
                _throwIOException = throwIOException;
                Length = length;
            }

            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                var left = (int)Math.Min(count, Length - Position);

                Position += left;

                if (Position > _throwAfter)
                {
                    if (_throwIOException)
                    {
                        throw new IOException($"Failed at {_offset}");
                    }
                    throw new InvalidOperationException();
                }

                for (int i = 0; i < left; i++)
                {
                    buffer[offset + i] = _offset++;
                }

                return Task.FromResult(left);
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return ReadAsync(buffer, offset, count).GetAwaiter().GetResult();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotImplementedException();
            }

            public override bool CanRead { get; } = true;
            public override bool CanSeek { get; } = false;
            public override long Length { get; }
            public override long Position { get; set; }
        }
    }
}
