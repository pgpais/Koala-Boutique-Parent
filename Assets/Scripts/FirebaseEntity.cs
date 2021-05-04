public abstract class FirebaseEntity
{
    public static string firebaseReferenceName;
}

public interface IFirebaseEntity
{
    string GetFirebaseReferenceName();
}