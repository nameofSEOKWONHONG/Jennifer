using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Jennifer.SharedKernel.Events;

public class RabbitMqConsumerServiceBase : BackgroundService, IAsyncDisposable
{
    private readonly ILogger<RabbitMqConsumerServiceBase> _logger;
    private readonly string _exchangeName;

    private IConnection _connection;
    private IChannel _channel;
    private AsyncEventingBasicConsumer _consumer;

    public RabbitMqConsumerServiceBase(ILogger<RabbitMqConsumerServiceBase> logger, string exchangeName)
    {
        _logger = logger;
        _exchangeName = exchangeName;
    }

    protected virtual IDictionary<string, object> GetHeaders() => new Dictionary<string, object>
    {
        { "x-match", "all" },
        { "event-type", "user.created" }
    };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "admin",
            Password = "password",
        };

        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await _channel.ExchangeDeclareAsync(exchange: _exchangeName, type: ExchangeType.Headers, durable: true, autoDelete: false, cancellationToken: stoppingToken);

        var queueDeclareOk = await _channel.QueueDeclareAsync(cancellationToken: stoppingToken);
        var queueName = queueDeclareOk.QueueName;

        await _channel.QueueBindAsync(queue: queueName, exchange: _exchangeName, routingKey: "", arguments: GetHeaders(), cancellationToken: stoppingToken);

        _consumer = new AsyncEventingBasicConsumer(_channel);
        _consumer.ReceivedAsync += OnMessageReceivedAsync;

        await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: _consumer, cancellationToken: stoppingToken);

        _logger.LogInformation("RabbitMQ Consumer started on queue: {Queue}", queueName);

        // 서비스 지속 유지
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }

        _logger.LogInformation("RabbitMQ Consumer stopping...");
    }

    protected virtual async Task OnMessageReceivedAsync(object model, BasicDeliverEventArgs e)
    {
        try
        {
            var message = Encoding.UTF8.GetString(e.Body.ToArray());
            _logger.LogInformation("[RabbitMQ] Received message: {Message}", message);

            // TODO: 메시지 처리

            if (_channel is not null)
            {
                await _channel.BasicAckAsync(e.DeliveryTag, false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[RabbitMQ] 처리 중 오류 발생");
        }

        await Task.Yield(); // 비동기 컨텍스트 유지
    }

    public async ValueTask DisposeAsync()
    {
        if (_consumer is not null)
        {
            _consumer.ReceivedAsync -= OnMessageReceivedAsync;
        }

        if (_channel is not null)
        {
            await _channel.CloseAsync();
            _channel.Dispose();
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync();
            _connection.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}
