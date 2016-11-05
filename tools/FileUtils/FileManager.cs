using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class FileManager
{
#if UNITY_EDITOR || UNITY_IPHONE
        private const string FILEPROTOCOL = "";
#else
        private const string FILEPROTOCOL = "file://";
#endif


}

