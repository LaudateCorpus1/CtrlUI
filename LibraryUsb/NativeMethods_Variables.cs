﻿using System;

namespace LibraryUsb
{
    public class NativeMethods_Variables
    {
        public static Guid GuidClassHidDevice = new Guid("4D1E55B2-F16F-11CF-88CB-001111000030");
        public static Guid GuidClassHidClass = new Guid("745A17A0-74D3-11D0-B6FE-00A0C90F57DA");
        public static Guid GuidClassSystem = new Guid("4D36E97D-E325-11CE-BFC1-08002BE10318");
        public static Guid GuidClassXboxBus = new Guid("F679F562-3164-42CE-A4DB-E7DDBE723909");
        public static Guid GuidClassDS3ScpDriver = new Guid("E2824A09-DBAA-4407-85CA-C8E8FF5F6FFA");

        public const int INVALID_HANDLE_VALUE = -1;

        public enum CREATION_FLAG : uint
        {
            CREATE_NEW = 1,
            CREATE_ALWAYS = 2,
            OPEN_EXISTING = 3,
            OPEN_ALWAYS = 4,
            TRUNCATE_EXISTING = 5
        }

        public enum GENERIC_MODE : uint
        {
            GENERIC_READ = 0x80000000,
            GENERIC_WRITE = 0x40000000,
            GENERIC_EXECUTE = 0x20000000,
            GENERIC_ALL = 0x10000000
        }

        public enum FILE_SHARE : uint
        {
            FILE_SHARE_NONE = 0x00000000,
            FILE_SHARE_READ = 0x00000001,
            FILE_SHARE_WRITE = 0x00000002,
            FILE_SHARE_READ_WRITE = 0x00000003,
            FILE_SHARE_DELETE = 0x00000004,
            FILE_SHARE_VALID_FLAGS = 0x00000007
        }

        public enum FILE_ATTRIBUTE : uint
        {
            FILE_ATTRIBUTE_READONLY = 0x00000001,
            FILE_ATTRIBUTE_HIDDEN = 0x00000002,
            FILE_ATTRIBUTE_SYSTEM = 0x00000004,
            FILE_ATTRIBUTE_DIRECTORY = 0x00000010,
            FILE_ATTRIBUTE_ARCHIVE = 0x00000020,
            FILE_ATTRIBUTE_DEVICE = 0x00000040,
            FILE_ATTRIBUTE_NORMAL = 0x00000080,
            FILE_ATTRIBUTE_TEMPORARY = 0x00000100,
            FILE_ATTRIBUTE_SPARSE_FILE = 0x00000200,
            FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400,
            FILE_ATTRIBUTE_COMPRESSED = 0x00000800,
            FILE_ATTRIBUTE_OFFLINE = 0x00001000,
            FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x00002000,
            FILE_ATTRIBUTE_ENCRYPTED = 0x00004000,
            FILE_ATTRIBUTE_INTEGRITY_STREAM = 0x00008000,
            FILE_ATTRIBUTE_VIRTUAL = 0x00010000,
            FILE_ATTRIBUTE_NO_SCRUB_DATA = 0x00020000,
            FILE_ATTRIBUTE_RECALL_ON_OPEN = 0x00040000,
            FILE_ATTRIBUTE_RECALL_ON_DATA_ACCESS = 0x00400000
        }

        public enum FILE_FLAG : uint
        {
            FILE_FLAG_NORMAL = 0x00000000,
            FILE_FLAG_OPEN_REQUIRING_OPLOCK = 0x00040000,
            FILE_FLAG_FIRST_PIPE_INSTANCE = 0x00080000,
            FILE_FLAG_OPEN_NO_RECALL = 0x00100000,
            FILE_FLAG_OPEN_REPARSE_POINT = 0x00200000,
            FILE_FLAG_SESSION_AWARE = 0x00800000,
            FILE_FLAG_POSIX_SEMANTICS = 0x01000000,
            FILE_FLAG_BACKUP_SEMANTICS = 0x02000000,
            FILE_FLAG_DELETE_ON_CLOSE = 0x04000000,
            FILE_FLAG_SEQUENTIAL_SCAN = 0x08000000,
            FILE_FLAG_RANDOM_ACCESS = 0x10000000,
            FILE_FLAG_NO_BUFFERING = 0x20000000,
            FILE_FLAG_OVERLAPPED = 0x40000000,
            FILE_FLAG_WRITE_THROUGH = 0x80000000
        }
    }
}