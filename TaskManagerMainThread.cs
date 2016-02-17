using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingTask {
	public IEnumerator c;
	public bool autoStart;

	public WaitingTask(IEnumerator c, bool autoStart) {
		this.c = c;
		this.autoStart = autoStart;
	}
}

public class TaskManagerMainThread : MonoSingleton<TaskManagerMainThread> {
	private static List<WaitingTask> queuedTasks = new List<WaitingTask>();

	// Create game object instance
	public static void Instantiate() {
		if (Instance != null) {
			return;
		}

		GameObject gameObject = new GameObject();
		gameObject.name = "TaskManagerMainThread";
		gameObject.AddComponent<TaskManagerMainThread>();
	}

	// Add task to queue to be executed on main thread.
	// This is thread safe as it gets a lock the queue list reference.
	public static void Queue(IEnumerator c, bool autoStart = true) {
		lock ((object)queuedTasks) {
			queuedTasks.Add(new WaitingTask(c, autoStart));
		}
	}

	// Instantiate queued tasks and clear queue list.
	// This is thread safe as it gets a lock to the queue list reference.
	void Update () {
		lock ((object)queuedTasks) {
			foreach (WaitingTask task in queuedTasks) {
				new Task(task.c, task.autoStart);
			}

			queuedTasks.Clear();
		}
	}
}
