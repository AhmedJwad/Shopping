using Azure;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Shopping.Common;
using Shopping.Data;
using Shopping.Data.Entities;
using Shopping.Migrations;
using System.Net.Http.Headers;

namespace Shopping.Helpers
{
    public static class SessionHelper 
    {
       
        public const string CartSessionKey = "Cart";
       
       

        public static void SetObjectAsJson<T>(ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(ISession session, string key)
            {
                var value = session.GetString(key);             
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
            }

        public static float GetObjectCount<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            if (value == null)
            {
                return 0;
            }

            var items = JsonConvert.DeserializeObject<T[]>(value);
            return items.Length;
        }
        
    }
    }

