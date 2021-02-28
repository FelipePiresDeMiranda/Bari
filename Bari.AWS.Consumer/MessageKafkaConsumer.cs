using System;
using System.Linq;
using System.Threading;
using System.Timers;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Bari.AWS.Infrastructure;
using Confluent.Kafka;
using Serilog;

namespace Bari.AWS.Consumer
{
    class MessageKafkaConsumer
    {        

        public static void Receiver()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            using (var timer = new System.Timers.Timer())
            {
                timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                timer.Interval = 5000;
                timer.Enabled = true;

                Console.WriteLine("Press \'q\' to quit the Messages Consumer.");
                while (Console.Read() != 'q') ;
            }
            Log.CloseAndFlush();
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            var config = new ProducerConfig { BootstrapServers = Consts.HOST };
            var msg = "";
            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe("Bari-Topic");
                CancellationTokenSource cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) =>
                {
                    e.Cancel = true;
                    cts.Cancel();
                };
                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = consumer.Consume(cts.Token);
                            msg = $"Consumed message '{cr.Message.Key}' at: '{cr.TopicPartitionOffset}' Body: '{cr.Message.Value}' TimeStamp: '{cr.Message.Timestamp}'.";
                            Console.WriteLine(msg);
                            Log.Logger.Information(msg);
                        }
                        catch (ConsumeException ex)
                        {
                            msg = $"Error occurred: {ex.Error.Reason}";
                            Console.WriteLine(msg);
                            Log.Error(msg);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    Log.Logger.Information("End of process.");
                    consumer.Close();
                }
            }

        }
    }
}
