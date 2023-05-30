using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class TestWebRequest : MonoBehaviour
{
    [ContextMenu("Test Get")]
   public async void TestGet()
   {
        var url = "https://jsonplaceholder.typicode.com/todos/1";

        using var www = UnityWebRequest.Get(url);

        www.SetRequestHeader("Conetent-Type", "application/json");

        var operation = www.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield();

        if (www.result == UnityWebRequest.Result.Success)
            Debug.Log($"Success: {www.downloadHandler.text}");
        else
            Debug.Log($"Failed: {www.error}");
   }
}
