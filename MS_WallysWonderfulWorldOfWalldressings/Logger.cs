/*
** File Name:           Logger.cs
**	Project Name:	    MS_WallysWonderfulWorldOfWalldressings
** Author:                 Matthew G. Schatz
** Date:                    November 29, 2019
**	Description:	        This file contains the class definition for the logger class. This class is used to log events broken down into three categories - errors, status messages,
**                              and a combined collection which contains a combination of error and status messages, so that they can be compared to each other in time.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace MS_WallysWonderfulWorldOfWalldressings
{
    /*
    **	Class Name:	    RecordedEvent
    **	Description:	This class accompanies the Logger class as it is the object that logger uses to keep track of events. Every event that is logged is encapsulated by a RecordEvent which provides details relevant about the event.
    */
    class RecordedEvent
    {
        // This variable is used to give the recorded event a time stamp.
        public DateTime TimeStamp { get; set; }

        // This variable contains the message from the event.
        public string Message { get; set; }

        /*
        **	Method Name:	RecordedEvent()
        **	Parameters:		None. (Is constructor).
        **	Return Values:	Void. (Is constructor).
        **	Description:	This method is the constructor and just sets the initial values for this object.
        */
        public RecordedEvent()
        {
            TimeStamp = new DateTime();
            Message = String.Empty;
        }

        /*
        **	Method Name:	 RecordedEvent
        **	Parameters:		 string IncommingMessage: This parameter represents the incomming message to be recorded as part of the event.
        **                          DateTime IncommingTimeStamp: This parameter is used to provide a timestamp for the event.
        **	Return Values:	 None. (Is Constructor)
        **	Description:	     This is the constructor for the RecordEvent class. It takes a message and a timestamp and combines them into a fully formed RecordEvent.
        */
        public RecordedEvent(string IncommingMessage, DateTime IncommingTimeStamp)
        {
            TimeStamp = IncommingTimeStamp;
            Message = IncommingMessage;
        }

        /*
        **	Method Name:	    ToString
        **	Parameters:		    None
        **	Return Values:	    Void.
        **	Description:	        This is an override of the ToString method, which provides a more relevant string representation of this object.
        */
        public override string ToString()
        {
            return $"[{TimeStamp.ToShortDateString()} - {TimeStamp.ToLongTimeString()}] {Message}";  
        }
    }

    /*
    **	Class Name:	    Logger
    **	Description:	
    */
    static class Logger
    {
        // Enumeration of the three types of logs that exist in this class.
        private enum WhichLogEnum
        {
            ErrorLog,
            StatusLog,
            CombinedLog
        }

        // These objects are used as lock objects, to synchronize multi-threaded access to the desired resources.
        private static readonly object  _ErrorLogLockObject = new object();
        private static readonly object _StatusMessageLogLockObject = new object();
        private static readonly object _CombinedLogLogLockObject = new object();
        private static readonly object _ErrorLogFileLockObject = new object();
        private static readonly object _StatusLogFileLockObject = new object();
        private static readonly object _CombinedLogFileLockObject = new Object();
        private static readonly object _ThreadPoolLockObj = new object();

        // A ThreadPool to keep track of active threads.
        private static List<Thread> ThreadPool = new List<Thread>();

        // I used three sorted lists for my logs, so that there is a seperate Error log and Status log, and then a Combined log which contains all messages produced.
        private static SortedList<int, RecordedEvent> ErrorLog = new SortedList<int, RecordedEvent>();
        private static SortedList<int, RecordedEvent> StatusMessageLog = new SortedList<int, RecordedEvent>();
        private static SortedList<int, RecordedEvent> CombinedLog = new SortedList<int, RecordedEvent>();

        // Counters for the three log containers.
        private static int ErrorCount = 0;
        private static int StatusMessageCount = 0;
        private static int CombinedLogCount = 0;

        // This string is added if a seperate logs directory is desired.
        //private static string LogFilesDirectoryPath = @"Logs\";

        // The locations of the files used to record the logs to. These will be placed in the same directory as the running executable.
        private static string ErrorLogFilePath =  "ErrorLog.txt";
        private static string StatusLogFilePath =  "StatusLog.txt";
        private static string CombinedLogFilePath = "CombinedLog.txt";

        // This bool toggles multi-threaded mode on or off. In off mode the logger does not use any threads or tasks.
        public static bool IsMultiThreaded = true;

        // Public property to allow access to viewing the database, but no setter so this is a read-only property.
        public static SortedList<int, RecordedEvent> ErrorMessages
        {
            get
            {
                lock (_ErrorLogLockObject)
                {
                    return ErrorLog;
                }
            }
        }

        // Public property to allow access to viewing the database, but no setter so this is a read-only property.
        public static SortedList<int, RecordedEvent> StatusMessages
        {
            get
            {
                lock (_StatusMessageLogLockObject)
                {
                    return StatusMessageLog;
                }
            }
        }
        
        // Public property to allow access to viewing the database, but no setter so this is a read-only property.
        public static SortedList<int, RecordedEvent> CombinedMessages
        {
            get
            {
                lock (_CombinedLogLogLockObject)
                {
                    return CombinedLog;
                }
            }
        }

        /*
        **	Method Name:	    HandleErrorLogEntry
        **	Parameters:        String Msg: This string holds the message to be recorded as part of the event.
        **	Return Values:	    Void.
        **	Description:	        This method is used to take a message and combine it with a time stamp as a RecordedEvent object.
        **                              After it passes the newly created RecordedEvent object to methods that will write it to disk as an Error event.		
        */
        private static void HandleErrorLogEntry(string Msg)
        {
            RecordedEvent RE = new RecordedEvent
            {
                Message = Msg,
                TimeStamp = DateTime.Now
            };

            WriteRecordToErrorLog(RE);

            WriteRecordToCombinedLog(RE);

            RemoveThreadFromPool();
        }

        /*
        **	Method Name:	HandleStatusLogEntry()
        **	Parameters:		String Msg: This string holds the message to be recorded as part of the event.
        **	Return Values:	Void.
        **	Description:	    This method is used to take a message and combine it with a time stamp as a RecordedEvent object.
        **                          After it passes the newly created RecordedEvent object to methods that will write it to disk as a Status event.
        */
        private static void HandleStatusLogEntry(string Msg)
        {
            RecordedEvent RE = new RecordedEvent
            {
                Message = Msg,
                TimeStamp = DateTime.Now
            };

            WriteRecordToStatusLog(RE);

            WriteRecordToCombinedLog(RE);

            RemoveThreadFromPool();
        }

        /*
        **	Method Name:	WriteRecordToErrorLog()
        **	Parameters:		RecordedEvent RE: This parameter is a reference to the RecordedEvent to be logged.
        **	Return Values:	Void.
        **	Description:	    This method writes a RecordedEvent to the log file at the location pointed to by ErrorLogFilePath. It also keeps a copy of all logs written to a SortedList in
        **                          memory, incase writing to the disk fails for any reason.
        */
        private static void WriteRecordToErrorLog(RecordedEvent RE)
        {

            try
            {
                Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

                lock (_ErrorLogFileLockObject)
                {
                    using (StreamWriter sw = File.AppendText(ErrorLogFilePath))
                    {
                        sw.WriteLine(RE.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                string ErrorMessage = $"[System-Logger]: Exception caught. Details: {e.Message}";

                RE.Message = ErrorMessage;
                RE.TimeStamp = DateTime.Now;

                lock (_ErrorLogLockObject)
                {
                    ErrorLog.Add(ErrorCount, RE);
                    ErrorCount++;
                }
                lock (_CombinedLogLogLockObject)
                {
                    CombinedLog.Add(CombinedLogCount, RE);
                    CombinedLogCount++;
                }
            }
            
        }

        /*
        **	Method Name:	WriteRecordToStatusLog()
        **	Parameters:		RecordedEvent RE: This parameter is a reference to the RecordedEvent to be logged.
        **	Return Values:	Void.
        **	Description:	    This method writes a RecordedEvent to the log file at the location pointed to by StatusLogFilePath. It also keeps a copy of all logs written to a SortedList in
        **                          memory, incase writing to the disk fails for any reason.
        */
        private static void WriteRecordToStatusLog(RecordedEvent RE)
        {
            try
            {
                Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

                lock (_StatusLogFileLockObject)
                {
                    using (StreamWriter sw = File.AppendText(StatusLogFilePath))
                    {
                        sw.WriteLine(RE.ToString());
                    }
                }

                lock (_StatusMessageLogLockObject)
                {
                    StatusMessageLog.Add(StatusMessageCount, RE);
                    StatusMessageCount++;
                }
            }
            catch(Exception e)
            {

                string ErrorMessage = $"[System-Logger]: Exception caught. Details: {e.Message}";

                RE.Message = ErrorMessage;
                RE.TimeStamp = DateTime.Now;

                lock (_ErrorLogLockObject)
                {
                    ErrorLog.Add(ErrorCount, RE);
                    ErrorCount++;
                }

                lock (_CombinedLogLogLockObject)
                {
                    CombinedLog.Add(CombinedLogCount, RE);
                    CombinedLogCount++;
                }
            }
            
        }

        /*
        **	Method Name:	WriteRecordToCombinedLog()
        **	Parameters:		RecordedEvent RE: This parameter is a reference to the RecordedEvent to be logged.
        **	Return Values:	Void.
        **	Description:	    This method writes a RecordedEvent to the log file at the location pointed to by CombinedLogFilepath. It also keeps a copy of all logs written to a SortedList in
        **                          memory, incase writing to the disk fails for any reason.
        */
        private static void WriteRecordToCombinedLog(RecordedEvent RE)
        {
            try
            {
                Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

                lock (_CombinedLogFileLockObject)
                {
                    using (StreamWriter sw = File.AppendText(CombinedLogFilePath))
                    {
                        sw.WriteLine(RE.ToString());
                    }
                }

                lock (_CombinedLogLogLockObject)
                {
                    CombinedLog.Add(CombinedLogCount, RE);
                    CombinedLogCount++;
                }
            }
            catch(Exception e)
            {
                string ErrorMessage = $"[System-Logger]: Exception caught. Details: {e.Message}";

                RE.Message = ErrorMessage;
                RE.TimeStamp = DateTime.Now;

                lock (_ErrorLogLockObject)
                {
                    ErrorLog.Add(ErrorCount, RE);
                    ErrorCount++;
                }

                lock (_CombinedLogLogLockObject)
                {
                    CombinedLog.Add(CombinedLogCount, RE);
                    CombinedLogCount++;
                }
            }
            
        }

        /*
        **	Method Name:    	LogEvent()
        **	Parameters:		    String Msg: This parameter reprsents the msg to be recorded as part of the log.
        *                               int WhichLog: This parameter determines which type of log this event will be recorded as.
        **	Return Values:	    Void.
        **	Description:	        This method is used to record a recordevent into the relevant log. Locks are used to ensure multithreaded compatibility.
        */
        private static void LogEvent(string Msg, int WhichLog)
        {
            if(WhichLog == (int)WhichLogEnum.ErrorLog)
            {
                try
                {
                    if(IsMultiThreaded == true)
                    {
                        Thread th = new Thread(o => HandleErrorLogEntry((string)o));

                        AddThreadToPool(th);

                        th.Start(Msg);
                    }
                    else
                    {
                        HandleErrorLogEntry(Msg);
                    }
                    
                }
                catch (Exception e)
                {
                    string ErrorMessage = $"[System-Logger]: Exception caught. Details: {e.Message}";
                    HandleErrorLogEntry(ErrorMessage);
                }

            }
            else if (WhichLog == (int)WhichLogEnum.StatusLog)
            {
                try
                {
                    if(IsMultiThreaded == true)
                    {
                        Thread th = new Thread(o => HandleStatusLogEntry((string)o));

                        AddThreadToPool(th);

                        th.Start(Msg);
                    }
                    else
                    {
                        HandleStatusLogEntry(Msg);
                    }
                }
                catch(Exception e)
                {
                    string ErrorMessage = $"[System-Logger]: Exception caught. Details: {e.Message}";
                    HandleErrorLogEntry(ErrorMessage);
                }
            }
            else
            {
                try
                {
                    string LoggerErrorMsg = $"[{DateTime.Now.ToLongTimeString()}] [System-Logger]: Encountered error; Invalid WhichLog value detected. Value detected as: {WhichLog.ToString()}";
                    
                    if(IsMultiThreaded == true)
                    {
                        Thread th = new Thread(o => HandleErrorLogEntry((string)o));

                        AddThreadToPool(th);

                        th.Start(LoggerErrorMsg);
                    }
                    else
                    {
                        HandleErrorLogEntry(LoggerErrorMsg);
                    }
                }
                catch (Exception e)
                {
                    string ErrorMessage = $"[System-Logger]: Exception caught. Details: {e.Message}";
                    HandleErrorLogEntry(ErrorMessage);
                }
                
            }
        }

        /*
        **	Method Name:	    LogError()
        **	Parameters:		    string ErrorMsg: This parameter represents the error message to be recorded.
        **	Return Values:	    Void.
        **	Description:	        This method is used to record a error into the correct database. It simply calls the LogEvent method with an added parameter telling it which type of event it is.
        */
        public static void LogError(string ErrorMsg)
        {
            LogEvent(ErrorMsg, (int)WhichLogEnum.ErrorLog);
        }

        /*
        **	Method Name:	    LogStatus()
        **	Parameters:		    string StatusMsg: This parameter represents the status message to be recorded.
        **	Return Values:	    Void.	
        **	Description:	        This method is used to record a status message into the correct database. It simply calls the LogEvent method with an added parameter telling it which type of event it is.
        */
        public static void LogStatus(string StatusMsg)
        {
            LogEvent(StatusMsg, (int)WhichLogEnum.StatusLog);
        }

        /*
        **	Method Name:	    RemoveThreadFromPool()
        **	Parameters:		    None.
        **	Return Values:	    Void.
        **	Description:	        This method is used to remove a thread from the thread pool. It first determines if a reference to it exists and if so, removes it.
        */
        private static void RemoveThreadFromPool()
        {
            try
            {
                // Remove CurrentThread from thread pool.
                if (ThreadPool.Contains(Thread.CurrentThread))
                {
                    lock (_ThreadPoolLockObj)
                    {
                        ThreadPool.Remove(Thread.CurrentThread);
                    }
                }
            }
            catch(Exception e)
            {
                string ErrorMessage = $"[System-Logger]: Exception caught. Details: {e.Message}";
                HandleErrorLogEntry(ErrorMessage);
            }
            
        }

        /*
        **	Method Name:	    AddThreadToPool()
        **	Parameters:		    Thread ThreadToRemove: This parameter is a reference to the thread to be added.
        **	Return Values:	    Void.
        **	Description:	        This method is used to add a thread to the thread pool. It first determines if a reference to it exists and if not, adds it.
        */
        private static void AddThreadToPool(Thread ThreadToAdd)
        {
            try
            {
                // Add CurrentThread to the thread pool.
                if (ThreadPool.Contains(Thread.CurrentThread) == false)
                {
                    lock (_ThreadPoolLockObj)
                    {
                        ThreadPool.Add(ThreadToAdd);
                    }
                }
            }
            catch (Exception e)
            {
                string ErrorMessage = $"[System-Logger]: Exception caught. Details: {e.Message}";
                HandleErrorLogEntry(ErrorMessage);
            }
            
        }
    }


}
