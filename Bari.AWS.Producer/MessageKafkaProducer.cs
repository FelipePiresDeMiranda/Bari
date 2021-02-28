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
using Confluent.Kafka;
using Serilog;

namespace Bari.Aws
{
    class MessageKafkaProducer
    {        
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

                using (var timer = new System.Timers.Timer())
                {
                    timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                    timer.Interval = 5000;
                    timer.Enabled = true;

                    Log.Information("Message sended to BariMessagesQueue.");
                    Console.WriteLine("Press \'q\' to quit the Messages Producer.");
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
            var message = new BariMessage();

            message.MessageId = Guid.NewGuid().ToString();
            message.MicroServiceId = _microserviceId;
            message.RequisitionId = Guid.NewGuid().ToString();

            var config = new ProducerConfig { BootstrapServers = Consts.HOST };
            Console.WriteLine("Sending a message to queue BariMessagesQueue...\n");
            Log.Information("Sending a message to queue BariMessagesQueue...");

            using (var p = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    var dr = await p.ProduceAsync("Bari-Topic", new Message<Null, string> { Value = "Hello World" });
                    Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                    Log.Information($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                }
                catch (ProduceException<Null, string> ex)
                {
                    Console.WriteLine($"Delivery failed: {ex.Error.Reason}");
                    Log.Error($"Delivery failed: {ex.Error.Reason}");
                }
            }

            Console.WriteLine("Message sended to BariMessagesQueue.\n");
            Log.Information("Message sended to BariMessagesQueue.");
        }
    }
}
