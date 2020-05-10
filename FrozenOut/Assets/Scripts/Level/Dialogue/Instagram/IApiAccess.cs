﻿using RestSharp;

namespace Scripts.Level.Dialogue.Instagram
{
    public abstract class IAPIAccess
    {
        public abstract IRestResponse<T> DoRequest<T>(string queryStringResource, Method method)
            where T : new();
    }
}