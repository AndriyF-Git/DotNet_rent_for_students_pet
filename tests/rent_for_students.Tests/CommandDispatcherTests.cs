using rent_for_students.Application.Commands;
using rent_for_students.Application.Common;

namespace rent_for_students.Tests
{
    public class CommandDispatcherTests
    {
        private readonly CommandDispatcher _sut = new();

        // ── Stub commands for testing ───────────────────────────────

        private sealed class SuccessCommand : ICommand<string>
        {
            public Task<Result<string>> ExecuteAsync(CancellationToken ct = default)
                => Task.FromResult(Result<string>.Success("ok"));
        }

        private sealed class FailureCommand : ICommand<string>
        {
            public Task<Result<string>> ExecuteAsync(CancellationToken ct = default)
                => Task.FromResult(Result<string>.Failure("ERR", "something went wrong"));
        }

        private sealed class ValueCommand : ICommand<int>
        {
            private readonly int _value;
            public ValueCommand(int value) => _value = value;

            public Task<Result<int>> ExecuteAsync(CancellationToken ct = default)
                => Task.FromResult(Result<int>.Success(_value));
        }

        private sealed class CancellationAwareCommand : ICommand<bool>
        {
            public Task<Result<bool>> ExecuteAsync(CancellationToken ct = default)
                => Task.FromResult(Result<bool>.Success(ct.IsCancellationRequested));
        }

        // ── Tests ───────────────────────────────────────────────────

        [Fact]
        public async Task DispatchAsync_ReturnsSuccess_WhenCommandSucceeds()
        {
            Result<string> result = await _sut.DispatchAsync(new SuccessCommand());

            Assert.True(result.IsSuccess);
            Assert.Equal("ok", result.Value);
        }

        [Fact]
        public async Task DispatchAsync_ReturnsFailure_WhenCommandFails()
        {
            Result<string> result = await _sut.DispatchAsync(new FailureCommand());

            Assert.False(result.IsSuccess);
            Assert.Equal("ERR", result.ErrorCode);
            Assert.Equal("something went wrong", result.Message);
        }

        [Fact]
        public async Task DispatchAsync_PassesCorrectValue()
        {
            Result<int> result = await _sut.DispatchAsync(new ValueCommand(42));

            Assert.True(result.IsSuccess);
            Assert.Equal(42, result.Value);
        }

        [Fact]
        public async Task DispatchAsync_ThrowsArgumentNullException_WhenCommandIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _sut.DispatchAsync<string>(null!));
        }

        [Fact]
        public async Task DispatchAsync_ForwardsCancellationToken()
        {
            CancellationTokenSource cts = new();
            cts.Cancel();

            Result<bool> result = await _sut.DispatchAsync(new CancellationAwareCommand(), cts.Token);

            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
        }
    }
}
