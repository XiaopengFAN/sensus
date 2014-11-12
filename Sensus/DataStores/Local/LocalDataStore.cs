﻿using Sensus.Probes;
using Sensus.Protocols;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sensus.DataStores.Local
{
    /// <summary>
    /// Responsible for storing data generated by a protocol on the device's local media.
    /// </summary>
    public abstract class LocalDataStore : DataStore
    {
        private Protocol _protocol;

        protected Protocol Protocol
        {
            get { return _protocol; }
        }

        public override bool NeedsToBeRunning
        {
            get { return _protocol.Running; }
        }

        public LocalDataStore()
        {
            CommitDelayMS = 5000;
        }

        public void Start(Protocol protocol)
        {
            _protocol = protocol;
            Start();
        }

        protected override ICollection<Datum> GetDataToCommit()
        {
            List<Datum> data = new List<Datum>();
            foreach (Probe probe in _protocol.Probes)
                lock (probe.CollectedData)
                    if (probe.CollectedData.Count > 0)
                        data.AddRange(probe.CollectedData);

            return data;
        }

        protected override void DataCommitted(ICollection<Datum> data)
        {
            foreach (Probe probe in _protocol.Probes)
                probe.ClearCommittedData(data);
        }

        public abstract ICollection<Datum> GetDataForRemoteDataStore();

        public abstract void ClearDataCommittedToRemoteDataStore(ICollection<Datum> data);
    }
}