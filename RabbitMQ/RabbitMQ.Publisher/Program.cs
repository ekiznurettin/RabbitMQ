using RabbitMQ.Client;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace RabbitMQ.Publisher
{
    internal class Program
    {
        static void Main1(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://jhsmvvbc:D9eZr9WK0GxjymoMamptheYXVLX4Iw5q@chimpanzee.rmq.cloudamqp.com/jhsmvvbc");

            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare("hello-queue", true, false, false);

            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                string message = $"Hello World: {x}";

                var messageBody = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(string.Empty, "hello-queue", null, messageBody);

                Console.WriteLine($"Mesaj Gönderildi:{message}");
            });



            Console.ReadLine();
        }

        static void Main2(string[] args)//Fenout Exchange
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://jhsmvvbc:D9eZr9WK0GxjymoMamptheYXVLX4Iw5q@chimpanzee.rmq.cloudamqp.com/jhsmvvbc");

            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout);

            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                string message = $"Log: {x}";

                var messageBody = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish("logs-fanout", "", null, messageBody);

                Console.WriteLine($"Mesaj Gönderildi:{message}");
            });



            Console.ReadLine();
        }

        static void Main3(string[] args)//Direct Exchange
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://jhsmvvbc:D9eZr9WK0GxjymoMamptheYXVLX4Iw5q@chimpanzee.rmq.cloudamqp.com/jhsmvvbc");

            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare("logs-direct", durable: true, type: ExchangeType.Direct);

            Enum.GetNames(typeof(LogNames)).ToList().ForEach(x =>
            {
                var routeKey = $"route-{x}";
                var queueName = $"direct-queue-{x}";
                channel.QueueDeclare(queueName, true, false, false);
                channel.QueueBind(queueName, "logs-direct", routeKey);
            });

            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                LogNames log = (LogNames)new Random().Next(1, 5);

                string message = $"Log: {log}";

                var messageBody = Encoding.UTF8.GetBytes(message);

                var routeKey = $"route-{log}";

                channel.BasicPublish("logs-direct", routeKey, null, messageBody);

                Console.WriteLine($"Log Gönderildi:{message}");
            });

            Console.ReadLine();
        }

        static void Main4(string[] args)//Topic Exchange
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://jhsmvvbc:D9eZr9WK0GxjymoMamptheYXVLX4Iw5q@chimpanzee.rmq.cloudamqp.com/jhsmvvbc");

            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare("logs-topic", durable: true, type: ExchangeType.Topic);

            Random rnd = new Random();

            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                LogNames log1 = (LogNames)rnd.Next(1, 5);
                LogNames log2 = (LogNames)rnd.Next(1, 5);
                LogNames log3 = (LogNames)rnd.Next(1, 5);
                LogNames log4 = (LogNames)rnd.Next(1, 5);

                var routeKey = $"{log1}-{log2}-{log3}-{log4}";

                var message = $"log-type : {log1}-{log2}-{log3}-{log4}";

                var messageBody = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish("logs-topic", routeKey, null, messageBody);

                Console.WriteLine($"Log Gönderildi:{message}");
            });

            Console.ReadLine();
        }
        static void Main(string[] args)//Header Exchange
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://jhsmvvbc:D9eZr9WK0GxjymoMamptheYXVLX4Iw5q@chimpanzee.rmq.cloudamqp.com/jhsmvvbc");

            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

            Dictionary<string, object> headers = new Dictionary<string, object>();

            headers.Add("format", "pdf");
            headers.Add("shape", "a4");

            var properties = channel.CreateBasicProperties();
            properties.Headers = headers;

            properties.Persistent = true;//mesajları kalıcı hale getirmek için kullanılır.

            Product product = new Product
            {
                Id =1,
                Name ="Kalem",
                Price =5,
                Stock=3
            };

            var productJsonString = JsonSerializer.Serialize(product);

            channel.BasicPublish("headers-exchage", string.Empty, properties, Encoding.UTF8.GetBytes(productJsonString));

            Console.WriteLine("Mesaj Gönderildi");

            Console.ReadLine();
        }
    }

    public enum LogNames
    {
        Critical = 1,
        Error = 2,
        Warning = 3,
        Information = 4
    }
}
