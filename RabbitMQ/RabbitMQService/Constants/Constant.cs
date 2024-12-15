namespace RabbitMQService.Constants
{
    public static class Constant
    {

    }

    public static class ServiceNames
    {
        public const string Identity = "IdentityService";
        public const string Notification = "NotificationService";
    }

    public static class Exchanges
    {
        public const string RegisterUserExchange = "RegisterUserExchange";
    }

    public static class RoutingKeys
    {
        public const string RegisterUserRoutingKey = "RegisterUserRoutingKey";
    }
}
