using ContactList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BasicContactList
{
    internal sealed class ContactManager : IContactManager
    {
        private const string ContactFilePath = "Contact.txt";
        public static List<Contact> Contacts = new();

        public void AddContact(string name, string phoneNumber, string? email, ContactType contactType)
        {
            try
            {
                if (IsContactExist(phoneNumber))
                {
                    throw new ContactsException("Contact Already Exists!");
                }

                List<string> existingContacts = File.ReadAllLines(ContactFilePath).ToList();

                if (existingContacts.Any(c => c.Contains($"Phone No: {phoneNumber}")))
                {
                    throw new ContactsException("Contact Already Exists in File!");
                }

                int id = Contacts.Count > 0 ? Contacts.Max(c => c.Id) + 1 : 1;

                var contact = new Contact
                {
                    Id = id,
                    Name = name,
                    PhoneNumber = phoneNumber,
                    Email = email,
                    ContactType = contactType,
                    CreatedAt = DateTime.Now
                };

                Contacts.Add(contact);
                Console.WriteLine("Contact added successfully.");

                using (StreamWriter writeContact = new StreamWriter(ContactFilePath, true))
                {
                    string line = $"Id: {contact.Id} | Name: {contact.Name} | Phone No: {contact.PhoneNumber} | E-mail: {contact.Email} | Type: {contact.ContactType} | Time Created: {contact.CreatedAt}";

                    writeContact.WriteLine(line);
                }

            }
            catch (ContactsException ex)
            {
                Console.WriteLine($"Error adding contact: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
            }
        }

        public void DeleteContact(string phoneNumber)
        {
            try
            {
                var contact = FindContact(phoneNumber);

                if (contact is null)
                {
                    throw new ContactsException("Unable to delete contact as it does not exist!");

                }

                Contacts.Remove(contact);

                RefreshContactFile(phoneNumber);
                // SaveContactsToFile();

                Console.WriteLine("Contact Deleted successfully!");

            }
            catch (ContactsException ex)
            {
                Console.WriteLine($"Error deleting contact: {ex.Message}");
            }

        }

        private static void RefreshContactFile(string phoneNumber)
        {
            try
            {
                List<string> existingContacts = File.ReadAllLines(ContactFilePath).ToList();

                // Find the same contact in file
                string? contactToRemove = existingContacts.FirstOrDefault(line => line.Contains($"Phone No: {phoneNumber}"));

                if (contactToRemove != null)
                {
                    existingContacts.Remove(contactToRemove);

                    // Update the Contact.txt file
                    File.WriteAllLines(ContactFilePath, existingContacts);
                    Console.WriteLine("Delete initialized");
                }
                else
                {
                    throw new ContactsException("Contact not find in the file.");
                }
            }
            catch (ContactsException ex)
            {
                Console.WriteLine($"Error updating contacts in the file: {ex.Message}");
            }
        }

        public Contact? FindContact(string phoneNumber)
        {
            var contact = Contacts.Find(c => c.PhoneNumber == phoneNumber);

            if (contact != null)
            {
                return contact;
            }

            try
            {
                // If not found in the contact list, search in the Contact.txt file
                List<string> existingContacts = File.ReadAllLines(ContactFilePath).ToList();

                foreach (var contactLine in existingContacts)
                {
                    if (contactLine.Contains($"Phone No: {phoneNumber}"))
                    {
                        string[] contactDetails = contactLine.Split("|");

                        if (contactDetails.Length < 6)
                        {
                            Console.WriteLine("Invalid contact line format in the file.");
                            return null;
                        }

                        // Extract individual values from the contact line
                        string id = contactDetails[0].Replace("Id: ", "");
                        string name = contactDetails[1].Replace("Name: ", "");
                        string phoneNo = contactDetails[2].Replace("Phone No: ", "");
                        string email = contactDetails[3].Replace("E-mail: ", "");
                        string typeString = contactDetails[4].Replace("Type: ", "");
                        string createdAt = contactDetails[5].Replace("Time Created: ", "");

                        // Create a new Contact object
                        var newContact = new Contact
                        {
                            Id = int.Parse(id),
                            Name = name.ToUpper(),
                            PhoneNumber = phoneNo,
                            Email = email,
                            ContactType = Enum.Parse<ContactType>(typeString),
                            CreatedAt = createdAt.ToLower() == "now" ? DateTime.Now : DateTime.Parse(createdAt)
                        };

                        return newContact;
                    }
                }
            }
            catch (ContactsException ex)
            {
                Console.WriteLine($"Error searching contact file: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
            }

            return null;
        }

        public void GetContact(string phoneNumber)
        {
            var contact = FindContact(phoneNumber);
            if (contact is null)
            {
                Console.WriteLine($"Contact with {phoneNumber} not found");
            }
            else
            {
                Print(contact);
            }
        }

        public void GetAllContacts()
        {
            // int contactCount = Contacts.Count;

            // Console.WriteLine("You have " + "contact".ToQuantity(contactCount));

            // if (contactCount == 0)
            // {
            //     Console.WriteLine("There are no contacts added yet.");
            //     return;
            // }
            List<string> existingContacts = File.ReadAllLines(ContactFilePath).ToList();
            if (existingContacts == null)
            {
                Console.WriteLine("There are no contacts added yet.");
                return;
            }
            else
            {
                System.Console.WriteLine(existingContacts);
            }

            // var table = new ConsoleTable("Id", "Name", "Phone Number", "Email", "Contact Type", "Date Created");

            // foreach (var contact in Contacts)
            // {
            //     table.AddRow(contact.Id, contact.Name, contact.PhoneNumber, contact.Email, ((ContactType)contact.ContactType).Humanize(), contact.CreatedAt.Humanize());
            // }

            // table.Write(Format.Alternative);

        }

        public void UpdateContact(string phoneNumber, string name, string email)
        {
            var contact = FindContact(phoneNumber);

            if (contact is null)
            {
                Console.WriteLine("Contact does not exist!");
                return;
            }
            else
            {
                contact.Name = name;
                contact.Email = email;

                UpdateContactFile(contact);
            }


        }

        private void UpdateContactFile(Contact contact)
        {
            try
            {
                List<string> existingContacts = File.ReadAllLines(ContactFilePath).ToList();

                string? contactToUpdate = existingContacts.FirstOrDefault(line => line.Contains($"Phone No: {contact.PhoneNumber}"));

                if (contactToUpdate != null)
                {
                    int index = existingContacts.IndexOf(contactToUpdate);
                    string updatedContactLine = $"Id: {contact.Id} | Name: {contact.Name} | Phone No: {contact.PhoneNumber} | E-mail: {contact.Email} | Type: {contact.ContactType} | Time Created: {contact.CreatedAt}";

                    existingContacts[index] = updatedContactLine;

                    // Update the Contact.txt file
                    File.WriteAllLines(ContactFilePath, existingContacts);
                    Console.WriteLine("Contact updated in the file.");
                }
                else
                {
                    throw new ContactsException("Contact not found in the file.");
                }
            }
            catch (ContactsException ex)
            {
                Console.WriteLine($"Error updating contacts in the file: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
            }
        }

        private void Print(Contact contact)
        {
            Console.WriteLine($"Name: {contact.Name}\nPhone Number: {contact.PhoneNumber}\nEmail: {contact.Email}");
        }

        private bool IsContactExist(string phoneNumber)
        {
            return Contacts.Any(c => c.PhoneNumber == phoneNumber);
        }
    }
}
