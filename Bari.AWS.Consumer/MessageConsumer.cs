using System;
using System.Linq;
using System.Timers;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace Bari.AWS.Consumer
{
    class MessageConsumer
    {        

        public static void Receiver()
        {            

            using (var timer = new System.Timers.Timer())
            {
                timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                timer.Interval = 5000;
                timer.Enabled = true;

                Console.WriteLine("Press \'q\' to quit the Message Producer.");
                while (Console.Read() != 'q') ;
            }                        
        }

        private async static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            var sqs = new AmazonSQSClient(RegionEndpoint.SAEast1);                        
            var queueUrl = sqs.GetQueueUrlAsync("BariMessagesQueue").Result.QueueUrl;

            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                MaxNumberOfMessages = 10
            };

            Console.WriteLine("Checking for new messages in the queue BariMessagesQueue...\n");

            var receiveMessageResponse = sqs.ReceiveMessageAsync(receiveMessageRequest).Result;

            var counter = receiveMessageResponse.Messages.Count;

            

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

                Console.WriteLine($" Cleaning a Message in Queue... \n");
                var messageReceptHandle = message.ReceiptHandle;

                var deleteResquest = new DeleteMessageRequest
                {
                    QueueUrl = queueUrl,
                    ReceiptHandle = messageReceptHandle
                };

                await sqs.DeleteMessageAsync(deleteResquest);                
            }
            
            if (counter == 0)
                Console.WriteLine("There is no new messages.\n");
            else
                Console.WriteLine($" {counter} New Messages Received. \n");
        }
    }
}
