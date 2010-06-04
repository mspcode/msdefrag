﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MSDefragLib.Defragmenter
{
    public abstract class BaseDefragmenter : IDefragmenter
    {
        #region EventDispatcher

        public DefragEventDispatcher defragEventDispatcher { get; set; }

        public BaseDefragmenter()
        {
            defragEventDispatcher = new DefragEventDispatcher();
        }

        public void ShowLogMessage(Int16 level, String message)
        {
            defragEventDispatcher.AddLogMessage(level, message);
        }

        public void ShowFilteredClusters(Int32 clusterBegin, Int32 clusterEnd)
        {
            IList<MapClusterState> clusters = diskMap.GetFilteredClusters(clusterBegin, clusterEnd);

            defragEventDispatcher.AddFilteredClusters(clusters);
        }

        public void ShowProgress(Double progress, Double all)
        {
            defragEventDispatcher.UpdateProgress(progress, all);
        }

        public void ResendAllClusters()
        {
            if (diskMap == null || defragEventDispatcher == null)
            {
                return;
            }

            IList<MapClusterState> clusters = diskMap.GetAllFilteredClusters();
            defragEventDispatcher.AddFilteredClusters(clusters);
        }

        public void Pause()
        {
            defragEventDispatcher.Pause = true;
        }

        public void Continue()
        {
            defragEventDispatcher.Continue = true;

            ResendAllClusters();
        }

        #endregion

        #region Events

        public event EventHandler<ProgressEventArgs> ProgressEvent
        {
            add { defragEventDispatcher.ProgressEvent += value; }
            remove { defragEventDispatcher.ProgressEvent -= value; }
        }

        public event EventHandler<FilteredClusterEventArgs> UpdateFilteredDiskMapEvent
        {
            add { defragEventDispatcher.UpdateFilteredDiskMapEvent += value; }
            remove { defragEventDispatcher.UpdateFilteredDiskMapEvent -= value; }
        }

        public event EventHandler<LogMessagesEventArgs> LogMessageEvent
        {
            add { defragEventDispatcher.UpdateLogMessagesEvent += value; }
            remove { defragEventDispatcher.UpdateLogMessagesEvent -= value; }
        }

        #endregion

        #region Threading

        private Thread defragThread;
        private Thread eventDispatcherThread;

        public abstract void BeginDefragmentation(string parameter);
        public abstract void FinishDefragmentation(int timeoutMS);

        public void StartDefragmentation(string parameter)
        {
            defragThread = new Thread(Defrag);
            defragThread.Priority = ThreadPriority.Lowest;

            defragThread.Start();

            eventDispatcherThread = new Thread(EventDispatcher);
            eventDispatcherThread.Priority = ThreadPriority.Normal;

            eventDispatcherThread.Start();
        }

        public void StopDefragmentation(int timeoutMs)
        {
            FinishDefragmentation(5000);

            if (defragThread.IsAlive)
            {
                defragThread.Interrupt();
                defragThread.Join();
            }

            if (eventDispatcherThread.IsAlive)
            {
                eventDispatcherThread.Interrupt();
                eventDispatcherThread.Join();
            }
        }

        private void Defrag()
        {
            BeginDefragmentation(@"C:\*");
        }

        private void EventDispatcher()
        {
            defragEventDispatcher.StartEventDispatcher();
        }

        #endregion

        #region DiskMap

        public abstract DiskMap diskMap { get; set; }

        public Int32 NumFilteredClusters
        {
            get
            {
                if (diskMap != null)
                {
                    return diskMap.NumFilteredClusters;
                }
                else
                {
                    return 0;
                }
            }

            set
            {
                if (diskMap != null)
                {
                    diskMap.NumFilteredClusters = value;
                    ResendAllClusters();
                }
            }
        }

        public void DisplayCluster(Int32 clusterBegin, Int32 clusterEnd, eClusterState newState)
        {
            if (diskMap == null)
            {
                return;
            }

            diskMap.SetClusterState(clusterBegin, clusterEnd, newState, defragEventDispatcher.Pause == false);

            ShowFilteredClusters(clusterBegin, clusterEnd);
        }

        #endregion
    }
}
