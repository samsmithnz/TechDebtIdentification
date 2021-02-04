using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TechDebtID.Core.Statistics;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using LibGit2Sharp;

namespace TechDebtID.Core
{
    public class RepoSyncWithStorage
    {
        //A lot of steps here.
        //1. Connect to GitHub and get a list of repos
        //2. Log into Azure Storage and clone each repo to blogs
        //3. Run our scanner on the repos/blogs

        public void CloneRepoToAzureStorage(string azureStorageConnectionString, string repo, string destination)
        {
            DirectoryInfo dir = new DirectoryInfo(destination);
            if (dir.Exists == false)
            {
                dir.Create();
            }
            FileInfo[] files = dir.GetFiles("*.*", SearchOption.AllDirectories);
            if (files.Length > 0)
            {
                //clean up and remove the .git hidden folder and files
                foreach (FileInfo file in files)
                {
                    //remove the readonly attribute
                    if ((file.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        file.Attributes &= ~FileAttributes.ReadOnly;
                    }
                    //remove the hidden attribute
                    if ((file.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    {
                        file.Attributes &= ~FileAttributes.Hidden;
                    }
                }
                Directory.Delete(destination, true);
            }
            Repository.Clone($"https://github.com/{repo}", destination);
        }

        private FileInfo[] GetFilesFromDirectory(string dir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            return dirInfo.GetFiles();
        }

        //        public async Task<List<string>> GetListOfReposFromGitHub()
        //        {
        //            List<string> repos = new List<string>();

        //            return repos;
        //        }

        //        public static async Task CloneRepoToStorageBlobs(string storageConnectionString, string sourceContainerName, string tempFolderLocation, List<string> files, bool filesHaveFullPath, string partsContainerName)
        //        {

        //            //BlobContainerClient client;
        //            //CloudBlobContainer cloudBlobContainer;
        //            ////string sourceFile = null;
        //            ////string destinationFile = null;

        //            //// Check whether the connection string can be parsed.
        //            //if (CloudStorageAccount.TryParse(storageConnectionString, out CloudStorageAccount storageAccount))
        //            //{
        //            //    try
        //            //    {
        //            //        // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
        //            //        CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

        //            //        // Create a new container  
        //            //        cloudBlobContainer = cloudBlobClient.GetContainerReference(sourceContainerName);
        //            //        bool containerExists = cloudBlobContainer == null || await cloudBlobContainer.ExistsAsync();
        //            //        if (containerExists == false)
        //            //        {
        //            //            await cloudBlobContainer.CreateAsync();
        //            //            Console.WriteLine("Created container '{0}'", cloudBlobContainer.Name);
        //            //        }
        //            //        // Set the permissions so the blobs are read only. 
        //            //        BlobContainerPermissions permissions = new BlobContainerPermissions
        //            //        {
        //            //            PublicAccess = BlobContainerPublicAccessType.Blob
        //            //        };
        //            //        await cloudBlobContainer.SetPermissionsAsync(permissions);

        //            //        //Create the parts location, if it doesn't already exist
        //            //        CloudBlobContainer partsContainer = cloudBlobClient.GetContainerReference(partsContainerName);
        //            //        bool containerExists2 = partsContainer == null || await partsContainer.ExistsAsync();
        //            //        if (containerExists2 == false)
        //            //        {
        //            //            await partsContainer.CreateAsync();
        //            //            Console.WriteLine("Created container '{0}'", partsContainer.Name);
        //            //        }
        //            //        // Set the permissions so the blobs are read only. 
        //            //        BlobContainerPermissions permissions2 = new BlobContainerPermissions
        //            //        {
        //            //            PublicAccess = BlobContainerPublicAccessType.Blob
        //            //        };
        //            //        await partsContainer.SetPermissionsAsync(permissions2);


        //            //        foreach (string file in files)
        //            //        {
        //            //            string fileToUpload = file;
        //            //            if (filesHaveFullPath == false)
        //            //            {
        //            //                fileToUpload = tempFolderLocation + @"\" + file;
        //            //            }
        //            //            Console.WriteLine("Uploading to Blob storage as blob '{0}'", file);

        //            //            // Get a reference to the blob address, then upload the file to the blob.
        //            //            // Use the value of localFileName for the blob name.
        //            //            if (File.Exists(fileToUpload) == true)
        //            //            {
        //            //                //Now strip the full path off so we can store any folders with the files we extracted
        //            //                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileToUpload.Replace(tempFolderLocation + @"\", ""));
        //            //                await cloudBlockBlob.UploadFromFileAsync(fileToUpload);
        //            //            }
        //            //            else
        //            //            {
        //            //                Console.WriteLine("File '" + fileToUpload + "' not found...");
        //            //            }
        //            //        }

        //            //        // List the blobs in the container.
        //            //        Console.WriteLine("Listing blobs in container.");
        //            //        List<string> filesInBlob = await AzureBlobManagement.ListBlobs(storageConnectionString, sourceContainerName);
        //            //        foreach (string item in filesInBlob)
        //            //        {
        //            //            Console.WriteLine(item);
        //            //        }

        //            //        //BlobContinuationToken blobContinuationToken = null;
        //            //        //do
        //            //        //{
        //            //        //    var results = await cloudBlobContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
        //            //        //    // Get the value of the continuation token returned by the listing call.
        //            //        //    blobContinuationToken = results.ContinuationToken;
        //            //        //    foreach (IListBlobItem item in results.Results)
        //            //        //    {
        //            //        //        Console.WriteLine(item.Uri);
        //            //        //    }
        //            //        //} while (blobContinuationToken != null); // Loop while the continuation token is not null.
        //            //        ////Console.WriteLine();
        //            //    }
        //            //    catch (StorageException ex)
        //            //    {
        //            //        Console.WriteLine("Error returned from the service: {0}", ex.Message);
        //            //    }
        //            //}
        //            //else
        //            //{
        //            //    Console.WriteLine(
        //            //        "A connection string has not been defined in the system environment variables. " +
        //            //        "Add a environment variable named 'storageconnectionstring' with your storage " +
        //            //        "connection string as a value.");
        //            //}
        //        }

        //        //    public async Task<bool> DownloadFiles(string connectionString, string container)
        //        //    {
        //        //        bool connected = await ConnectToFileShare(connectionString, container);
        //        //        if (connected == true)
        //        //        {
        //        //            //Download the git repos to the file system

        //        //        }

        //        //        return true;
        //        //    }

        //        //    public async Task<bool> ConnectToFileShare(string connectionString, string containerName)
        //        //    {
        //        //        // Parse the connection string for the storage account.
        //        //        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

        //        //        // Create a CloudFileClient object for credentialed access to File storage.
        //        //        CloudFileClient fileClient = storageAccount.CreateCloudFileClient();

        //        //        // Get a reference to the file share we created previously.
        //        //        CloudFileShare share = fileClient.GetShareReference(containerName);

        //        //        // Ensure that the share exists.
        //        //        if (await share.ExistsAsync())
        //        //        {
        //        //            return true;
        //        //        }
        //        //        else
        //        //        {
        //        //            return false;
        //        //        }

        //        //    }


    }
}
