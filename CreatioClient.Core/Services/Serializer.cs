using CreatioClient.Core.Exceptions;
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

        /// <summary>Deserializes response into a .NET Type</summary>
        /// <typeparam name="TResult">The target type</typeparam>
        /// <param name="response">response to deserialize</param>
        /// <param name="serializedWith">Serializer to use</param>
        /// <returns>TValue representation of the response</returns>
        /// <exception cref="CreatioSerializationException"></exception>

        internal static async Task<TResult> DeserializeResponse<TResult>(HttpResponseMessage response, SerializedWith serializedWith) where TResult: class
        {

            if(response == null)
            {
                return default;
            }

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

        /// <summary>
        /// <inheritdoc cref="Newtonsoft.Json.JsonConvert.DeserializeObject{T}(string)"/>
        /// </summary>
        /// <typeparam name="TResult"><inheritdoc cref="Newtonsoft.Json.JsonConvert.DeserializeObject{T}(string)"/></typeparam>
        /// <param name="content">HttpContent to desirealize</param>
        /// <returns><inheritdoc cref="Newtonsoft.Json.JsonConvert.DeserializeObject{T}(string)"/></returns>
        /// <exception cref="CreatioSerializationException"></exception>
        private static async Task<TResult> DeserilizeNewtonsoft<TResult>(HttpContent content) where TResult: class
        {
            TResult result = default;
            string str = await content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    result = Newtonsoft.Json.JsonConvert.DeserializeObject<TResult>(str);
                }
                catch (Newtonsoft.Json.JsonReaderException ex)
                {
                    //TODO: Add Logging
                    throw new CreatioSerializationException($"Failed to deserialize {str}", SerializedWith.Newtonsoft, ex);
                }
                catch (Newtonsoft.Json.JsonSerializationException ex) 
                {
                    // TODO: Add Logging
                    throw new CreatioSerializationException($"Failed to deserialize {str}", SerializedWith.Newtonsoft, ex);
                }
                catch (Newtonsoft.Json.JsonWriterException ex) 
                {
                    // TODO: Add Logging
                    throw new CreatioSerializationException($"Failed to deserialize {str}", SerializedWith.Newtonsoft, ex);
                }
                finally
                {
                    content.Dispose();
                }

            }
            return result ?? default;
        }

        /// <summary>
        /// <inheritdoc cref="System.Text.Json.JsonSerializer.DeserializeAsync{TValue}(Stream, System.Text.Json.JsonSerializerOptions?, System.Threading.CancellationToken)"/>
        /// </summary>
        /// <typeparam name="TResult"><inheritdoc cref="System.Text.Json.JsonSerializer.DeserializeAsync{TValue}(Stream, System.Text.Json.JsonSerializerOptions?, System.Threading.CancellationToken)"/></typeparam>
        /// <param name="content">HttpContent to desirealize</param>
        /// <returns><inheritdoc cref="System.Text.Json.JsonSerializer.DeserializeAsync{TValue}(Stream, System.Text.Json.JsonSerializerOptions?, System.Threading.CancellationToken)"/></returns>
        /// <exception cref="CreatioSerializationException"></exception>
        private static async Task<TResult> DeserilizeMicrosoft<TResult>(HttpContent content) where TResult: class
        {
            TResult result = default;  
            using(Stream str = await content.ReadAsStreamAsync())
            {
                try
                {
                    result = await System.Text.Json.JsonSerializer.DeserializeAsync<TResult>(str);
                }
                catch (ArgumentNullException ex)
                {
                    //json is null.

                    //TODO: Add Error logging

                    throw new CreatioSerializationException($"Failed to deserialize {str}", SerializedWith.Microsoft, ex);
                }
                catch (System.Text.Json.JsonException ex)
                {
                    //  The JSON is invalid. -or- TValue is not compatible with the JSON. -or- There
                    //  is remaining data in the string beyond a single JSON value.

                    //TODO: Add Error logging
                    throw new CreatioSerializationException($"Failed to deserialize {str}", SerializedWith.Microsoft, ex);
                }
                catch (NotSupportedException ex)
                {
                    // There is no compatible System.Text.Json.Serialization.JsonConverter for TValue
                    // or its serializable members

                    //TODO: Add Error logging
                    throw new CreatioSerializationException($"Failed to deserialize {str}", SerializedWith.Microsoft, ex);
                }
                finally
                {
                    content.Dispose();
                }
            }
            return result ?? default;
        }

        /// <summary>
        /// <inheritdoc cref="ProtoBuf.Serializer.Deserialize{T}(Stream)"/>
        /// </summary>
        /// <typeparam name="TResult"><inheritdoc cref="ProtoBuf.Serializer.Deserialize{T}(Stream)"/></typeparam>
        /// <param name="content">HttpContent to desirealize</param>
        /// <returns><inheritdoc cref="ProtoBuf.Serializer.Deserialize{T}(Stream)"/></returns>
        /// <exception cref="CreatioSerializationException"></exception>
        private static async Task<TResult> DeserilizeProtobufNet<TResult>(HttpContent content) where TResult: class
        {
            TResult result = default;
            using(Stream str = await content.ReadAsStreamAsync())
            {
                try
                {
                    result = ProtoBuf.Serializer.Deserialize<TResult>(str);

                }
                catch (ProtoBuf.ProtoException ex)
                {
                    //TODO: Add Logging
                    throw new CreatioSerializationException($"Failed to deserialize {str}", SerializedWith.ProtobufNet, ex);
                }
                finally
                {
                    content.Dispose();
                }
            }
            return result ?? default;
        }
    }
}
