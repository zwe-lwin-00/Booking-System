using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.DTOs;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Interfaces;

namespace BookingSystem.Application.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;

    public RoomService(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task<RoomDto?> GetByIdAsync(Guid id)
    {
        var room = await _roomRepository.GetByIdAsync(id);
        return room == null ? null : MapToDto(room);
    }

    public async Task<IEnumerable<RoomDto>> GetAllAsync()
    {
        var rooms = await _roomRepository.GetAllAsync();
        return rooms.Select(MapToDto);
    }

    public async Task<PagedResultDto<RoomDto>> GetPagedAsync(RoomQueryDto query)
    {
        var allRooms = await _roomRepository.GetAllAsync();
        
        // Apply filters
        var filtered = allRooms.AsEnumerable().AsQueryable();
        
        if (!string.IsNullOrEmpty(query.RoomType))
            filtered = filtered.Where(r => r.RoomType.Contains(query.RoomType));
        
        if (query.MinPrice.HasValue)
            filtered = filtered.Where(r => r.PricePerNight >= query.MinPrice.Value);
        
        if (query.MaxPrice.HasValue)
            filtered = filtered.Where(r => r.PricePerNight <= query.MaxPrice.Value);
        
        if (query.MinCapacity.HasValue)
            filtered = filtered.Where(r => r.Capacity >= query.MinCapacity.Value);
        
        if (query.IsAvailable.HasValue)
            filtered = filtered.Where(r => r.IsAvailable == query.IsAvailable.Value);
        
        var totalCount = filtered.Count();
        var items = filtered
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(MapToDto)
            .ToList();
        
        return new PagedResultDto<RoomDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }

    public async Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut)
    {
        var rooms = await _roomRepository.GetAvailableRoomsAsync(checkIn, checkOut);
        return rooms.Select(MapToDto);
    }

    public async Task<RoomDto> CreateAsync(CreateRoomDto createRoomDto)
    {
        // Check if room number already exists
        var existingRoom = await _roomRepository.GetByRoomNumberAsync(createRoomDto.RoomNumber);
        if (existingRoom != null)
            throw new InvalidOperationException($"Room with number {createRoomDto.RoomNumber} already exists");

        var room = new Room
        {
            RoomNumber = createRoomDto.RoomNumber,
            RoomType = createRoomDto.RoomType,
            PricePerNight = createRoomDto.PricePerNight,
            Capacity = createRoomDto.Capacity,
            Description = createRoomDto.Description,
            IsAvailable = true
        };

        var createdRoom = await _roomRepository.AddAsync(room);
        return MapToDto(createdRoom);
    }

    public async Task<RoomDto> UpdateAsync(Guid id, CreateRoomDto updateRoomDto)
    {
        var room = await _roomRepository.GetByIdAsync(id);
        if (room == null)
            throw new NotFoundException(nameof(Room), id);

        // Check if room number is being changed and if it conflicts
        if (room.RoomNumber != updateRoomDto.RoomNumber)
        {
            var existingRoom = await _roomRepository.GetByRoomNumberAsync(updateRoomDto.RoomNumber);
            if (existingRoom != null)
                throw new InvalidOperationException($"Room with number {updateRoomDto.RoomNumber} already exists");
        }

        room.RoomNumber = updateRoomDto.RoomNumber;
        room.RoomType = updateRoomDto.RoomType;
        room.PricePerNight = updateRoomDto.PricePerNight;
        room.Capacity = updateRoomDto.Capacity;
        room.Description = updateRoomDto.Description;

        await _roomRepository.UpdateAsync(room);
        return MapToDto(room);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _roomRepository.DeleteAsync(id);
    }

    private static RoomDto MapToDto(Room room)
    {
        return new RoomDto
        {
            Id = room.Id,
            RoomNumber = room.RoomNumber,
            RoomType = room.RoomType,
            PricePerNight = room.PricePerNight,
            Capacity = room.Capacity,
            Description = room.Description,
            IsAvailable = room.IsAvailable
        };
    }
}
