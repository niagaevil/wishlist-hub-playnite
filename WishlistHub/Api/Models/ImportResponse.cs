using System;
using System.Collections.Generic;

namespace WishlistHub.Api.Models
{
    public class ImportResponse
    {
        public bool Success { get; set; }

        public ImportResponseData Data { get; set; }
    }

    public class ImportResponseData
    {
        public List<ImportResult> Result { get; set; }

        public string Message { get; set; }
    }

    public class ImportResult
    {
        public Guid Id { get; set; }

        public string Status { get; set; }

        public string Message { get; set; }

        public string Url { get; set; }
    }
}
