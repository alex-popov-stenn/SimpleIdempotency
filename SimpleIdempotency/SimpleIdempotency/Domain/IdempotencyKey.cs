namespace SimpleIdempotency.Domain
{
    //For simplicity: domain entity and persistence entity are the same
    public class IdempotencyKey
    {
        protected IdempotencyKey()
        {
        }

        public static IdempotencyKey Create(string @namespace, string key, string? payload, TimeSpan expiration)
        {
            var expiresAt = DateTime.UtcNow.Add(expiration);
            return Create(@namespace, key, payload, expiresAt);
        }

        public static IdempotencyKey Create(string @namespace, string key, string? payload,
            DateTime absoluteExpirationTime)
        {
            var idempotencyKey = new IdempotencyKey(@namespace, key, payload, absoluteExpirationTime);
            return idempotencyKey;
        }

        private IdempotencyKey(string @namespace, string key, string? payload, DateTime expiresAt)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Payload = payload;
            Namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
            ExpiresAt = expiresAt;
        }
//в принципе .net 8 - можно и через requre + init понтануться
        public string Namespace { get; protected internal set; } = null!;
        public string Key { get; protected internal set; } = null!;
        public string? Payload { get; protected internal set; }
        public DateTime ExpiresAt { get; protected internal set; }
    }
}