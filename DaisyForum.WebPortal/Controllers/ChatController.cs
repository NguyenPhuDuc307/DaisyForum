using DaisyForum.ViewModels;
using DaisyForum.ViewModels.Contents;
using DaisyForum.WebPortal.Services;
using Microsoft.AspNetCore.Mvc;


public class ChatController : Controller
{
    private readonly IUploadApiClient _uploadApiClient;
    private readonly IRoomApiClient _roomApiClient;
    private readonly IConfiguration _configuration;
    private readonly IMessageApiClient _messageApiClient;
    private readonly IUserApiClient _userApiClient;

    public ChatController(
        IUploadApiClient uploadApiClient,
        IRoomApiClient roomApiClient,
        IConfiguration configuration,
        IMessageApiClient messageApiClient,
        IUserApiClient userApiClient)
    {
        _uploadApiClient = uploadApiClient;
        _roomApiClient = roomApiClient;
        _configuration = configuration;
        _messageApiClient = messageApiClient;
        _userApiClient = userApiClient;
    }

    [HttpGet]
    public async Task<IActionResult> GetRooms()
    {
        var result = await _roomApiClient.Get();
        return Ok(result);
    }

    // [HttpGet("{id}")]
    public async Task<IActionResult> GetRoomById(int id)
    {
        var result = await _roomApiClient.Get(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRoom(RoomViewModel request)
    {
        var result = await _roomApiClient.Create(request);
        return Ok(result);
    }

    // [HttpPut("{id}")]
    public async Task<IActionResult> EditRoom(int id, RoomViewModel request)
    {
        await _roomApiClient.Edit(id, request);
        return Ok();
    }

    // [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        await _roomApiClient.Delete(id);
        return Ok();
    }

    // [HttpGet("{id}")]
    public async Task<IActionResult> GetMessageById(int id)
    {
        var result = await _messageApiClient.Get(id);
        return Ok(result);
    }

    [HttpGet("Room/{roomName}")]
    public async Task<IActionResult> GetMessages(string roomName)
    {
        var result = await _messageApiClient.GetMessages(roomName);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMessage(MessageViewModel request)
    {
        var result = await _messageApiClient.Create(request);
        return Ok(result);
    }

    // [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMessage(int id)
    {
        await _messageApiClient.Delete(id);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Upload(UploadViewModel request)
    {
        await _uploadApiClient.Upload(request);
        return Ok();
    }
}