using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace MemoryPressure
{
    class Program
    {
        [DllImport("psapi.dll")]
        public static extern bool EmptyWorkingSet(IntPtr hProcess);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetPriorityClass(IntPtr handle, PriorityClass priorityClass);

        public enum PriorityClass : uint
        {
            ABOVE_NORMAL_PRIORITY_CLASS = 0x8000,
            BELOW_NORMAL_PRIORITY_CLASS = 0x4000,
            HIGH_PRIORITY_CLASS = 0x80,
            IDLE_PRIORITY_CLASS = 0x40,
            NORMAL_PRIORITY_CLASS = 0x20,
            PROCESS_MODE_BACKGROUND_BEGIN = 0x100000,// 'Windows Vista/2008 and higher
            PROCESS_MODE_BACKGROUND_END = 0x200000,//   'Windows Vista/2008 and higher
            REALTIME_PRIORITY_CLASS = 0x100
        }

        static void Main(string[] args)
        {
            if (args.Length < 1 || args.Length > 2)
            {
                Console.WriteLine($"Usage: ChangeWS.exe <processName> [empty/lowpri] ");
                return;
            }

            string processName = args[0];
            string action = args.Length == 1 ? "empty" : args[1];

            if (action.Equals("empty", StringComparison.OrdinalIgnoreCase))
            {
                bool result = ChangeWS(processName, true);
                Console.WriteLine($"EmptyWS: processName Succeeded {result}");
            }
            else
            {
                bool result = ChangeWS(processName, false);
                Console.WriteLine($"SetPriorityClass: processName Succeeded {result}");
            }
        }

        private static bool ChangeWS(string programName, bool emptyWS)
        {

            using (Process process = Process.GetProcessesByName(programName).FirstOrDefault())
            {
                if (process != null)
                {
                    if (emptyWS)
                    {
                        Console.WriteLine($"EmptyWS: {process.ProcessName}");
                        return EmptyWorkingSet(process.Handle);
                    }
                    else
                    {
                        Console.WriteLine($"SetPriorityClass: {process.ProcessName}");
                        return SetPriorityClass(process.Handle, PriorityClass.PROCESS_MODE_BACKGROUND_BEGIN);
                    }
                }
                else
                {
                    Console.WriteLine($"ChangeWS: can not find: {programName}");

                    return false;
                }
            }
        }
    }
}
