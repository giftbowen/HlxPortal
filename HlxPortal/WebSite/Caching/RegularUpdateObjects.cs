namespace LeSan.HlxPortal.WebSite
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using LeSan.HlxPortal.Common;

    /// <summary>
    /// The regular update objects class
    /// </summary>
    /// <typeparam name="T">The update objects type</typeparam>
    public class RegularUpdateObjects<T>
    {
        /// <summary>
        /// A field for the instance of the class
        /// </summary>
        private static RegularUpdateObjects<T> instance = new RegularUpdateObjects<T>();

        /// <summary>
        /// The last update time for each object
        /// </summary>
        private Dictionary<string, UpdateTask> updateTasks;

        /// <summary>
        /// Prevents a default instance of the <see cref="RegularUpdateObjects&lt;T&gt;" /> class from being created.
        /// </summary>
        private RegularUpdateObjects()
        {
            this.updateTasks = new Dictionary<string, UpdateTask>(0);
        }

        /// <summary>
        /// The instance of the class. 
        /// </summary>
        public static RegularUpdateObjects<T> Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Get the object instance with default key. 
        /// </summary>
        public static T DefaultObjectInstance
        {
            get
            {
                return GetObjectInstance(RegularUpdateTasks.DefaultKey);
            }
        }

        /// <summary>
        /// Get the object instance with the specific key
        /// </summary>
        /// <param name="key">The key of the object</param>
        /// <returns>The cached object</returns>
        public static T GetObjectInstance(string key)
        {
            if (!Instance.updateTasks.ContainsKey(key))
            {
                throw new KeyNotFoundException("Please make sure the RegularUpdateObjects<>.Instance.Add(key, ...) are called in the Application_Start()");
            }

            return Instance.updateTasks[key].UpdatedObject;
        }

        /// <summary>
        /// Get all the registered object instances. 
        /// </summary>
        /// <returns>All the registered object instances. </returns>
        public static IEnumerable<T> GetAllObjectInstances()
        {
            return Instance.updateTasks.Values.Select(t => t.UpdatedObject);
        }

        /// <summary>
        /// Add a cached object with default key, expiration time span and creation function. 
        /// </summary>
        /// <param name="expirationTimeSpan">The expairation time span</param>
        /// <param name="creationFunc">The creation function</param>
        /// <returns>Task of the creation action</returns>
        public Task AddDefault(TimeSpan expirationTimeSpan, Func<T> creationFunc)
        {
            return this.Add(RegularUpdateTasks.DefaultKey, expirationTimeSpan, creationFunc);
        }

        /// <summary>
        /// Add a cached object with specific key, expiration time span and creation function. 
        /// </summary>
        /// <param name="key">The key of the cached object</param>
        /// <param name="expirationTimeSpan">The expairation time span</param>
        /// <param name="creationFunc">The creation function</param>
        /// <returns>Task of the creation action</returns>
        public Task Add(string key, TimeSpan expirationTimeSpan, Func<T> creationFunc)
        {
            if (this.updateTasks.ContainsKey(key))
            {
                throw new ArgumentException("Each regular update object should be configured once only but we hit twice. ");
            }

            Thread thread = new Thread(new ParameterizedThreadStart(this.UpdateThreadFunction));

            var updateTask = new UpdateTask()
            {
                CreationFunc = creationFunc,
                UpdateThread = thread,
                UpdatedObject = default(T),
                Info = new RegularUpdateTasks.UpdateTaskInfo()
                {
                    ObjectType = typeof(T),
                    Key = key,
                    ExpirationTimeSpan = expirationTimeSpan,
                    UpdatedTimes = 0,
                    SuccessfullyUpdatedTimes = 0,
                }
            };

            this.updateTasks[key] = updateTask;

            thread.Start(updateTask);

            return Task.Run(() => this.Update(updateTask));
        }

        /// <summary>
        /// Update thread function
        /// </summary>
        /// <param name="obj">The thread specific object</param>
        private void UpdateThreadFunction(object obj)
        {
            var task = (UpdateTask)obj;

            while (true)
            {
                if (task.Info.UpdatedTimes > 0 && DateTime.UtcNow - task.Info.LastUpdateTime > task.Info.ExpirationTimeSpan)
                {
                    // The first update is called outside of the thread, and the time now has reached the expiration time. 
                    try
                    {
                        this.Update(task);
                    }
                    catch (Exception ex)
                    {
                        SharedTraceSources.Global.TraceEvent(TraceEventType.Error, 0x0004f847 /* tag_abp7h */, "Exception occured while updating cache");
                        SharedTraceSources.Global.TraceException(ex);
                    }
                }
                else
                {
                    Thread.Sleep(RegularUpdateTasks.MinimalUpdateCycle);
                }
            }
        }

        /// <summary>
        /// Updates lastUpdateTime and take the update action
        /// </summary>
        /// <param name="task">The update task object</param>
        private void Update(UpdateTask task)
        {
            var updateStartTime = DateTime.UtcNow;
            task.Info.LastUpdateTime = updateStartTime;

            try
            {
                task.UpdatedObject = task.CreationFunc();
                task.Info.LastSucceededUpdateTime = updateStartTime;
                task.Info.SuccessfullyUpdatedTimes++;
            }
            finally
            {
                task.Info.UpdatedTimes++;
                RegularUpdateTasks.OnEndUpdate(task.Info);
            }
        }

        /// <summary>
        /// The update task class
        /// </summary>
        private class UpdateTask
        {
            /// <summary>
            /// The updated object with related update functions
            /// </summary>
            public T UpdatedObject { get; set; }

            /// <summary>
            /// The creation function for update
            /// </summary>
            public Func<T> CreationFunc { get; set; }

            /// <summary>
            /// The update thread
            /// </summary>
            public Thread UpdateThread { get; set; }

            /// <summary>
            /// The update task info
            /// </summary>
            public RegularUpdateTasks.UpdateTaskInfo Info { get; set; }
        }
    }
}
