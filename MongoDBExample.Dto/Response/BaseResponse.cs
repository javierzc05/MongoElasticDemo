﻿namespace MongoDBExample.Dto.Response
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class BaseResponseGeneric<T> : BaseResponse
    {
        public T? Data { get; set; }
    }
}
