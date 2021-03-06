﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mantle.BlobStorage.Interfaces;
using Mantle.Configuration.Attributes;
using Mantle.Extensions;
using Mantle.FaultTolerance.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Mantle.BlobStorage.Azure.Clients
{
    public class AzureBlobStorageClient : IBlobStorageClient
    {
        private readonly ITransientFaultStrategy transientFaultStrategy;

        private CloudBlobClient cloudBlobClient;
        private CloudStorageAccount cloudStorageAccount;

        public AzureBlobStorageClient(ITransientFaultStrategy transientFaultStrategy)
        {
            this.transientFaultStrategy = transientFaultStrategy;
        }

        public CloudBlobClient CloudBlobClient => GetCloudBlobClient();

        public CloudStorageAccount CloudStorageAccount => GetCloudStorageAccount();

        [Configurable(IsRequired = true)]
        public string ContainerName { get; set; }

        [Configurable(IsRequired = true)]
        public string StorageConnectionString { get; set; }

        public bool BlobExists(string blobName)
        {
            blobName.Require(nameof(blobName));

            var container = CloudBlobClient.GetContainerReference(ContainerName);

            if (transientFaultStrategy.Try(() => container.Exists()) == false)
                return false;

            return transientFaultStrategy.Try(() => container.GetBlockBlobReference(blobName).Exists());
        }

        public void DeleteBlob(string blobName)
        {
            blobName.Require(nameof(blobName));

            var container = CloudBlobClient.GetContainerReference(ContainerName);

            if (transientFaultStrategy.Try(() => container.Exists()) == false)
                throw new InvalidOperationException($"Container [{ContainerName}] does not exist.");

            var blob = container.GetBlockBlobReference(blobName);

            if (transientFaultStrategy.Try(() => blob.Exists()) == false)
                throw new InvalidOperationException($"Blob [{ContainerName}/{blobName}] does not exist.");

            transientFaultStrategy.Try(() => blob.Delete());
        }

        public Stream DownloadBlob(string blobName)
        {
            blobName.Require(nameof(blobName));

            var container = CloudBlobClient.GetContainerReference(ContainerName);

            if (transientFaultStrategy.Try(() => container.Exists()) == false)
                throw new InvalidOperationException($"Container [{ContainerName}] does not exist.");

            var blob = container.GetBlockBlobReference(blobName);

            if (transientFaultStrategy.Try(() => blob.Exists()) == false)
                throw new InvalidOperationException($"Blob [{ContainerName}/{blobName}] does not exist.");

            var stream = new MemoryStream();

            transientFaultStrategy.Try(() => blob.DownloadToStream(stream));
            stream.TryToRewind();

            return stream;
        }

        public IEnumerable<string> ListBlobs()
        {
            var container = CloudBlobClient.GetContainerReference(ContainerName);

            if (transientFaultStrategy.Try(() => container.Exists()) == false)
                throw new InvalidOperationException($"Container [{ContainerName}] does not exist.");

            return transientFaultStrategy.Try(
                () => container.ListBlobs().OfType<CloudBlockBlob>().Select(b => b.Name).ToList());
        }

        public void UploadBlob(Stream source, string blobName)
        {
            source.Require(nameof(source));
            blobName.Require(nameof(blobName));

            if (source.Length == 0)
                throw new ArgumentException($"[{nameof(source)}] is empty.", nameof(source));

            source.TryToRewind();

            var container = CloudBlobClient.GetContainerReference(ContainerName);

            transientFaultStrategy.Try(() => container.CreateIfNotExists());

            var blob = container.GetBlockBlobReference(blobName);

            transientFaultStrategy.Try(() => blob.UploadFromStream(source));
        }

        private CloudBlobClient GetCloudBlobClient()
        {
            return (cloudBlobClient = (cloudBlobClient ??
                                       CloudStorageAccount.CreateCloudBlobClient()));
        }

        private CloudStorageAccount GetCloudStorageAccount()
        {
            return (cloudStorageAccount = (cloudStorageAccount ??
                                           CloudStorageAccount.Parse(StorageConnectionString)));
        }
    }
}