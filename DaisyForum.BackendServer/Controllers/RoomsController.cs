﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using DaisyForum.BackendServer.Data;
using DaisyForum.BackendServer.Hubs;
using DaisyForum.ViewModels.Contents;
using DaisyForum.BackendServer.Data.Entities;
using DaisyForum.BackendServer.Controllers;

namespace Chat.Web.Controllers;

public class RoomsController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHubContext<ChatHub> _hubContext;

    public RoomsController(ApplicationDbContext context,
        IMapper mapper,
        IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _mapper = mapper;
        _hubContext = hubContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomViewModel>>> Get()
    {
        var rooms = await _context.Rooms
            .Include(r => r.Admin)
            .ToListAsync();

        var roomsViewModel = _mapper.Map<IEnumerable<Room>, IEnumerable<RoomViewModel>>(rooms);

        return Ok(roomsViewModel.ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Room>> Get(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
            return NotFound();

        var roomViewModel = _mapper.Map<Room, RoomViewModel>(room);
        return Ok(roomViewModel);
    }

    [HttpPost]
    public async Task<ActionResult<RoomViewModel>> Create(RoomViewModel viewModel)
    {
        if (_context.Rooms.Any(r => r.Name == viewModel.Name))
            return BadRequest("Invalid room name or room already exists");

        var user = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
        var room = new Room()
        {
            Name = viewModel.Name,
            Admin = user
        };

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        var createdRoom = _mapper.Map<Room, RoomViewModel>(room);
        await _hubContext.Clients.All.SendAsync("addChatRoom", createdRoom);

        return CreatedAtAction(nameof(Get), new { id = room.Id }, createdRoom);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Edit(int id, RoomViewModel viewModel)
    {
        if (_context.Rooms.Any(r => r.Name == viewModel.Name))
            return BadRequest("Invalid room name or room already exists");

        var room = await _context.Rooms
            .Include(r => r.Admin)
            .Where(r => r.Id == id && r.Admin.UserName == User.Identity.Name)
            .FirstOrDefaultAsync();

        if (room == null)
            return NotFound();

        room.Name = viewModel.Name;
        await _context.SaveChangesAsync();

        var updatedRoom = _mapper.Map<Room, RoomViewModel>(room);
        await _hubContext.Clients.All.SendAsync("updateChatRoom", updatedRoom);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var room = await _context.Rooms
            .Include(r => r.Admin)
            .Where(r => r.Id == id && r.Admin.UserName == User.Identity.Name)
            .FirstOrDefaultAsync();

        if (room == null)
            return NotFound();

        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("removeChatRoom", room.Id);
        await _hubContext.Clients.Group(room.Name).SendAsync("onRoomDeleted");

        return Ok();
    }
}
