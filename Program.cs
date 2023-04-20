
class Program
{
    static int[] array = Enumerable.Range(1, 100000).Select(i => new Random().Next(1, 21)).ToArray();  //масив array з 100000 елементів, який заповнюється випадковими числами від 1 до 20
    static int allSumArray = 0;  //суму всіх елементів array
    static object lockObj = new object();
    static ManualResetEvent manualResEvent = new ManualResetEvent(false);

    static void Main(string[] args)
    {

        static void NegativeOrZero()    // перевірка на нуль чи (- числа)
        {
            if (array.Any(num => num <= 0))
            {
                Console.WriteLine("В масиві є від'ємні числа або 0");
                Environment.Exit(0);
            }

            manualResEvent.Set();
        }

        Thread thread = new Thread(NegativeOrZero);
        thread.Start();

        manualResEvent.WaitOne();

        for (int i = 0; i < 4; i++)
        {                                               //array поділив на чотири потоки, кожен потік знаходить суму чисел в array
            int startIndex = i * (array.Length / 4);    // i - потік
            int endIndex = (i + 1) * (array.Length / 4);  // (і + 1) - наступний потік
            new Thread(() =>
            {
                int sum = 0;
                for (int k = startIndex; k < endIndex; k++)
                {
                    sum += array[k];
                }

                lock (lockObj)          // блокування до змінної allSumArray декількох зразу потоків
                {
                    allSumArray += sum;  // сюди додається сума кожного потоку в общий, кінцевий результат
                }
            }).Start();
        }
        thread.Join(); // перевірка що весь масив обчислений перед виводом його в консоль
        Console.WriteLine($"Сума всих чисел в масиві: {allSumArray}");  // кігцевий результат 
    }

}
