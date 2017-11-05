using System;

namespace PowerManagementAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            var powerManagementApi = new PowerManagementApi();
            var systemInfo = powerManagementApi.GetSystemPowerInformation();
            Console.WriteLine(systemInfo.CoolingMode);
            var batteryState = powerManagementApi.GetSystemBatteryState();
            Console.WriteLine(batteryState.BatteryPresent);
            var lastSleepTime = powerManagementApi.GetLastSleepTime();
            Console.WriteLine(lastSleepTime);
            var lastWakeTime = powerManagementApi.GetLastWakeTime();
            Console.WriteLine(lastWakeTime);
            //powerManagementApi.SetHibernateState();
            Console.ReadKey();
        }
    }
}
