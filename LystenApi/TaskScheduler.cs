using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;


namespace LystenApi
{
    public class TaskScheduler
    {
        static Timer timer;

        public static void Main(string[] args)
        {
            schedule_Timer();
            Console.ReadLine();
        }

        public  void schedule_Timer()
        {
            Console.WriteLine("### Timer Started ###");

            DateTime nowTime = DateTime.Now;
            DateTime scheduledTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 0, 0, 10, 0); //Specify your scheduled time HH,MM,SS [8am and 42 minutes]
            if (nowTime > scheduledTime)
            {
                scheduledTime = scheduledTime.AddMinutes(10);
            }

            double tickTime = (double)(scheduledTime - DateTime.Now).TotalMilliseconds;
            timer = new Timer(tickTime);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

        public  void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("### Timer Stopped ### \n");
            timer.Stop();
            Console.WriteLine("### Scheduled Task Started ### \n\n");
            Console.WriteLine("Hello World!!! - Performing scheduled task\n");
            Console.WriteLine("### Task Finished ### \n\n");
            schedule_Timer();
        }
    }
}
