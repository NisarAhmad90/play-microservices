using System;
using System.ComponentModel.DataAnnotations;

namespace Play.Catalog.Service.Dtos
{
    // Represents an item as returned by the API
    public record ItemDto(
        Guid Id,
        string Name,
        string Description,
        decimal Price,
        DateTimeOffset CreatedDate
    );

    // Used when creating a new item
    public record CreateItemDto(
        [Required] string Name,
        string Description,
        [Range(0, 1000)] decimal Price
    );

    // Used when updating an existing item
    public record UpdateItemDto(
        [Required] string Name,
        string Description,
        [Range(0, 1000)] decimal Price
    );
}