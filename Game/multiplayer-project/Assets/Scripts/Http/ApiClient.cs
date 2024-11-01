using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Http
{
    public class ApiClient : MonoBehaviour
    {
        [Header("API Configuration")]
        [SerializeField] private string baseUrl = "https://api.example.com";
        [SerializeField] private string endpoint = "/default-endpoint";
        [SerializeField] private float timeout = 30f;  // Timeout in seconds

        private const string TestEndpoint = "/test";

        public void MakeTestRequest()
        {
            MakeRequest(
                UnityWebRequest.kHttpVerbGET,
                endpointOverride: TestEndpoint,
                onSuccess: OnTestRequestSuccess,
                onError:OnTestRequestError
                );
        }

        private void OnTestRequestSuccess(string response)
        {
            Debug.Log($"Test request succeeded: {response}");
        }
        
        private void OnTestRequestError(string response)
        {
            Debug.Log($"Test request failed: {response}");
        }
        
        /// <summary>
        /// Makes a configurable API call using coroutines.
        /// </summary>
        /// <param name="method">HTTP method: GET, POST, PUT, DELETE</param>
        /// <param name="headers">Optional headers</param>
        /// <param name="jsonBody">Optional JSON body for POST/PUT requests</param>
        /// <param name="endpointOverride">Optional endpoint override</param>
        /// <param name="onSuccess">Action callback on success</param>
        /// <param name="onError">Action callback on error</param>
        private void MakeRequest(
            string method,
            Dictionary<string, string> headers = null,
            string jsonBody = null,
            string endpointOverride = null,
            Action<string> onSuccess = null,
            Action<string> onError = null)
        {
            string url = $"{baseUrl}{(string.IsNullOrEmpty(endpointOverride) ? endpoint : endpointOverride)}";
            StartCoroutine(SendRequest(url, method, headers, jsonBody, onSuccess, onError));
        }

        private System.Collections.IEnumerator SendRequest(
            string url,
            string method,
            Dictionary<string, string> headers,
            string jsonBody,
            Action<string> onSuccess,
            Action<string> onError)
        {
            using UnityWebRequest request = new UnityWebRequest(url, method);
            // Set headers
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.SetRequestHeader(header.Key, header.Value);
                }
            }

            // Set body for POST and PUT
            if (method == UnityWebRequest.kHttpVerbPOST || method == UnityWebRequest.kHttpVerbPUT)
            {
                if (!string.IsNullOrEmpty(jsonBody))
                {
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    request.SetRequestHeader("Content-Type", "application/json");
                }
            }

            request.downloadHandler = new DownloadHandlerBuffer();
            request.timeout = Mathf.RoundToInt(timeout);

            // Send request and wait for completion
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                onError?.Invoke($"Error {request.responseCode}: {request.error}");
            }
        }
    }
}
