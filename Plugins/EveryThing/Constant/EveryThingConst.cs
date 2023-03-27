using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekDesk.Plugins.EveryThing.Constant
{
    public class EveryThingConst
    {

        public const int EVERYTHING_OK = 0;
        public const int EVERYTHING_ERROR_MEMORY = 1;
        public const int EVERYTHING_ERROR_IPC = 2;
        public const int EVERYTHING_ERROR_REGISTERCLASSEX = 3;
        public const int EVERYTHING_ERROR_CREATEWINDOW = 4;
        public const int EVERYTHING_ERROR_CREATETHREAD = 5;
        public const int EVERYTHING_ERROR_INVALIDINDEX = 6;
        public const int EVERYTHING_ERROR_INVALIDCALL = 7;
        public const int EVERYTHING_REQUEST_FILE_NAME = 0x00000001;
        public const int EVERYTHING_REQUEST_PATH = 0x00000002;
        public const int EVERYTHING_REQUEST_FULL_PATH_AND_FILE_NAME = 0x00000004;
        public const int EVERYTHING_REQUEST_EXTENSION = 0x00000008;
        public const int EVERYTHING_REQUEST_SIZE = 0x00000010;
        public const int EVERYTHING_REQUEST_DATE_CREATED = 0x00000020;
        public const int EVERYTHING_REQUEST_DATE_MODIFIED = 0x00000040;
        public const int EVERYTHING_REQUEST_DATE_ACCESSED = 0x00000080;
        public const int EVERYTHING_REQUEST_ATTRIBUTES = 0x00000100;
        public const int EVERYTHING_REQUEST_FILE_LIST_FILE_NAME = 0x00000200;
        public const int EVERYTHING_REQUEST_RUN_COUNT = 0x00000400;
        public const int EVERYTHING_REQUEST_DATE_RUN = 0x00000800;
        public const int EVERYTHING_REQUEST_DATE_RECENTLY_CHANGED = 0x00001000;
        public const int EVERYTHING_REQUEST_HIGHLIGHTED_FILE_NAME = 0x00002000;
        public const int EVERYTHING_REQUEST_HIGHLIGHTED_PATH = 0x00004000;
        public const int EVERYTHING_REQUEST_HIGHLIGHTED_FULL_PATH_AND_FILE_NAME = 0x00008000;
        public const int EVERYTHING_SORT_NAME_ASCENDING = 1;
        public const int EVERYTHING_SORT_NAME_DESCENDING = 2;
        public const int EVERYTHING_SORT_PATH_ASCENDING = 3;
        public const int EVERYTHING_SORT_PATH_DESCENDING = 4;
        public const int EVERYTHING_SORT_SIZE_ASCENDING = 5;
        public const int EVERYTHING_SORT_SIZE_DESCENDING = 6;
        public const int EVERYTHING_SORT_EXTENSION_ASCENDING = 7;
        public const int EVERYTHING_SORT_EXTENSION_DESCENDING = 8;
        public const int EVERYTHING_SORT_TYPE_NAME_ASCENDING = 9;
        public const int EVERYTHING_SORT_TYPE_NAME_DESCENDING = 10;
        public const int EVERYTHING_SORT_DATE_CREATED_ASCENDING = 11;
        public const int EVERYTHING_SORT_DATE_CREATED_DESCENDING = 12;
        public const int EVERYTHING_SORT_DATE_MODIFIED_ASCENDING = 13;
        public const int EVERYTHING_SORT_DATE_MODIFIED_DESCENDING = 14;
        public const int EVERYTHING_SORT_ATTRIBUTES_ASCENDING = 15;
        public const int EVERYTHING_SORT_ATTRIBUTES_DESCENDING = 16;
        public const int EVERYTHING_SORT_FILE_LIST_FILENAME_ASCENDING = 17;
        public const int EVERYTHING_SORT_FILE_LIST_FILENAME_DESCENDING = 18;
        public const int EVERYTHING_SORT_RUN_COUNT_ASCENDING = 19;
        public const int EVERYTHING_SORT_RUN_COUNT_DESCENDING = 20;
        public const int EVERYTHING_SORT_DATE_RECENTLY_CHANGED_ASCENDING = 21;
        public const int EVERYTHING_SORT_DATE_RECENTLY_CHANGED_DESCENDING = 22;
        public const int EVERYTHING_SORT_DATE_ACCESSED_ASCENDING = 23;
        public const int EVERYTHING_SORT_DATE_ACCESSED_DESCENDING = 24;
        public const int EVERYTHING_SORT_DATE_RUN_ASCENDING = 25;
        public const int EVERYTHING_SORT_DATE_RUN_DESCENDING = 26;
        public const int EVERYTHING_TARGET_MACHINE_X86 = 1;
        public const int EVERYTHING_TARGET_MACHINE_X64 = 2;
        public const int EVERYTHING_TARGET_MACHINE_ARM = 3;
    }
}
