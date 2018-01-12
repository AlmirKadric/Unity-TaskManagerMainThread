using System.Collections;
using System.Collections.Generic;

namespace AlmirKadric.Utils
{
	public class WaitingTask
	{
		public IEnumerator c;
		public bool autoStart;

		public WaitingTask(IEnumerator c, bool autoStart) {
			this.c = c;
			this.autoStart = autoStart;
		}
	}

	public class TaskManagerMainThread : MonoSingleton<TaskManagerMainThread>
	{
		private static List<WaitingTask> queuedTasks = new List<WaitingTask>();

		// Add task to queue to be executed on main thread.
		// This is thread safe as it gets a lock the queue list reference.
		public static void Queue(IEnumerator c, bool autoStart = true) {
			lock ((object)queuedTasks) {
				queuedTasks.Add(new WaitingTask(c, autoStart));
			}
		}

		// Instantiate queued tasks and clear queue list.
		// This is thread safe as it gets a lock to the queue list reference.
		void Update() {
			lock ((object)queuedTasks) {
				foreach (WaitingTask task in queuedTasks) {
					new Task(task.c, task.autoStart);
				}

				queuedTasks.Clear();
			}
		}
	}
}
