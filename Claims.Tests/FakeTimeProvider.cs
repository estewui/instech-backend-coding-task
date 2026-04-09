namespace Claims.Tests
{
    /// <summary>
    /// A fake TimeProvider that always returns a fixed point in time.
    /// </summary>
    public sealed class FakeTimeProvider : TimeProvider
    {
        private DateTimeOffset _utcNow;

        public FakeTimeProvider(DateTimeOffset fixedUtcNow)
        {
            _utcNow = fixedUtcNow;
        }

        public override DateTimeOffset GetUtcNow() => _utcNow;

        public void SetUtcNow(DateTimeOffset value) => _utcNow = value;

        public void Advance(TimeSpan delta) => _utcNow = _utcNow.Add(delta);
    }
}
