using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.PeopleService.v1;
using Google.Apis.PeopleService.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util;
using Google.Contacts;
using Google.GData.Client;
using System;

namespace AddGmailContact
{
	internal class Program
	{
		static void Main(string[] args)
		{
			// copy from your google cloud  ClientId
			var clientId = "415456299225-jm1o0hgi40pmdk347fm97hbjd62tlqig.apps.googleusercontent.com";
			// copy from your google cloud  ClientSecret
			var clientSecret = "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";

			string[] scopes = new string[] { "https://www.googleapis.com/auth/contacts",
				"https://www.googleapis.com/auth/gmail.readonly",
				"https://www.googleapis.com/auth/youtube" };

			var credentilas = GoogleWebAuthorizationBroker.AuthorizeAsync(
				new ClientSecrets()
				{
					ClientId = clientId,
					ClientSecret = clientSecret,
				}, scopes, "user", CancellationToken.None).Result;

			if(credentilas.Token.IsExpired(SystemClock.Default) == true)
			{
				credentilas.RefreshTokenAsync(CancellationToken.None).Wait();
			}

			Console.WriteLine("Enter new contact name :");
			string contactName = Console.ReadLine();
			Console.WriteLine("Enter new contact email :");
			string contactEmail = Console.ReadLine();

			PeopleServiceService peopleService = new PeopleServiceService(new BaseClientService.Initializer()
			{
				HttpClientInitializer=credentilas,
			});

			Console.WriteLine("Wait ...");
			List<Name> names = new List<Name>() { new Name() {DisplayName= contactName } };
			List<EmailAddress> emails = new List<EmailAddress>() { new EmailAddress() { Value = contactEmail } };
			Person person = new Person()
			{
				Names = (IList<Name>)names,
				EmailAddresses = emails,
			};

			Person newContact = peopleService.People.CreateContact(person).Execute();

			Console.WriteLine($"Contact with email {newContact.EmailAddresses.First().Value} added to your gmail contacts");
		}
	}
}