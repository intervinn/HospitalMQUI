using HospitalCommon;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace HospitalMQ.Services
{
    public class MessageConsumeService : IHostedService, IDisposable
    {
        private ILogger<MessageConsumeService> _logger;
        private IConfiguration _config;
        private StorageService _storage;

        private IChannel? _channel;
        private IConnection? _connection;

        public MessageConsumeService(ILogger<MessageConsumeService> logger, IConfiguration config, StorageService storage)
        {
            _logger = logger;
            _config = config;
            _storage = storage;
        }

        public async Task StartAsync(CancellationToken token)
        {
            Console.WriteLine(_config.GetSection("RabbitMQ"));
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _config.GetSection("RabbitMQ")["HostName"] ?? throw new Exception("HostName обязателен"),
                    UserName = _config.GetSection("RabbitMQ")["UserName"] ?? string.Empty,
                    Password = _config.GetSection("RabbitMQ")["Password"] ?? string.Empty
                };
                _connection = await factory.CreateConnectionAsync(token);
                _channel = await _connection.CreateChannelAsync(null, token);

                await _channel.QueueDeclareAsync("hospital_queue", false, false, false);
                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += OnReceiveAsync;
                _logger.LogInformation("Сервер подключен к RabbitMQ");
                await _channel.BasicConsumeAsync("hospital_queue", true, consumer);
                _logger.LogInformation("Сервер слушает очередь hospital_queue");
            } catch (Exception e)
            {
                _logger.LogCritical($"Ошибка при запуске Consumer: {e}");
                throw;
            }
        }

        public async Task OnReceiveAsync(object sender, BasicDeliverEventArgs args)
        {
            try
            {
                var json = Encoding.UTF8.GetString(args.Body.ToArray());
                _logger.LogInformation($"Получена запись: {json}");
                try
                {
                    var patient = JsonSerializer.Deserialize<Patient>(json);
                    _logger.LogInformation("Сохраняю запись в базу данных:");
                    await _storage.SavePatientAsync(patient);
                } catch (Exception e)
                {
                    _logger.LogWarning($"Сообщение не получилось десериализовать: {e}");
                }
            } catch (Exception e)
            {
                _logger.LogError($"Ошибка при получении сообщение: {e}");
            }
        }

        public async Task StopAsync(CancellationToken token)
        {

        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
