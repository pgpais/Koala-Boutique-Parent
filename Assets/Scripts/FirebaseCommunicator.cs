using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using System.Threading.Tasks;
using UnityEngine.Events;

public class FirebaseCommunicator : MonoBehaviour
{
    public static FirebaseCommunicator instance;
    public static UnityEvent LoggedIn = new UnityEvent();

    public FirebaseUser User { get; private set; }

    Firebase.Auth.FirebaseAuth auth;
    DatabaseReference database;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }

        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        database = FirebaseDatabase.DefaultInstance.RootReference;

        StartCoroutine(LoginAnonymously());
    }

    public IEnumerator LoginAnonymously()
    {
        var task = new YieldTask<Firebase.Auth.FirebaseUser>(auth.SignInAnonymouslyAsync());
        yield return task;

        if (task.Task.IsCanceled)
        {
            Debug.LogError("Sign in canceled");
            yield break;
        }

        if (task.Task.IsFaulted)
        {
            Debug.LogError("Error Signing in: " + task.Task.Exception);
            yield break;
        }

        var user = task.Task.Result;
        if (user == null)
        {
            Debug.LogError("Unknown error signing in!");
            yield break;
        }

        User = user;

        Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
        LoggedIn.Invoke();
        yield break;
    }

    public IEnumerator LoginWithEmailAndPassword(string email, string password, Action<bool> afterLoginAction)
    {
        Debug.Log("loggi");
        var task = new YieldTask<Firebase.Auth.FirebaseUser>(auth.SignInWithEmailAndPasswordAsync(email, password));
        yield return task;

        bool success = false;

        if (task.Task.IsCanceled)
        {
            Debug.LogError("Sign in canceled");

        }

        else if (task.Task.IsFaulted)
        {
            Debug.LogError("Error Signing in: " + task.Task.Exception);

        }

        else
        {
            var user = task.Task.Result;
            if (user == null)
            {
                Debug.LogError("Unknown error signing in!");

            }
            else
            {
                User = user;
                success = true;
                Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
            }
        }
        afterLoginAction(success);
        yield break;
    }

    // public IEnumerator CreateNewRoom(string roomId)
    // {
    //     Room newRoom = new Room("Família Lopes da Silva", "Dummy Room");
    //     string json = JsonUtility.ToJson(newRoom);

    //     Debug.Log("Creating Room with id " + roomId);

    //     var task = new YieldTask(database.Child("rooms").Child(roomId).SetRawJsonValueAsync(json));
    //     yield return task;

    //     if (task.Task.IsCanceled)
    //     {
    //         Debug.LogError("Save canceled");
    //     }

    //     if (task.Task.IsFaulted)
    //     {
    //         Debug.LogError("Something went wrong: " + task.Task.Exception);
    //     }

    //     // Should something happen here?

    //     yield break;
    // }

    // public void GetRoomByIdAsync(string roomId, Action<Room> afterFetchTask)
    // {
    //     TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
    //     database.Child("rooms").Child(roomId).GetValueAsync().ContinueWith(task =>
    //     {
    //         if (task.IsFaulted)
    //         {
    //             afterFetchTask(null);
    //         }

    //         afterFetchTask(JsonUtility.FromJson<Room>(task.Result.GetRawJsonValue()));
    //     }, scheduler);
    // }

    // public void SendRoomItems(string itemsJson, string roomId)
    // {
    //     database.Child("items").Child(roomId).SetRawJsonValueAsync(itemsJson).ContinueWith(task =>
    //     {
    //         if (task.IsCanceled)
    //         {
    //             Debug.LogError("save items canceled");
    //             return;
    //         }
    //         if (task.IsFaulted)
    //         {
    //             Debug.LogError("save items task failed");
    //             return;
    //         }
    //         if (task.IsCompleted)
    //         {
    //             Debug.Log("saving tasks success!");
    //         }
    //     });
    // }

    // public void GetRoomItems(string roomId, Action<Task<DataSnapshot>> afterFetchTask)
    // {
    //     TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
    //     database.Child("items").Child(roomId).GetValueAsync().ContinueWith(afterFetchTask, scheduler);
    // }

    // public void InitOnItemsChangedListener(Action<string> OnValueChanged, string roomId)
    // {
    //     database.Child("items").Child(roomId).ValueChanged += (sender, args) =>
    //     {

    //         OnValueChanged(args.Snapshot.GetRawJsonValue());
    //     };
    // }
}
