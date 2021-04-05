using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerImplementation
{
    public static class TaskManager
    {
        private static readonly HashSet<Tuple<string, Task>> _tasks = new HashSet<Tuple<string, Task>>();
        public static IReadOnlyCollection<Tuple<string, Task>> Tasks => _tasks;

        public static void Add(string name, Task task)
        {
            _tasks.Add(Tuple.Create(name, task));
        }
    }
}
