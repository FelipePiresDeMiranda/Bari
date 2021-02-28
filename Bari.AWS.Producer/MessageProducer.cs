using System;
using System.Timers;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Bari.AWS.Domain;
using Bari.AWS.Infrastructure;
using Serilog;

namespace Bari.Aws
{
    class MessageProducer
    {
        public static IAmazonSQS sqs = new AmazonSQSClient(RegionEndpoint.SAEast1);
        public static string myQueueUrl;
        public static Guid _microserviceId = Guid.NewGuid();

        public static void Sender()
        {            
            Log.Logger = new LoggerConfiguration()                
                .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();            
            try
                {                                
                Console.WriteLine("Creating a new Messages Queue...\n");
                Log.Information("Creating a new Messages Queue...");

                var sqsRequest = new CreateQueueRequest
                {
                    QueueName = "BariMessagesQueue"
                };

                var queueResponse = sqs.CreateQueueAsync(sqsRequest).Result;
                myQueueUrl = queueResponse.QueueUrl;

                var listQueuesRequest = new ListQueuesRequest();

                var listQueueResponse = sqs.ListQueuesAsync(listQueuesRequest);                

                Console.WriteLine("Queues AWS SQS List \n");

                foreach (var queueUrl in listQueueResponse.Result.QueueUrls)
                {
                    Console.WriteLine($"QueueUrl: {queueUrl}");
                }
                using (var timer = new System.Timers.Timer())
                    {
                        timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                        timer.Interval = 5000;
                        timer.Enabled = true;

                        Log.Information("Message sended to BariMessagesQueue.");
                        Console.WriteLine("Press \'q\' to quit the Message Producer.");
                        while (Console.Read() != 'q') ;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.InnerException.Message);
                    throw ex;
                }
                finally 
                {
                    Log.CloseAndFlush();
                }                         
        }

        private static async void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            
            Console.WriteLine("Sending a message to queue BariMessagesQueue...\n");

            var message = new BariMessage();

            var sqsMessageRequest = new SendMessageRequest
            {
                QueueUrl = myQueueUrl,
                MessageBody = message.Body                
            };
            message.MessageId = Guid.NewGuid().ToString();
            message.MicroServiceId = _microserviceId;
            message.RequisitionId = Guid.NewGuid().ToString();
            
            foreach (var _property in message.GetType().GetProperties())
            {
                var attribute = new MessageAttributeValue();
                var value = _property.GetValue(message, null) ?? "(null)";
                attribute.StringValue = value.ToString();
                attribute.DataType = "String";
                sqsMessageRequest.MessageAttributes.Add(_property.Name, attribute);
            }            
            await sqs.SendMessageAsync(sqsMessageRequest);
            Console.WriteLine("Message sended to BariMessagesQueue.\n");
        }
    }
}
