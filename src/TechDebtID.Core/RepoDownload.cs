using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TechDebtID.Core.Statistics;

namespace TechDebtID.Core
{
    //public class RepoDownload
    //{

    //    public async Task<bool> DownloadFiles(string connectionString, string container)
    //    {
    //        bool connected = await ConnectToFileShare(connectionString, container);
    //        if (connected == true)
    //        {
    //            //Download the git repos to the file system

    //        }

    //        return true;
    //    }

    //    public async Task<bool> ConnectToFileShare(string connectionString, string containerName)
    //    {
    //        // Parse the connection string for the storage account.
    //        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

    //        // Create a CloudFileClient object for credentialed access to File storage.
    //        CloudFileClient fileClient = storageAccount.CreateCloudFileClient();

    //        // Get a reference to the file share we created previously.
    //        CloudFileShare share = fileClient.GetShareReference(containerName);

    //        // Ensure that the share exists.
    //        if (await share.ExistsAsync())
    //        {
    //            return true;
    //        }
    //        else
    //        {
    //            return false;
    //        }

    //    }


    //}
}
