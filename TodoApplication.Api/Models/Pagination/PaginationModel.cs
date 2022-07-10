using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TodoApplication.Api.Models.Pagination;

public class PaginationModel
{
    [DefaultValue(5)]
    [Range(1, 25)]
    public int PageSize { get; set; } = 5;
    
    [DefaultValue(1)]
    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;
}