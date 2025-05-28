static double Profile(string description, int iterations, Action func, Lvl log4j)
{
    //Run at highest priority to minimize fluctuations caused by other processes/threads
    Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
    Thread.CurrentThread.Priority = ThreadPriority.Highest;

    // warm up 
    func();

    var watch = new Stopwatch();

    // clean up
    GC.Collect();
    GC.WaitForPendingFinalizers();
    GC.Collect();

    watch.Start();
    for (int i = 0; i < iterations; i++)
    {
        func();
    }
    watch.Stop();
    Console.Write(description);
    log4j.Log(description + " ->" + watch.Elapsed.TotalMilliseconds + " ms of time Elapsed");
    return watch.Elapsed.TotalMilliseconds;
}

/*        Nota svona:
Profile("ConvertCur", 1, () =>
{ // kóði sem á að mæla byrjar hér
    var cur = log.ConvertCur(GetData(curPos));
    if (cur != 0)
    {
        rec.Currency = cur;
        rec.IgnoreCurDif = true; // we do not want to do any regulation in a conversion
        rec.AmountCur = importLog.ToDouble(GetData(curAmountPos));
        if (rec.Amount == rec.AmountCur || rec.Amount * rec.AmountCur < 0d) // different sign
        {
            rec.Currency = null;
            rec.AmountCur = 0;
        }
    }
} // kóði sem á að mæla endar hér
, log4j);
*/