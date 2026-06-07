using Microsoft.AspNetCore.SignalR;
using practo_backend.Models;
using practo_backend.Data;

namespace practo_backend.Hubs;

public class ChatHub : Hub
{
    private readonly ApplicationDbContext _context;

    public ChatHub(ApplicationDbContext context)
    {
        _context = context;
    }

    // Connect user to a specific appointment room
    public async Task JoinConsultationRoom(string appointmentId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, appointmentId);
    }

    // Leave the room
    public async Task LeaveConsultationRoom(string appointmentId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, appointmentId);
    }

    // Send a message to the room
    public async Task SendMessage(string appointmentId, int senderId, string message)
    {
        var appointmentIdInt = int.Parse(appointmentId);

        // Save to DB
        var chatMessage = new ChatMessage
        {
            AppointmentId = appointmentIdInt,
            SenderId = senderId,
            Message = message,
            SentAt = DateTime.UtcNow
        };

        _context.ChatMessages.Add(chatMessage);
        await _context.SaveChangesAsync();

        // Broadcast to all clients in the group
        await Clients.Group(appointmentId).SendAsync("ReceiveMessage", chatMessage.Id, senderId, message, chatMessage.SentAt);
    }
}
