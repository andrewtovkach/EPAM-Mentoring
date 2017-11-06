using System;
using System.Runtime.InteropServices;

namespace PowerManagementAPI
{
    [Guid("2478fe3f-682b-4fd0-ac1d-41c71e509a94")]
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IPowerManagementApi
    {
        SYSTEM_POWER_INFORMATION GetSystemPowerInformation();
        SYSTEM_BATTERY_STATE GetSystemBatteryState();
        DateTime GetLastSleepTime();
        DateTime GetLastWakeTime();
        bool GetIsReserveHiberFile();
        void SetHibernateState();
        void SetStandbyState();
    }

    public struct SYSTEM_POWER_INFORMATION
    {
        public uint MaxIdlenessAllowed;
        public uint Idleness;
        public uint TimeRemaining;
        public byte CoolingMode;
    }

    public struct SYSTEM_BATTERY_STATE
    {
        public bool AcOnLine;
        public bool BatteryPresent;
        public bool Charging;
        public bool Discharging;
        public byte Spare1;
        public byte Spare2;
        public byte Spare3;
        public byte Spare4;
        public uint MaxCapacity;
        public uint RemainingCapacity;
        public uint Rate;
        public uint EstimatedTime;
        public uint DefaultAlert1;
        public uint DefaultAlert2;
    }

    [Guid("1483bebd-6bb1-4ae1-8e95-3f877f861705")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public class PowerManagementApi: IPowerManagementApi
    {
        private enum InformationLevel
        {
            SystemPowerInformation = 12,
            SystemBatteryState = 5,
            LastSleepTime = 15,
            LastWakeTime = 14,
            SystemReserveHiberFile = 10
        }

        [DllImport("powrprof.dll")]
        private static extern uint CallNtPowerInformation(
            int informationLevel,
            IntPtr lpInputBuffer,
            int nInputBufferSize,
            out SYSTEM_POWER_INFORMATION spi,
            int nOutputBufferSize
        );

        [DllImport("powrprof.dll")]
        private static extern uint CallNtPowerInformation(
            int informationLevel,
            IntPtr lpInputBuffer,
            int nInputBufferSize,
            out SYSTEM_BATTERY_STATE sbs,
            int nOutputBufferSize
        );

        [DllImport("powrprof.dll")]
        private static extern uint CallNtPowerInformation(
            int informationLevel,
            IntPtr lpInputBuffer,
            int nInputBufferSize,
            out ulong lst,
            int nOutputBufferSize
        );

        [DllImport("powrprof.dll")]
        private static extern uint CallNtPowerInformation(
            int informationLevel,
            IntPtr lpInputBuffer,
            int nInputBufferSize,
            out bool srhf,
            int nOutputBufferSize
        );

        [DllImport("Powrprof.dll", SetLastError = true)]
        private static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);

        public SYSTEM_POWER_INFORMATION GetSystemPowerInformation()
        {
            SYSTEM_POWER_INFORMATION systemPowerInformation;
            CallNtPowerInformation(
                (int)InformationLevel.SystemPowerInformation,
                IntPtr.Zero,
                0,
                out systemPowerInformation,
                Marshal.SizeOf(typeof(SYSTEM_POWER_INFORMATION))
            );
            return systemPowerInformation;
        }

        public SYSTEM_BATTERY_STATE GetSystemBatteryState()
        {
            SYSTEM_BATTERY_STATE systemBatteryState;
            CallNtPowerInformation(
                (int)InformationLevel.SystemBatteryState,
                IntPtr.Zero,
                0,
                out systemBatteryState,
                Marshal.SizeOf(typeof(SYSTEM_BATTERY_STATE))
            );
            return systemBatteryState;
        }

        private static DateTime GetTime(ulong nanoseconds, ulong ticksPerNanosecond)
        {
            DateTime pointOfReference = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long ticks = (long)(nanoseconds / ticksPerNanosecond);
            return pointOfReference.AddTicks(ticks);
        }

        public DateTime GetLastSleepTime()
        {
            ulong lastSleepTime;
            CallNtPowerInformation(
                (int)InformationLevel.LastSleepTime,
                IntPtr.Zero,
                0,
                out lastSleepTime,
                Marshal.SizeOf(typeof(ulong))
            );
            return GetTime(lastSleepTime, 100);
        }

        public DateTime GetLastWakeTime()
        {
            ulong lastWakeTime;
            CallNtPowerInformation(
                (int)InformationLevel.LastWakeTime,
                IntPtr.Zero,
                0,
                out lastWakeTime,
                Marshal.SizeOf(typeof(ulong))
            );
            return GetTime(lastWakeTime, 100);
        }

        public bool GetIsReserveHiberFile()
        {
            bool isReserveHiberFile;
            CallNtPowerInformation(
                (int)InformationLevel.SystemReserveHiberFile,
                IntPtr.Zero,
                0,
                out isReserveHiberFile,
                Marshal.SizeOf(typeof(bool))
            );
            return isReserveHiberFile;
        }

        public void SetHibernateState()
        {
            SetSuspendState(true, true, true);
        }

        public void SetStandbyState()
        {
            SetSuspendState(false, true, true);
        }
    }
}