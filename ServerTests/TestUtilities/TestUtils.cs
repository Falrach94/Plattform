using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SererTests.TestUtilities
{
    public static class TestUtils
    {
        public static T AssertException<T>(Task task, int timeoutMs = 500) where T : Exception
        {
            var waitTask = Task.Run(() => Assert.ThrowsExceptionAsync<T>(async () => await task));

            Assert.IsTrue(waitTask.Wait(timeoutMs), "timeout");

            return waitTask.Result;
        }
        public static T AssertTask<T>(Task<T> task, int timeoutMs = 100)
        {
            var waitTask = Task.Run(() => task);
            Assert.IsTrue(waitTask.Wait(timeoutMs), "timeout");
            return task.Result;
        }
        public static void AssertTask(Task task, int timeoutMs = 100)
        {
            var waitTask = Task.Run(() => task);
            Assert.IsTrue(waitTask.Wait(timeoutMs), "timeout");
        }

        internal static void AssertTaskTimeout(Task task, int timeoutMs = 100)
        {
            var waitTask = Task.Run(() => task);
            Assert.IsFalse(waitTask.Wait(timeoutMs), "timeout");
        }
    }
}
