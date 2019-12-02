using System;
using System.Configuration;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;


namespace Comp306_Assign01_Ques01_SruthiSugumaran
{
    class Program
    {
        //Bucket Details
        private const string bucketName = "comp306.sruthi-sugumaran.assignment01.bucket01";
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.CACentral1;
        private static IAmazonS3 s3Client;

        //File Details
        private const string keyName = "image01.jpg";
        private const string filePath = "C:\\academics\\college\\sem6\\COMP306\\assignments\\Assignment01\\images\\image01.jpg";


        static void Main(string[] args)
        {
            var credentials = new BasicAWSCredentials(ConfigurationManager.AppSettings["accessId"], ConfigurationManager.AppSettings["secretKey"]);

            using (AmazonS3Client s3CLient = new AmazonS3Client(credentials, RegionEndpoint.CACentral1))
            {

                s3Client = new AmazonS3Client(credentials, RegionEndpoint.CACentral1);
                CreateBucketAsync().Wait();

                UploadFileAsync().Wait();
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        //method to create a bucket
        private static async Task CreateBucketAsync()
        {
            try
            {
                if (!(await AmazonS3Util.DoesS3BucketExistAsync(s3Client, bucketName)))
                {
                    var putBucketRequest = new PutBucketRequest
                    {
                        BucketName = bucketName,
                        UseClientRegion = true
                    };

                    PutBucketResponse putBucketResponse = await s3Client.PutBucketAsync(putBucketRequest);

                    Console.WriteLine("Bucket created successfully...");
                }
                else
                {
                    Console.WriteLine("Bucket already exists...");
                }

                //Retrieve the bucket Location
                string bucketLocation = await FindBucketLocationAsync(s3Client);

                Console.WriteLine("Bucket name:\t" + bucketName + "\nLocation:\t" + bucketLocation);
                ;
            }

            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
        }

        //method to retrieve bucket location
        private static async Task<string> FindBucketLocationAsync(IAmazonS3 client)
        {
            string bucketLocation;
            var request = new GetBucketLocationRequest()
            {
                BucketName = bucketName
            };

            GetBucketLocationResponse response = await client.GetBucketLocationAsync(request);
            bucketLocation = response.Location.ToString();
            return bucketLocation;
        }

        //method to upload a file
        private static async Task UploadFileAsync()
        {
            try
            {
                var fileTransferUtility = new TransferUtility(s3Client);
                await fileTransferUtility.UploadAsync(filePath, bucketName, keyName);
                Console.WriteLine("Upload completed...");
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message: '{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message: '{0}' when writing an object", e.Message);
            }
        }

    }
}