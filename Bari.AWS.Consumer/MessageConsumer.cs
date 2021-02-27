using System;
using System.Linq;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace Bari.AWS.Consumer
{
    class MessageConsumer
    {
        public static void Receiver()
        {
            var sqs = new AmazonSQSClient(RegionEndpoint.SAEast1);

            var queueUrl = sqs.GetQueueUrlAsync("BariMessagesQueue").Result.QueueUrl;

            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl
            };

            var receiveMessageResponse = sqs.ReceiveMessageAsync(receiveMessageRequest).Result;
            
            foreach (var message in receiveMessageResponse.Messages)
            {
                Console.WriteLine("Message \n");
                Console.WriteLine($" MessageId: {message.MessageId} \n");
                Console.WriteLine($" ReceiptHandle: {message.ReceiptHandle} \n");
                Console.WriteLine($" MD5OfBody: {message.MD5OfBody} \n");
                Console.WriteLine($" Body: {message.Body} \n");
                
                foreach (var atribute in message.Attributes)
                {
                    Console.WriteLine($" Key:{atribute.Key} ");
                    Console.WriteLine($" Value:{atribute.Value} \n");
                }

                Console.WriteLine($" Cleaning Message in Queue... \n");
                var messageReceptHandle = message.ReceiptHandle;

                var deleteResquest = new DeleteMessageRequest
                {
                    QueueUrl = queueUrl,
                    ReceiptHandle = messageReceptHandle
                };

                sqs.DeleteMessageAsync(deleteResquest);

                Console.WriteLine($" Message Received. \n");                
            }
        Console.ReadLine();
        }
    }
}
