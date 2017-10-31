using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Krista.FM.Common
{
    #region Общие моменты из Win32 API

    /// <summary>
    /// Helper method for returning the description associated with a 
    /// <see cref="System.Runtime.InteropServices.Marshal.GetLastWin32Error"/>
    /// error code.
    /// </summary>
    public class Win32Error
    {
        private const int FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x100;
        private const int FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x2000;
        private const int FORMAT_MESSAGE_FROM_HMODULE = 0x800;
        private const int FORMAT_MESSAGE_FROM_STRING = 0x400;
        private const int FORMAT_MESSAGE_FROM_SYSTEM = 0x1000;
        private const int FORMAT_MESSAGE_IGNORE_INSERTS = 0x200;
        private const int FORMAT_MESSAGE_MAX_WIDTH_MASK = 0xFF;

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        private static extern int FormatMessage(
            int dwFlags,
            IntPtr lpSource,
            int dwMessageId,
            int dwLanguageId,
            [MarshalAs(UnmanagedType.LPTStr)]
			StringBuilder lpBuffer,
            int nSize,
            IntPtr Arguments);

        /// <summary>
        /// Returns a string containing the error message for a Win32 API error code.
        /// </summary>
        /// <param name="lastWin32Error">Win32 Error</param>
        /// <returns>Error Message associated with the Win32 Error</returns>
        public static string ErrorMessage(
            int lastWin32Error
            )
        {
            StringBuilder msg = new StringBuilder(256, 256);
            int size = FormatMessage(
                FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
                IntPtr.Zero, lastWin32Error, 0,
                msg, msg.MaxCapacity, IntPtr.Zero);
            return msg.ToString();
        }

        /// <summary>
        /// Private constructor - static methods.
        /// </summary>
        private Win32Error()
        {
            // intentionally blank
        }
    }

    #endregion

    #region Информация о памяти
    public sealed class MemoryStatus
    {
        #region Поля
        public uint MemoryLoad;
        public ulong TotalPhys;
        public ulong AvailPhys;
        public ulong TotalPageFile;
        public ulong AvailPageFile;
        public ulong TotalVirtual;
        public ulong AvailVirtual;
        #endregion

        #region MemoryStatusEx, GlobalMemoryStatusEx, MemoryStatus, GlobalMemoryStatus
        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;

            public MEMORYSTATUSEX()
            {
                this.dwLength = (uint)Marshal.SizeOf(typeof(MemoryStatus.MEMORYSTATUSEX));
            }
        }

        /// <summary>
        /// The MEMORYSTATUS structure contains information about the current state of both physical and virtual memory.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MEMORYSTATUS
        {
            /// <summary>
            /// Size of the MEMORYSTATUS data structure, in bytes. You do not need to set this member before calling the GlobalMemoryStatus function; the function sets it.
            /// </summary>
            public uint dwLength;

            /// <summary>
            /// Number between 0 and 100 that specifies the approximate percentage of physical memory that is in use (0 indicates no memory use and 100 indicates full memory use).
            /// Windows NT:  Percentage of approximately the last 1000 pages of physical memory that is in use.
            /// </summary>
            public uint dwMemoryLoad;

            /// <summary>
            /// Total size of physical memory, in bytes.
            /// </summary>
            public uint dwTotalPhys;

            /// <summary>
            /// Size of physical memory available, in bytes
            /// </summary>
            public uint dwAvailPhys;

            /// <summary>
            /// Size of the committed memory limit, in bytes.
            /// </summary>
            public uint dwTotalPageFile;

            /// <summary>
            /// Size of available memory to commit, in bytes.
            /// </summary>
            public uint dwAvailPageFile;

            /// <summary>
            /// Total size of the user mode portion of the virtual address space of the calling process, in bytes.
            /// </summary>
            public uint dwTotalVirtual;

            /// <summary>
            /// Size of unreserved and uncommitted memory in the user mode portion of the virtual address space of the calling process, in bytes.
            /// </summary>
            public uint dwAvailVirtual;

        } // class MEMORYSTATUS

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GlobalMemoryStatus([In, Out] MEMORYSTATUS lpBuffer);
        #endregion

        public MemoryStatus()
        {
            MEMORYSTATUS ms = new MEMORYSTATUS();
            if (!GlobalMemoryStatus(ms))
                throw new Exception("Не удалось получить информацию о состоянии памяти");

            this.MemoryLoad = ms.dwMemoryLoad;
            this.TotalPageFile = ms.dwTotalPageFile;
            this.AvailPhys = ms.dwAvailPhys;
            this.TotalPageFile = ms.dwTotalPageFile;
            this.AvailPageFile = ms.dwAvailPageFile;
            this.TotalVirtual = ms.dwTotalVirtual;
            this.AvailVirtual = ms.dwAvailVirtual;
        }

        private static string SizeInBytesToString(ulong size)
        {
            ulong gb = size / (1024 * 1024 * 1024);
            size -= gb * 1024 * 1024 * 1024;
            ulong mb = size / (1024 * 1024);
            size -= mb * 1024 * 1024;
            ulong kb = size / 1024;

            return String.Format("\t{0,6}Гб {1,6}Мб {2,6}Кб",
                gb, mb, kb);
        }

        public static string GetAvaillableMemoryReport()
        {
            MemoryStatus mms = new MemoryStatus();
            StringBuilder sb = new StringBuilder(256);
            sb.AppendLine("Состояние памяти:");
            sb.AppendFormat("\tЗагрузка памяти: \t\t\t{0,6}%{1}", mms.MemoryLoad, Environment.NewLine);
            sb.AppendFormat("\tСвободно физической памяти: {0}{1}", SizeInBytesToString(mms.AvailPhys), Environment.NewLine);
            sb.AppendFormat("\tСвободно в файле подкачки: {0}{1}", SizeInBytesToString(mms.AvailPageFile), Environment.NewLine);
            sb.AppendFormat("\tСвободно виртуальной памяти: {0}{1}", SizeInBytesToString(mms.AvailVirtual), Environment.NewLine);
            if (mms.MemoryLoad > 90)
            {
                sb.AppendLine();
                sb.AppendLine("Недостаточно памяти. Закройте неиспользуемые приложения.");
            }
            return sb.ToString();
        }

    }
    #endregion
}