using System;

using FGS.Primitives.Time.Abstractions;

namespace FGS.Primitives.Time
{
    public sealed class FreezableClock : IClock, IFreezableClock
    {
        private readonly object _trackingSyncLock = new object();
        private readonly IClock _clock;

        private bool _frozen;
        private DateTimeOffset? _frozenNow;
        private DateTimeOffset? _frozenUtcNow;

        public FreezableClock(IClock clock)
        {
            _clock = clock;
        }

        public DateTimeOffset Now
        {
            get
            {
                return UseTrackingSyncLock(() =>
                {
                    if (_frozen)
                    {
                        if (!_frozenNow.HasValue)
                            _frozenNow = _clock.Now;

                        return _frozenNow.Value;
                    }

                    return _clock.Now;
                });
            }
        }

        public DateTimeOffset UtcNow
        {
            get
            {
                return UseTrackingSyncLock(() =>
                {
                    if (_frozen)
                    {
                        if (!_frozenUtcNow.HasValue)
                            _frozenUtcNow = _clock.UtcNow;

                        return _frozenUtcNow.Value;
                    }

                    return _clock.UtcNow;
                });
            }
        }

        public void FreezeTime()
        {
            UseTrackingSyncLock(() => { _frozen = true; });
        }

        public void UnfreezeTime()
        {
            UseTrackingSyncLock(() =>
            {
                _frozen = false;
                _frozenNow = default(DateTimeOffset?);
                _frozenUtcNow = default(DateTimeOffset?);
            });
        }

        private TReturnType UseTrackingSyncLock<TReturnType>(Func<TReturnType> func)
        {
            lock (_trackingSyncLock)
            {
                return func();
            }
        }

        private void UseTrackingSyncLock(Action action)
        {
            lock (_trackingSyncLock)
            {
                action();
            }
        }
    }
}
