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
    public static UnityEvent LoggedOut = new UnityEvent();
    public static string familyIDSavePath = "family.dat";

    public FirebaseUser User { get; private set; }

    public int FamilyId => familyId;
    [SerializeField] int familyId = 1234;

    FirebaseAuth auth;
    DatabaseReference database;


    void Awake()
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

#if UNITY_EDITOR
        var db = FirebaseDatabase.DefaultInstance;
        db.SetPersistenceEnabled(false);
#endif

        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        database = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private void Start()
    {
        LogsManager.SendLogDirectly(new Log(
            LogType.GameStarted
        ));

        string familyId = FileUtils.ReadFileToString(FileUtils.GetPathToPersistent(familyIDSavePath));
        if (!string.IsNullOrEmpty(familyId))
        {
            Debug.Log("Already have id! Logging in...");
            LoginAnonymously(familyId);
        }
    }

    public void LoginAnonymously(string familyId)
    {
        this.familyId = Int32.Parse(familyId);
        StartCoroutine(LoginAnonymously());
    }

    public IEnumerator LoginAnonymously()
    {
        var task = new YieldTask<FirebaseUser>(auth.SignInAnonymouslyAsync());
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

        LogsManager.SendLogDirectly(new Log(
            LogType.LoggedIn
        ));

        LoggedIn.Invoke();
        yield break;
    }

    public void Logout()
    {
        FileUtils.DeleteFile(FileUtils.GetPathToPersistent(familyIDSavePath));

        LogsManager.SendLogDirectly(new Log(
            LogType.LoggedOut
        ));

        LoggedOut.Invoke();
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

    public string Push(string firebaseReferenceName)
    {
        return database.Child(firebaseReferenceName).Child(familyId.ToString()).Push().Key;
    }

    public void SendObject(string objJSON, string firebaseReferenceName, Action<Task> afterSendAction)
    {
        TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Debug.Log($"saving {objJSON} to {firebaseReferenceName}/{familyId.ToString()}");
        database.Child(firebaseReferenceName).Child(familyId.ToString()).SetRawJsonValueAsync(objJSON).ContinueWith(afterSendAction, scheduler);
    }


    public void SendObject(string objJSON, string firebaseReferenceName, string extraReferenceName, Action<Task> afterSendAction)
    {
        TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Debug.Log($"saving {objJSON} to {firebaseReferenceName}/{familyId.ToString()}/{extraReferenceName}");
        database.Child(firebaseReferenceName).Child(familyId.ToString()).Child(extraReferenceName).SetRawJsonValueAsync(objJSON).ContinueWith(afterSendAction, scheduler);
    }

    /// <summary>
    /// Saves object to cloud without familyId
    /// </summary>
    /// <param name="objJSON"></param>
    /// <param name="firebaseReferenceNames"></param>
    /// <param name="afterSendAction"></param>
    public void SendObject(string objJSON, string[] firebaseReferenceNames, Action<Task> afterSendAction)
    {
        TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Debug.Log($"saving {objJSON} to {firebaseReferenceNames}");

        var db = database;
        Debug.Log(db.Key);

        foreach (var referenceName in firebaseReferenceNames)
        {
            Debug.Log(referenceName);
            db = db.Child(referenceName);
            Debug.Log(db.Key);
        }
        db.SetRawJsonValueAsync(objJSON).ContinueWith(afterSendAction, scheduler);
    }

    public void UpdateObject(Dictionary<string, System.Object> updates, string firebaseReferenceName, Action<Task> afterUpdateAction)
    {
        TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();

        foreach (var key in updates.Keys)
        {
            Debug.Log($"updating {firebaseReferenceName}/{familyId.ToString()}/{key}");
        }

        database.Child(firebaseReferenceName).Child(familyId.ToString()).UpdateChildrenAsync(updates).ContinueWith(afterUpdateAction, scheduler);
    }

    public void GetObject(string firebaseReferenceName, Action<Task<DataSnapshot>> afterSendAction)
    {
        TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Debug.Log($"getting from {firebaseReferenceName}/{familyId.ToString()}");

        database.Child(firebaseReferenceName).Child(familyId.ToString()).GetValueAsync().ContinueWith(afterSendAction, scheduler);
    }

    public void GetObject(string[] firebaseReferenceNames, Action<Task<DataSnapshot>> afterSendAction)
    {
        TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Debug.Log($"getting from {firebaseReferenceNames}/{familyId.ToString()}");

        var db = database;

        foreach (var reference in firebaseReferenceNames)
        {
            db = db.Child(reference);
        }

        db.GetValueAsync().ContinueWith(afterSendAction, scheduler);
    }

    public void RemoveObject(string firebaseReferenceName, string objectToRemove, Action<Task, object> afterRemoveAction)
    {
        TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Debug.Log($"removing {firebaseReferenceName}/{familyId.ToString()}/{objectToRemove}");

        database.Child(firebaseReferenceName).Child(familyId.ToString()).Child(objectToRemove).RemoveValueAsync().ContinueWith(afterRemoveAction, scheduler);
    }

    public void RemoveObject(string firebaseReferenceName, Action<Task, object> afterRemoveAction)
    {
        TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Debug.Log($"removing {firebaseReferenceName}/{familyId.ToString()}");

        database.Child(firebaseReferenceName).Child(familyId.ToString()).RemoveValueAsync().ContinueWith(afterRemoveAction, scheduler);
    }


    public void SetupListenForValueChangedEvents(string[] firebaseReferences, EventHandler<ValueChangedEventArgs> onValueChangedAction)
    {
        DatabaseReference dbReference = database;
        foreach (var reference in firebaseReferences)
        {
            dbReference = dbReference.Child(reference);
        }

        dbReference.ValueChanged += onValueChangedAction;
    }
    public void SetupListenForValueChangedEvents(string firebaseReference, EventHandler<ValueChangedEventArgs> onValueChangedAction)
    {
        database.Child(firebaseReference).Child(familyId.ToString()).ValueChanged += onValueChangedAction;
    }
    public void RemoveValueChangedListener(string[] firebaseReferences, EventHandler<ValueChangedEventArgs> onValueChangedAction)
    {
        DatabaseReference dbReference = database;
        foreach (var reference in firebaseReferences)
        {
            dbReference = dbReference.Child(reference);
        }

        dbReference.ValueChanged -= onValueChangedAction;
    }

    public void SetupListenForChildChangedEvents(string[] firebaseReferences, EventHandler<ChildChangedEventArgs> onChildChangedAction)
    {
        DatabaseReference dbReference = database;
        foreach (var reference in firebaseReferences)
        {
            dbReference = dbReference.Child(reference);
        }

        dbReference.ChildChanged += onChildChangedAction;
    }

    public void SetupListenForChildChangedEvents(string firebaseReference, EventHandler<ChildChangedEventArgs> onChildChangedAction)
    {


        database.Child(firebaseReference).Child(familyId.ToString()).ChildChanged += onChildChangedAction;
    }

    public void RemoveChildChangedListener(string[] firebaseReferences, EventHandler<ChildChangedEventArgs> onChildChangedAction)
    {
        DatabaseReference dbReference = database;
        foreach (var reference in firebaseReferences)
        {
            dbReference = dbReference.Child(reference);
        }

        dbReference.ChildChanged -= onChildChangedAction;
    }

    public void SetupListenForChildAddedEvents(string[] firebaseReferences, EventHandler<ChildChangedEventArgs> onChildAddedAction)
    {
        DatabaseReference dbReference = database;
        foreach (var reference in firebaseReferences)
        {
            dbReference = dbReference.Child(reference);
        }

        dbReference.ChildAdded += onChildAddedAction;
    }

    public void SetupListenForChildRemovedEvents(string[] firebaseReferences, EventHandler<ChildChangedEventArgs> onChildRemovedAction)
    {
        DatabaseReference dbReference = database;
        foreach (var reference in firebaseReferences)
        {
            dbReference = dbReference.Child(reference);
        }

        dbReference.ChildRemoved += onChildRemovedAction;
    }

    private void OnApplicationQuit()
    {
#if UNITY_EDITOR
        var db = FirebaseDatabase.DefaultInstance;
        db.SetPersistenceEnabled(false);
#endif

        LogsManager.SendLogDirectly(new Log(
            LogType.GameQuit
        ));
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
