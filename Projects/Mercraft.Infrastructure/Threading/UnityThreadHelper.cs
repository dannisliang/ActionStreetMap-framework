using System.Linq;
using System.Collections.Generic;
using System.Collections;

#if !NO_UNITY
using UnityEngine;
#endif

namespace Mercraft.Infrastructure.Threading
{
#if !NO_UNITY
    [ExecuteInEditMode]
    public class UnityThreadHelper : MonoBehaviour
#else
public class UnityThreadHelper
#endif
    {
        private static UnityThreadHelper instance = null;
        private static object syncRoot = new object();

        public static void EnsureHelper()
        {
            lock (syncRoot)
            {
#if !NO_UNITY
                if (null == (object) instance)
                {
                    instance = FindObjectOfType(typeof (UnityThreadHelper)) as UnityThreadHelper;
                    if (null == (object) instance)
                    {
                        var go = new GameObject("[UnityThreadHelper]");
                        go.hideFlags = HideFlags.NotEditable | HideFlags.HideInHierarchy | HideFlags.HideInInspector;
                        instance = go.AddComponent<UnityThreadHelper>();
                        instance.EnsureHelperInstance();
                    }
                }
#else
		    if (null == instance)
		    {
			    instance = new UnityThreadHelper();
			    instance.EnsureHelperInstance();
		    }
#endif
            }
        }

        private static UnityThreadHelper Instance
        {
            get
            {
                EnsureHelper();
                return instance;
            }
        }

        /// <summary>
        /// Returns the GUI/Main Dispatcher.
        /// </summary>
        public static Mercraft.Infrastructure.Threading.Dispatcher Dispatcher
        {
            get
            {
                return Instance.CurrentDispatcher;
            }
        }

        /// <summary>
        /// Returns the TaskDistributor.
        /// </summary>
        public static Mercraft.Infrastructure.Threading.TaskDistributor TaskDistributor
        {
            get
            {
                return Instance.CurrentTaskDistributor;
            }
        }

        private Mercraft.Infrastructure.Threading.Dispatcher dispatcher;

        public Mercraft.Infrastructure.Threading.Dispatcher CurrentDispatcher
        {
            get
            {
                return dispatcher;
            }
        }

        private Mercraft.Infrastructure.Threading.TaskDistributor taskDistributor;

        public Mercraft.Infrastructure.Threading.TaskDistributor CurrentTaskDistributor
        {
            get
            {
                return taskDistributor;
            }
        }

        private void EnsureHelperInstance()
        {
            dispatcher = Mercraft.Infrastructure.Threading.Dispatcher.MainNoThrow ??
                         new Mercraft.Infrastructure.Threading.Dispatcher();
            taskDistributor = Mercraft.Infrastructure.Threading.TaskDistributor.MainNoThrow ??
                              new Mercraft.Infrastructure.Threading.TaskDistributor("TaskDistributor");
        }

        /// <summary>
        /// Creates new thread which runs the given action. The given action will be wrapped so that any exception will be catched and logged.
        /// </summary>
        /// <param name="action">The action which the new thread should run.</param>
        /// <param name="autoStartThread">True when the thread should start immediately after creation.</param>
        /// <returns>The instance of the created thread class.</returns>
        public static Mercraft.Infrastructure.Threading.ActionThread CreateThread(
            System.Action<Mercraft.Infrastructure.Threading.ActionThread> action, bool autoStartThread)
        {
            Instance.EnsureHelperInstance();

            System.Action<Mercraft.Infrastructure.Threading.ActionThread> actionWrapper = currentThread =>
            {
                try
                {
                    action(currentThread);
                }
                catch (System.Exception ex)
                {
                    UnityEngine.Debug.LogError(ex);
                }
            };
            var thread = new Mercraft.Infrastructure.Threading.ActionThread(actionWrapper, autoStartThread);
            Instance.RegisterThread(thread);
            return thread;
        }

        /// <summary>
        /// Creates new thread which runs the given action and starts it after creation. The given action will be wrapped so that any exception will be catched and logged.
        /// </summary>
        /// <param name="action">The action which the new thread should run.</param>
        /// <returns>The instance of the created thread class.</returns>
        public static Mercraft.Infrastructure.Threading.ActionThread CreateThread(
            System.Action<Mercraft.Infrastructure.Threading.ActionThread> action)
        {
            return CreateThread(action, true);
        }

        /// <summary>
        /// Creates new thread which runs the given action. The given action will be wrapped so that any exception will be catched and logged.
        /// </summary>
        /// <param name="action">The action which the new thread should run.</param>
        /// <param name="autoStartThread">True when the thread should start immediately after creation.</param>
        /// <returns>The instance of the created thread class.</returns>
        public static Mercraft.Infrastructure.Threading.ActionThread CreateThread(System.Action action,
            bool autoStartThread)
        {
            return CreateThread((thread) => action(), autoStartThread);
        }

        /// <summary>
        /// Creates new thread which runs the given action and starts it after creation. The given action will be wrapped so that any exception will be catched and logged.
        /// </summary>
        /// <param name="action">The action which the new thread should run.</param>
        /// <returns>The instance of the created thread class.</returns>
        public static Mercraft.Infrastructure.Threading.ActionThread CreateThread(System.Action action)
        {
            return CreateThread((thread) => action(), true);
        }

        #region Enumeratable

        /// <summary>
        /// Creates new thread which runs the given action. The given action will be wrapped so that any exception will be catched and logged.
        /// </summary>
        /// <param name="action">The enumeratable action which the new thread should run.</param>
        /// <param name="autoStartThread">True when the thread should start immediately after creation.</param>
        /// <returns>The instance of the created thread class.</returns>
        public static Mercraft.Infrastructure.Threading.ThreadBase CreateThread(
            System.Func<Mercraft.Infrastructure.Threading.ThreadBase, IEnumerator> action, bool autoStartThread)
        {
            Instance.EnsureHelperInstance();

            var thread = new Mercraft.Infrastructure.Threading.EnumeratableActionThread(action, autoStartThread);
            Instance.RegisterThread(thread);
            return thread;
        }

        /// <summary>
        /// Creates new thread which runs the given action and starts it after creation. The given action will be wrapped so that any exception will be catched and logged.
        /// </summary>
        /// <param name="action">The enumeratable action which the new thread should run.</param>
        /// <returns>The instance of the created thread class.</returns>
        public static Mercraft.Infrastructure.Threading.ThreadBase CreateThread(
            System.Func<Mercraft.Infrastructure.Threading.ThreadBase, IEnumerator> action)
        {
            return CreateThread(action, true);
        }

        /// <summary>
        /// Creates new thread which runs the given action. The given action will be wrapped so that any exception will be catched and logged.
        /// </summary>
        /// <param name="action">The enumeratable action which the new thread should run.</param>
        /// <param name="autoStartThread">True when the thread should start immediately after creation.</param>
        /// <returns>The instance of the created thread class.</returns>
        public static Mercraft.Infrastructure.Threading.ThreadBase CreateThread(System.Func<IEnumerator> action,
            bool autoStartThread)
        {
            System.Func<Mercraft.Infrastructure.Threading.ThreadBase, IEnumerator> wrappedAction =
                (thread) => { return action(); };
            return CreateThread(wrappedAction, autoStartThread);
        }

        /// <summary>
        /// Creates new thread which runs the given action and starts it after creation. The given action will be wrapped so that any exception will be catched and logged.
        /// </summary>
        /// <param name="action">The action which the new thread should run.</param>
        /// <returns>The instance of the created thread class.</returns>
        public static Mercraft.Infrastructure.Threading.ThreadBase CreateThread(System.Func<IEnumerator> action)
        {
            System.Func<Mercraft.Infrastructure.Threading.ThreadBase, IEnumerator> wrappedAction =
                (thread) => { return action(); };
            return CreateThread(wrappedAction, true);
        }

        #endregion

        private List<Mercraft.Infrastructure.Threading.ThreadBase> registeredThreads =
            new List<Mercraft.Infrastructure.Threading.ThreadBase>();

        private void RegisterThread(Mercraft.Infrastructure.Threading.ThreadBase thread)
        {
            if (registeredThreads.Contains(thread))
            {
                return;
            }

            registeredThreads.Add(thread);
        }

#if !NO_UNITY

        private void OnDestroy()
        {
            foreach (var thread in registeredThreads)
                thread.Dispose();

            if (dispatcher != null)
                dispatcher.Dispose();
            dispatcher = null;

            if (taskDistributor != null)
                taskDistributor.Dispose();
            taskDistributor = null;

            if (instance == this)
                instance = null;
        }

        private void Update()
        {
            if (dispatcher != null)
                dispatcher.ProcessTasks();

            var finishedThreads = registeredThreads.Where(thread => !thread.IsAlive).ToArray();
            foreach (var finishedThread in finishedThreads)
            {
                finishedThread.Dispose();
                registeredThreads.Remove(finishedThread);
            }
        }
#endif
    }
}