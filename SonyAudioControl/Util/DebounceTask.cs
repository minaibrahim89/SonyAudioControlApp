using System;
using System.Threading;
using System.Threading.Tasks;

namespace SonyAudioControl.Util
{
    public class DebounceTask<TArg>
    {
        private readonly Action<TArg> _action;
        private CancellationTokenSource _cancellationTokenSource;

        public DebounceTask(Action<TArg> action)
        {
            _action = action;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task Run(TimeSpan debounceTime, TArg arg)
        {
            await RunInternal(debounceTime, arg).ConfigureAwait(false);
        }

        private async Task RunInternal(TimeSpan debounceTime, TArg arg)
        {
            try
            {
                Interlocked.Exchange(ref _cancellationTokenSource, new CancellationTokenSource()).Cancel();

                await Task.Delay(debounceTime, _cancellationTokenSource.Token)

                    .ContinueWith(async task => await Task.Run(() => _action(arg)),
                        CancellationToken.None,
                        TaskContinuationOptions.OnlyOnRanToCompletion,
                        TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch{}
        }
    }
}
