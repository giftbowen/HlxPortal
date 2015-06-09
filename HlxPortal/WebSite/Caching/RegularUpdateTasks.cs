namespace LeSan.HlxPortal.WebSite
{
    using System;

    /// <summary>
    /// The regular update tasks class
    /// </summary>
    public static class RegularUpdateTasks
    {
        /// <summary>
        /// The default key of cached object.
        /// </summary>
        public static readonly string DefaultKey = string.Empty;

        /// <summary>
        /// Check for expiration and wait for certain time
        /// </summary>
        public static readonly TimeSpan MinimalUpdateCycle = TimeSpan.FromMinutes(1);

        /// <summary>
        /// The event handler for object updated
        /// </summary>
        public static event EventHandler<UpdateEventArgs> EndUpdate;

        /// <summary>
        /// Called when update task finished
        /// </summary>
        /// <param name="info">The update task info</param>
        public static void OnEndUpdate(UpdateTaskInfo info)
        {
            if (EndUpdate != null)
            {
                var args = new UpdateEventArgs(info);

                EndUpdate(null, args);
            }
        }

        /// <summary>
        /// The update event args class
        /// </summary>
        public class UpdateEventArgs : EventArgs
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="UpdateEventArgs" /> class.
            /// </summary>
            /// <param name="info">The update task info ojbect</param>
            public UpdateEventArgs(UpdateTaskInfo info)
            {
                this.Info = info;
            }

            /// <summary>
            /// The <see cref="UpdateTaskInfo"/> object
            /// </summary>
            public UpdateTaskInfo Info { get; private set; }
        }

        /// <summary>
        /// The update task info
        /// </summary>
        public class UpdateTaskInfo
        {
            /// <summary>
            /// The object type of the task
            /// </summary>
            public Type ObjectType { get; set; }

            /// <summary>
            ///  The key of the updated object
            /// </summary>
            public string Key { get; set; }

            /// <summary>
            /// The expiration time span for each update
            /// </summary>
            public TimeSpan ExpirationTimeSpan { get; set; }

            /// <summary>
            /// The last update time for the updated object
            /// </summary>
            public DateTime LastUpdateTime { get; set; }

            /// <summary>
            /// The last succeeded update time for the updated object
            /// </summary>
            public DateTime LastSucceededUpdateTime { get; set; }

            /// <summary>
            /// The executed times of the update action
            /// </summary>
            public int UpdatedTimes { get; set; }

            /// <summary>
            /// The successfully executed times of the update action
            /// </summary>
            public int SuccessfullyUpdatedTimes { get; set; }
        }
    }
}
