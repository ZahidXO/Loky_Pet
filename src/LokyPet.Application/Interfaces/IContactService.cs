namespace LokyPet.Application.Interfaces;
using LokyPet.Domain.Entities;

public interface IContactService
{
    Task<bool> SendMessageAsync(ContactMessage message);
    Task<List<ContactMessage>> GetAllMessagesAsync();
    Task UpdateMessageStatusAsync(Guid id, string status);
}
