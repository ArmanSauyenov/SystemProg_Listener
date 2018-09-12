using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CSharpReference.Multithreading
{
    #region Multithreading Beginner
    delegate void NotifyCurrentThread();
    class ThreadListener
    {
        public event NotifyCurrentThread EventOccuredOnThread;

        private string _dataFromThread;
        public string DataFromThread
        {
            get { return _dataFromThread; }
            set
            {
                _dataFromThread = value;
                EventOccuredOnThread();
            }
        }
    }
    /// <summary>
    /// Chapters I, II from Albahari book "C# in a Nutshell"
    /// </summary>
    class MultithreadingBeginner
    {
        private ICollection<int> _integers = new List<int>();
        private static readonly object _locker = new object();
        public bool _isActive = true;
        private void Flip()
        {
            if (_isActive)
            {
                Console.WriteLine("IsActive were true");
                _isActive = false;
            }
        }
        public void ExecuteUndeterminedState()
        {
            Thread first = new Thread(Flip);
            Thread second = new Thread(Flip);

            first.Start();
            second.Start();
        }
        public void ExecuteCommonReference()
        {
            Thread[] threads = new Thread[]
            {
                new Thread(() => AppendInteger(_integers)),
                new Thread(() => AppendInteger(_integers)),
                new Thread(() => AppendInteger(_integers))
            };

            foreach (var item in threads)
            {
                item.Start();
                item.Join();
            }
            Console.WriteLine(_integers.Count);
        }
        public void AppendInteger(ICollection<int> _collection)
        {
            Random random = new Random();
            lock (_locker)
            {
                for (int i = 0; i < 100; i++)
                    _collection.Add(random.Next(0, 10));
            }
        }
        public void ExecuteRaceCondition()
        {
            Thread first = new Thread(() => WriteChar('X'));
            Thread second = new Thread(() => WriteChar('Y'));
            first.Start();
            second.Start();
        }
        public void WriteChar(char c)
        {
            for (int i = 0; i < 100; i++)
                Console.Write(c);
        }

        public void TimeConsumingOperation(ThreadListener listener)
        {
            Thread.Sleep(5000);
            listener.DataFromThread = Guid.NewGuid().ToString();
        }
        public Thread ExecuteTimeConsumingOperationOnBackground()
        {
            ThreadListener myListener = new ThreadListener();
            myListener.EventOccuredOnThread += () =>
                Console.WriteLine(myListener.DataFromThread);

            Thread thread = new Thread(() => TimeConsumingOperation(myListener));
            thread.Start();
            return thread;
        }
    }

    #endregion
    class Program
    {
        static void Main(string[] args)
        {
            MultithreadingBeginner mt = new MultithreadingBeginner();
            Thread token = mt.ExecuteTimeConsumingOperationOnBackground();

            token.Join();

            Console.WriteLine("Completed working on long-running task");

            Console.ReadLine();
        }
    }
}
