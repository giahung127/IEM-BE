using System.Text.Json;

namespace IEM.Application.Models.Pagination
{
    public class PaginationModel
    {
        public object Data { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class PaginationModel<T> : PaginationModel
    {
        public new T Data
        {
            get
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };
                return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(base.Data, options), options);
            }

            set
            {
                base.Data = value;
            }
        }
    }
}
