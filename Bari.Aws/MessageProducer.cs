using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Amazon;
using Amazon.Runtime.Internal.Transform;
using Amazon.SQS;
using Amazon.SQS.Model;
using Bari.AWS.Domain;
using Bari.AWS.Infrastructure;
//using Confluent.Kafka;

namespace Bari.Aws
{
    class MessageProducer
    {
        public static IAmazonSQS sqs = new AmazonSQSClient(RegionEndpoint.SAEast1);
        public static string myQueueUrl;

        public static void Sender()
        {
            //using var awsProducer = new ProducerBuilder<Null, string>(new ProducerConfig { BootstrapServers = Consts.HOST }).Build();
            //{
                try
                {                

                Console.WriteLine("Creating a new Messages Queue...\n");

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

                        Console.WriteLine("Press \'q\' to quit the Message Producer.");
                        while (Console.Read() != 'q') ;
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            //}            
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

            //Ao preencher as propriedades da mensagem a mesma foi rejeitada pelo serviço da Amazon.
            //foreach (var _property in message.GetType().GetProperties())
            //{
            //    var attribute = new MessageAttributeValue();
            //    var value = _property.GetValue(message, null) ?? "(null)";
            //    attribute.StringValue = value.ToString();
            //    sqsMessageRequest.MessageAttributes.Add(_property.Name, attribute);
            //}
            await sqs.SendMessageAsync(sqsMessageRequest);
            Console.WriteLine("Message sended to BariMessagesQueue.\n");
        }
    }
}
