using System;
using System.Threading;
using Altom.AltUnityDriver.Logging;
using NLog;

namespace Altom.AltUnityDriver.Commands
{
    public class AltUnityWaitForObjectNotBePresent : AltUnityBaseFindObjects
    {
        readonly Logger logger = DriverLogManager.Instance.GetCurrentClassLogger();
        AltUnityFindObject findObject;
        private readonly string path;
        double timeout;
        double interval;
        public AltUnityWaitForObjectNotBePresent(IDriverCommunication commHandler, By by, string value, By cameraBy, string cameraValue, bool enabled, double timeout, double interval) : base(commHandler)
        {
            findObject = new AltUnityFindObject(commHandler, by, value, cameraBy, cameraValue, enabled);
            path = SetPath(by, value);

            this.timeout = timeout;
            this.interval = interval;
            if (timeout <= 0) throw new ArgumentOutOfRangeException("timeout");
            if (interval <= 0) throw new ArgumentOutOfRangeException("interval");
        }
        public void Execute()
        {
            double time = 0;
            bool found = false;
            AltUnityObject altElement;

            logger.Debug("Waiting for element " + path + " to not be present");
            while (time < timeout)
            {
                found = false;
                try
                {
                    altElement = findObject.Execute();
                    found = true;
                    Thread.Sleep(System.Convert.ToInt32(interval * 1000));
                    time += interval;

                }
                catch (NotFoundException)
                {
                    break;
                }
            }
            if (found)
                throw new WaitTimeOutException("Element " + path + " still found after " + timeout + " seconds");
        }
    }
}