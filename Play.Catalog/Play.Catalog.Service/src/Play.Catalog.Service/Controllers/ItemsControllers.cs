using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Extensions;
using Play.Common;
using Play.Catalog.Contracts;



namespace Play.Catalog.Service.Controllers
{


    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        // In-memory list simulating a database
        // private static readonly List<ItemDto> items = new()
        // {
        //     new ItemDto(Guid.NewGuid(), "Potion", "Restores a small amount of HP", 5, DateTimeOffset.UtcNow),
        //     new ItemDto(Guid.NewGuid(), "Antidote", "Cures poison", 7, DateTimeOffset.UtcNow),
        //     new ItemDto(Guid.NewGuid(), "Bronze sword", "Deals a small amount of damage", 20, DateTimeOffset.UtcNow)
        // };

        // GET /items
        // [HttpGet]
        // public IEnumerable<ItemDto> Get()
        // {
        //     return items;
        // }

        private readonly IRepository<Item> itemsRepository;

        // private readonly IPublishEndpoint publishEndpoint;
        private readonly IBusControl publishEndpoint;
        public ItemsController(IRepository<Item> itemsRepository, IBusControl publishEndpoint)
        {
            this.itemsRepository = itemsRepository;
            this.publishEndpoint = publishEndpoint;

        }


        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetAsync()
        {
            var items = await itemsRepository.GetAllAsync();
            return items.Select(item => item.AsDto());
        }

        // GET /items/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var item = await itemsRepository.GetAsync(id);
            if (item is null)
            {
                return NotFound();
            }
            // return item.AsDto();
            return Ok(item.AsDto());
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
        {
            var item = new Item
            {
                // Id = Guid.NewGuid().ToString(),
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Price = createItemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };
            await itemsRepository.CreateAsync(item);
            await publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));


            // items.Add(item);

            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
        }

        // PUT /items/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
        {
            var existingItem = await itemsRepository.GetAsync(id);

            if (existingItem == null)
            {
                return NotFound();
            }

            existingItem.Name = updateItemDto.Name;
            existingItem.Description = updateItemDto.Description;
            existingItem.Price = updateItemDto.Price;

            await itemsRepository.UpdateAsync(existingItem);
            await publishEndpoint.Publish(new CatalogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description));


            return NoContent();
        }

        // DELETE /items/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var item = await itemsRepository.GetAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            await itemsRepository.RemoveAsync(item.Id);
            await publishEndpoint.Publish(new CatalogItemDeleted(item.Id));

            return NoContent();
        }



    }
}
