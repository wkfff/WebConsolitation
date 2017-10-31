using System;
using System.Security;
using System.Runtime.InteropServices;
using System.IO;

namespace Krista.FM.Common.FileUtils
{
    /// <summary>
    /// Прототипы взяты с http://www.pinvoke.net/
    /// </summary>
    [SuppressUnmanagedCodeSecurityAttribute()]
    internal sealed class Win32
    {
        private Win32()
        {
            // this class is non creatable
        }

        #region Constants

        //
        //  Invalid handle value
        //
        public static readonly IntPtr InvalidHandle = new IntPtr(-1);

        //
        //  Local Memory Flags
        //
        public const uint LMEM_FIXED = 0x0000;
        public const uint LMEM_MOVEABLE = 0x0002;
        public const uint LMEM_NOCOMPACT = 0x0010;
        public const uint LMEM_NODISCARD = 0x0020;
        public const uint LMEM_ZEROINIT = 0x0040;
        public const uint LMEM_MODIFY = 0x0080;
        public const uint LMEM_DISCARDABLE = 0x0F00;
        public const uint LMEM_VALID_FLAGS = 0x0F72;
        public const uint LMEM_INVALID_HANDLE = 0x8000;
        public const uint LMEM_DISCARDED = 0x4000;
        public const uint LMEM_LOCKCOUNT = 0x00FF;

        #endregion

        #region Error Codes

        public const int ERROR_SUCCESS = 0;
        public const int ERROR_OUTOFMEMORY = 14;

        #endregion

        #region Kernel32 imports

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = false)]
        public static extern bool CloseHandle(
            [In]      IntPtr hObject                   // handle
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = false)]
        public static extern IntPtr LocalAlloc(
            [In]      uint nFlags,                     // allocation type
            [In]      uint nSize                       // bytes to allocate
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = false)]
        public static extern IntPtr LocalFree(
            [In]      IntPtr pBuffer                   // pointer to buffer
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = false)]
        public static extern void RtlZeroMemory(
            [In]      IntPtr pBuffer,                  // pointer to buffer
            [In]      uint nSize                       // buffer length
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = false)]
        public static extern void CopyMemory(
            [In]      IntPtr pDestBuffer,              // pointer to destination buffer
            [In]      IntPtr pSourceBuffer,            // pointer to source buffer
            [In]      uint nSize                       // buffer length
            );

        #endregion
    }

    [SuppressUnmanagedCodeSecurityAttribute()]
    internal sealed class FileNative
    {
        private FileNative()
		{
			// this class is non creatable
		}

        #region Константы
        public const long READ_CONTROL = 0x00020000L;

        public const int FILE_SHARE_NONE = 0x00000000;
        public const int FILE_SHARE_READ = 0x00000001;
        public const int FILE_SHARE_WRITE = 0x00000002;
        public const int FILE_SHARE_DELETE = 0x00000004;

        public const int CREATE_NEW = 1;
        public const int CREATE_ALWAYS = 2;
        public const int OPEN_EXISTING = 3;
        public const int OPEN_ALWAYS = 4;
        public const int TRUNCATE_EXISTING = 5;

        public const int FILE_READ_DATA = 0x0001;

        public const int FILE_FLAG_WRITE_THROUGH = unchecked((int)0x80000000);
        public const int FILE_FLAG_OVERLAPPED = 0x40000000;
        public const int FILE_FLAG_NO_BUFFERING = 0x20000000;
        public const int FILE_FLAG_RANDOM_ACCESS = 0x10000000;
        public const int FILE_FLAG_SEQUENTIAL_SCAN = 0x08000000;
        public const int FILE_FLAG_DELETE_ON_CLOSE = 0x04000000;
        public const int FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;
        public const int FILE_FLAG_POSIX_SEMANTICS = 0x01000000;
        public const int FILE_FLAG_OPEN_REPARSE_POINT = 0x00200000;
        public const int FILE_FLAG_OPEN_NO_RECALL = 0x00100000;
        public const int FILE_FLAG_FIRST_PIPE_INSTANCE = 0x00080000;

        public const int FILE_ATTRIBUTE_READONLY = 0x00000001;
        public const int FILE_ATTRIBUTE_HIDDEN = 0x00000002;
        public const int FILE_ATTRIBUTE_SYSTEM = 0x00000004;
        public const int FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
        public const int FILE_ATTRIBUTE_ARCHIVE = 0x00000020;
        public const int FILE_ATTRIBUTE_DEVICE = 0x00000040;
        public const int FILE_ATTRIBUTE_NORMAL = 0x00000080;
        public const int FILE_ATTRIBUTE_TEMPORARY = 0x00000100;
        public const int FILE_ATTRIBUTE_SPARSE_FILE = 0x00000200;
        public const int FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400;
        public const int FILE_ATTRIBUTE_COMPRESSED = 0x00000800;
        public const int FILE_ATTRIBUTE_OFFLINE = 0x00001000;
        public const int FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x00002000;
        public const int FILE_ATTRIBUTE_ENCRYPTED = 0x00004000;

        public const int SECURITY_ANONYMOUS = 0 << 16;
        public const int SECURITY_IDENTIFICATION = 1 << 16;
        public const int SECURITY_IMPERSONATION = 2 << 16;
        public const int SECURITY_DELEGATION = 3 << 16;

        public const int SECURITY_CONTEXT_TRACKING = 0x00040000;
        public const int SECURITY_EFFECTIVE_ONLY = 0x00080000;

        public const int SECURITY_SQOS_PRESENT = 0x00100000;
        public const int SECURITY_VALID_SQOS_FLAGS = 0x001F0000;

        public const int GENERIC_READ = unchecked((int)0x80000000);
        public const int GENERIC_WRITE = 0x40000000;
        public const int GENERIC_EXECUTE = 0x20000000;
        public const int GENERIC_ALL = 0x10000000;

        #endregion

        #region Структуры

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public class SecurityAttributes
        {
            // Specifies the size, in bytes, of this structure
            public int Length = Marshal.SizeOf(typeof(SecurityAttributes));
            // Pointer to a security descriptor for the object that controls the sharing of it
            public IntPtr SecurityDescriptor = IntPtr.Zero;
            // Specifies whether the returned handle is inherited when a new process is created
            public bool InheritHandle = false;
            // Size of this structure
            public static readonly int Size = Marshal.SizeOf(typeof(SecurityAttributes));
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public class Overlapped
        {
            // Reserved for operating system use
            public IntPtr InternalLow = IntPtr.Zero;
            public IntPtr InternalHigh = IntPtr.Zero;
            // File position at which to start the transfer
            public int OffsetLow = 0;
            public int OffsetHigh = 0;
            // Handle to an event set to the signaled state when the operation has been completed
            public IntPtr hEvent = IntPtr.Zero;
            // Fileds offset in this structure
            public static readonly int InternalLowOffset = (int)Marshal.OffsetOf(typeof(Overlapped), "InternalLow");
            public static readonly int InternalHighOffset = (int)Marshal.OffsetOf(typeof(Overlapped), "InternalHigh");
            public static readonly int OffsetLowOffset = (int)Marshal.OffsetOf(typeof(Overlapped), "OffsetLow");
            public static readonly int OffsetHighOffset = (int)Marshal.OffsetOf(typeof(Overlapped), "OffsetHigh");
            public static readonly int hEventOffset = (int)Marshal.OffsetOf(typeof(Overlapped), "hEvent");
            // Size of this structure
            public static readonly int Size = Marshal.SizeOf(typeof(Overlapped));
        }

        #endregion

        #region Kernel32 imports
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CreateFile(
            [In]      string lpFileName,               // file name
            [In]      int dwDesiredAccess,             // accessMode mode
            [In]      int dwShareMode,                 // share mode
            [In]      SecurityAttributes attr,         // security attributes
            [In]      int dwCreation,                  // how to create
            [In]      int dwFlagsAndAttributes,        // file attributes
            [In]      IntPtr hTemplateFile             // handle to template file
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool ReadFile(
            [In]      IntPtr hFile,                    // handle to file
            [In]      IntPtr lpBuffer,                 // data buffer
            [In]      int nNumberOfBytesToRead,        // number of bytes to read
            [Out]     out int lpNumberOfBytesRead,     // number of bytes read
            [In]      IntPtr lpOverlapped              // overlapped structure
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool ReadFile(
            [In]      IntPtr hFile,                    // handle to file
            [In]      IntPtr lpBuffer,                 // data buffer
            [In]      int nNumberOfBytesToRead,        // number of bytes to read
            [In]      IntPtr lpNumberOfBytesRead,      // number of bytes read
            [In]      IntPtr lpOverlapped              // overlapped structure
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern unsafe bool ReadFile(int hFile,
           void* lpBuffer, int nBytesToRead,
           int* nBytesRead, int overlapped);


        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool WriteFile(
            [In]      IntPtr hFile,                    // handle to file
            [In]      IntPtr lpBuffer,                 // data buffer
            [In]      int nNumberOfBytesToWrite,       // number of bytes to write
            [Out]     out int lpNumberOfBytesWritten,  // number of bytes read
            [In]      IntPtr lpOverlapped              // overlapped structure
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool WriteFile(
            [In]      IntPtr hFile,                    // handle to file
            [In]      IntPtr lpBuffer,                 // data buffer
            [In]      int nNumberOfBytesToWrite,       // number of bytes to write
            [In]      IntPtr lpNumberOfBytesWritten,   // number of bytes written
            [In]      IntPtr lpOverlapped              // overlapped structure
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool FlushFileBuffers(
            [In]      IntPtr hFile                     // handle to file
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool GetOverlappedResult(
            [In]      IntPtr hFile,                    // handle to file
            [In]      IntPtr lpOverlapped,             // overlapped structure
            [Out]     out int lpNumberOfBytes,         // number of bytes transferred
            [In]      bool bWait                       // wait for completion
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetFileSize(
            [In]    IntPtr hFile, 
            [Out]   out int lpFileSizeHigh
            );

        #endregion


    }

    /// <summary>
    /// Вспомогательный класс для работы с файлами с помощью WinAPI.
    /// Для тех случаев когда невозможно обойтись штатными средтствами
    /// (например чтение заблокированных файлов)
    /// </summary>
    public struct FileHelper
    {
        private static unsafe int Read(IntPtr fileHandle, byte[] buffer, int index, int count)
        {
            int n = 0;
            fixed (byte* p = buffer)
            {
                FileNative.ReadFile((int)fileHandle, p + index, count, &n, 0);
            }
            return n;
        }

        /// <summary>
        /// Проверить занят ли файл другим процессом
        /// </summary>
        /// <param name="fileName">имя файла</param>
        /// <returns>true/false</returns>
        public static bool FileIsVacant(string fileName)
        {
            IntPtr fileHandle = FileNative.CreateFile(
                fileName,
                (int)FileNative.FILE_READ_DATA,
                FileNative.FILE_SHARE_NONE,
                null,
                FileNative.OPEN_EXISTING,
                FileNative.FILE_ATTRIBUTE_NORMAL,
                IntPtr.Zero
            );
            bool fileIsVacant = (fileHandle != Win32.InvalidHandle);
            if (fileIsVacant) 
                Win32.CloseHandle(fileHandle);
            return fileIsVacant;
        }

        public static bool FileAllowExclusiveAccess(string fileName)
        {
            IntPtr fileHandle = FileNative.CreateFile(
                fileName,
                (int)FileNative.FILE_READ_DATA,
                FileNative.FILE_SHARE_WRITE,
                null,
                FileNative.OPEN_EXISTING,
                FileNative.FILE_ATTRIBUTE_NORMAL,
                IntPtr.Zero
            );
            bool allow = (fileHandle != Win32.InvalidHandle);
            if (allow)
                Win32.CloseHandle(fileHandle);
            return allow;

        }

        /// <summary>
        /// Прочитать таки содержимое файла (независимо от того занят он или нет).
        /// </summary>
        /// <param name="fileName">путь к файлу</param>
        /// <returns>содержимое файла (массив байт)</returns>
        public static byte[] ReadFileData(string fileName)
        {
            byte[] fileData = null;
            // проверяем наличие файла
            if (!File.Exists(fileName))
                throw new Exception(String.Format("Файл '{0}' не существует", fileName));
            IntPtr fileHandle = FileNative.CreateFile(
                fileName, 
                (int)FileNative.FILE_READ_DATA,
                FileNative.FILE_SHARE_READ|FileNative.FILE_SHARE_WRITE,
                null,
                FileNative.OPEN_EXISTING,
                FileNative.FILE_ATTRIBUTE_NORMAL,
                IntPtr.Zero
            );
            if (fileHandle == Win32.InvalidHandle)
                throw new Exception(String.Format("Невозможно открыть файл '{0}'", fileName));

            try
            {
                int fileSizeH = 0;
                int fileSize = FileNative.GetFileSize(fileHandle, out fileSizeH);
                fileData = new byte[fileSize];
                Read(fileHandle, fileData, 0, fileSize);
            }
            finally
            {
                Win32.CloseHandle(fileHandle);
            }
            return fileData;
        }

        /// <summary>
        /// Проверка значения атрибута 'readOnly'
        /// </summary>
        /// <param name="FileName">Имя проверяемого файла</param>
        /// <returns>Возвращае true, если фаыл открыт только для чтения, и false в противном случае   </returns>
        public static bool IsFileReadOnly(string FileName)
        {
            FileInfo fInfo = new FileInfo(FileName);
            return fInfo.IsReadOnly;
        }

        /// <summary>
        /// Изменение значения атрибута "readOnly"
        /// </summary>
        /// <param name="FileName">Полный адресс, изменяемого файла</param>
        /// <param name="SetReadOnly">Значение атрибута</param>
        public static void SetFileReadAccess(string FileName, bool SetReadOnly)
        {
            FileInfo fInfo = new FileInfo(FileName);
            fInfo.IsReadOnly = SetReadOnly;
        }
    }

}