using System;

namespace FGS.Pump.Extensions.Lifetime
{
    /// <summary>
    /// An <see cref="IDisposable"/> object that exists only to run a custom action when it is disposed.
    /// </summary>
    /// <remarks>
    /// Taken and modified from: https://github.com/foxguardsolutions/psa/blob/38ad1a86f24152d31559249d45a5b5e3b9eb22a0/CPSA.Core/OnDispose.cs
    /// </remarks>
    public sealed class OnDispose : Disposable
    {
        private readonly Action _action;

        public OnDispose(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            this._action = action;
        }

        protected override void Dispose(bool disposing)
        {
            _action();
        }

        public static readonly Disposable DoNothing = new OnDispose(() => { });
    }
}
