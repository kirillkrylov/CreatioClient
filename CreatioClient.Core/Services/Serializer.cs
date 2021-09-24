using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CreatioClient.Core.Services
{

    /// <summary>
    /// Provides methods to deserialize HttpContent
    /// </summary>
    internal static class CreatioSerializer
    {
        internal static async Task<TResult> DeserializeResponse<TResult>(HttpResponseMessage response, SerializedWith serializedWith) where TResult: class
        {
            if(serializedWith == SerializedWith.Microsoft)
            {
                return await DeserilizeMicrosoft<TResult>(response.Content);
            }
            
            else if(serializedWith == SerializedWith.Newtonsoft)
            {
                return await DeserilizeNewtonsoft<TResult>(response.Content);
            }
            
            else if (serializedWith == SerializedWith.ProtobufNet)
            {
                return await DeserilizeProtobufNet<TResult>(response.Content);
            }
            
            else
            {
                return default;
            }
        }

        private static async Task<TResult> DeserilizeNewtonsoft<TResult>(HttpContent content) where TResult: class
        {
            TResult result = default;
            string str = await content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(str))
            {
                result = Newtonsoft.Json.JsonConvert.DeserializeObject<TResult>(str);
            }
            return result ?? default;
        }

        private static async Task<TResult> DeserilizeMicrosoft<TResult>(HttpContent content) where TResult: class
        {
            TResult result = default;  
            using(Stream str = await content.ReadAsStreamAsync())
            {
                try
                {
                    result = await System.Text.Json.JsonSerializer.DeserializeAsync<TResult>(str);
                }
                catch (ArgumentNullException)
                {
                    //json is null.
                    
                    //TODO: Add Error logging
                }
                catch (System.Text.Json.JsonException)
                {
                    //  The JSON is invalid. -or- TValue is not compatible with the JSON. -or- There
                    //  is remaining data in the string beyond a single JSON value.

                    //TODO: Add Error logging
                }
                catch (NotSupportedException)
                {
                    // There is no compatible System.Text.Json.Serialization.JsonConverter for TValue
                    // or its serializable members

                    //TODO: Add Error logging
                }
                finally
                {
                    content.Dispose();
                }
            }
            return result ?? default;
        }

        private static async Task<TResult> DeserilizeProtobufNet<TResult>(HttpContent content) where TResult: class
        {
            TResult result = default;
            using(Stream str = await content.ReadAsStreamAsync())
            {
                result = ProtoBuf.Serializer.Deserialize<TResult>(str);
            }
            return result ?? default;
        }
    }
}
