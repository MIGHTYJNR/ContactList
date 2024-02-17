namespace BasicContactList
{
    internal interface IContactManager
    {
        void AddContact(string name, string phoneNumber, string? email, ContactType contactType);
        void DeleteContact(string phoneNumber);
        Contact? FindContact(string phoneNumber);
        void GetContact(string phoneNumber);
        void GetAllContacts();
        void UpdateContact(string phoneNumber, string name, string email);
    }
}