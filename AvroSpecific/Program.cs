using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AvroSpecific
{
    class Program
    {
        static void Main(string[] args)
        {
           

            string bootstrapServers = "localhost:9092";
            string schemaRegistryUrl = "http://localhost:8081";
            string topicName = "some-topic";

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = bootstrapServers
            };

            var schemaRegistryConfig = new SchemaRegistryConfig
            {
                // Note: you can specify more than one schema registry url using the
                // schema.registry.url property for redundancy (comma separated list). 
                // The property name is not plural to follow the convention set by
                // the Java implementation.
                Url = schemaRegistryUrl
            };

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = "avro-specific-example-group"
            };

            var avroSerializerConfig = new Confluent.SchemaRegistry.Serdes.AvroSerializerConfig
            {
                // optional Avro serializer properties:
                BufferBytes = 100
            };

            CancellationTokenSource cts = new CancellationTokenSource();
            var consumeTask = Task.Run(() =>
            {
                using (var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig))
                using (var consumer =
                    new ConsumerBuilder<string, User>(consumerConfig)
                        .SetKeyDeserializer(new Confluent.SchemaRegistry.Serdes.AvroDeserializer<string>(schemaRegistry).AsSyncOverAsync())
                        .SetValueDeserializer(new Confluent.SchemaRegistry.Serdes.AvroDeserializer<User>(schemaRegistry).AsSyncOverAsync())
                        .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                        .Build())
                {
                    consumer.Subscribe(topicName);

                    try
                    {
                        while (true)
                        {
                            try
                            {
                                var consumeResult = consumer.Consume(cts.Token);
                                Console.WriteLine($"user name: {consumeResult.Message.Key}, favorite color: {consumeResult.Message.Value.favorite_color}, hourly_rate: {consumeResult.Message.Value.hourly_rate}");
                            }
                            catch (ConsumeException e)
                            {
                                Console.WriteLine($"Consume error: {e.Error.Reason}");
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        consumer.Close();
                    }
                }
            });

            using (var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig))
            using (var producer =
                new ProducerBuilder<string, User>(producerConfig)
                    .SetKeySerializer(new Confluent.SchemaRegistry.Serdes.AvroSerializer<string>(schemaRegistry, avroSerializerConfig))
                    .SetValueSerializer(new Confluent.SchemaRegistry.Serdes.AvroSerializer<User>(schemaRegistry, avroSerializerConfig))
                    .Build())
            {
                Console.WriteLine($"{producer.Name} producing on {topicName}. Enter user names, q to exit.");

                int i = 0;
                string text;
                while ((text = Console.ReadLine()) != "q")
                {
                    User user = new User { name = text, favorite_color = "green", favorite_number = i++, hourly_rate = new Avro.AvroDecimal(67.99) };
                    producer
                        .ProduceAsync(topicName, new Message<string, User> { Key = text, Value = user })
                        .ContinueWith(task =>
                        {
                            if (!task.IsFaulted)
                            {
                                Console.WriteLine($"produced to: {task.Result.TopicPartitionOffset}");
                            }

                            // Task.Exception is of type AggregateException. Use the InnerException property
                            // to get the underlying ProduceException. In some cases (notably Schema Registry
                            // connectivity issues), the InnerException of the ProduceException will contain
                            // additional information pertaining to the root cause of the problem. Note: this
                            // information is automatically included in the output of the ToString() method of
                            // the ProduceException which is called implicitly in the below.
                            Console.WriteLine($"error producing message: {task.Exception.InnerException}");
                        });
                }
            }

            cts.Cancel();
        }
    }
}
