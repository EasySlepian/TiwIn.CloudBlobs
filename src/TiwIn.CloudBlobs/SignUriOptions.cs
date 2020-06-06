//-----------------------------------------------------------------------
// <copyright file="SignUriOptions.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs
{
    using System;
    using System.Diagnostics;
    using System.Text;

    public abstract class SignUriOptions
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Func<DateTimeOffset> _calcExpirationTime;


        [DebuggerNonUserCode]
        protected SignUriOptions()
        {
            StartsOn = DateTimeOffset.UtcNow;
            _calcExpirationTime = () => StartsOn;
        }


        public DateTimeOffset StartsOn { get; set; }

        public DateTimeOffset ExpiresOn
        {
            get => _calcExpirationTime.Invoke();
            set => _calcExpirationTime = () => value;
        }

        public TimeSpan TotalDuration => (ExpiresOn - StartsOn);

        public TimeSpan TimeLeft
        {
            get
            {
                var timeLeft = (ExpiresOn - DateTimeOffset.UtcNow);
                return (timeLeft.Ticks > 0) ? timeLeft : TimeSpan.Zero;
            }
        }

        public bool IsStarted => (DateTimeOffset.UtcNow - StartsOn).Ticks > 0;

        internal virtual void Assert()
        {
            if (TimeLeft.Ticks <= 0)
                throw new InvalidOperationException($"The defined permissions are expired already.");
        }


        public void Started(TimeSpan timeAgo)
        {
            StartsOn = DateTimeOffset.UtcNow.Add(timeAgo.Ticks > 0 ? (-timeAgo) : timeAgo);
        }

        public void ExpiresAfter(TimeSpan duration)
        {
            _calcExpirationTime = () =>
                IsStarted ? DateTimeOffset.UtcNow.Add(duration)
                    : StartsOn.Add(duration);
        }

        public override string ToString()
        {
            return new StringBuilder($" TotalDuration: {TotalDuration}.")
                .Append($" Range: [{StartsOn}, {ExpiresOn}]")
                .ToString();
        }
    }
}
